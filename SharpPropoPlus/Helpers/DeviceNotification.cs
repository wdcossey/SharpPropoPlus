using System;
using System.Runtime.InteropServices;
using SharpPropoPlus.vJoyMonitor;

namespace SharpPropoPlus.Helpers
{
    public static class DeviceNotification
    {
        //https://msdn.microsoft.com/en-us/library/aa363480(v=vs.85).aspx
        public const int DBT_DEVICEARRIVAL = 0x8000; // system detected a new device        
        public const int DBT_DEVICEREMOVECOMPLETE = 0x8004; // device is gone      
        public const int DBT_DEVNODES_CHANGED = 0x0007; //A device has been added to or removed from the system.
        public const int DBT_USERDEFINED = 0xFFFF; 

        public const int WM_DEVICECHANGE = 0x0219; // device change event    

        public const int DbtDevtypDeviceinterface = 5;
        //https://msdn.microsoft.com/en-us/library/aa363431(v=vs.85).aspx
        private const int DEVICE_NOTIFY_ALL_INTERFACE_CLASSES = 4;
        //private static readonly Guid GuidDevinterfaceUSBDevice = new Guid("A5DCBF10-6530-11D2-901F-00C04FB951ED"); // USB devices
        private static readonly Guid GUID_DEVINTERFACE_VJOY = new Guid("781EF630-72B2-11d2-B852-00C04FAD5101"); // USB devices
        private static IntPtr notificationHandle;

        /// <summary>
        /// Registers a window to receive notifications when devices are plugged or unplugged.
        /// </summary>
        /// <param name="windowHandle">Handle to the window receiving notifications.</param>
        /// <param name="usbOnly">true to filter to USB devices only, false to be notified for all devices.</param>
        public static void RegisterDeviceNotification(IntPtr windowHandle, bool usbOnly = false)
        {
            var dbi = new DevBroadcastDeviceinterface
            {
                DeviceType = DbtDevtypDeviceinterface,
                Reserved = 0,
                ClassGuid = /*DeviceEnumerator.VJoyProductGuid, //*/GUID_DEVINTERFACE_VJOY,
                Name = 0
            };

            dbi.Size = Marshal.SizeOf(dbi);
            IntPtr buffer = Marshal.AllocHGlobal(dbi.Size);
            Marshal.StructureToPtr(dbi, buffer, true);

            notificationHandle = RegisterDeviceNotification(windowHandle, buffer, usbOnly ? 0 : DEVICE_NOTIFY_ALL_INTERFACE_CLASSES);
        }

        /// <summary>
        /// Unregisters the window for device notifications
        /// </summary>
        public static void UnregisterDeviceNotification()
        {
            UnregisterDeviceNotification(notificationHandle);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr RegisterDeviceNotification(IntPtr recipient, IntPtr notificationFilter, int flags);

        [DllImport("user32.dll")]
        private static extern bool UnregisterDeviceNotification(IntPtr handle);

        [StructLayout(LayoutKind.Sequential)]
        public struct DevBroadcastDeviceinterface
        {
            internal int Size;
            internal int DeviceType;
            internal int Reserved;
            internal Guid ClassGuid;
            internal short Name;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DEV_BROADCAST_HDR
        {
            public uint dbch_Size;
            public uint dbch_DeviceType;
            public uint dbch_Reserved;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct DEV_BROADCAST_DEVICEINTERFACE
        {
            public int dbcc_size;
            public int dbcc_devicetype;
            public int dbcc_reserved;
            public Guid dbcc_classguid;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
            public string dbcc_name;
        }
    }
}