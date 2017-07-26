using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX.DirectInput;
using vJoyInterfaceWrap;

namespace SharpPropoPlus.vJoyMonitor
{
  public class DeviceEnumerator : IDisposable
  {

    public static readonly Guid VJoyProductGuid = new Guid("bead1234-0000-0000-0000-504944564944");
    
    public DeviceInformationCollection GetDevices(VjdStat[] status)
    {
      using (var input = new DirectInput())
      {
        var deviceInstances = input.GetDevices().Where(w => w.ProductGuid.Equals(VJoyProductGuid)).ToList();

        var result = new DeviceInformationCollection();
        for (short i = 0; i < 16; i++)
        {
          var deviceId = Convert.ToUInt32(i);

          try
          {
            var joystick = new vJoy();

            if (!joystick.isVJDExists(deviceId))
              continue;

            var joyStatus = joystick.GetVJDStatus(deviceId);

            if (!status.Contains(joyStatus))
              continue;

            foreach (var device in deviceInstances.Select(s => new Joystick(input, s.InstanceGuid)))
            {
              var objects = device.GetObjects(DeviceObjectTypeFlags.All);

              var first = objects.FirstOrDefault(fd => fd.ReportId.Equals(i));

              if (first == null)
                continue;

              result.Add(first.ReportId, device.Information.ProductName.TrimEnd('\0'), device.Information.InstanceGuid,
                joyStatus);

              break;

            }


          }
          catch (Exception)
          {
            throw;
          }
        }

        return result;
      }

    }

    public void Dispose()
    {

    }
  }
}