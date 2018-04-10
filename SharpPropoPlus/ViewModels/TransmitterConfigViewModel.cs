using System;
using System.Collections.ObjectModel;
using System.Linq;
using SharpPropoPlus.Contracts.Enums;
using SharpPropoPlus.Contracts.Interfaces;
using SharpPropoPlus.Decoder.Contracts;
using SharpPropoPlus.Decoder.EventArguments;
using SharpPropoPlus.Events;
using SharpPropoPlus.Interfaces;

namespace SharpPropoPlus.ViewModels
{
    public class TransmitterConfigViewModel : BaseViewModel, ITransmitterConfigViewModel
    {
        private ReadOnlyObservableCollection<Lazy<IPropoPlusDecoder, IDecoderMetadata>> _decoderCollection;
        private Lazy<IPropoPlusDecoder, IDecoderMetadata> _selectedDecoder;
        private int _rawChannels;
        private ObservableCollection<IChannelData> _rawChannelData;
        private ObservableCollection<IChannelData> _filteredChannelData;
        private bool _showSettings;
        private TransmitterType _transmitterType;

        public TransmitterConfigViewModel()
        {

            GlobalEventAggregator.Instance.AddListener<PollChannelsEventArgs>(PollChannelListner);
            GlobalEventAggregator.Instance.AddListener<ChannelsUpdateEventArgs>(ChannelsUpdateListener);

            DecoderCollection =
               new ReadOnlyObservableCollection<Lazy<IPropoPlusDecoder, IDecoderMetadata>>(new ObservableCollection<Lazy<IPropoPlusDecoder, IDecoderMetadata>>(Application.Instance.DecoderManager.Decoders.ToList()));

            SelectedDecoder = 
                DecoderCollection.FirstOrDefault(fd => fd.Value == Application.Instance.DecoderManager.Decoder);
        }

        private void PollChannelListner(PollChannelsEventArgs args)
        {
            if (args == null)
                return;

            RawChannels = args.RawChannels;
        }

        public ReadOnlyObservableCollection<Lazy<IPropoPlusDecoder, IDecoderMetadata>> DecoderCollection
        {
            get => _decoderCollection;
            private set
            {
                _decoderCollection = value;
                OnPropertyChanged();
            }
        }

        public Lazy<IPropoPlusDecoder, IDecoderMetadata> SelectedDecoder
        {
            get => _selectedDecoder;
            set
            {
                if (_selectedDecoder == value)
                {
                    return;
                }

                _selectedDecoder = value;

                Application.Instance.DecoderManager.ChangeDecoder(_selectedDecoder.Value);

                OnPropertyChanged();

                TransmitterType = _selectedDecoder.Metadata.TransmitterType;
                ShowSettings = _selectedDecoder.Metadata.TransmitterType == TransmitterType.Ppm && ShowSettings;
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

        private void ChannelsUpdateListener(ChannelsUpdateEventArgs args)
        {
            if (args == null)
                return;

            var rawChannelData = new int[16];
            Array.Copy(args.RawChannels, rawChannelData, Math.Min(Math.Min(args.RawChannels.Length, args.RawCount), rawChannelData.Length));

            RawChannelData = new ObservableCollection<IChannelData>(rawChannelData.Select(s => new ChannelDataViewModel("", s)));

            var filteredChannelData = new int[16];
            Array.Copy(args.FilterChannels, filteredChannelData, Math.Min(Math.Min(args.FilterChannels.Length, args.RawCount), filteredChannelData.Length));

            FilteredChannelData = new ObservableCollection<IChannelData>(filteredChannelData.Select(s => new ChannelDataViewModel("", s)));
        }

        public ObservableCollection<IChannelData> RawChannelData
        {
            get => _rawChannelData;
            set
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
            set
            {
                if (_filteredChannelData == value)
                    return;

                _filteredChannelData = value;

                OnPropertyChanged();
            }
        }

        public bool ShowSettings
        {
            get => _showSettings;
            set
            {
                if (_showSettings == value)
                {
                    return;
                }

                _showSettings = value;

                OnPropertyChanged();
            }
        }
        
        public TransmitterType TransmitterType
        {
            get => _transmitterType;
            set
            {
                if (_transmitterType == value)
                {
                    return;
                }

                _transmitterType = value;

                OnPropertyChanged();
            }
        }

        public override void Dispose()
        {
            GlobalEventAggregator.Instance.RemoveListener<PollChannelsEventArgs>(PollChannelListner);
            GlobalEventAggregator.Instance.RemoveListener<ChannelsUpdateEventArgs>(ChannelsUpdateListener);

            base.Dispose();
        }
    }
}