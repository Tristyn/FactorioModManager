using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Windows.Forms;
using FactorioModManager.Lib.Models;
using FactorioModManager.Lib.Web;
using FactorioModManager.UI.ViewModels;
using ReactiveUI;
using Splat;

namespace FactorioModManager.UI.Views
{
    public partial class InstallationView : UserControl, IViewFor<InstallationViewModel>, INotifyPropertyChanged
    {
        private InstallationViewModel _viewModel;

        public InstallationView()
        {
            if (Startup.IsInDesignMode)
                return;

            InitializeComponent();

            this.WhenAnyValue(view => view.ViewModel.Status)
                .Select(s => s.ToString())
                .BindTo(this, view => view.Status.Text);
            
            this.WhenAnyValue(view => view.ViewModel.Spec)
                .Select(spec => spec.ToString())
                .BindTo(this, view => view.Spec.Text);

            this.BindCommand(_viewModel,
                viewModel => viewModel.RefreshStatus,
                view => view.RefreshBtn);

            this.BindCommand(_viewModel,
                viewModel => viewModel.Play,
                view => view.PlayBtn);

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
                });

            this.WhenAnyObservable(view => view.ViewModel.InstallFileArchive.CanExecuteObservable)
                .BindTo(this, view => view.InstallArchiveBtn.Enabled);

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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ViewModel"));
            }
        }

        public InstallationViewModel ViewModel
        {
            get { return _viewModel; }
            set
            {
                _viewModel = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ViewModel"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
