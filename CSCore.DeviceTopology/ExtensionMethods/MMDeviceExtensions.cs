using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using CSCore.CoreAudioAPI;
using CSCore.Win32;

namespace CSCore.DeviceTopology.ExtensionMethods
{
    // ReSharper disable once InconsistentNaming
    public static class MMDeviceExtensions
    {
        public static IDeviceTopology DeviceTopology(this IMMDevice device)
        {
            var attribute = typeof(IDeviceTopology).GetCustomAttributes(typeof(GuidAttribute), true).Cast<GuidAttribute>().FirstOrDefault();

            if (attribute == null)
            {
                return null;
            }

            device.Activate(new Guid(attribute.Value), CLSCTX.CLSCTX_ALL, IntPtr.Zero, out var pInterface);

            return Marshal.GetObjectForIUnknown(pInterface) as IDeviceTopology;
        }

        public static IDeviceTopology DeviceTopology(this MMDevice device)
        {
            var attribute = typeof(IDeviceTopology).GetCustomAttributes(typeof(GuidAttribute), true).Cast<GuidAttribute>().FirstOrDefault();

            if (attribute == null)
            {
                return null;
            }

            var pInterface = device.Activate(new Guid(attribute.Value), CLSCTX.CLSCTX_ALL, IntPtr.Zero);

            return Marshal.GetObjectForIUnknown(pInterface) as IDeviceTopology;
        }

        public static IEnumerable<KSJACK_DESCRIPTION> GetJackDescriptions(this IMMDevice device)
        {
            try
            {
                var deviceTopology = device.DeviceTopology();

                var connector = deviceTopology.GetConnector();

                var part = connector.GetConnectedToAsPart();

                var jackDescription = part.KsJackDescription();

                if (jackDescription == null)
                {
                    return null;
                }

                jackDescription.GetJackCount(out var conCount);

                if (conCount <= 0)
                {
                    return null;
                }

                var result = new List<KSJACK_DESCRIPTION>();

                for (uint index = 0; index < conCount; index++)
                {
                    result.Add(jackDescription.GetJackDescription(index));
                }

                return result;
            }
            catch
            {
                return null;
            }
            
        }


        public static IEnumerable<KSJACK_DESCRIPTION> GetJackDescriptions(this MMDevice device)
        {
            try
            {
                var deviceTopology = device.DeviceTopology();

                var connector = deviceTopology.GetConnector();

                var part = connector.GetConnectedToAsPart();

                var jackDescription = part.KsJackDescription();

                if (jackDescription == null)
                {
                    return null;
                }

                jackDescription.GetJackCount(out var conCount);

                if (conCount <= 0)
                {
                    return null;
                }

                var result = new List<KSJACK_DESCRIPTION>();

                for (uint index = 0; index < conCount; index++)
                {
                    result.Add(jackDescription.GetJackDescription(index));
                }

                return result;
            }
            catch
            {
                return null;
            }

        }
    }
}