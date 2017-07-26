using System;
using System.Windows.Input;

namespace SharpPropoPlus.Commands
{
    public class RelayCommand : ICommand
    {
        private Action<object> command;
        private Func<bool> canExecute;

        public RelayCommand(Action<object> commandAction, Func<bool> canExecute = null)
        {
            this.command = commandAction;
            this.canExecute = canExecute;
        }

        /// <summary>
        /// Returns default true. 
        /// Customize to implement can execute logic.
        /// </summary>
        public bool CanExecute(object parameter)
        {
            return this.canExecute == null ? true : this.canExecute();
        }

        /// <summary>
        /// Implement changed logic if needed
        /// </summary>
        public event EventHandler CanExecuteChanged;


        public void Execute(object parameter)
        {
            if (this.command != null)
            {
                this.command(parameter);
            }
        }
    }
}