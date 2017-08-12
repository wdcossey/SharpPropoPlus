using System;

namespace SharpPropoPlus.Decoder
{
    public static class Constants
    {
        // Definition of some time limits
        // All values are in number of samples normalized to 192K samples per second
        public const double PW_FUTABA = 27.5;

        public const double PW_JR = 31.95;

        #region PPM

        /// <summary>
        /// PPM minimal pulse width (0.5 mSec)
        /// </summary>
        public const double PPM_MIN = 96.0;

        /// <summary>
        /// PPM maximal pulse width (1.5 mSec)
        /// </summary>
        public const double PPM_MAX = 288.0;

        /// <summary>
        /// PPM inter packet  separator pulse ( = 4.5mSec)
        /// </summary>
        public const double PPM_TRIG = 870.0;

        /// <summary>
        /// PPM inter-channel separator pulse  - this is a maximum value that can never occur
        /// </summary>
        public const double PPM_SEP = 95.0;

        /// <summary>
        /// Pulses of this size or less are just a glitch
        /// </summary>
        public const double PPM_GLITCH = 21.0;

        public const double PPMW_MIN = 78.4;

        public const double PPMW_MAX = 304.8;

        /// <summary>
        /// PPM inter packet  separator pulse ( = 4.5mSec)
        /// </summary>
        public const double PPMW_TRIG = 870.0;

        public const double PPMW_SEP = 65.3;

        public const double PPM_JITTER = 5.0;

        #endregion

        public const double SANWA1_MIN = 30;

        public const double SANWA2_MIN = 52.24;

        public const double PCMW_SYNC = 243.809;

        public const int MAX_JS_CH = 12;

        public const string MUTEX_STOP_START = "WaveIn Stopping and Starting are mutually exclusive";

        public const int MAX_BUF_SIZE = 1000;

        /// <summary>
        /// channel to Button/Axis mapping
        /// </summary>
        public const byte MAX_BUTTONS = 128;

        //#region HID Descriptor definitions


        //public const int HID_USAGE_X = 0x30;
        //public const int HID_USAGE_Y = 0x31;
        //public const int HID_USAGE_Z = 0x32;
        //public const int HID_USAGE_RX = 0x33;
        //public const int HID_USAGE_RY = 0x34;
        //public const int HID_USAGE_RZ = 0x35;
        //public const int HID_USAGE_SL0 = 0x36;
        //public const int HID_USAGE_SL1 = 0x37;
        //public const int HID_USAGE_WHL = 0x38;
        //public const int HID_USAGE_POV = 0x39;

        //#endregion
    }
}