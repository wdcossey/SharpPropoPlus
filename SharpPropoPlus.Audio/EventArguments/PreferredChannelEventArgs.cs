using System;
using SharpPropoPlus.Audio.Enums;

namespace SharpPropoPlus.Audio.EventArguments
{
  public class PreferredChannelEventArgs : EventArgs
  {
    private AudioChannel _channel;

    public AudioChannel Channel
    {
      get { return _channel; }
      internal set { _channel = value; }
    }

    public PreferredChannelEventArgs()
    {
      Channel = AudioChannel.Left;
    }

    public PreferredChannelEventArgs(AudioChannel channel)
      : this()
    {
      Channel = channel;
    }

    public PreferredChannelEventArgs(float leftPeak, float rightPeak)
      : this()
    {
      Channel = (rightPeak > leftPeak) ? AudioChannel.Right : AudioChannel.Left;
    }


  }
}