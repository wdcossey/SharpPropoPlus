using System;

namespace SharpPropoPlus.Decoder.Enums
{
    #region HID Descriptor definitions
    [Flags]
    public enum HidUsageFlags
    {
        // ReSharper disable InconsistentNaming
        HID_USAGE_X = 0x30,

        HID_USAGE_Y = 0x31,

        HID_USAGE_Z = 0x32,

        HID_USAGE_RX = 0x33,

        HID_USAGE_RY = 0x34,

        HID_USAGE_RZ = 0x35,

        HID_USAGE_SL0 = 0x36,

        HID_USAGE_SL1 = 0x37,

        HID_USAGE_WHL = 0x38,

        HID_USAGE_POV = 0x39,
        // ReSharper restore InconsistentNaming
    }
    #endregion

}