using System;
using System.Collections.ObjectModel;
using System.Linq;
using SharpPropoPlus.Audio;
using SharpPropoPlus.Audio.Enums;
using SharpPropoPlus.Audio.EventArguments;
using SharpPropoPlus.Audio.Models;
using SharpPropoPlus.Contracts.Enums;
using SharpPropoPlus.Contracts.EventArguments;
using SharpPropoPlus.Events;
using SharpPropoPlus.Interfaces;

namespace SharpPropoPlus.ViewModels
{
    public class AudioConfigViewModel : BaseViewModel, IAudioConfigViewModel
    {
        private ReadOnlyObservableCollection<AudioBitrate> _bitrateCollection;
        private ReadOnlyObservableCollection<AudioChannel> _channelCollection;
        private ReadOnlyObservableCollection<AudioEndPoint> _audioEndPointCollection;

        private AudioBitrate _selectedBitrateItem;
        private AudioChannel _selectedChannelItem;
        private AudioEndPoint _selectedAudioEndPoint;

        private int _leftChannelPeak;
        private int? _rightChannelPeak;
        private bool _muted;

        public AudioConfigViewModel()
        {

            BitrateCollection =
                new ReadOnlyObservableCollection<AudioBitrate>(new ObservableCollection<AudioBitrate>(Enum
                    .GetValues(typeof(AudioBitrate)).Cast<AudioBitrate>()));
            ChannelCollection =
                new ReadOnlyObservableCollection<AudioChannel>(new ObservableCollection<AudioChannel>(Enum
                    .GetValues(typeof(AudioChannel)).Cast<AudioChannel>()));

            AudioEndPointCollection =
                new ReadOnlyObservableCollection<AudioEndPoint>(new ObservableCollection<AudioEndPoint>(AudioHelper.Instance.Devices));

            SelectedAudioEndPoint =
                AudioEndPointCollection.FirstOrDefault(fd => fd.DeviceId == AudioHelper.Instance.Device.DeviceId);

            _selectedBitrateItem = AudioHelper.Instance.Bitrate;
            _selectedChannelItem = AudioHelper.Instance.Channel;

            GlobalEventAggregator.Instance.AddListener<PeakValueEventArgs>(PeakValueChangedListner);
            GlobalEventAggregator.Instance.AddListener<RecordingStateEventArgs>(RecordingStateListner);
        }

        private void RecordingStateListner(RecordingStateEventArgs args)
        {
            if (args.State == RecordingState.Stopped)
            {
                LeftChannelPeak = 0;
                if (RightChannelPeak.HasValue)
                {
                    RightChannelPeak = 0;
                }
            }
        }

        private void PeakValueChangedListner(PeakValueEventArgs args)
        {
            LeftChannelPeak = (int) Math.Ceiling(args.Values.Left * 100);
            RightChannelPeak = args.Values.Right.HasValue ? (int?) Math.Ceiling(args.Values.Right.Value * 100) : (int?)null;
            Muted = args.Values.Muted;
        }

        public ReadOnlyObservableCollection<AudioBitrate> BitrateCollection
        {
            get => _bitrateCollection;
            private set
            {
                _bitrateCollection = value;
                OnPropertyChanged();
            }
        }

        public AudioBitrate SelectedBitrateItem
        {
            get => _selectedBitrateItem;
            set
            {
                if (_selectedBitrateItem == value)
                {
                    return;
                }

                AudioHelper.Instance.SetBitrate(_selectedBitrateItem = value);

                OnPropertyChanged();
            }
        }

        public int LeftChannelPeak
        {
            get => _leftChannelPeak;
            private set
            {
                if (Equals(_leftChannelPeak, value))
                    return;

                _leftChannelPeak = value;
                OnPropertyChanged();
            }
        }

        public int? RightChannelPeak
        {
            get => _rightChannelPeak;
            private set
            {
                if (Equals(_rightChannelPeak, value))
                    return;

                _rightChannelPeak = value;
                OnPropertyChanged();
            }
        }

        public bool Muted
        {
            get => _muted;
            private set
            {
                if (Equals(_muted, value))
                    return;

                _muted = value;
                OnPropertyChanged();
            }
        }

        public ReadOnlyObservableCollection<AudioChannel> ChannelCollection
        {
            get => _channelCollection;
            private set
            {
                _channelCollection = value;
                OnPropertyChanged();
            }
        }

        public ReadOnlyObservableCollection<AudioEndPoint> AudioEndPointCollection
        {
            get { return _audioEndPointCollection; }
            private set
            {
                _audioEndPointCollection = value;
                OnPropertyChanged();
            }
        }

        public AudioChannel SelectedChannelItem
        {
            get => _selectedChannelItem;
            set
            {
                if (_selectedChannelItem == value)
                {
                    return;
                }

                AudioHelper.Instance.SetChannel(_selectedChannelItem = value);

                OnPropertyChanged();
            }
        }

        public AudioEndPoint SelectedAudioEndPoint
        {
            get => _selectedAudioEndPoint;
            set
            {
                if (_selectedAudioEndPoint == value)
                {
                    return;
                }

                _selectedAudioEndPoint = value;

                if (_selectedAudioEndPoint != null)
                {
                    if (Settings.Default.Enabled)
                    {
                        AudioHelper.Instance.StartRecording(_selectedAudioEndPoint);
                    }
                }
                else
                {
                    AudioHelper.Instance.StopRecording();
                }

                if (_selectedAudioEndPoint == null || _selectedAudioEndPoint.Channels == 1)
                {
                    SelectedChannelItem = AudioChannel.Automatic;
                }

                OnPropertyChanged();
            }
        }

        public override void Dispose()
        {
            GlobalEventAggregator.Instance.RemoveListener<PeakValueEventArgs>(PeakValueChangedListner);
            GlobalEventAggregator.Instance.RemoveListener<RecordingStateEventArgs>(RecordingStateListner);

            base.Dispose();
        }


    }

}