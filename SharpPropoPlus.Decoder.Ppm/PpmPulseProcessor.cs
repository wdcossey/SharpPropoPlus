using System;
using System.Linq;
using SharpPropoPlus.Decoder.Contracts;

namespace SharpPropoPlus.Decoder.Ppm
{
    public abstract class PpmPulseProcessor : PulseProcessor, IPropoPlusPpmDecoder
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
        public virtual double PpmMinPulseWidthDefault { get; } = PPM_MIN_PULSE_WIDTH_DEFAULT;

        /// <summary>
        /// PPM_MIN
        /// PPM minimal pulse width (0.5 mSec)
        /// </summary>
        public virtual double PpmMinPulseWidth { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual double PpmMaxPulseWidthDefault { get; } = PPM_MAX_PULSE_WIDTH_DEFAULT;

        /// <summary>
        /// PPM_MAX
        /// PPM maximal pulse width (1.5 mSec)
        /// </summary>
        public virtual double PpmMaxPulseWidth { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public virtual double PpmTrigDefault { get; } = PPM_TRIG_DEFAULT;

        /// <summary>
        /// PPM_TRIG
        /// PPM inter packet  separator pulse ( = 4.5mSec)
        /// </summary>
        public virtual double PpmTrig { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual double PpmSeparatorDefault { get; } = PPM_SEPARATOR_DEFAULT;

        /// <summary>
        /// PPM_SEP
        /// PPM inter-channel separator pulse  - this is a maximum value that can never occur
        /// </summary>
        public virtual double PpmSeparator { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual double PpmGlitchDefault { get; } = PPM_GLITCH_DEFAULT;

        /// <summary>
        /// PPM_GLITCH
        /// Pulses of this size or less are just a glitch
        /// </summary>
        public virtual double PpmGlitch { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual double PpmJitterDefault { get; } = PPM_JITTER_DEFAULT;

        /// <summary>
        /// PPM_JITTER
        /// Pulses of this size or less are just a glitch
        /// </summary>
        public virtual double PpmJitter { get; set; }

        /// <summary>
        /// Trims the input array, taking the last N values
        /// </summary>
        /// <param name="length"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private T[] TrimInput<T>(int length, params T[] input)
        {
            return input.Reverse().Take(length).Reverse().ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="length"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        protected double CalculateAverage(int length, params double[] input)
        {
            var trimmedInput = TrimInput(length, input);
            var sum = input.Sum();
            var result = sum / trimmedInput.Length;
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="length"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        protected double CalculateAverage(int length, params int[] input)
        {
            var trimmedInput = TrimInput(length, input);
            var sum = Convert.ToDouble(input.Sum());
            var result = sum / trimmedInput.Length;
            return result;
        }

        #endregion

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
