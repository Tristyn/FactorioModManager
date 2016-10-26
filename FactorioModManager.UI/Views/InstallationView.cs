using System.ComponentModel;
using System.Reactive.Linq;
using System.Windows.Forms;
using FactorioModManager.Lib.Models;
using FactorioModManager.Lib.Web;
using FactorioModManager.UI.ViewModels;
using ReactiveUI;

namespace FactorioModManager.UI.Views
{
    public partial class InstallationView : UserControl, IViewFor<InstallationViewModel>, INotifyPropertyChanged
    {
        private InstallationViewModel _viewModel;

        public InstallationView()
        {
            InitializeComponent();

            this.WhenAnyValue(view => view.ViewModel.Status)
                .Select(s => s.ToString())
                .BindTo(this, view => view.Status.Text);

            this.WhenAnyValue(view => view.ViewModel.Spec)
                .Select(spec => spec.ToString())
                .BindTo(this, view => view.Spec.Text);

            Observable.FromEventPattern(
                ev => InstallArchiveBtn.Click += ev,
                ev => InstallArchiveBtn.Click -= ev)
                .Select(eventArgs => ViewModel?.Spec)
                .Select(OpenArchiveDialogImpl)
                .Do(archivePath =>
                {
                    ViewModel.InstallFileArchiveFilePath = archivePath;
                    if (ViewModel?.InstallFileArchive.CanExecute(null) == true)
                        ViewModel?.InstallFileArchive.Execute(null);
                })
                .Publish().Connect();
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
