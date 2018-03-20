using System;
using System.Runtime.InteropServices;

namespace CSCore.DeviceTopology
{
    [Guid("45d37c3f-5140-444a-ae24-400789f3cbf3")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IControlInterface
    {
        [PreserveSig]
        int GetName(
            [Out] [MarshalAs(UnmanagedType.LPWStr)] out string name);

        [PreserveSig]
        // ReSharper disable once InconsistentNaming
        int GetIID(
            [Out] out Guid interfaceId);
    }
}