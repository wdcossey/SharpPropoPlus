using SharpPropoPlus.Audio.Interfaces;

namespace SharpPropoPlus.ViewModels
{
    public class AudioEndPointViewModel : BaseViewModel, IAudioEndPoint
    {
        private string _deviceName;
        private string _deviceId;
        private int _channels;
        private bool _disabled;
        private int? _jackColor;

        public string DeviceName
        {
            get => _deviceName;
            set
            {
                _deviceName = value;
                OnPropertyChanged();
            }
        }

        public string DeviceId
        {
            get => _deviceId;
            set
            {
                _deviceId = value;
                OnPropertyChanged();
            }
        }

        public int Channels
        {
            get => _channels;
            set
            {
                _channels = value;
                OnPropertyChanged();
            }
        }

        public bool Disabled
        {
            get => _disabled;
            set
            {
                _disabled = value;
                OnPropertyChanged();
            }
        }

        public int? JackColor
        {
            get => _jackColor;
            set
            {
                _jackColor = value;
                OnPropertyChanged();
            }
        }
    }
}