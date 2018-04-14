using SharpPropoPlus.Contracts.Interfaces;

namespace SharpPropoPlus.Decoder.Contracts
{
    public interface IPropoPlusPpmDecoder : IPropoPlusDecoder
    {
        double PpmMinPulseWidth { get; set; }

        double PpmMaxPulseWidth { get; set; }

        double PpmTrig { get; set; }

        double PpmSeparator { get; set; }

        double PpmGlitch { get; set; }

        double PpmJitter { get; set; }

        double[] PpmJitterAlpha { get; set; }
    }
}