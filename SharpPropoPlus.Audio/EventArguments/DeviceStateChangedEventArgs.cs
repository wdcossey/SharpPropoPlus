using System;
using SharpPropoPlus.Audio.Enums;

namespace SharpPropoPlus.Audio.EventArguments
{
    public class DeviceStateChangedEventArgs : EventArgs
    {
        public string DeviceId { get; }
        public AudioDeviceState State { get; }
        
        private DeviceStateChangedEventArgs()
        {

        }

        public DeviceStateChangedEventArgs(string deviceId, AudioDeviceState audioDeviceState)
            : this()
        {
            DeviceId = deviceId;
            State = audioDeviceState;
        }
    }
}