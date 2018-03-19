using System.ComponentModel;

namespace SharpPropoPlus.Contracts.Enums
{
    public enum TransmitterType
    {
        /// <summary>
        /// PPM (Pulse Position Modulation)
        /// </summary>
        [Description("PPM (Pulse Position Modulation)")]
        Ppm,

        /// <summary>
        /// PCM (Pulse Code Modulation)
        /// </summary>
        [Description("PCM (Pulse Code Modulation)")]
        Pcm
    }
}