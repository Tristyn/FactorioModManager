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

        public async Task<InstallationStatus> GetStatus()
        {
            using (await _directoryLock.LockAsync())
            {
                var executableExists = await AsyncDirectory.Exists(ExecutableAbsolutePath);

                if (!executableExists)
                    return InstallationStatus.NonExistant;

                return InstallationStatus.Ready;
            }
        }

        public async Task InstallFromArchive(GameArchive archive)
        {
            if (archive == null)
                throw new ArgumentNullException("archive");

            var currentOs = Environment.OSVersion.Platform.ToFactorioSupportedOperatingSystem();
            var archiveOs = archive.Spec.OperatingSystem;

            if (currentOs != archiveOs)
                throw new ArgumentException("Platform Mismatch: The operating system is not compatible with this archives files.");

            using (await _directoryLock.LockAsync())
            {
                foreach (var entry in archive.Entries())
                {
                    var destinationPath = Path.Combine(_storageDirectory, entry.RelativePath);
                    if (entry.IsFolder)
                    {
                        Directory.CreateDirectory(destinationPath);
                    }
                    else
                    {
                        // Assure that the directory exists, then create the file
                        var parentDirPath = Path.GetDirectoryName(destinationPath);
                        if (parentDirPath != null)
                        {
                            // If the parent dir is null, it means the file is located
                            // in the root folder, such as D:\foo.txt
                            Directory.CreateDirectory(parentDirPath);
                        }
                        using (var stream = File.Create(destinationPath))
                        {
                            entry.Extract(stream);
                        }
                    }
                }
            }

            Status = await GetStatus();
        }
    }
}
