using System;
using System.ComponentModel.Composition;
using SharpPropoPlus.Decoder.Enums;

namespace SharpPropoPlus.Decoder.Contracts
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property), MetadataAttribute]
  public class ExportPropoPlusDecoderAttribute : ExportAttribute, IDecoderMetadata
  {
    public ExportPropoPlusDecoderAttribute(string name, string description, TransmitterType type)
      : base(typeof(IPropoPlusDecoder))
    {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentException("Export requires a name", nameof(name));

      Name = name;
      Type = type;
      Description = description;
    }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public TransmitterType Type { get; private set; }
  }
}