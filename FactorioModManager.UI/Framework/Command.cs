using System;
using System.Windows.Input;

namespace FactorioModManager.UI.Framework
{
    public class Command : ICommand
    {
        private Action<object> _execute;
        private Predicate<object> _canExecute;

        public event EventHandler CanExecuteChanged;

        public Command(Action<object> execute = null, Predicate<object> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public virtual void NotifyCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke(parameter) ?? true;
        }

        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public void Execute(object parameter)
        {
            _execute?.Invoke(parameter);
        }
    }
}
