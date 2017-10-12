using SharpPropoPlus.Contracts.Interfaces;

namespace SharpPropoPlus.Decoder.Contracts
{
    public interface IPropoPlusPpmDecoder : IPropoPlusDecoder
    {
        double PpmMinPulseWidthDefault { get; }

        double PpmMinPulseWidth { get; set; }

        double PpmMaxPulseWidthDefault { get; }

        double PpmMaxPulseWidth { get; set; }

        double PpmTrigDefault { get; }

        double PpmTrig { get; set; }

        double PpmSeparatorDefault { get; }

        double PpmSeparator { get; set; }

        double PpmGlitchDefault { get; }

        double PpmGlitch { get; set; }

        double PpmJitterDefault { get; }
        
        double PpmJitter { get; set; }

        double[] PpmJitterAlpha { get; set; }
    }
}