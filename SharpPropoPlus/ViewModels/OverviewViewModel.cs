using System;
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

        public OverviewViewModel()
        {

            PrefferedChannel = AudioChannel.Left.ToString();

            GlobalEventAggregator.Instance.AddListener<PreferredChannelEventArgs>(PrefferedChannelListner);
            GlobalEventAggregator.Instance.AddListener<AudioEndPointEventArgs>(DeviceInfoListner);
            GlobalEventAggregator.Instance.AddListener<PollChannelsEventArgs>(PollChannelListner);
            GlobalEventAggregator.Instance.AddListener<JoystickChangedEventArgs>(JoystickChangedListener);
            GlobalEventAggregator.Instance.AddListener<DecoderChangedEventArgs>(DecoderChangedListener);

            DeviceName = AudioHelper.Instance.DeviceName;
            JoystickName = JoystickInteraction.Instance.CurrentDevice.Name;
        }

        private void DecoderChangedListener(DecoderChangedEventArgs args)
        {
            if (args == null)
                return;

            DecoderName = args.Name;
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

        private void DeviceInfoListner(AudioEndPointEventArgs args)
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
            GlobalEventAggregator.Instance.RemoveListener<AudioEndPointEventArgs>(DeviceInfoListner);
            GlobalEventAggregator.Instance.RemoveListener<PollChannelsEventArgs>(PollChannelListner);
            GlobalEventAggregator.Instance.RemoveListener<JoystickChangedEventArgs>(JoystickChangedListener);
            GlobalEventAggregator.Instance.RemoveListener<DecoderChangedEventArgs>(DecoderChangedListener);

            base.Dispose();
        }
    }
}