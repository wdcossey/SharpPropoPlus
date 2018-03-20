using System.Runtime.InteropServices;

namespace CSCore.DeviceTopology
{
    [StructLayout(LayoutKind.Explicit, Size = 3)]
    // ReSharper disable once InconsistentNaming
    public struct COLORREF
    {
        public COLORREF(byte r, byte g, byte b)
        {
            Value = 0;
            R = r;
            G = g;
            B = b;
        }

        public COLORREF(uint value)
        {
            R = 0;
            G = 0;
            B = 0;
            Value = value & 0xFFFFFFU;
        }

        [FieldOffset(0)]
        public byte B;

        [FieldOffset(1)]
        public byte G;

        [FieldOffset(2)]
        public byte R;

        [FieldOffset(0)]
        public uint Value;
    }
}