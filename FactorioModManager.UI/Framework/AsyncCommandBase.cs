using System;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace FactorioModManager.UI.Framework
{
    abstract class AsyncCommandBase : IAsyncCommand
    {
        public event EventHandler CanExecuteChanged;
        
        public abstract Task ExecuteAsync(object parameter);
        public abstract bool CanExecute(object parameter);

        public async void Execute(object parameter)
        {
            await ExecuteAsync(parameter);
        }

        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        protected void NotifyCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
