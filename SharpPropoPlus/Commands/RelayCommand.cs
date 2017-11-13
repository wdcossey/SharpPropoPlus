using System;
using System.Windows.Input;

namespace SharpPropoPlus.Commands
{
    public class RelayCommand : ICommand
    {
        private readonly Action _command;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action commandAction, Func<bool> canExecute = null)
        {
            this._command = commandAction;
            this._canExecute = canExecute;
        }

        /// <summary>
        /// Returns default true. 
        /// Customize to implement can execute logic.
        /// </summary>
        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke() ?? true;
        }

        /// <summary>
        /// Implement changed logic if needed
        /// </summary>
        public event EventHandler CanExecuteChanged;


        public void Execute(object parameter)
        {
            _command?.Invoke();
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _command;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action<T> commandAction, Func<bool> canExecute = null)
        {
            this._command = commandAction;
            this._canExecute = canExecute;
        }

        /// <summary>
        /// Returns default true. 
        /// Customize to implement can execute logic.
        /// </summary>
        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke() ?? true;
        }

        /// <summary>
        /// Implement changed logic if needed
        /// </summary>
        public event EventHandler CanExecuteChanged;


        public void Execute(object parameter)
        {
            if (parameter != null)
            {
                _command?.Invoke((T) parameter);
            }
        }
    }
}