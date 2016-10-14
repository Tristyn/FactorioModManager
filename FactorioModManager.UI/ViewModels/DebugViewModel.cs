using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Security;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Avalonia.Controls;
using FactorioModManager.Lib;
using FactorioModManager.Lib.Models;
using FactorioModManager.UI.Dialogs;
using FactorioModManager.UI.Extensions;
using FactorioModManager.UI.Framework;
using Nito.AsyncEx;

namespace FactorioModManager.UI.ViewModels
{
    class DebugViewModel : ViewModelBase
    {
        private bool _initialized;
        private StandaloneInstallationManager _factorio;
        private readonly AsyncLock _lock = new AsyncLock();
        private INotifyTaskCompletion _changeWorkingFolderTask;
        private string _error;
        private ObservableCollection<InstallationSpec> _installations;

        public DebugViewModel()
        {
            InitTask = NotifyTaskCompletion.Create(Initialize);

        }

        public async Task Initialize()
        {
            if (_initialized)
                return;
            _initialized = true;

            var dir = Path.Combine(Environment.CurrentDirectory, "working-folder");
            SetWorkingFolder(dir);
        }

        private void SetWorkingFolder(string path)
        {
            using (_lock.Lock())
            {
                try
                {
                    _factorio = StandaloneInstallationManager.Create(Path.Combine(path, "game"));
                    Installations = new ObservableCollection<InstallationSpec>(_factorio.GetInstallations());

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

        public ICommand ChangeWorkingFolder => new Command(ChangeWorkingFolderHandler);

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

        public ICommand NewInstall => new Command(NewInstallHandler);

        private void NewInstallHandler(object sender)
        {
            var specDialog = new InstallationSpecBuilderDialog();
            using (_lock.Lock())
            {

                if (specDialog.ShowDialog() != DialogResult.OK)
                    return;

                try
                {
                    _factorio.GetStandaloneInstallation(specDialog.SpecResult);
                    Installations.Clear();
                    Installations.AddRange(_factorio.GetInstallations());
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
            private set { Update(ref _installations, value); }
        }

        public string Error
        {
            get { return _error; }
            private set { Update(ref _error, value); }
        }

        public INotifyTaskCompletion InitTask { get; }
    }
}
