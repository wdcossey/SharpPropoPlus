using System;
using NAudio.CoreAudioApi;

namespace SharpPropoPlus.Audio.EventArguments
{
  public class DeviceInfoEventArgs : EventArgs
  {
    private string _name;

    public string Name
    {
      get { return _name; }
      internal set { _name = value; }
    }

    private string _deviceId;

    public string DeviceId
    {
      get { return _deviceId; }
      internal set { _deviceId = value; }
    }
    public DeviceInfoEventArgs()
    {

    }

    public DeviceInfoEventArgs(MMDevice device)
      : this()
    {
      Name = device.FriendlyName;
      DeviceId = device.ID;
    }
  }
}