using FactorioModManager.Lib.Contracts;

namespace FactorioModManager.Lib
{
    public class GameInstallation : IGameInstallation
    {
        private FactorioLauncher.AppServices _services;
        private readonly string _storageDirectory;
        
        internal GameInstallation(FactorioLauncher.AppServices services, string storageDirectory)
        {
            _services = services;
            _storageDirectory = storageDirectory;
        }

        string arch
    }
}
