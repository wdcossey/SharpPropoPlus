using SharpPropoPlus.Audio.Interfaces;

namespace SharpPropoPlus.Audio.Models
{
    public class AudioEndPoint : IAudioEndPoint
    {
        public int? JackColor { get; internal set; }

        public string DeviceName { get; internal set; }

        public string DeviceId { get; internal set; }

        public int Channels { get; internal set; }

        public bool Disabled { get; internal set; }

        public AudioEndPoint(string deviceName, string deviceId, int channels, bool disabled, int? deviceColor)
        {
            DeviceName = deviceName;
            DeviceId = deviceId;
            Channels = channels;
            Disabled = disabled;
            JackColor = deviceColor;
        }
    }
}