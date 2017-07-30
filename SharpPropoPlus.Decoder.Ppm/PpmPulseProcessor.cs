using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpPropoPlus.Decoder.Ppm
{
    public abstract class PpmPulseProcessor : PulseProcessor
    {
        #region PPM Values (General)

        /// <summary>
        /// PPM_MIN
        /// PPM minimal pulse width (0.5 mSec)
        /// </summary>
        protected virtual double PpmMinPulseWidth => 96.0;

        /// <summary>
        /// PPM_MAX
        /// PPM maximal pulse width (1.5 mSec)
        /// </summary>
        protected virtual double PpmMaxPulseWidth => 288.0;


        /// <summary>
        /// PPM_TRIG
        /// PPM inter packet  separator pulse ( = 4.5mSec)
        /// </summary>
        protected virtual double PpmTrig => 870.0; // 

        /// <summary>
        /// PPM_SEP
        /// PPM inter-channel separator pulse  - this is a maximum value that can never occur
        /// </summary>
        protected virtual double PpmSeparator => 95.0;

        /// <summary>
        /// PPM_GLITCH
        /// Pulses of this size or less are just a glitch
        /// </summary>
        protected virtual double PpmGlitch => 21.0;


        /// <summary>
        /// PPM_JITTER
        /// Pulses of this size or less are just a glitch
        /// </summary>
        protected virtual double PpmJitter => 5.0;

        #endregion

        
    }
}
