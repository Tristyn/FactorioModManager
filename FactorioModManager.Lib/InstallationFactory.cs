using System.IO;
using FactorioModManager.Lib.Contracts;
using FactorioModManager.Lib.Models;

namespace FactorioModManager.Lib
{
    internal class InstallationFactory
    {
        private readonly string _standaloneInstallationsDirectory;
        
        public InstallationFactory(string standaloneInstallationsDirectory)
        {
            _standaloneInstallationsDirectory = standaloneInstallationsDirectory;
        }

        public IInstallation CreateStandaloneInstallation(InstallationSpec spec)
        {
            var gamePath = Path.Combine(_standaloneInstallationsDirectory, spec.ToString());

            return new Installation(spec, gamePath);
        }
    }
}
