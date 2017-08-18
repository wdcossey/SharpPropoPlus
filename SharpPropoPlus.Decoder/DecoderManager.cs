using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Reflection;
using SharpPropoPlus.Decoder.Contracts;

namespace SharpPropoPlus.Decoder
{


    public class DecoderManager : IDecoderManager
    {
        [ImportMany]
        private IEnumerable<Lazy<IPropoPlusDecoder, IDecoderMetadata>> _decoders;

        private readonly AggregateCatalog _catalog;
        private readonly CompositionContainer _container;

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
        }

        public void Dispose()
        {
            _catalog.Dispose();
            _container.Dispose();
        }
    }
}