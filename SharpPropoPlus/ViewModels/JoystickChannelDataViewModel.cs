using SharpPropoPlus.Enums;
using SharpPropoPlus.Interfaces;

namespace SharpPropoPlus.ViewModels
{
    public class JoystickChannelDataViewModel : BaseViewModel, IJoystickChannelData
    {
        private int _value;
        private string _title;
        private string _description;
        private JoystickChannel _channel;
        private string _toolTip;

        public JoystickChannel Channel
        {
            get => _channel;
            private set
            {
                if (value == _channel && !string.IsNullOrWhiteSpace(ToolTip))
                {
                    return;
                }

                _channel = value;
                ToolTip = value.ToString();
                OnPropertyChanged();
            }
        }

        public int Value
        {
            get => _value;
            private set
            {
                if (value == _value)
                {
                    return;
                }

                _value = value;
                OnPropertyChanged();
            }
        }

        public string ToolTip
        {
            get => _toolTip;
            private set
            {
                _toolTip = value;
                OnPropertyChanged();
            }
        }

        public string Title
        {
            get => _title;
            set
            {
                if (value == _title)
                {
                    return;
                }

                _title = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                if (value == _description)
                {
                    return;
                }

                _description = value;
                OnPropertyChanged();
            }
        }

        public void SetValue(int value)
        {
            Value = value;
        }

        public JoystickChannelDataViewModel(JoystickChannel channel)
        {
            Channel = channel;
        }

        public JoystickChannelDataViewModel(JoystickChannel channel, int value)
        : this(channel)
        {
            Value = value;
        }
    }
}