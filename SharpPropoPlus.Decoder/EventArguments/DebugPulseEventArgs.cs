using System;

namespace SharpPropoPlus.Decoder.EventArguments
{
    public class DebugPulseEventArgs : EventArgs
    {
        public int[] Samples { get; }
        public int RawLength { get; }
        public int NormalizedLength { get; }
        public bool Negative { get; }

        private DebugPulseEventArgs()
        {

        }

        public DebugPulseEventArgs(int[] samples, int rawLength, int normalizedLength, bool negative)
            : this()
        {
            Samples = samples;
            RawLength = rawLength;
            NormalizedLength = normalizedLength;
            Negative = negative;
        }
    }
}