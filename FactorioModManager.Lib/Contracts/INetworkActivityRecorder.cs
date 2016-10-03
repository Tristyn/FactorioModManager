using System.Threading.Tasks;

namespace FactorioModManager.Lib.Contracts
{
    internal interface INetworkActivityRecorder
    {
        Task LogActivity(long bytesTransferred, bool isUpstream);
    }
}
