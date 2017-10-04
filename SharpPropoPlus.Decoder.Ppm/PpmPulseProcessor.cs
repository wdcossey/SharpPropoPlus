using System;
using System.Linq;
using SharpPropoPlus.Decoder.Contracts;

namespace SharpPropoPlus.Decoder.Ppm
{
    public abstract class PpmPulseProcessor : PulseProcessor, IPropoPlusPpmDecoder
    {

        private double _jitterAverage = 0d;

        private int _jitterPrevValue = 0;

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

        /// <summary>
        /// Advanced jitter filter by: https://github.com/Radrik5
        /// </summary>
        /// <param name="width"></param>
        /// <param name="alpha"></param>
        /// <returns></returns>
        protected int JitterFilter(int width, double[] alpha)
        {
            // Modified Moving Average with differnet ALPHAs
            //
            // Generic formula:
            //   <out> = ((ALPHA - 1) * <out> + <in>) / ALPHA
            //
            // The closer new value to previous value the higher should be ALPHA to remove jitter
            // The farther new value from previous value the lower should be ALPHA to be responsive

            // static const double ALPHA[] = { 8, 6, 4, 3, 2 }; // good but with small jitter
            //double[] alpha = { 16, 12, 6, 3, 2 };

            int diff = Math.Abs(_jitterPrevValue - width);
            int index = Math.Min(Math.Max(diff - 1, 0), alpha.Length - 1);

            _jitterAverage = ((alpha[index] - 1) * _jitterAverage + width) / alpha[index];

            // Go to new value only if the average is farther than 0.75 from the previous value
            // This is done to remove jitter between 10 and 11 when average is around 10.5
            // The average needs to go above 10.75 to switch from 10 to 11
            // and it needs to go below 10.25 to switch from 11 to 10
            double avgDiff = _jitterAverage - _jitterPrevValue;

            int sign = avgDiff < 0 ? -1 : 1;
            _jitterPrevValue += sign * Convert.ToInt32(Math.Abs(avgDiff) + 0.25);

            return _jitterPrevValue;
        }

        protected int JitterFilter(int width)
        {
            return JitterFilter(width, PpmJitterAlpha);
        }

        protected abstract override void Process(int width, bool input);

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
            PrevWidth = new int[BufferLength]; /* array of previous width values */
        }

    }
}
