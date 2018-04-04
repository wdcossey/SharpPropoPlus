using System;
using SharpPropoPlus.Audio.Models;

namespace SharpPropoPlus.Audio.EventArguments
{
    public class PeakValueEventArgs : EventArgs
    {
        public PeakValues Values { get; internal set; }

        private PeakValueEventArgs()
        {

        }

        public PeakValueEventArgs(PeakValues values)
            : this()
        {
            Values = values;
        }
    }
}