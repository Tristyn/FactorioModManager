using System;
using System.IO;
using System.Threading.Tasks;
using FactorioModManager.Lib.Archive;
using FactorioModManager.Lib.Contracts;
using FactorioModManager.Lib.Files;
using FactorioModManager.Lib.Models;
using Nito.AsyncEx;

namespace FactorioModManager.Lib
{
    public class Installation
    {
        public InstallationSpec Spec { get; }

        public InstallationStatus Status { get; private set; }

        private readonly string _storageDirectory;
        private readonly ShortcutFile _modPackShortcut;
        private readonly ShortcutFile _savesShortcut;
        private readonly ShortcutFile _configShortcut;
        private readonly AsyncLock _directoryLock = new AsyncLock();

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
            using (await _directoryLock.LockAsync())
            {
                return RefreshStatusInternal();
            }
        }

        private InstallationStatus RefreshStatusInternal()
        {
            var executableExists = Directory.Exists(GetExecutableAbsolutePath());

            if (executableExists)
                Status = InstallationStatus.Ready;
            else
                Status = InstallationStatus.NonExistant;

            return Status;
        }

        /// <exception cref="ArgumentNullException"><paramref name="archive"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">Platform Mismatch: The operating system is not compatible with the archive files.</exception>
        public async Task InstallFromArchive(GameArchive archive)
        {
            if (archive == null)
                throw new ArgumentNullException("archive");
            
            var currentOs = OperatingSystemEx.CurrentOSVersion;
            var archiveOs = archive.Spec.OperatingSystem;

            if (currentOs != archiveOs)
                throw new ArgumentException("Platform Mismatch: The operating system is not compatible with the archive files.", "archive");

            using (await _directoryLock.LockAsync())
            {
                try
                {
                    Status = InstallationStatus.Installing;
                    
                    await archive.Extract(_storageDirectory);

                    RefreshStatusInternal();
                }
                catch
                {
                    Status = InstallationStatus.Unknown;
                    throw;
                }
            }
        }

        public void SetModPack(ModPackDirectory modPackDirectory)
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

            _modPackShortcut.SetTarget(modPackDirectory.Directory);
        }

        public string GetExecutableAbsolutePath()
        {
            return Path.Combine(_storageDirectory, Spec.ExecutableRelativePath);
        }

        public override string ToString()
        {
            return Spec.ToString();
        }
    }
}
