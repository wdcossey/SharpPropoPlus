using System;
using System.Collections.Generic;
using SharpPropoPlus.Contracts.Interfaces;
using SharpPropoPlus.Filter.Contracts;

namespace SharpPropoPlus.Filter
{
    public interface IFilterManager : IDisposable
    {
        IEnumerable<Lazy<IPropoPlusFilter, IFilterMetadata>> Filters { get; }

        IPropoPlusFilter Filter { get; }

        IFilterMetadata GetFilterMetadata(IPropoPlusFilter filter);

        void ChangeFilter(IPropoPlusFilter filter);
        
        bool IsEnabled { get; }

        bool SetEnabled(bool enabled);

        void Notify();
    }
}