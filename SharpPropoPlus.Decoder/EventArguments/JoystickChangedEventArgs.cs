using System;

namespace SharpPropoPlus.Decoder.EventArguments
{
  public class JoystickChangedEventArgs : EventArgs
  {
    private readonly int _deviceId;
    private readonly string _name;
    private readonly Guid _productGuid;

    internal JoystickChangedEventArgs()
    {

    }

    public JoystickChangedEventArgs(int deviceId, string name, Guid productGuid)
    {
      _deviceId = deviceId;
      _name = name;
      _productGuid = productGuid;
    }

    public int Id => _deviceId;

    public string Name => _name;

    public Guid Guid => _productGuid;
  }
}