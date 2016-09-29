using System.Threading.Tasks;

namespace FactorioModManager.Lib.Contracts
{
    interface IModPackManager
    {
        Task GetModPacksSnapshot();
    }
}
