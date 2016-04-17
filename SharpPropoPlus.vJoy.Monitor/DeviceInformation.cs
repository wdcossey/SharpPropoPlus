using System;

namespace SharpPropoPlus.vJoyMonitor
{
  public class DeviceInformation
  {
    private readonly int _deviceId;
    private readonly string _name;
    private readonly Guid _productGuid;
    private readonly VjdStat _status;

    internal DeviceInformation()
    {
      
    }

    public DeviceInformation(int deviceId, string name, Guid productGuid, VjdStat status)
    {
      _deviceId = deviceId;
      _name = name;
      _productGuid = productGuid;
      _status = status;
    }

    public int Id => _deviceId;

    public VjdStat Status => _status;

    public string Name => $"{_name} #{_deviceId}";

    public Guid Guid => _productGuid;
  }
}