using System;
using System.IO;
using System.Threading.Tasks;
using FactorioModManager.Lib.Archive;
using FactorioModManager.Lib.Contracts;
using FactorioModManager.Lib.Models;
using Nito.AsyncEx;

namespace FactorioModManager.Lib
{
    public class Installation : IInstallation
    {
        public InstallationSpec Spec { get; }

        public InstallationStatus Status { get; private set; }

        private readonly string _storageDirectory;
        private readonly AsyncLock _directoryLock = new AsyncLock();

        public string ExecutableAbsolutePath => Path.Combine(_storageDirectory, Spec.ExecutableRelativePath);

        internal Installation(InstallationSpec spec, string storageDirectory)
        {
            if (spec == null)
                throw new ArgumentNullException("spec");
            if (storageDirectory == null)
                throw new ArgumentNullException("storageDirectory");

            Spec = spec;
            _storageDirectory = storageDirectory;
        }

        public async Task<InstallationStatus> RefreshStatus()
        {
            using (await _directoryLock.LockAsync())
            {
                var executableExists = await AsyncDirectory.Exists(ExecutableAbsolutePath);

                if (!executableExists)
                    Status = InstallationStatus.NonExistant;

                Status = InstallationStatus.Ready;
            }
            return Status;
        }

        public async Task InstallFromArchive(GameArchive archive)
        {
            if (archive == null)
                throw new ArgumentNullException("archive");

            var currentOs = Environment.OSVersion.Platform.ToFactorioSupportedOperatingSystem();
            var archiveOs = archive.Spec.OperatingSystem;

            if (currentOs != archiveOs)
                throw new ArgumentException("Platform Mismatch: The operating system is not compatible with the archive files.");

            using (await _directoryLock.LockAsync())
            using (new DisposalFunc(() => Status = InstallationStatus.Unknown))
            {
                Status = InstallationStatus.Installing;

                await archive.Extract(_storageDirectory);
            }
            await RefreshStatus();
        }
    }
}
