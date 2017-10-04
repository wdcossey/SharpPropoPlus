using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using SharpPropoPlus.Decoder.Contracts;
using SharpPropoPlus.Decoder.EventArguments;
using SharpPropoPlus.Events;

namespace SharpPropoPlus.Decoder
{


    public class DecoderManager : IDecoderManager
    {
        [ImportMany]
        private IEnumerable<Lazy<IPropoPlusDecoder, IDecoderMetadata>> _decoders;

        private readonly AggregateCatalog _catalog;
        private readonly CompositionContainer _container;
        private IPropoPlusDecoder _decoder;

        public IEnumerable<Lazy<IPropoPlusDecoder, IDecoderMetadata>> Decoders => _decoders;

        public DecoderManager()
        {
            GlobalEventAggregator.Instance.AddListener<DecoderChangedEventArgs>(DecoderChangedListener);

            //An aggregate catalog that combines multiple catalogs
            _catalog = new AggregateCatalog();

            //Add all the parts found in all assemblies in
            //the same directory as the executing program
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            _catalog.Catalogs.Add(new DirectoryCatalog(path));

            //Create the CompositionContainer with the parts in the catalog.
            _container = new CompositionContainer(_catalog);

            //Fill the imports of this object
            _container.ComposeParts(this);

            Decoder = GetDefaultDecoder();
        }

        private void DecoderChangedListener(DecoderChangedEventArgs args)
        {
            ChangeDecoder(args.Decoder);
        }

        private IPropoPlusDecoder GetDefaultDecoder()
        {
            return Decoders.First()?.Value;
        }
        
        public void ChangeDecoder(IPropoPlusDecoder decoder)
        {
            Decoder = decoder;
        }
        
        public void Notify()
        {
            var decoder = Decoders.FirstOrDefault(fd => fd.Value == Decoder);

            if (decoder != null)
            {
                GlobalEventAggregator.Instance.SendMessage(new DecoderChangedEventArgs(decoder.Metadata.Name,
                    decoder.Metadata.Description, decoder.Metadata.TransmitterType, decoder.Value));
            }
        }
        
        public IPropoPlusDecoder Decoder
        {
            get => _decoder;
            private set
            {
                if (value == null || _decoder == value)
                {
                    return;
                }

                _decoder = value;
            }
        }

        public void Dispose()
        {
            _catalog.Dispose();
            _container.Dispose();
        }
    }
}