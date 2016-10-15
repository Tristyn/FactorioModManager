using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Nito.AsyncEx;

namespace FactorioModManager.UI.Framework
{
    class AsyncCommand<TResult> : AsyncCommandBase, INotifyPropertyChanged
    {
        private readonly Func<CancellationToken, Task<TResult>> _command;
        private readonly CancelAsyncCommand _cancelCommand;
        private readonly Func<bool> _canExecute;
        private INotifyTaskCompletion<TResult> _execution;

        public event PropertyChangedEventHandler PropertyChanged;

        public INotifyTaskCompletion<TResult> Execution
        {
            get { return _execution; }
            set
            {
                if (_execution == value)
                    return;

                _execution = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Execution"));
            }
        }

        public ICommand CancelCommand => _cancelCommand;

        /// <summary>
        /// When false, CanExecute returns false if the command is already executing.
        /// </summary>
        public bool EnabledDuringExecution { get; set; }

        /// <summary>
        /// When true, runs the command on the current threading context.
        /// In the case of WPF, it will run on the UI thread while it is not waiting on async IO.
        /// When false, the command will run in the background on the thread pool.
        /// </summary>
        public bool ExecuteOnCapturedContext { get; set; }

        public AsyncCommand(Func<CancellationToken, Task<TResult>> command = null, Func<bool> canExecute = null)
        {
            _command = command;
            _canExecute = canExecute;
            _cancelCommand = new CancelAsyncCommand();
        }

        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public override bool CanExecute(object parameter)
        {
            if (!EnabledDuringExecution && Execution != null && Execution.IsNotCompleted)
                return false;

            return _canExecute == null || _canExecute();
        }

        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public override async Task ExecuteAsync(object parameter)
        {
            _cancelCommand.NotifyCommandStarting();
            var executeTask = ExecuteOnCapturedContext
                ? _command(_cancelCommand.Token) 
                : Task.Run(() => _command(_cancelCommand.Token));
            Execution = NotifyTaskCompletion.Create(executeTask);
            NotifyCanExecuteChanged();
            
            // Don't propagate exceptions to the UI main loop.
            // Instead, return TaskCompletion and handle exceptions by data binding to Execution.
            await Execution.TaskCompleted;
            _cancelCommand.NotifyCommandFinished();
            NotifyCanExecuteChanged();
        }

        private sealed class CancelAsyncCommand : ICommand
        {
            private CancellationTokenSource _cts = new CancellationTokenSource();

            private bool _commandExecuting;

            public event EventHandler CanExecuteChanged;

            public CancellationToken Token { get { return _cts.Token; } }

            public void NotifyCommandStarting()
            {
                _commandExecuting = true;
                if (!_cts.IsCancellationRequested)
                    return;
                _cts = new CancellationTokenSource();
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }

            public void NotifyCommandFinished()
            {
                _commandExecuting = false;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }

            bool ICommand.CanExecute(object parameter)
            {
                return _commandExecuting && !_cts.IsCancellationRequested;
            }

            void ICommand.Execute(object parameter)
            {
                _cts.Cancel();
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
