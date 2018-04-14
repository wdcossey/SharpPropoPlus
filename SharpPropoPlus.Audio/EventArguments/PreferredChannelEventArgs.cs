using System;
using SharpPropoPlus.Audio.Enums;

namespace SharpPropoPlus.Audio.EventArguments
{
    public class PreferredChannelEventArgs : EventArgs
    {
        public AudioChannel Channel { get; internal set; }

        public PreferredChannelEventArgs()
        {
            Channel = AudioChannel.Left;
        }

        public PreferredChannelEventArgs(AudioChannel channel)
            : this()
        {
            Channel = channel;
        }

        public PreferredChannelEventArgs(float leftPeak, float? rightPeak)
            : this()
        {
            Channel = (rightPeak.HasValue && rightPeak > leftPeak) ? AudioChannel.Right : AudioChannel.Left;
        }
    }
}