using System.IO;
using System.Threading.Tasks;
using FactorioModManager.Lib.Contracts;
using FactorioModManager.Lib.Models;

namespace FactorioModManager.Lib
{
    /// <summary>
    /// Provides access to the factorio updater and modpack management.
    /// </summary>
    public class FactorioLauncher
    {
        /* The FactorioLauncher acts as a bootstrapper and mediator for the 
         * various APIs and infrastructure services of the application. 
         */
        
        private readonly InstallationFactory _installationFactory;

        private const string InstallationsFolder = "installs";

        public static Task<FactorioLauncher> Create(string storageDirectory)
        {
            var launcher = new FactorioLauncher(storageDirectory);
            return Task.FromResult(launcher);
        }

        private FactorioLauncher(string storageDirectory)
        {
            var installationsAbsolutePath = Path.Combine(storageDirectory, InstallationsFolder);
            _installationFactory = new InstallationFactory(installationsAbsolutePath);
        }

        public IInstallation GetStandaloneInstallation(InstallationSpec spec)
        {
            return _installationFactory.CreateStandaloneInstallation(spec);
        }
    }
}
