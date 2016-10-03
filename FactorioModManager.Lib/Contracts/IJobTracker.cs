using FactorioModManager.Lib.Models;

namespace FactorioModManager.Lib.Contracts
{
    internal interface IJobTracker
    {
        void TrackProgress(Job job);
    }
}
