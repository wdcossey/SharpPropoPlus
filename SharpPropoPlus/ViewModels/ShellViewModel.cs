using System;
using System.Windows.Input;
using SharpPropoPlus.Commands;
using SharpPropoPlus.Contracts.EventArguments;
using SharpPropoPlus.Events;
using SharpPropoPlus.Interfaces;

namespace SharpPropoPlus.ViewModels
{
    public class ShellViewModel : BaseViewModel, IShellViewModel
    {
        public ShellViewModel()
        {
            _isShellStateChecked = Settings.Default.Enabled;
            _shellStateCommand = new RelayCommand<bool>(CommandAction);
        }

        private void CommandAction(bool enabled)
        {

            GlobalEventAggregator.Instance.SendMessage(new SleepStateEventArgs(enabled));

            Settings.Default.Enabled = enabled;
            Settings.Default.Save();
        }

        private ICommand _shellStateCommand;

        public ICommand ShellStateCommand
        {
            get { return _shellStateCommand; }
            private set { _shellStateCommand = value; }
        }

        private bool _isShellStateChecked;

        public bool IsShellStateChecked
        {
            get { return _isShellStateChecked; }
            set { _isShellStateChecked = value; }
        }
    }
}