using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using FactorioModManager.Lib;
using FactorioModManager.Lib.Archive;
using FactorioModManager.Lib.Models;
using FactorioModManager.Lib.Web;
using FactorioModManager.UI.Dialogs;
using FactorioModManager.UI.Framework;
using Nito.AsyncEx;
using ReactiveUI;
using ReactiveUI.Winforms;

namespace FactorioModManager.UI.ViewModels
{
    public class DebugViewModel : ViewModelBase
    {
        private bool _initialized;
        private InstallationRepository _factorio;
        private readonly AsyncLock _lock = new AsyncLock();
        private string _error;
        private ReactiveBindingList<InstallationSpec> _installations;
        private InstallationSpec _selectedInstallationSpec;
        private InstallationViewModel _installation;

        public DebugViewModel()
        {
            InitTask = NotifyTaskCompletion.Create(Initialize);
            ChangeWorkingFolder = new Command(ChangeWorkingFolderHandler);
            NewInstallCommand = new AsyncCommand(async token => await NewInstallHandler());
            InstallZipCommand = new AsyncCommand(async token => await InstallZipHandler());
            
            this.WhenAnyValue(model => model.SelectedInstallationSpec)
                .InvokeCommand(ReactiveCommand.CreateAsyncTask(
                o => LoadInstallation(SelectedInstallationSpec)));

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
                    _factorio = InstallationRepository.Create(Path.Combine(path, "game"));
                    Installations = new ReactiveBindingList<InstallationSpec>(_factorio.ScanInstallationsDirectory());

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

        public ReactiveBindingList<InstallationSpec> Installations
        {
            get { return _installations; }
            private set { this.RaiseAndSetIfChanged(ref _installations, value); }
        }

        public InstallationSpec SelectedInstallationSpec
        {
            get { return _selectedInstallationSpec; }
            set { this.RaiseAndSetIfChanged(ref _selectedInstallationSpec, value); }
        }
        
        public InstallationViewModel Installation
        {
            get { return _installation; }
            private set { this.RaiseAndSetIfChanged(ref _installation, value); }
        }

        private async Task LoadInstallation(InstallationSpec spec)
        {
            if (spec == null)
                return;

            Installation model;
            using (await _lock.LockAsync())
                model = await _factorio.GetStandaloneInstallation(spec);

            
            Installation = new InstallationViewModel(model);
        }

        public ICommand InstallZipCommand { get; }

        private async Task InstallZipHandler()
        {
            var selectedInstall = SelectedInstallationSpec;
            if (selectedInstall == null)
                return;

            var os = OperatingSystemEx.CurrentOS;
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
                    
                    using (var archive = new GameArchive(zipDialog.FileName, archiveSpec))
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
