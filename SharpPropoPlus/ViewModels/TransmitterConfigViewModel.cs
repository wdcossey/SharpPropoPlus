using System;
using System.Collections.ObjectModel;
using System.Linq;
using SharpPropoPlus.Audio.Enums;
using SharpPropoPlus.Decoder;
using SharpPropoPlus.Decoder.Contracts;
using SharpPropoPlus.Interfaces;

namespace SharpPropoPlus.ViewModels
{
    public class TransmitterConfigViewModel : BaseViewModel, ITransmitterConfigViewModel
    {
        private ReadOnlyObservableCollection<Lazy<IPropoPlusDecoder, IDecoderMetadata>> _decoderCollection;
        private Lazy<IPropoPlusDecoder, IDecoderMetadata> _selectedDecoder;

        public TransmitterConfigViewModel()
        {
            DecoderCollection =
                new ReadOnlyObservableCollection<Lazy<IPropoPlusDecoder, IDecoderMetadata>>(new ObservableCollection<Lazy<IPropoPlusDecoder, IDecoderMetadata>>(Application.Instance.DecoderManager.Decoders.ToList()));
        }

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
                _selectedDecoder = value;
                OnPropertyChanged();
            }
        }
    }
}