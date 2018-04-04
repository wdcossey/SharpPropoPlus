using System;

namespace SharpPropoPlus.Decoder.EventArguments
{
    public class PollChannelsEventArgs : EventArgs
    {
        private PollChannelsEventArgs()
        {

        }

        public PollChannelsEventArgs(int channels)
            : this()
        {
            RawChannels = channels;
        }

        public int RawChannels { get; protected set; }
    }
}