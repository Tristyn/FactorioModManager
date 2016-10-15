using System.Threading.Tasks;
using System.Windows.Input;

namespace FactorioModManager.UI.Framework
{
    interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(object parameter);
    }
}
