using System;
using SharpPropoPlus.Contracts.Interfaces;
using SharpPropoPlus.Decoder.Contracts;

namespace SharpPropoPlus.Decoder.Ppm
{
    public abstract class PpmPulseProcessor<TJitterFilter> : PulseProcessor<TJitterFilter>, IPropoPlusPpmDecoder
        where TJitterFilter : IJitterFilter
    {
        private bool _loadingConfig = false;

        private double _ppmJitter;
        private double _ppmGlitch;
        private double _ppmSeparator;
        private double _ppmTrig;
        private double _ppmMaxPulseWidth;
        private double _ppmMinPulseWidth;

        /// <summary>
        /// 
        /// </summary>
        public abstract IPropoPlusPpmSettings Settings { get; }

        #region PPM Values (General)

        /// <summary>
        /// PPM_MIN
        /// PPM minimal pulse width (0.5 mSec)
        /// </summary>
        public virtual double PpmMinPulseWidth
        {
            get => _ppmMinPulseWidth;
            set
            {
                _ppmMinPulseWidth = value;

                if (!_ppmMinPulseWidth.Equals(Settings.PpmMinPulseWidth))
                {
                    SaveConfigInternal();
                }
            }
        }

        /// <summary>
        /// PPM_MAX
        /// PPM maximal pulse width (1.5 mSec)
        /// </summary>
        public virtual double PpmMaxPulseWidth
        {
            get => _ppmMaxPulseWidth;
            set
            {
                _ppmMaxPulseWidth = value;

                if (!_ppmMaxPulseWidth.Equals(Settings.PpmMaxPulseWidth))
                {
                    SaveConfigInternal();
                }
            }
        }

        /// <summary>
        /// PPM_TRIG
        /// PPM inter packet  separator pulse ( = 4.5mSec)
        /// </summary>
        public virtual double PpmTrig
        {
            get => _ppmTrig;
            set
            {
                _ppmTrig = value;

                if (!_ppmTrig.Equals(Settings.PpmTrig))
                {
                    SaveConfigInternal();
                }
            }
        }

        /// <summary>
        /// PPM_SEP
        /// PPM inter-channel separator pulse  - this is a maximum value that can never occur
        /// </summary>
        public virtual double PpmSeparator
        {
            get => _ppmSeparator;
            set
            {
                _ppmSeparator = value;

                if (!_ppmSeparator.Equals(Settings.PpmSeparator))
                {
                    SaveConfigInternal();
                }
            }
        }

        /// <summary>
        /// PPM_GLITCH
        /// Pulses of this size or less are just a glitch
        /// </summary>
        public virtual double PpmGlitch
        {
            get => _ppmGlitch;
            set
            {
                _ppmGlitch = value;

                if (!_ppmGlitch.Equals(Settings.PpmGlitch))
                {
                    SaveConfigInternal();
                }                
            }
        }

        /// <summary>
        /// PPM_JITTER
        /// Pulses of this size or less are just a glitch
        /// </summary>
        public virtual double PpmJitter
        {
            get => _ppmJitter;
            set
            {
                _ppmJitter = value;
                SaveConfigInternal();
            }
        }

        public virtual double[] PpmJitterAlpha { get; set; } = { 16d, 12d, 6d, 3d, 2d };

        #endregion

        protected abstract override void Process(int width, bool input, bool filterChannels, IPropoPlusFilter filter);

        public abstract override string[] Description { get; }

        public override void Reset()
        {
            try
            {
                _loadingConfig = true;

                if (Settings.UpgradeRequired)
                {
                    Settings.Upgrade();
                    Settings.UpgradeRequired = false;
                }

                PpmMinPulseWidth =  Settings.PpmMinPulseWidth.Equals(0d) ? Settings.PpmMinPulseWidthDefault : Settings.PpmMinPulseWidth;
                PpmMaxPulseWidth = Settings.PpmMaxPulseWidth.Equals(0d) ? Settings.PpmMaxPulseWidthDefault : Settings.PpmMaxPulseWidth;
                PpmTrig = Settings.PpmTrig.Equals(0d) ? Settings.PpmTrigDefault : Settings.PpmTrig;
                PpmSeparator = Settings.PpmSeparator.Equals(0d) ? Settings.PpmSeparatorDefault : Settings.PpmSeparator;
                PpmGlitch = Settings.PpmGlitch.Equals(0d) ? Settings.PpmGlitchDefault : Settings.PpmGlitch;
                PpmJitter = Settings.PpmJitter.Equals(0d) ? Settings.PpmJitterDefault : Settings.PpmJitter;

                ChannelData = new int[BufferLength];

                Sync = false;

                DataBuffer = new int[BufferLength]; /* Array of pulse widthes in joystick values */
                DataCount = 0; /* pulse index (corresponds to channel index) */

                FormerSync = false;

                PosUpdateCounter = 0;

                //static int i = 0;
                PrevWidth = new TJitterFilter[BufferLength]; /* array of previous width values */

                SaveConfig();
            }
            finally
            {
                _loadingConfig = false;
            }
        }

        private void SaveConfig()
        {
            Settings.PpmMinPulseWidth = PpmMinPulseWidth;
            Settings.PpmMaxPulseWidth = PpmMaxPulseWidth;
            Settings.PpmTrig = PpmTrig;
            Settings.PpmSeparator = PpmSeparator;
            Settings.PpmGlitch = PpmGlitch;
            Settings.PpmJitter = PpmJitter;

            Settings.Save();
        }

        private void SaveConfigInternal()
        {
            if (!_loadingConfig)
            {
                SaveConfig();
            }
        }

    }
}
