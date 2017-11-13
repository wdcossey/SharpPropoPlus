using System;
using System.ComponentModel.Composition;
using SharpPropoPlus.Contracts.Interfaces;

namespace SharpPropoPlus.Filter.Contracts
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property), MetadataAttribute]
    public class ExportPropoPlusFilterAttribute : ExportAttribute, IFilterMetadata
    {
        public ExportPropoPlusFilterAttribute(string uniqueIdentifier, string name, string description)
            : base(typeof(IPropoPlusFilter))
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Export requires a name", nameof(name));

            UniqueIdentifier = uniqueIdentifier;
            Name = name;
            Description = description;
        }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public string UniqueIdentifier { get; private set; }
    }
}