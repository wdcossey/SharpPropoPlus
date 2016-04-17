using System;

namespace SharpPropoPlus.Decoder.Models
{
  public class JoystickInformation
  {
    private readonly int _deviceId;
    private readonly string _name;
    private readonly Guid _productGuid;

    internal JoystickInformation()
    {
      
    }

    public JoystickInformation(int deviceId, string name, Guid productGuid)
      :this()
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