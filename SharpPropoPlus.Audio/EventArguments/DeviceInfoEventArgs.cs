using System;
using SharpPropoPlus.Audio.Interfaces;

namespace SharpPropoPlus.Audio.EventArguments
{
    public class AudioEndPointEventArgs : EventArgs, IAudioEndPoint
    {
        public string DeviceName { get; internal set; }

        public string DeviceId { get; internal set; }

        public int Channels { get; internal set; }

        public bool Disabled { get; internal set; }

        public int? JackColor { get; internal set; }

        public AudioEndPointEventArgs()
        {

        }
         
        public AudioEndPointEventArgs(string deviceName, string deviceId, int channels, bool disabled, int? deviceColor)
        {
            DeviceName = deviceName;
            DeviceId = deviceId;
            Channels = channels;
            Disabled = disabled;
            JackColor = deviceColor;
        }
    }
}