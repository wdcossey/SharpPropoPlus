using System;
using System.Runtime.InteropServices;

namespace CSCore.DeviceTopology
{
    // ReSharper disable once InconsistentNaming
    [StructLayout(LayoutKind.Sequential)]
    public struct KSJACK_DESCRIPTION
    {
        [MarshalAs(UnmanagedType.U4)]
        public uint ChannelMapping;

        /// <summary>
        /// The jack color.
        /// </summary>
        [MarshalAs(UnmanagedType.Struct)]
        public COLORREF Color;

        /// <summary>
        /// The connection type.
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public uint ConnectionType;

        /// <summary>
        /// The geometric location of the jack.
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public uint GeoLocation;

        /// <summary>
        /// The general location of the jack.
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public uint GenLocation;

        /// <summary>
        /// The type of port represented by the jack.
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public uint PortConnection;

        /// <summary>
        /// Indicates whether an endpoint device is plugged into the jack, if supported by the adapter.
        /// </summary>
        [MarshalAs(UnmanagedType.Bool)]
        public bool IsConnected;
    }
}