using System;
using System.ComponentModel.Composition;
using SharpPropoPlus.Decoder.Contracts;
using SharpPropoPlus.Decoder.Enums;

namespace SharpPropoPlus.Decoder.Ppm.Walkera
{
    //[Export(typeof(IPropoPlusDecoder))]
    //[ExportMetadata("Type", TransmitterType.Ppm)]
    [ExportPropoPlusDecoder("Walkera", "Walkera WK-2401 (PPM) pulse processor", TransmitterType.Ppm)]
    public class Program : PpmPulseProcessor, IPropoPlusDecoder
    {

        private static int[] _mPosition;

        private static bool _sync;
        private static bool _polarity;

        /// <summary>
        /// Array of pulse widthes in joystick values
        /// </summary>
        private static int[] _data;

        /// <summary>
        /// Pulse index (corresponds to channel index)
        /// </summary>
        private static int _datacount;

        //private static int _formerSync = 0;

        //static int i = 0;


        #region MyRegion

        protected double PpmWalkeraMinPulseWidth => 78.4;

        protected double PpmWalkeraMaxPulseWidth => 304.8;

        protected double PpmWalkeraSeparator => 65.3;

        protected double PpmWalkeraTrig => PpmTrig;


        #endregion PPM Values (Walkera)


        /// <summary>
        /// Array of previous width values
        /// </summary>
        private static int[] _prevWidth = new int[14];

        public string[] Description
        {
            get
            {
                return new[]
                {
                    "Pulse processor for Walkera WK-2401 PPM",
                    "This is just a permiscuous PPM that does not follow the PPM standard"
                };
            }
        }

        public Program()
        {
            Reset();
        }

        /// <summary>
        /// <para>Process Pulse for Walkera WK-2401 PPM</para>
        /// <para>This is just a permiscuous PPM that does not follow the PPM standard</para>
        /// <para>This is how it works:</para>
        /// <para>1. Minimal pulse width is 5</para>
        /// <para>2. Any pulse of over PPMW_TRIG(=200) is considered as a _sync pulse.</para>
        /// <para>3. _polarity('input') of the Sync pulse is the polarity of the following _data pulses</para>
        /// </summary>
        /// <param name="width"></param>
        /// <param name="input"></param>
        private void ProcessPulseWk2401Ppm(int width, bool input)
        {
            //var tbuffer = new char[9];

            if (width < 5)
                return;

            //if (gDebugLevel >= 2 && gCtrlLogFile && !(_strtime_s(tbuffer, 9)))
            //  fprintf(gCtrlLogFile, "\n%s - ProcessPulseWk2401Ppm(width=%d, input=%d)", tbuffer, width, input);

            //_sync is detected at the end of a very long pulse (over  4.5mSec)
            if (width > PpmWalkeraTrig)
            {
                _sync = true;
                if (!_datacount.Equals(0))
                {
                    //  m_PosUpdateCounter++;
                }

                //m_nChannels = _datacount;
                RawChannelCount = _datacount;

                _datacount = 0;
                _polarity = input;
                return;
            }

            if (!_sync)
                return; /* still waiting for _sync */

            // If this pulse is a separator - read the next pulse
            if (_polarity != input)
                return;

            // Cancel jitter /* Version 3.3.3 */
            if (Math.Abs(_prevWidth[_datacount] - width) < PpmJitter)
                width = _prevWidth[_datacount];
            _prevWidth[_datacount] = width;


            /* convert pulse width in samples to joystick Position values (newdata)
            joystick Position of 0 correspond to width over 100 samples (2.25mSec)
            joystick Position of 1023 correspond to width under 30 samples (0.68mSec)*/
            var newdata = (int) ((width - PpmWalkeraMinPulseWidth) / (PpmWalkeraMaxPulseWidth - PpmWalkeraMinPulseWidth) * 1024);

            /* Trim values into 0-1023 boundries */
            if (newdata < 0)
            {
                newdata = 0;
            }
            else if (newdata > 1023)
            {
                newdata = 1023;
            }

            /* Update _data - do not allow abrupt change */
            if (_data[_datacount] - newdata > 100)
            {
                _data[_datacount] -= 100;
            }
            else if (newdata - _data[_datacount] > 100)
            {
                _data[_datacount] += 100;
            }
            else
            {
                _data[_datacount] = (_data[_datacount] + newdata) / 2;
            }


            //Assign _data to joystick channels
            _mPosition[_datacount] = _data[_datacount];

            // Send Position and number of channels to the virtual joystick
            ////SendPPJoy(11, _mPosition);
            JoystickInteraction.Instance.Send(11, ref _mPosition);

            //if (gDebugLevel >= 3 && gCtrlLogFile /*&& !(i++%50)*/)
            //  fprintf(gCtrlLogFile, " _data[%d]=%d", _datacount, _data[_datacount]);

            //Debug.WriteLine($"_data[{_datacount}]={_data[_datacount]}");

            if (_datacount == 11)
                _sync = false; /* Reset _sync after channel 12 */

            _datacount++;
        }


        public void ProcessPulse(int sampleRate, int sample)
        {
            var negative = false;

            var pulseLength = CalculatePulseLength(sampleRate, sample, ref negative);

            ProcessPulseWk2401Ppm(pulseLength.Normalized, negative);
        }

        /// <summary>
        /// Resets the static variables.
        /// </summary>
        public void Reset()
        {
            _mPosition = new int[Constants.MAX_JS_CH];

            _sync = false;
            _polarity = false;
            _data = new int[14]; /* Array of pulse widthes in joystick values */
            _datacount = 0; /* pulse index (corresponds to channel index) */
            //private static int _formerSync = 0;

            //static int i = 0;
            _prevWidth = new int[14]; /* array of previous width values */
        }
    }


}