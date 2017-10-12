using System;
using SharpPropoPlus.Contracts.Interfaces;
using SharpPropoPlus.Filter.Contracts;

namespace SharpPropoPlus.Filter.EventArguments
{
    public class FilterChangedEventArgs : EventArgs
    {
        private FilterChangedEventArgs()
        {
            IsEnabled = false;
        }

        public FilterChangedEventArgs(bool isEnabled, string name = null, string description = null, IPropoPlusFilter filter = null)
            : this()
        {
            Filter = filter;
            Name = name;
            Description = description;
            IsEnabled = isEnabled && filter != null;
        }

        public bool IsEnabled { get; }

        public string Name { get; }

        public string Description { get; }

        public IPropoPlusFilter Filter { get; }
    }
}