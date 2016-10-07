using System.IO;
using FactorioModManager.Lib.Contracts;
using FactorioModManager.Lib.Models;

namespace FactorioModManager.Lib
{
    internal class InstallationFactory
    {
        private readonly FactorioLauncher.AppServices _services;
        private readonly string _storageDirectory;
        private readonly JobFactory _jobFactory;
        
        public InstallationFactory(FactorioLauncher.AppServices services, string storageDirectory)
        {
            _services = services;
            _storageDirectory = storageDirectory;
            _jobFactory = new JobFactory(_services.JobScheduler);
        }

        public Job<IInstallation> Create(InstallationSpec spec)
        {
            var gamePath = Path.Combine(_storageDirectory, spec.ToString());

            var install = new Installation(spec, _services, gamePath);
            
            return _jobFactory.FromResult<IInstallation>(install);
        }
    }
}
