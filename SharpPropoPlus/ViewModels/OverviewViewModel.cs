using SharpPropoPlus.Audio;
using SharpPropoPlus.Audio.Enums;
using SharpPropoPlus.Audio.EventArguments;
using SharpPropoPlus.Decoder;
using SharpPropoPlus.Events;
using SharpPropoPlus.Decoder.EventArguments;

namespace SharpPropoPlus.ViewModels
{
    public class OverviewViewModel : BaseViewModel
    {
        private string _prefferedChannel = string.Empty;
        private string _deviceName = string.Empty;
        private int _rawChannels;
        private string _joystickName = string.Empty;
        private string _decoderName;
        private string _decoderDescription;

        public OverviewViewModel()
        {

            PrefferedChannel = AudioChannel.Left.ToString();

            GlobalEventAggregator.Instance.AddListener<PreferredChannelEventArgs>(PrefferedChannelListner);
            GlobalEventAggregator.Instance.AddListener<AudioEndPointEventArgs>(AudioEndPointListner);
            GlobalEventAggregator.Instance.AddListener<PollChannelsEventArgs>(PollChannelListner);
            GlobalEventAggregator.Instance.AddListener<JoystickChangedEventArgs>(JoystickChangedListener);
            GlobalEventAggregator.Instance.AddListener<DecoderChangedEventArgs>(DecoderChangedListener);

            Application.Instance.DecoderManager.Notify();
            
            DeviceName = AudioHelper.Instance.DeviceName;
            JoystickName = JoystickInteraction.Instance.CurrentDevice.Name;
            
        }

        private void DecoderChangedListener(DecoderChangedEventArgs args)
        {
            if (args == null)
                return;

            DecoderName = $"{args.TransmitterType.ToString().ToUpperInvariant()} - {args.Name}";
            DecoderDescription = args.Description;
        }

        public string DecoderName
        {
            get => _decoderName;

            private set
            {
                if (_decoderName == value)
                    return;

                _decoderName = value;
                OnPropertyChanged();
            }
        }

        public string DecoderDescription
        {
            get => _decoderDescription;

            private set
            {
                if (_decoderDescription == value)
                    return;

                _decoderDescription = value;
                OnPropertyChanged();
            }
        }

        private void JoystickChangedListener(JoystickChangedEventArgs args)
        {
            if (args == null)
                return;

            JoystickName = args.Name;
        }

        private void PollChannelListner(PollChannelsEventArgs args)
        {
            if (args == null)
                return;

            RawChannels = args.RawChannels;
        }

        private void AudioEndPointListner(AudioEndPointEventArgs args)
        {
            if (args == null)
                return;

            DeviceName = args.DeviceName;
        }

        private void PrefferedChannelListner(PreferredChannelEventArgs args)
        {
            if (args == null)
                return;

            PrefferedChannel = args.Channel.ToString();
        }

        public string DeviceName
        {
            get => _deviceName;

            private set
            {
                if (_deviceName == value)
                    return;

                _deviceName = value;
                OnPropertyChanged();
            }
        }

        public string PrefferedChannel
        {
            get => _prefferedChannel;

            private set
            {
                if (_prefferedChannel == value)
                    return;

                _prefferedChannel = value;
                OnPropertyChanged();
            }
        }

        public int RawChannels
        {
            get => _rawChannels;

            private set
            {
                if (_rawChannels == value)
                    return;

                _rawChannels = value;
                OnPropertyChanged();
            }
        }

        public string JoystickName
        {
            get => _joystickName;

            private set
            {
                if (_joystickName == value)
                    return;

                _joystickName = value;
                OnPropertyChanged();
            }
        }

        public override void Dispose()
        {
            GlobalEventAggregator.Instance.RemoveListener<PreferredChannelEventArgs>(PrefferedChannelListner);
            GlobalEventAggregator.Instance.RemoveListener<AudioEndPointEventArgs>(AudioEndPointListner);
            GlobalEventAggregator.Instance.RemoveListener<PollChannelsEventArgs>(PollChannelListner);
            GlobalEventAggregator.Instance.RemoveListener<JoystickChangedEventArgs>(JoystickChangedListener);
            GlobalEventAggregator.Instance.RemoveListener<DecoderChangedEventArgs>(DecoderChangedListener);

            base.Dispose();
        }
    }
}