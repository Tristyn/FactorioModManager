using System.Threading.Tasks;

namespace FactorioModManager.Lib.Contracts
{
    public interface INetworkActivityRecorder
    {
        Task LogActivity(long bytesTransferred, bool isUpstream);
    }
}
