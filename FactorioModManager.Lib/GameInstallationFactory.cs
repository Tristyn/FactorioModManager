using System.Runtime.InteropServices;
using FactorioModManager.Lib.Contracts;
using FactorioModManager.Lib.Models;

namespace FactorioModManager.Lib
{
    internal class GameInstallationFactory
    {
        private readonly FactorioLauncher.AppServices _services;
        private readonly string _storageDirectory;
        private readonly JobFactory _jobFactory;
        

        public GameInstallationFactory(FactorioLauncher.AppServices services, string storageDirectory)
        {
            _services = services;
            _storageDirectory = storageDirectory;
            _jobFactory = new JobFactory(_services.JobScheduler);
        }

        public Job<IGameInstallation> Create()
        {
            var install = new GameInstallation(_services, _storageDirectory);

            return _jobFactory.FromResult<IGameInstallation>(install);
        }
    }
}
