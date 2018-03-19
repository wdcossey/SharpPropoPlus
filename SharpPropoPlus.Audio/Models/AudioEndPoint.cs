using SharpPropoPlus.Audio.Interfaces;

namespace SharpPropoPlus.Audio.Models
{
    public class AudioEndPoint : IAudioEndPoint
    {
        public string DeviceName { get; }

        public string DeviceId { get; }

        public int Channels { get; }

        public bool Disabled { get; }

        public AudioEndPoint(string deviceName, string deviceId, int channels, bool disabled)
        {
            DeviceName = deviceName;
            DeviceId = deviceId;
            Channels = channels;
            Disabled = disabled;
        }
    }
}