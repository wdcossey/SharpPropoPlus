using CSCore.Win32;
using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace CSCore.DeviceTopology.ExtensionMethods
{
    public static class PartExtensions
    {
        public static IKsJackDescription KsJackDescription(this IPart part)
        {
            var attribute = typeof(IKsJackDescription).GetCustomAttributes(typeof(GuidAttribute), true).Cast<GuidAttribute>().FirstOrDefault();

            if (attribute == null)
            {
                return null;
            }

            var iidKsJackDescription = new Guid(attribute.Value);

            part.Activate((uint)CLSCTX.CLSCTX_INPROC_SERVER, ref iidKsJackDescription, out var instancePtr);

            return instancePtr as IKsJackDescription;
        }
    }
}