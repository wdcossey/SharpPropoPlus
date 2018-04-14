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
            if (Settings.Default.UpgradeRequired)
            {
                Settings.Default.Upgrade();
                Settings.Default.UpgradeRequired = false;
                Settings.Default.Save();
            }

            IsShellStateChecked = Settings.Default.Enabled;
            ShellStateCommand = new RelayCommand<bool>(CommandAction);
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
            get => _shellStateCommand;
            private set
            {
                _shellStateCommand = value;
                OnPropertyChanged();
            }
        }

        private bool _isShellStateChecked;

        public bool IsShellStateChecked
        {
            get => _isShellStateChecked;
            set
            {
                _isShellStateChecked = value;
                OnPropertyChanged();
            }
        }
    }
}