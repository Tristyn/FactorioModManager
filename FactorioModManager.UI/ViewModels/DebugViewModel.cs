using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using FactorioModManager.Lib;
using FactorioModManager.Lib.Archive;
using FactorioModManager.Lib.Models;
using FactorioModManager.Lib.Web;
using FactorioModManager.UI.Dialogs;
using FactorioModManager.UI.Extensions;
using FactorioModManager.UI.Framework;
using Nito.AsyncEx;
using ReactiveUI;

namespace FactorioModManager.UI.ViewModels
{
    class DebugViewModel : ViewModelBase
    {
        private bool _initialized;
        private InstallationManager _factorio;
        private readonly AsyncLock _lock = new AsyncLock();
        private string _error;
        private ObservableCollection<InstallationSpec> _installations;
        private InstallationSpec _selectedInstallation;
        private InstallationStatus _installationStatus;

        public DebugViewModel()
        {
            InitTask = NotifyTaskCompletion.Create(Initialize);
            ChangeWorkingFolder = new Command(ChangeWorkingFolderHandler);
            NewInstallCommand = new AsyncCommand(async token => await NewInstallHandler());
            InstallZipCommand = new AsyncCommand(async token => await InstallZipHandler());
            
            var getInstallationCmd =
                ReactiveCommand.CreateAsyncObservable(o => ReactiveCommand.CreateAsyncTask(GetInstallation));

            this.WhenAnyValue(model => model.SelectedInstallation)
                .InvokeCommand(getInstallationCmd);

            getInstallationCmd.ToProperty(this, model => model.Installation, out _installation);
            /*
            this.WhenAnyValue(model => model.Installation)
                .SelectMany(model => model.Status)
                .ToProperty(this, model => model.InstallationStatus);*/
               
        }

        public string Error
        {
            get { return _error; }
            private set { this.RaiseAndSetIfChanged(ref _error, value); }
        }

        public async Task Initialize()
        {
            if (_initialized)
                return;
            _initialized = true;

            var dir = Path.Combine(Environment.CurrentDirectory, "working-folder");
            SetWorkingFolder(dir);
        }

        public INotifyTaskCompletion InitTask { get; }

        private void SetWorkingFolder(string path)
        {
            using (_lock.Lock())
            {
                try
                {
                    _factorio = InstallationManager.Create(Path.Combine(path, "game"));
                    Installations = new ObservableCollection<InstallationSpec>(_factorio.ScanInstallationsDirectory());

                    // Locking Command, or SemaphoreCommand?
                    // CanExecuteChange also based on task status for these commands
                    // ReentrantAsyncCommand?
                }
                catch (ArgumentException)
                {
                    Error = "I don't like that directory path :(";
                }
                catch (UnauthorizedAccessException)
                {
                    Error = "Can not access the directory.";
                }
                catch (SecurityException)
                {
                    Error = "Missing the required permission to access the folder.";
                }
                catch (PathTooLongException)
                {
                    Error = "Directory path too long.";
                }
                catch (NotSupportedException)
                {
                    Error = "File IO not supported on this system.";
                }
                catch (IOException)
                {
                    Error = "The directory is actually a file or could not access the directory.";
                }
            }
        }

        public ICommand ChangeWorkingFolder { get; }

        private void ChangeWorkingFolderHandler(object sender)
        {
            var folderBrowser = new FolderBrowserDialog
            {
                ShowNewFolderButton = true
            };
            if (folderBrowser.ShowDialog() != DialogResult.OK)
                return;
            var path = folderBrowser.SelectedPath;

            SetWorkingFolder(path);
        }

        #region Installs

        public ICommand NewInstallCommand { get; }

        private async Task NewInstallHandler()
        {
            var specDialog = new InstallationSpecBuilderDialog();
            if (specDialog.ShowDialog() != DialogResult.OK)
                return;

            using (_lock.Lock())
            {

                try
                {
                    await Task.Run(() => _factorio.GetStandaloneInstallation(specDialog.SpecResult));
                    Installations.Clear();
                    Installations.AddRange(_factorio.ScanInstallationsDirectory());
                }
                catch (UnauthorizedAccessException)
                {
                    Error = "Could not access the directory.";
                }
                catch (DirectoryNotFoundException)
                {
                    Error = "Invalid directory.";
                }
                catch (IOException)
                {
                    Error = "The path exists as a file, or is on an unmapped drive.";
                }
                catch (SecurityException)
                {
                    Error = "Not authorized to access the directory.";
                }
            }
        }

        public ObservableCollection<InstallationSpec> Installations
        {
            get { return _installations; }
            private set { this.RaiseAndSetIfChanged(ref _installations, value); }
        }

        public InstallationSpec SelectedInstallation
        {
            get { return _selectedInstallation; }
            set { this.RaiseAndSetIfChanged(ref _selectedInstallation, value); }
        }
        
        private ObservableAsPropertyHelper<Installation> _installation; 
        public Installation Installation
        {
            get { return _installation.Value; }
        }

        private async Task<Installation> GetInstallation(object o)
        {

            if (SelectedInstallation == null)
                return null;

            using (await _lock.LockAsync())
                return await _factorio.GetStandaloneInstallation(SelectedInstallation);
        }

        public InstallationStatus InstallationStatus
        {
            get { return _installationStatus; }
            private set { this.RaiseAndSetIfChanged(ref _installationStatus, value); }
        }

        public ICommand InstallZipCommand { get; }

        private async Task InstallZipHandler()
        {
            var selectedInstall = SelectedInstallation;
            if (selectedInstall == null)
                return;

            var os = OperatingSystemEx.CurrentOSVersion;
            var archiveSpec = new GameArchiveSpec(selectedInstall, os);
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

            try
            {
                Installation install = null;
                await Task.Run(async () =>
                {
                    using (await _lock.LockAsync())
                    {
                        install = await _factorio.GetStandaloneInstallation(selectedInstall);
                    }

                    using (var stream = new FileStream(zipDialog.FileNames.First(), FileMode.Open))
                    using (var archive = new GameArchive(stream, archiveSpec))
                    {
                        await install.InstallFromArchive(archive);
                    }
                });
            }
            catch (UnauthorizedAccessException)
            {
                Error = "Unauthorized access";
            }
            catch (DirectoryNotFoundException)
            {
                Error = "The specified path is invalid (for example, it is on an unmapped drive).";
            }
            catch (IOException)
            {
                Error = "The directory is a file.-or-The network name is not known.";
            }
            catch (SecurityException)
            {
                Error = "Security exception";
            }

        }

        #endregion
    }
}
