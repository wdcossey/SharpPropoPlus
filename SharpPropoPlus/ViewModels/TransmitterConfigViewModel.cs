using System;
using System.Collections.ObjectModel;
using System.Linq;
using SharpPropoPlus.Decoder.Contracts;
using SharpPropoPlus.Decoder.EventArguments;
using SharpPropoPlus.Events;
using SharpPropoPlus.Interfaces;

namespace SharpPropoPlus.ViewModels
{
    public class TransmitterConfigViewModel : BaseViewModel, ITransmitterConfigViewModel
    {
        private ReadOnlyObservableCollection<Lazy<IPropoPlusDecoder, IDecoderMetadata>> _decoderCollection;
        //private ListCollectionView _decoderCollection;
        private Lazy<IPropoPlusDecoder, IDecoderMetadata> _selectedDecoder;

        public TransmitterConfigViewModel()
        {
            //ListCollectionView lcv = new ListCollectionView(Application.Instance.DecoderManager.Decoders.ToList());

            //lcv.GroupDescriptions?.Add(new PropertyGroupDescription($"{nameof(Lazy<IPropoPlusDecoder, IDecoderMetadata>.Metadata)}.{nameof(IDecoderMetadata.TransmitterType)}"));

            //DecoderCollection = lcv;

            DecoderCollection =
               new ReadOnlyObservableCollection<Lazy<IPropoPlusDecoder, IDecoderMetadata>>(new ObservableCollection<Lazy<IPropoPlusDecoder, IDecoderMetadata>>(Application.Instance.DecoderManager.Decoders.ToList()));
        }

        //public ListCollectionView DecoderCollection
        //{
        //    get => _decoderCollection;

        //    private set
        //    {
        //        _decoderCollection = value;
        //        OnPropertyChanged();
        //    }
        //}

        public ReadOnlyObservableCollection<Lazy<IPropoPlusDecoder, IDecoderMetadata>> DecoderCollection
        {
            get { return _decoderCollection; }
            private set
            {
                _decoderCollection = value;
                OnPropertyChanged();
            }
        }

        public Lazy<IPropoPlusDecoder, IDecoderMetadata> SelectedDecoder
        {
            get { return _selectedDecoder; }
            set
            {
                if (_selectedDecoder == value)
                    return;

                _selectedDecoder = value;

                var eventArgs = new DecoderChangedEventArgs(_selectedDecoder.Metadata.Name, _selectedDecoder.Metadata.Description, _selectedDecoder.Metadata.TransmitterType);

                //Publish the message 
                GlobalEventAggregator.Instance.SendMessage(eventArgs);

                OnPropertyChanged();
            }
        }
    }
}