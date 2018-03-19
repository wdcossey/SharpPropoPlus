using System;
using System.Collections.ObjectModel;
using System.Linq;
using SharpPropoPlus.Audio;
using SharpPropoPlus.Audio.Enums;
using SharpPropoPlus.Audio.EventArguments;
using SharpPropoPlus.Contracts.Enums;
using SharpPropoPlus.Contracts.EventArguments;
using SharpPropoPlus.Decoder;
using SharpPropoPlus.Events;
using SharpPropoPlus.Decoder.EventArguments;
using SharpPropoPlus.Enums;
using SharpPropoPlus.Filter.EventArguments;
using SharpPropoPlus.Interfaces;
using SharpPropoPlus.vJoyMonitor;
using SharpPropoPlus.vJoyMonitor.EventArguments;

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
        private string _filterName;
        private string _filterDescription;
        private bool _isFilterEnabled;
        private ObservableCollection<IJoystickChannelData> _channelData;
        
        public OverviewViewModel()
        {

            PrefferedChannel = AudioChannel.Left.ToString();

            GlobalEventAggregator.Instance.AddListener<PreferredChannelEventArgs>(PrefferedChannelListner);
            GlobalEventAggregator.Instance.AddListener<AudioEndPointEventArgs>(AudioEndPointListner);
            GlobalEventAggregator.Instance.AddListener<PollChannelsEventArgs>(PollChannelListner);
            GlobalEventAggregator.Instance.AddListener<JoystickChangedEventArgs>(JoystickChangedListener);
            GlobalEventAggregator.Instance.AddListener<DecoderChangedEventArgs>(DecoderChangedListener);
            GlobalEventAggregator.Instance.AddListener<JoystickUpdateEventArgs>(JoystickUpdateListener);
            GlobalEventAggregator.Instance.AddListener<FilterChangedEventArgs>(FilterChangedListener);
            GlobalEventAggregator.Instance.AddListener<RecordingStateEventArgs>(RecordingStateListner);

            var currentDecoder = Application.Instance.DecoderManager.GetDecoderMetadata(Application.Instance.DecoderManager.Decoder);

            DecoderName = $"{currentDecoder.TransmitterType.ToString().ToUpperInvariant()} - {currentDecoder.Name}";
            DecoderDescription = currentDecoder.Description;

            var currentFilter = Application.Instance.FilterManager.GetFilterMetadata(Application.Instance.FilterManager.Filter);

            FilterName = currentFilter.Name;
            FilterDescription = currentFilter.Description;
            IsFilterEnabled = Application.Instance.FilterManager.IsEnabled;

            DeviceName = AudioHelper.Instance.DeviceName;
            JoystickName = JoystickInteraction.Instance.CurrentDevice.Name;

            ChannelData = new ObservableCollection<IJoystickChannelData>()
            {
                new JoystickChannelDataViewModel(JoystickChannel.JoystickAxisX) { Value = 0 },
                new JoystickChannelDataViewModel(JoystickChannel.JoystickAxisY) { Value = 0 },
                new JoystickChannelDataViewModel(JoystickChannel.JoystickAxisZ) { Value = 0 },
                new JoystickChannelDataViewModel(JoystickChannel.JoystickRotationX) { Value = 0 },
                new JoystickChannelDataViewModel(JoystickChannel.JoystickRotationY) { Value = 0 },
                new JoystickChannelDataViewModel(JoystickChannel.JoystickRotationZ) { Value = 0 },
                new JoystickChannelDataViewModel(JoystickChannel.JoystickSlider0) { Value = 0 },
                new JoystickChannelDataViewModel(JoystickChannel.JoystickSlider1) { Value = 0 },
            };

        }

        private void RecordingStateListner(RecordingStateEventArgs args)
        {
            switch (args.State)
            {
                case RecordingState.Started:
                    break;
                case RecordingState.Stopped:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        private void FilterChangedListener(FilterChangedEventArgs args)
        {
            if (args == null)
                return;

            IsFilterEnabled = args.IsEnabled;
            FilterName = args.Name;
            FilterDescription = args.Description;
        }

        private void JoystickUpdateListener(JoystickUpdateEventArgs args)
        {
            if (args == null)
                return;

            ChannelData?.FirstOrDefault(fd => fd.Channel == JoystickChannel.JoystickAxisX)?.SetValue(args.AxisX);
            ChannelData?.FirstOrDefault(fd => fd.Channel == JoystickChannel.JoystickAxisY)?.SetValue(args.AxisY);
            ChannelData?.FirstOrDefault(fd => fd.Channel == JoystickChannel.JoystickAxisZ)?.SetValue(args.AxisZ);
            ChannelData?.FirstOrDefault(fd => fd.Channel == JoystickChannel.JoystickRotationX)?.SetValue(args.RotationX);
            ChannelData?.FirstOrDefault(fd => fd.Channel == JoystickChannel.JoystickRotationY)?.SetValue(args.RotationY);
            ChannelData?.FirstOrDefault(fd => fd.Channel == JoystickChannel.JoystickRotationZ)?.SetValue(args.RotationZ);
            ChannelData?.FirstOrDefault(fd => fd.Channel == JoystickChannel.JoystickSlider0)?.SetValue(args.Slider0);
            ChannelData?.FirstOrDefault(fd => fd.Channel == JoystickChannel.JoystickSlider1)?.SetValue(args.Slider1);

        }

        private void DecoderChangedListener(DecoderChangedEventArgs args)
        {
            if (args == null)
                return;

            DecoderName = $"{args.TransmitterType.ToString().ToUpperInvariant()} - {args.Name}";
            DecoderDescription = args.Description;
        }

        public ObservableCollection<IJoystickChannelData> ChannelData
        {
            get => _channelData;

            private set
            {
                if (_channelData == value)
                    return;

                _channelData = value;
                OnPropertyChanged();
            }
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

        public string FilterName
        {
            get => _filterName;

            private set
            {
                if (_filterName == value)
                    return;

                _filterName = value;
                OnPropertyChanged();
            }
        }

        public string FilterDescription
        {
            get => _filterDescription;

            private set
            {
                if (_filterDescription == value)
                    return;

                _filterDescription = value;
                OnPropertyChanged();
            }
        }

        public bool IsFilterEnabled
        {
            get => _isFilterEnabled;

            private set
            {
                if (_isFilterEnabled == value)
                    return;

                _isFilterEnabled = value;
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
            GlobalEventAggregator.Instance.RemoveListener<JoystickUpdateEventArgs>(JoystickUpdateListener);
            GlobalEventAggregator.Instance.RemoveListener<FilterChangedEventArgs>(FilterChangedListener);
            GlobalEventAggregator.Instance.RemoveListener<RecordingStateEventArgs>(RecordingStateListner); ;

            base.Dispose();
        }
    }
}