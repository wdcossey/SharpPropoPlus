using System;
using System.Runtime.InteropServices;
using System.Text;

namespace CSCore.DeviceTopology
{
    [Guid("AE2DE0E4-5BCA-4F2D-AA46-5D13F8FDB3A9")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPart
    {
        [PreserveSig]
        int GetName([Out] [MarshalAs(UnmanagedType.LPWStr)]
            out string name);

        [PreserveSig]
        int GetLocalId([Out] [MarshalAs(UnmanagedType.U4)] out uint localId);

        [PreserveSig]
        int GetGlobalId([Out] [MarshalAs(UnmanagedType.LPWStr)]
            out string globalId);

        [PreserveSig]
        int GetPartType([Out] out PartType partType);

        [PreserveSig]
        int GetSubType([Out] out Guid subType);

        [PreserveSig]
        int GetControlInterfaceCount([Out] [MarshalAs(UnmanagedType.U4)] out uint count);

        [PreserveSig]
        int GetControlInterface([In] [MarshalAs(UnmanagedType.U4)] uint index,
            [Out] [MarshalAs(UnmanagedType.Interface)]
            out IControlInterface control);

        [PreserveSig]
        int EnumPartsIncoming([Out] [MarshalAs(UnmanagedType.Interface)]
            out IPartsList partList);

        [PreserveSig]
        int EnumPartsOutgoing([Out] [MarshalAs(UnmanagedType.Interface)]
            out IPartsList partList);

        [PreserveSig]
        int GetTopologyObject([Out] [MarshalAs(UnmanagedType.Interface)]
            out IDeviceTopology deviceTopology);

        [PreserveSig]
        int Activate([In] uint classContext, [In] ref Guid interfaceId,
            [Out, Optional] [MarshalAs(UnmanagedType.IUnknown)]
            out object instancePtr);

        [PreserveSig]
        int RegisterControlChangeCallback([In] ref Guid interfaceId, [In] IControlChangeNotify client);

        [PreserveSig]
        int UnregisterControlChangeCallback([In] IControlChangeNotify client);
    }
}