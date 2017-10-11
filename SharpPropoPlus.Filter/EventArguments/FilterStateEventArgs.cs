using System;

namespace SharpPropoPlus.Filter.EventArguments
{
    public class FilterStateEventArgs : EventArgs
    {
        private FilterStateEventArgs()
        {
            IsEnabled = false;
        }

        public FilterStateEventArgs(bool isEnabled)
            : this()
        {
            IsEnabled = isEnabled;
        }

        public bool IsEnabled { get; }
    }
}