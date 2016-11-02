using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using FactorioModManager.Lib.Archive;
using FactorioModManager.Lib.Files;
using FactorioModManager.Lib.Models;
using Nito.AsyncEx;

namespace FactorioModManager.Lib
{
    public class Installation
    {
        public InstallationSpec Spec { get; }

        public IObservable<InstallationStatus> Status
        {
            get { return _status; }
        }

        private readonly string _storageDirectory;
        private readonly ShortcutFile _modPackShortcut;
        private readonly ShortcutFile _savesShortcut;
        private readonly ShortcutFile _configShortcut;
        private readonly AsyncLock _lock = new AsyncLock();
        private readonly BehaviorSubject<InstallationStatus> _status = new BehaviorSubject<InstallationStatus>(InstallationStatus.Unknown);

        private Process _gameProcess;

        /// <exception cref="ArgumentNullException"><paramref name="spec"/> or <paramref name="storageDirectory"/> is <see langword="null" />.</exception>
        internal Installation(InstallationSpec spec, string storageDirectory)
        {
            if (spec == null)
                throw new ArgumentNullException("spec");
            if (storageDirectory == null)
                throw new ArgumentNullException("storageDirectory");

            Spec = spec;
            _storageDirectory = storageDirectory;
            _modPackShortcut = ShortcutFile.New(Path.Combine(storageDirectory, "mods"));
            _savesShortcut = ShortcutFile.New(Path.Combine(storageDirectory, "saves"));
            _configShortcut = ShortcutFile.New(Path.Combine(storageDirectory, "config"));
        }

        public async Task<InstallationStatus> RefreshStatus()
        {
            using (await _lock.LockAsync())
            {
                return RefreshStatusInternal();
            }
        }

        private InstallationStatus RefreshStatusInternal()
        {
            if (_gameProcess != null && !_gameProcess.HasExited)
            {
                _status.OnNext(InstallationStatus.Running);
                return InstallationStatus.Running;
            }

            var gameBinaryExists = File.Exists(GetExecutableAbsolutePath());

            var nextStatus = gameBinaryExists
                ? InstallationStatus.Ready
                : InstallationStatus.NotReady;

            _status.OnNext(nextStatus);
            return nextStatus;
        }

        /// <exception cref="ArgumentNullException"><paramref name="archive"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">Platform Mismatch: The operating system is not compatible with the archive files.</exception>
        /// <exception cref="ArchiveException"></exception>
        /// <exception cref="InvalidGameArchiveContentsException"></exception>
        /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission.</exception>
        /// <exception cref="FailedToLaunchSevenZipException">The 7Zip executable could not be found or failed to launch.</exception>
        /// <exception cref="SevenZipExitCodeException">Seven zip returned with a bad exit code.</exception>
        public async Task InstallFromArchive(GameArchive archive)
        {
            if (archive == null)
                throw new ArgumentNullException("archive");

            var currentOs = OperatingSystemEx.CurrentOS;
            var archiveOs = archive.Spec.OperatingSystem;

            if (currentOs != archiveOs)
                throw new ArgumentException("Platform Mismatch: The operating system is not compatible with the archive files.", "archive");

            using (await _lock.LockAsync())
            {
                _status.OnNext(InstallationStatus.Installing);

                try
                {
                    await archive.Extract(_storageDirectory);
                }
                finally
                {
                    RefreshStatusInternal();
                }
            }
        }

        public void SetModPack(/*ModPackDirectory modPackDirectory*/ string directory)
        {
            // Fuck the mods directory, we only use shortcuts and sym links
            var modsPath = Path.Combine(_storageDirectory, "mods");
            var modsSymLink = new FileInfo(modsPath);
            try
            {
                Directory.Delete(Path.Combine(_storageDirectory, "mods"), true);
            }
            catch (IOException)
            {
                // among many possible reasons, 'mods' may be a symlink file which caused the exception
            }

            _modPackShortcut.SetTarget(directory);
        }

        public string GetExecutableAbsolutePath()
        {
            return Path.Combine(_storageDirectory, Spec.ExecutableRelativePath);
        }

        public async Task<bool> LaunchGame()
        {
            using (await _lock.LockAsync())
            {
                var status = RefreshStatusInternal();
                if (status != InstallationStatus.Ready)
                    return false;

                var gameBinary = GetExecutableAbsolutePath();
                try
                {
                    _gameProcess = Process.Start(gameBinary);
                    _status.OnNext(InstallationStatus.Running);
                }
                catch (Win32Exception ex)
                {
                    // Binary not found or invalid format
                    _status.OnNext(InstallationStatus.NotReady);
                    return false;
                }
                return true;
            }
        }

        public override string ToString()
        {
            return Spec.ToString();
        }
    }
}
