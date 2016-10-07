using System;
using System.IO;
using System.Threading.Tasks;
using FactorioModManager.Lib.Contracts;
using FactorioModManager.Lib.Models;

namespace FactorioModManager.Lib
{
    public class Installation : IInstallation
    {
        public InstallationSpec Spec { get; }

        private readonly FactorioLauncher.AppServices _services;
        private readonly string _storageDirectory;

        public string ExecutableAbsolutePath => Path.Combine(_storageDirectory, Spec.ExecutableRelativePath);

        internal Installation(InstallationSpec spec, FactorioLauncher.AppServices services, string storageDirectory)
        {
            if (spec == null)
                throw new ArgumentNullException("spec");
            if (services == null)
                throw new ArgumentNullException("services");
            if (storageDirectory == null)
                throw new ArgumentNullException("storageDirectory");
            
            Spec = spec;
            _services = services;
            _storageDirectory = storageDirectory;
        }

        public async Task<InstallationStatus> GetStatus()
        {
            var executableExists = await AsyncDirectory.Exists(ExecutableAbsolutePath);

            if (!executableExists)
                return InstallationStatus.NonExistant;

            return InstallationStatus.Ready;
        }


    }
}
