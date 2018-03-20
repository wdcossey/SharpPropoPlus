using System;
using SharpPropoPlus.Audio.Interfaces;

namespace SharpPropoPlus.Audio.EventArguments
{
    public class AudioEndPointEventArgs : EventArgs, IAudioEndPoint
    {
        public string DeviceName { get; }

        public string DeviceId { get; }

        public int Channels { get; }

        public bool Disabled { get; }

        public int? DeviceColor { get; }

        public AudioEndPointEventArgs()
        {

        }
         
        public AudioEndPointEventArgs(string deviceName, string deviceId, int channels, bool disabled, int? deviceColor)
        {
            DeviceName = deviceName;
            DeviceId = deviceId;
            Channels = channels;
            Disabled = disabled;
            DeviceColor = deviceColor;
        }
    }
}