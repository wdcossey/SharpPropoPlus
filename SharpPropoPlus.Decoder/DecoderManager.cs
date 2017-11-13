using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using SharpPropoPlus.Contracts.Interfaces;
using SharpPropoPlus.Decoder.Contracts;
using SharpPropoPlus.Decoder.EventArguments;
using SharpPropoPlus.Events;

namespace SharpPropoPlus.Decoder
{


    public class DecoderManager : IDecoderManager
    {
        [ImportMany]
#pragma warning disable 649
        private IEnumerable<Lazy<IPropoPlusDecoder, IDecoderMetadata>> _decoders;
#pragma warning restore 649

        private readonly AggregateCatalog _catalog;
        private readonly CompositionContainer _container;
        private IPropoPlusDecoder _decoder;

        public IEnumerable<Lazy<IPropoPlusDecoder, IDecoderMetadata>> Decoders => _decoders;

        public DecoderManager()
        {
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

        private IPropoPlusDecoder GetDefaultDecoder()
        {
            if (!string.IsNullOrWhiteSpace(Settings.Default.Decoder))
            {
                var decoder = GetDecoder(Settings.Default.Decoder);

                if (decoder != null)
                    return decoder.Value;
            }

            return Decoders.First()?.Value;
        }

        public void ChangeDecoder(IPropoPlusDecoder decoder)
        {
            Decoder = decoder;

            Settings.Default.Decoder = Decoders.First(f => f.Value == decoder)?.Metadata?.UniqueIdentifier;
            Settings.Default.Save();
        }

        public void Notify()
        {
            var decoder = GetDecoder();

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

                value.Reset();

                _decoder = value;

                var lazyDecoder = GetDecoder(_decoder);

                var message = new DecoderChangedEventArgs(lazyDecoder.Metadata.Name, lazyDecoder.Metadata.Description, lazyDecoder.Metadata.TransmitterType, lazyDecoder.Value);
                GlobalEventAggregator.Instance.SendMessage(message);
            }
        }

        private Lazy<IPropoPlusDecoder, IDecoderMetadata> GetDecoder(IPropoPlusDecoder decoder = null)
        {
            return Decoders.FirstOrDefault(fd => fd.Value == (decoder ?? Decoder));
        }

        private Lazy<IPropoPlusDecoder, IDecoderMetadata> GetDecoder(string uniqueIdentifier)
        {
            return Decoders.Single(s => s.Metadata.UniqueIdentifier == uniqueIdentifier);
        }

        public IDecoderMetadata GetDecoderMetadata(IPropoPlusDecoder decoder)
        {
            return GetDecoder()?.Metadata;
        }

        public void Dispose()
        {
            _catalog.Dispose();
            _container.Dispose();
        }
    }
}