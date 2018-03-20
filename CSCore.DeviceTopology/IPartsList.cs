using System;
using System.Runtime.InteropServices;

namespace CSCore.DeviceTopology
{
    [Guid("6DAA848C-5EB0-45CC-AEA5-998A2CDA1FFB")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPartsList
    {
        [PreserveSig]
        int GetCount(
            [Out] [MarshalAs(UnmanagedType.U4)] out uint count);

        [PreserveSig]
        int GetPart(
            [In]  [MarshalAs(UnmanagedType.U4)] uint index,
            [Out] [MarshalAs(UnmanagedType.Interface)] out IPart part);
    }
}