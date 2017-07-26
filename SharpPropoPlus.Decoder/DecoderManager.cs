using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
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

      //var result = CallAllComponents(125, 5, 10, 27, 45, 19, 10);
      //foreach (string s in result)
      //{
      //  Console.WriteLine(s);
      //}
    }

    //public List<string> CallAllComponents(params double[] args)
    //{
    //  var result = new List<string>();

    //  foreach (var com in Decoders)
    //  {
    //    Console.WriteLine(string.Join("\n", com.Value.Description) );
    //    Console.WriteLine(com.Metadata.Type);
    //    com.Value.ProcessPulse(192000, -6724);
    //  }

    //  return result;
    //}

    public void Dispose()
    {
      _catalog.Dispose();
      _container.Dispose();
    }
  }



}