using System.ComponentModel;

namespace SharpPropoPlus.Contracts.Types
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