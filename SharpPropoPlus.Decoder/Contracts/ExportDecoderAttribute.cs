using System;
using System.ComponentModel.Composition;
using SharpPropoPlus.Contracts.Interfaces;
using SharpPropoPlus.Contracts.Types;
namespace SharpPropoPlus.Decoder.Contracts
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property), MetadataAttribute]
    public class ExportPropoPlusDecoderAttribute : ExportAttribute, IDecoderMetadata
    {
        public ExportPropoPlusDecoderAttribute(string uniqueIdentifier, string name, string description, TransmitterType type)
            : base(typeof(IPropoPlusDecoder))
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Export requires a name", nameof(name));

            UniqueIdentifier = uniqueIdentifier;
            Name = name;
            TransmitterType = type;
            Description = description;
        }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public string UniqueIdentifier { get; private set; }

        public TransmitterType TransmitterType { get; private set; }
    }
}