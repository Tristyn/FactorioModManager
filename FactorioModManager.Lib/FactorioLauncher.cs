using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace FactorioModManager.Lib
{
    /// <summary>
    /// Provides access to the factorio updater and modpack management.
    /// </summary>
    public class FactorioLauncher : IDisposable
    {
        /* The FactorioLauncher acts as a bootstrapper and mediator for the 
         * various APIs and infrastructure services of the application. 
         */
        

        internal AppServices Services { get; private set; }

        private GameInstallationFactory _gameInstallationFactory;
        
        public static Task<FactorioLauncher> Create(string storageDirectory)
        {
            var jobScheduler = new JobScheduler();
            var modPortalClient = new FactorioWebClient();

            var services = new AppServices(jobScheduler, modPortalClient);
            
            var launcher = new FactorioLauncher
            {
                Services = services,
                _gameInstallationFactory = new GameInstallationFactory(services, storageDirectory)
            };
            await launcher._gameInstallationFactory.Create()
            return Task.FromResult(launcher);
        }

        public void Dispose()
        {
            // Will await all tasks in the job scheduler
            // then call Dispose() on private members.
        }

        internal class AppServices
        {
            public JobScheduler JobScheduler { get; private set; }
            public FactorioWebClient FactorioWebClient { get; private set; }

            public AppServices(JobScheduler jobScheduler, FactorioWebClient factorioWebClient)
            {
                JobScheduler = jobScheduler;
                FactorioWebClient = factorioWebClient;
            }
        }
    }
}
