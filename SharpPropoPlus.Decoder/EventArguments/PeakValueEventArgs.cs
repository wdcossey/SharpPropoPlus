using System;

namespace SharpPropoPlus.Decoder.EventArguments
{
    public class PollChannelsEventArgs : EventArgs
    {
        private int _rawChannels;

        private PollChannelsEventArgs()
        {

        }

        public PollChannelsEventArgs(int channels)
            : this()
        {
            RawChannels = channels;
        }

        public int RawChannels
        {
            get { return _rawChannels; }
            protected set { _rawChannels = value; }
        }
    }
}