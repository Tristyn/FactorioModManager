using System;
using System.IO;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Security;
using System.Threading.Tasks;
using System.Windows.Forms;
using FactorioModManager.Lib;
using FactorioModManager.Lib.Archive;
using FactorioModManager.Lib.Models;
using FactorioModManager.Lib.Web;
using FactorioModManager.UI.Extensions;
using FactorioModManager.UI.Framework;
using ReactiveUI;

namespace FactorioModManager.UI.ViewModels
{
    public class InstallationViewModel : ViewModelBase
    {
        private Installation _model;

        private ObservableAsPropertyHelper<string> _status;
        private ObservableAsPropertyHelper<InstallationSpec> _spec;
        private string _installFileArchiveFilePath;

        public InstallationViewModel(Installation model)
        {
            Model = model;

            this.WhenActivated(() =>
            {
                var disposer = new CompositeDisposable();

                this.WhenAnyObservable(viewModel => viewModel.Model.Status)
                    .Select(status => status.ToString())
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .ToProperty(this, viewModel => viewModel.Status, out _status)
                    .AddTo(disposer);

                this.WhenAnyValue(viewModel => viewModel.Model.Spec)
                    .ToProperty(this, viewModel => viewModel.Spec, out _spec)
                    .AddTo(disposer);

                Play = ReactiveCommand.CreateAsyncTask(
                    this.WhenAnyObservable(viewModel => viewModel.Model.Status)
                        .Select(status => status == InstallationStatus.Ready)
                        .ObserveOn(RxApp.MainThreadScheduler),
                    o => Model?.LaunchGame() ?? Task.CompletedTask)
                    .AddTo(disposer);

                RefreshStatus = ReactiveCommand.CreateAsyncTask(
                    this.WhenAny(viewModel => viewModel.Model, m => m != null),
                    o => Model?.RefreshStatus() ?? Task.CompletedTask);

                // Auto refresh status on model change
                this.WhenAnyValue(viewModel => viewModel.Model)
                    .InvokeCommand(RefreshStatus);

                InstallFileArchive = ReactiveCommand.CreateAsyncTask(o => InstallArchiveImpl());

                return disposer;
            });

        }

        public Task<Stream> OpenArchiveFileImpl()
        {
            throw new NotImplementedException();
        }

        public async Task InstallArchiveImpl()
        {
            /*
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
                return;
                */
            // Do this: http://stackoverflow.com/a/25630554
            // Pass FileName or stream or something as an argument to the second command using Observable.Invoke
            if (InstallFileArchiveFilePath == null)
                return;

            var os = OperatingSystemEx.CurrentOS;
            var archiveSpec = new GameArchiveSpec(Model.Spec, os);

            try
            {
                var archive = new GameArchive(InstallFileArchiveFilePath, archiveSpec);
                await Model.InstallFromArchive(archive);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Unauthorized access");
            }
            catch (DirectoryNotFoundException)
            {
                MessageBox.Show("The specified path is invalid (for example, it is on an unmapped drive).");
            }
            catch (IOException)
            {
                MessageBox.Show("The directory is a file.-or-The network name is not known.");
            }
            catch (SecurityException)
            {
                MessageBox.Show("Security exception");
            }
        }

        public Installation Model
        {
            get { return _model; }
            set { this.RaiseAndSetIfChanged(ref _model, value); }
        }

        public string Status => string.Format("Status: {0}", _status.Value);

        public InstallationSpec Spec => _spec.Value;

        public IReactiveCommand RefreshStatus { get; private set; }

        public IReactiveCommand Play { get; private set; }

        public string InstallFileArchiveFilePath
        {
            get { return _installFileArchiveFilePath; }
            set { this.RaiseAndSetIfChanged(ref _installFileArchiveFilePath, value); }
        }

        public IReactiveCommand InstallFileArchive { get; private set; }
    }
}
