using System;
using System.IO;
using System.Reactive.Disposables;
using System.Security;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using FactorioModManager.Lib;
using FactorioModManager.Lib.Models;
using FactorioModManager.UI.Extensions;
using FactorioModManager.UI.Views;
using Nito.AsyncEx;
using ReactiveUI;
using ReactiveUI.Winforms;

namespace FactorioModManager.UI.ViewModels
{
    public class DebugViewModel : ViewModelBase
    {
        private InstallationRepository _factorio;
        private readonly AsyncLock _lock = new AsyncLock();
        private string _error;
        private ReactiveBindingList<InstallationSpec> _installations;
        private InstallationSpec _selectedInstallationSpec;
        private InstallationViewModel _installation;

        public DebugViewModel()
        {
            this.WhenActivated(() =>
            {
                var disposer = new CompositeDisposable();

                var loadInstallation = ReactiveCommand.CreateAsyncTask(
                        o => LoadInstallation(SelectedInstallationSpec))
                    .AddTo(disposer);

                this.WhenAnyValue(model => model.SelectedInstallationSpec)
                    .InvokeCommand(loadInstallation)
                    .AddTo(disposer);

                var dir = Path.Combine(Environment.CurrentDirectory, "working-folder");
                SetWorkingFolder(dir);

                return disposer;
            });
            
            Installation = new InstallationViewModel(null);
        }

        public string Error
        {
            get { return _error; }
            private set { this.RaiseAndSetIfChanged(ref _error, value); }
        }

        private void SetWorkingFolder(string path)
        {
            using (_lock.Lock())
            {
                try
                {
                    _factorio = InstallationRepository.Create(Path.Combine(path, "game"));
                    Installations = new ReactiveBindingList<InstallationSpec>(_factorio.EnumerateInstallations());
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
                    Installations.AddRange(_factorio.EnumerateInstallations());
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

            Installation.Model = model;
        }

        #endregion
    }
}
