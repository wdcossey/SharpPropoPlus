using SharpPropoPlus.Contracts.Interfaces;
using SharpPropoPlus.Decoder.Contracts;

namespace SharpPropoPlus.Decoder.Ppm
{
    public abstract class PpmPulseProcessor<TJitterFilter> : PulseProcessor<TJitterFilter>, IPropoPlusPpmDecoder
        where TJitterFilter : IJitterFilter
    {

        #region PPM Values (General)

        private const double PPM_SEPARATOR_DEFAULT = 95.0;

        private const double PPM_MIN_PULSE_WIDTH_DEFAULT = 96.0;
        
        private const double PPM_MAX_PULSE_WIDTH_DEFAULT = 288.0;

        private const double PPM_TRIG_DEFAULT = 870.0;

        private const double PPM_GLITCH_DEFAULT = 21.0;

        private const double PPM_JITTER_DEFAULT = 5.0;

        /// <summary>
        /// 
        /// </summary>
        public virtual double PpmMinPulseWidthDefault => PPM_MIN_PULSE_WIDTH_DEFAULT;

        /// <summary>
        /// PPM_MIN
        /// PPM minimal pulse width (0.5 mSec)
        /// </summary>
        public virtual double PpmMinPulseWidth { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual double PpmMaxPulseWidthDefault => PPM_MAX_PULSE_WIDTH_DEFAULT;

        /// <summary>
        /// PPM_MAX
        /// PPM maximal pulse width (1.5 mSec)
        /// </summary>
        public virtual double PpmMaxPulseWidth { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual double PpmTrigDefault => PPM_TRIG_DEFAULT;

        /// <summary>
        /// PPM_TRIG
        /// PPM inter packet  separator pulse ( = 4.5mSec)
        /// </summary>
        public virtual double PpmTrig { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual double PpmSeparatorDefault => PPM_SEPARATOR_DEFAULT;

        /// <summary>
        /// PPM_SEP
        /// PPM inter-channel separator pulse  - this is a maximum value that can never occur
        /// </summary>
        public virtual double PpmSeparator { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual double PpmGlitchDefault => PPM_GLITCH_DEFAULT;

        /// <summary>
        /// PPM_GLITCH
        /// Pulses of this size or less are just a glitch
        /// </summary>
        public virtual double PpmGlitch { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual double PpmJitterDefault => PPM_JITTER_DEFAULT;

        /// <summary>
        /// PPM_JITTER
        /// Pulses of this size or less are just a glitch
        /// </summary>
        public virtual double PpmJitter { get; set; }

        public virtual double[] PpmJitterAlpha { get; set; } = { 16d, 12d, 6d, 3d, 2d };

        #endregion



        protected abstract override void Process(int width, bool input, bool filterChannels, IPropoPlusFilter filter);

        public abstract override string[] Description { get; }

        public override void Reset()
        {
            PpmMinPulseWidth = PpmMinPulseWidthDefault;
            PpmMaxPulseWidth = PpmMaxPulseWidthDefault;
            PpmTrig = PpmTrigDefault;
            PpmSeparator = PpmSeparatorDefault;
            PpmGlitch = PpmGlitchDefault;
            PpmJitter = PpmJitterDefault;

            ChannelData = new int[BufferLength];

            Sync = false;

            DataBuffer = new int[BufferLength]; /* Array of pulse widthes in joystick values */
            DataCount = 0; /* pulse index (corresponds to channel index) */

            FormerSync = false;

            PosUpdateCounter = 0;

            //static int i = 0;
            PrevWidth = new TJitterFilter[BufferLength]; /* array of previous width values */
        }

    }
}
