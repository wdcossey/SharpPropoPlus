using SharpPropoPlus.Decoder.Contracts;

namespace SharpPropoPlus.Decoder.Pcm
{
    public abstract class PcmPulseProcessor : PulseProcessor, IPropoPlusDecoder
    {
        #region PCM Values (General)
        

        #endregion

        protected abstract void Process(int width, bool input);

        public abstract string[] Description { get; }

        public virtual void ProcessPulse(int sampleRate, int sample)
        {
            var negative = false;

            var pulseLength = CalculatePulseLength(sampleRate, sample, ref negative);

            Process(pulseLength.Normalized, negative);
        }

        public abstract void Reset();

    }
}
