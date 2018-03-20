using System;
using System.Runtime.InteropServices;

namespace CSCore.DeviceTopology
{
    [Guid("A09513ED-C709-4d21-BD7B-5F34C47F3947")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IControlChangeNotify
    {
        [PreserveSig]
        int OnNotify(
            [In] [MarshalAs(UnmanagedType.U4)] uint processId,
            [In, Optional] [MarshalAs(UnmanagedType.LPStruct)] Guid eventContext);
    }
}