﻿using System;
using SharpPropoPlus.Audio.Interfaces;

namespace SharpPropoPlus.Audio.EventArguments
{
    public class AudioEndPointEventArgs : EventArgs, IAudioEndPoint
    {
        private string _deviceName;

        public string DeviceName
        {
            get { return _deviceName; }
            internal set { _deviceName = value; }
        }

        private string _deviceId;

        public string DeviceId
        {
            get { return _deviceId; }
            internal set { _deviceId = value; }
        }

        public AudioEndPointEventArgs()
        {

        }

        public AudioEndPointEventArgs(string deviceName, string deviceId)
            : this()
        {
            DeviceName = deviceName;
            DeviceId = deviceId;
        }
    }
}