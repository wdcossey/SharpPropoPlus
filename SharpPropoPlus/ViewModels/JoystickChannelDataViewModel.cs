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

        public JoystickChannel Channel
        {
            get => _channel;
            set
            {
                if (value == _channel)
                {
                    return;
                }

                _channel = value;
                OnPropertyChanged();
            }
        }

        public int Value
        {
            get => _value;
            set
            {
                if (value == _value)
                {
                    return;
                }

                _value = value;
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
    }
}