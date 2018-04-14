using System;
using System.Collections.ObjectModel;
using System.Linq;
using SharpPropoPlus.Audio;
using SharpPropoPlus.Audio.Enums;
using SharpPropoPlus.Audio.EventArguments;
using SharpPropoPlus.Audio.Interfaces;
using SharpPropoPlus.Contracts.Enums;
using SharpPropoPlus.Contracts.EventArguments;
using SharpPropoPlus.Decoder;
using SharpPropoPlus.Events;
using SharpPropoPlus.Decoder.EventArguments;
using SharpPropoPlus.Enums;
using SharpPropoPlus.Filter.EventArguments;
using SharpPropoPlus.Interfaces;
using SharpPropoPlus.vJoyMonitor.EventArguments;

namespace SharpPropoPlus.ViewModels
{
    public class OverviewViewModel : BaseViewModel
    {
        private string _prefferedChannel = string.Empty;
        private IAudioEndPoint _device = null;
        private int _rawChannels;
        private string _joystickName = string.Empty;
        private string _decoderName;
        private string _decoderDescription;
        private string _filterName;
        private string _filterDescription;
        private bool _isFilterEnabled;
        private ObservableCollection<IJoystickChannelData> _channelData;
        private ObservableCollection<IChannelData> _rawChannelData;
        private ObservableCollection<IChannelData> _filteredChannelData;

        public OverviewViewModel()
        {

            PrefferedChannel = AudioChannel.Left.ToString();

            GlobalEventAggregator.Instance.AddListener<PreferredChannelEventArgs>(PrefferedChannelListner);
            GlobalEventAggregator.Instance.AddListener<AudioEndPointEventArgs>(AudioEndPointListner);
            GlobalEventAggregator.Instance.AddListener<PollChannelsEventArgs>(PollChannelListner);
            GlobalEventAggregator.Instance.AddListener<JoystickChangedEventArgs>(JoystickChangedListener);
            GlobalEventAggregator.Instance.AddListener<DecoderChangedEventArgs>(DecoderChangedListener);
            GlobalEventAggregator.Instance.AddListener<JoystickUpdateEventArgs>(JoystickUpdateListener);
            GlobalEventAggregator.Instance.AddListener<ChannelsUpdateEventArgs>(ChannelsUpdateListener);
            GlobalEventAggregator.Instance.AddListener<FilterChangedEventArgs>(FilterChangedListener);
            GlobalEventAggregator.Instance.AddListener<RecordingStateEventArgs>(RecordingStateListner);

            var currentDecoder = Application.Instance.DecoderManager.GetDecoderMetadata(Application.Instance.DecoderManager.Decoder);

            DecoderName = $"{currentDecoder.TransmitterType.ToString().ToUpperInvariant()} - {currentDecoder.Name}";
            DecoderDescription = currentDecoder.Description;

            var currentFilter = Application.Instance.FilterManager.GetFilterMetadata(Application.Instance.FilterManager.Filter);

            FilterName = currentFilter.Name;
            FilterDescription = currentFilter.Description;
            IsFilterEnabled = Application.Instance.FilterManager.IsEnabled;

            Device = AudioHelper.Instance.Device;
            JoystickName = JoystickInteraction.Instance.CurrentDevice.Name;

            ChannelData = new ObservableCollection<IJoystickChannelData>
            {
                new JoystickChannelDataViewModel(JoystickChannel.JoystickAxisX, 0),
                new JoystickChannelDataViewModel(JoystickChannel.JoystickAxisY, 0),
                new JoystickChannelDataViewModel(JoystickChannel.JoystickAxisZ, 0),
                new JoystickChannelDataViewModel(JoystickChannel.JoystickRotationX, 0),
                new JoystickChannelDataViewModel(JoystickChannel.JoystickRotationY, 0),
                new JoystickChannelDataViewModel(JoystickChannel.JoystickRotationZ, 0),
                new JoystickChannelDataViewModel(JoystickChannel.JoystickSlider0, 0),
                new JoystickChannelDataViewModel(JoystickChannel.JoystickSlider1, 0),
            };

            const string strRawChannel = "Transmitter Channel";
            const string strFilterChannel = "Filter Channel";

            RawChannelData = new ObservableCollection<IChannelData>();
            FilteredChannelData = new ObservableCollection<IChannelData>();
            for (var i = 1; i < 9; i++)
            {
                RawChannelData.Add(new ChannelDataViewModel($"{strRawChannel} {i}", 0));
                FilteredChannelData.Add(new ChannelDataViewModel($"{strFilterChannel} {((char)(64+i))}", 0));
            }
        }

        private void RecordingStateListner(RecordingStateEventArgs args)
        {
            switch (args.State)
            {
                case RecordingState.Started:
                    break;
                case RecordingState.Stopped:

                    for (var i = 0; i < ChannelData.Count; i++)
                    {
                        RawChannelData[i].SetValue(0);
                        FilteredChannelData[i].SetValue(0);
                    }

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

        private void ChannelsUpdateListener(ChannelsUpdateEventArgs args)
        {
            if (args == null)
                return;

            //var rawChannelData = new int[8];
            //Array.Copy(args.RawChannels, rawChannelData, Math.Min(args.RawCount, rawChannelData.Length));
            //RawChannelData = new ObservableCollection<IChannelData>(rawChannelData.Select(s => new ChannelDataViewModel("", s)));

            if (RawChannelData != null)
            {
                for (var i = 0; i < RawChannelData.Count; i++)
                {
                    RawChannelData[i].SetValue(args.RawChannels[i]);
                }
            }

            //var filteredChannelData = new int[8];
            //Array.Copy(args.FilterChannels, filteredChannelData, Math.Min(args.RawCount, filteredChannelData.Length));
            //FilteredChannelData = new ObservableCollection<IChannelData>(filteredChannelData.Select(s => new ChannelDataViewModel("", s)));

            if (FilteredChannelData != null)
            {
                for (var i = 0; i < FilteredChannelData.Count; i++)
                {
                    FilteredChannelData[i].SetValue(args.FilterChannels[i]);
                }
            }

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

        public ObservableCollection<IChannelData> RawChannelData
        {
            get => _rawChannelData;

            private set
            {
                if (_rawChannelData == value)
                    return;

                _rawChannelData = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<IChannelData> FilteredChannelData
        {
            get => _filteredChannelData;

            private set
            {
                if (_filteredChannelData == value)
                    return;

                _filteredChannelData = value;
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

            Device = args;
        }

        private void PrefferedChannelListner(PreferredChannelEventArgs args)
        {
            if (args == null)
                return;

            PrefferedChannel = args.Channel.ToString();
        }

        public IAudioEndPoint Device
        {
            get => _device;

            private set
            {
                if (_device == value)
                    return;

                _device = value;
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
            GlobalEventAggregator.Instance.RemoveListener<ChannelsUpdateEventArgs>(ChannelsUpdateListener);
            GlobalEventAggregator.Instance.RemoveListener<FilterChangedEventArgs>(FilterChangedListener);
            GlobalEventAggregator.Instance.RemoveListener<RecordingStateEventArgs>(RecordingStateListner); ;

            base.Dispose();
        }
    }
}