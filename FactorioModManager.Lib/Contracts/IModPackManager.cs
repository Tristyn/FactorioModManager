using System.Threading.Tasks;

namespace FactorioModManager.Lib.Contracts
{
    public interface IModPackManager
    {
        Task GetModPacksSnapshot();
    }
}
