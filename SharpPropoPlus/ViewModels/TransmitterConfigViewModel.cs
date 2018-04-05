using System;
using System.Collections.ObjectModel;
using System.Linq;
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
            Array.Copy(args.RawChannels, rawChannelData, Math.Min(args.RawCount, rawChannelData.Length));

            RawChannelData = new ObservableCollection<IChannelData>(rawChannelData.Select(s => new ChannelDataViewModel("", s)));

            var filteredChannelData = new int[16];
            Array.Copy(args.FilterChannels, filteredChannelData, Math.Min(filteredChannelData.Length, args.RawCount));

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

        public override void Dispose()
        {
            GlobalEventAggregator.Instance.RemoveListener<PollChannelsEventArgs>(PollChannelListner);
            GlobalEventAggregator.Instance.RemoveListener<ChannelsUpdateEventArgs>(ChannelsUpdateListener);

            base.Dispose();
        }
    }
}