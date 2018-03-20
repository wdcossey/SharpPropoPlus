using System;
using System.Runtime.InteropServices;

namespace CSCore.DeviceTopology
{
    [Guid("2A07407E-6497-4A18-9787-32F79BD0D98F")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IDeviceTopology
    {
        [PreserveSig]
        int GetConnectorCount(
            [Out] [MarshalAs(UnmanagedType.U4)] out uint count);

        [PreserveSig]
        int GetConnector(
            [In] [MarshalAs(UnmanagedType.U4)] uint index,
            [Out] [MarshalAs(UnmanagedType.Interface)]
            out IConnector connector);

        [PreserveSig]
        int GetSubunitCount(
            [Out] [MarshalAs(UnmanagedType.U4)] out uint subunitCount);

        [PreserveSig]
        int GetSubunit(
            [In] [MarshalAs(UnmanagedType.U4)] uint subunitIndex, [Out] [MarshalAs(UnmanagedType.Interface)]
            out ISubunit subunit);

        [PreserveSig]
        int GetPartById(
            [In] [MarshalAs(UnmanagedType.U4)] uint partId,
            [Out] [MarshalAs(UnmanagedType.Interface)]
            out IPart part);

        [PreserveSig]
        int GetDeviceId(
            [Out] [MarshalAs(UnmanagedType.LPWStr)]
            out string deviceId);

        [PreserveSig]
        int GetSignalPath(
            [In] [MarshalAs(UnmanagedType.Interface)]
            IPart partFrom,
            [In] [MarshalAs(UnmanagedType.Interface)]
            IPart partTo,
            [In] [MarshalAs(UnmanagedType.Bool)] bool rejectMixedPaths,
            [Out] [MarshalAs(UnmanagedType.Interface)]
            out IPartsList partList);
    }
}