using System.Runtime.InteropServices;

namespace CSCore.DeviceTopology
{
    [Guid("4509F757-2D46-4637-8E62-CE7DB944F57B")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IKsJackDescription
    {
        [PreserveSig]
        int GetJackCount(
            [Out] out uint count);

        [PreserveSig]
        int GetJackDescription(
            [In] uint index,
            [Out] out KSJACK_DESCRIPTION descriptor);
    }
}