using SharpPropoPlus.Decoder.Contracts;

namespace SharpPropoPlus.Decoder.Ppm
{
    public abstract class PpmPulseProcessor : PulseProcessor, IPropoPlusDecoder
    {
        #region PPM Values (General)

        /// <summary>
        /// PPM_MIN
        /// PPM minimal pulse width (0.5 mSec)
        /// </summary>
        protected virtual double PpmMinPulseWidth()
        {
            return 96.0;
        }

        /// <summary>
        /// PPM_MAX
        /// PPM maximal pulse width (1.5 mSec)
        /// </summary>
        protected virtual double PpmMaxPulseWidth()
        {
            return 288.0;
        }

        /// <summary>
        /// PPM_TRIG
        /// PPM inter packet  separator pulse ( = 4.5mSec)
        /// </summary>
        protected virtual double PpmTrig()
        {
            return 870.0;
        }

        /// <summary>
        /// PPM_SEP
        /// PPM inter-channel separator pulse  - this is a maximum value that can never occur
        /// </summary>
        protected virtual double PpmSeparator()
        {
            return 95.0;
        }

        /// <summary>
        /// PPM_GLITCH
        /// Pulses of this size or less are just a glitch
        /// </summary>
        protected virtual double PpmGlitch()
        {
            return 21.0;
        }

        /// <summary>
        /// PPM_JITTER
        /// Pulses of this size or less are just a glitch
        /// </summary>
        protected virtual double PpmJitter()
        {
            return 5.0;
        }

        #endregion

        protected abstract void Process(int width, bool input);

        public abstract string[] Description { get; }

        public virtual void ProcessPulse(int sampleRate, int sample)
        {
            var negative = false;

            var pulseLength = CalculatePulseLength(sampleRate, sample, ref negative);

            Process(pulseLength.Normalized, negative);
        }

        public virtual void Reset()
        {
            ChannelData = new int[BufferLength];

            Sync = false;

            DataBuffer = new int[BufferLength]; /* Array of pulse widthes in joystick values */
            DataCount = 0; /* pulse index (corresponds to channel index) */

            FormerSync = false;

            PosUpdateCounter = 0;

            //static int i = 0;
            PrevWidth = new int[BufferLength]; /* array of previous width values */
        }

    }
}
