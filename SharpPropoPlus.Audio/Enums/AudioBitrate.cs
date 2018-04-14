using System.ComponentModel;

namespace SharpPropoPlus.Audio.Enums
{
    public enum AudioBitrate
    {
        /// <summary>
        /// Automatic
        /// </summary>
        [Description("Automatic (Default)")] Automatic,

        /// <summary>
        /// 8bit
        /// </summary>
        [Description("8bit")] EightBit,

        /// <summary>
        /// 16bit
        /// </summary>
        [Description("16bit")] SixteenBit
    }
}