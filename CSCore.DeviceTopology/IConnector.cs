using System.Runtime.InteropServices;
using CSCore.CoreAudioAPI;

namespace CSCore.DeviceTopology
{
    [Guid("9c2c4058-23f5-41de-877a-df3af236a09e")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IConnector
    {
        [PreserveSig]
        int GetType(
            [Out] [MarshalAs(UnmanagedType.I4)] out ConnectorType connectorType);

        [PreserveSig]
        int GetDataFlow(
            [Out] [MarshalAs(UnmanagedType.I4)] out DataFlow dataFlow);

        [PreserveSig]
        int ConnectTo(
            [In] [MarshalAs(UnmanagedType.Interface)]
            IConnector connector);

        [PreserveSig]
        int Disconnect();

        [PreserveSig]
        int IsConnected(
            [Out] [MarshalAs(UnmanagedType.Bool)] out bool isConnected);

        [PreserveSig]
        int GetConnectedTo(
            [Out] [MarshalAs(UnmanagedType.Interface)]
            out IConnector connector);

        [PreserveSig]
        int GetConnectorIdConnectedTo(
            [Out] [MarshalAs(UnmanagedType.LPWStr)]
            out string connectorId);

        [PreserveSig]
        int GetDeviceIdConnectedTo(
            [Out] [MarshalAs(UnmanagedType.LPWStr)]
            out string deviceId);

    }
}