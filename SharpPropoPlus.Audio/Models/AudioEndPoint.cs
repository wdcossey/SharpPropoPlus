using CSCore;
using SharpPropoPlus.Audio.Interfaces;

namespace SharpPropoPlus.Audio.Models
{
    public class AudioEndPoint : IAudioEndPoint
    {
        private string _deviceName;
        private string _deviceId;
        private WaveFormat _waveFormat;

        public string DeviceName
        {
            get { return _deviceName; }
            internal set { _deviceName = value; }
        }

        public string DeviceId
        {
            get { return _deviceId; }
            internal set { _deviceId = value; }
        }

        public AudioEndPoint(string deviceName, string deviceId)
        {
            DeviceName = deviceName;
            DeviceId = deviceId;
        }
    }
}