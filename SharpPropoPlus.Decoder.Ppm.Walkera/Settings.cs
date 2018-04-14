namespace SharpPropoPlus.Decoder.Ppm.Walkera
{
    public class Settings : PpmSettings
    {
        public override double PpmMinPulseWidthDefault => 78.4d;
        public override double PpmMaxPulseWidthDefault => 304.8d;
        public override double PpmSeparatorDefault => 65.3d;
        public override double PpmJitterDefault => 4.25d;
    }
}