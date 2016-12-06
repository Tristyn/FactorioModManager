using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Forms;
using FactorioModManager.Lib.Models;
using FactorioModManager.Lib.Web;
using FactorioModManager.UI.Extensions;
using FactorioModManager.UI.ViewModels;
using ReactiveUI;
using Splat;

namespace FactorioModManager.UI.Views
{
    public partial class InstallationView : UserControl, IViewFor<InstallationViewModel>
    {
        private InstallationViewModel _viewModel;

        public InstallationView()
        {
            if (Startup.IsInDesignMode)
                return;

            InitializeComponent();

            this.WhenActivated(() =>
            {
                var disposer = new CompositeDisposable();

                //this.WhenAnyValue(view => view.ViewModel.Status)
                //    .Select(s => s.ToString())
                //    .BindTo(this, view => view.Status.Text)
                //    .AddTo(disposer);

                this.Bind(ViewModel, viewModel => viewModel.Status, view => view.Status.Text)
                    .AddTo(disposer);

                this.WhenAnyValue(view => view.ViewModel.Spec)
                    .Select(spec => spec?.ToString() ?? "")
                    .BindTo(this, view => view.Spec.Text)
                    .AddTo(disposer);

                this.BindCommand(_viewModel,
                    viewModel => viewModel.RefreshStatus,
                    view => view.RefreshBtn)
                    .AddTo(disposer);

                this.BindCommand(_viewModel,
                    viewModel => viewModel.Play,
                    view => view.PlayBtn)
                    .AddTo(disposer);

                // hacky event handler to show off the sign form
                InstallWebBtn.Click += (sender, args) => { new SignInView(new SignInViewModel(new AnonymousFactorioWebClient())).ShowDialog(this); };

                Observable.FromEventPattern(
                    ev => InstallArchiveBtn.Click += ev,
                    ev => InstallArchiveBtn.Click -= ev)
                    .Select(eventArgs => ViewModel?.Spec)
                    .Select(OpenArchiveDialogImpl)
                    .Subscribe(archivePath =>
                    {
                        ViewModel.InstallFileArchiveFilePath = archivePath;
                        if (ViewModel?.InstallFileArchive.CanExecute(null) == true)
                            ViewModel?.InstallFileArchive.Execute(null);
                    })
                    .AddTo(disposer);

                this.WhenAnyObservable(view => view.ViewModel.InstallFileArchive.CanExecuteObservable)
                    .BindTo(this, view => view.InstallArchiveBtn.Enabled)
                    .AddTo(disposer);

                return disposer;
            });
        }

        private string OpenArchiveDialogImpl(InstallationSpec spec)
        {

            var os = OperatingSystemEx.CurrentOS;
            var archiveSpec = new GameArchiveSpec(spec, os);
            var archiveExtension = GameArchiveSpec.GetArchiveExtension(os);

            var zipDialog = new OpenFileDialog
            {
                AutoUpgradeEnabled = true,
                CheckFileExists = true,
                DefaultExt = archiveExtension,
                SupportMultiDottedExtensions = true,
                Filter = string.Format("Archive Files (*{0}) | *{0}", archiveExtension)
            };

            if (zipDialog.ShowDialog() != DialogResult.OK)
                return null;

            return zipDialog.FileName;
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set
            {
                ViewModel = value as InstallationViewModel;
            }
        }

        public InstallationViewModel ViewModel
        {
            get { return _viewModel; }
            set
            {
                _viewModel = value;
            }
        }
    }
}
