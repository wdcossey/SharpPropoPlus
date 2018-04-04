using System;
using System.Threading;

namespace SharpPropoPlus.Decoder.EventArguments
{
    public class ChannelsUpdateEventArgs : EventArgs
    {
        public int[] RawChannels { get; }

        public int RawCount { get; }

        public int[] FilterChannels { get; }

        public int FilterCount { get; }

        private ChannelsUpdateEventArgs()
        {

        }

        public ChannelsUpdateEventArgs(int[] rawChannels, int rawCount, int[] filterChannels, int filterCount)
            : this()
        {
            RawChannels = rawChannels;
            RawCount = rawCount;
            FilterChannels = filterChannels;
            FilterCount = filterCount;
        }
    }
}