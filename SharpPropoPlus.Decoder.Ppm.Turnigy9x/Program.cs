using System;
using System.ComponentModel.Composition;
using SharpPropoPlus.Decoder.Contracts;
using SharpPropoPlus.Decoder.Enums;

namespace SharpPropoPlus.Decoder.Ppm.Turnigy9x
{
    //[Export(typeof(IPropoPlusDecoder))]
    //[ExportMetadata("Type", TransmitterType.Ppm)]
    [ExportPropoPlusDecoder("Turnigy 9X", "Turnigy 9X (PPM) pulse processor", TransmitterType.Ppm)]
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

        private static bool _formerSync = false;

        static bool _prevSep = false;

        private int _jsChPostProcSelected = -1;

        /// <summary>
        /// Added to sypport PPM for Turngy 9x 
        /// </summary>
        static int _lastSeparatorWidth = 0; 

        //static int i = 0;


        public double PpmTurnigySeparator => 61.44;

        protected override double PpmTrig => 870.0;

        protected override double PpmMinPulseWidth => 130.56;

        protected override double PpmMaxPulseWidth => 322.56;

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
                    "Pulse processor for Turnigy 9X PPM"
                };
            }
        }

        public Program()
        {
            Reset();
        }

        /// <summary>
        /// <para>Process Pulse for Turnigy 9X PPM</para>
        /// <para>This is how it works:</para>
        /// <para>Cycle: 22.46mS</para>
        /// <para>Separator: 0.30mS-0.34mS - In Normalized values: 	 57.6-65.3</para>
        /// <para>Data Pulse: 0.68mS , 1.18mS, 1.68mS (Min, Mid, Max) 130.56,  226.56, 	322.56</para>
        /// </summary>
        /// <param name="width"></param>
        /// <param name="input"></param>
        private void ProcessPulseTurnigy9XPpm(int width, bool input)
        {
            //var tbuffer = new char[9];

            if (width < PpmGlitch)
                return;

            //if (gDebugLevel >= 2 && gCtrlLogFile && !(_strtime_s(tbuffer, 10))/*&& !(i++%50)*/)
            //    fprintf(gCtrlLogFile, "\n%s - ProcessPulseTurnigy9XPpm(width=%d, input=%d)", tbuffer, width, input);

            /* If pulse is a separator then go to the next one */
            if (width < PpmTurnigySeparator || _formerSync)
            {
                _prevSep = true;
                _formerSync = false;
                return;
                _lastSeparatorWidth = width;   /* Added to sypport PPM for Turngy 9x */
            };


            // Two separators in a row is an error - resseting
            if ((width < PpmTurnigySeparator) && _prevSep)
            {
                _prevSep = true;
                RawChannelCount = 0;
                _datacount = 0;
                return;
            };


            /* sync is detected at the end of a very long pulse (over 200 samples = 4.5mSec) */
            if (/*sync == 0 && */width > PpmTrig)
            {
                _sync = true;
                if (!_datacount.Equals(0))
                {
                    //m_PosUpdateCounter++;
                }

                RawChannelCount = _datacount;
                _datacount = 0;
                _formerSync = true;
                _prevSep = false;
                return;
            }

            if (!_sync)
            {
                /* still waiting for sync */
                return; 
            }

            // Two long pulse in a row is an error - resseting
            if (width > base.PpmSeparator)
            {
                if (!_prevSep)
                {
                    RawChannelCount = 0;
                    _datacount = 0;
                    _prevSep = false;
                    return;
                }
                else
                {
                    _prevSep = false;
                }
            };



            // Cancel jitter
            if (Math.Abs(_prevWidth[_datacount] - width) < PpmJitter)
            {
                width = _prevWidth[_datacount];
            }

            _prevWidth[_datacount] = width;

            int newdata;

            /* convert pulse width in samples to joystick position values (newdata)  */
            if (input || _jsChPostProcSelected != -1)
                newdata = (int)(1024 - (width - PpmMinPulseWidth) / (PpmMaxPulseWidth - PpmMinPulseWidth) * 1024); /* JR */
            else
                newdata = (int)((width - PpmMinPulseWidth) / (PpmMaxPulseWidth - PpmMinPulseWidth) * 1024);     /* Futaba */


            /* Trim values into 0-1023 boundries */
            if (newdata < 0)
            {
                newdata = 0;
            }
            else if (newdata > 1023)
            {
                newdata = 1023;
            }

            /* Update data - do not allow abrupt change */
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


            //if (input|| m_JsChPostProc_selected!=-1)
            _mPosition[_datacount] = _data[_datacount];    /* JR - Assign data to joystick channels */
            //else
            //	switch (datacount)
            //{ // Futaba
            //case 0: 	m_Position[1]  = data[datacount];	break;/* Assign data to joystick channels */
            //case 1: 	m_Position[2]  = data[datacount];	break;/* Assign data to joystick channels */
            //case 2: 	m_Position[0]  = data[datacount];	break;/* Assign data to joystick channels */
            //case 3: 	m_Position[3]  = data[datacount];	break;/* Assign data to joystick channels */
            //case 4: 	m_Position[4]  = data[datacount];	break;/* Assign data to joystick channels */
            //case 5: 	m_Position[5]  = data[datacount];	break;/* Assign data to joystick channels */
            //case 6: 	m_Position[6]  = data[datacount];	break;/* Assign data to joystick channels */
            //case 7: 	m_Position[7]  = data[datacount];	break;/* Assign data to joystick channels */
            //case 8: 	m_Position[8]  = data[datacount];	break;/* Assign data to joystick channels */
            //case 9: 	m_Position[9]  = data[datacount];	break;/* Assign data to joystick channels */
            //case 10: 	m_Position[10] = data[datacount];	break;/* Assign data to joystick channels */
            //case 11: 	m_Position[11] = data[datacount];	break;/* Assign data to joystick channels */
            //};

            
            // Send Position and number of channels to the virtual joystick
            JoystickInteraction.Instance.Send(RawChannelCount, ref _mPosition);

            //if (gDebugLevel >= 3 && gCtrlLogFile /*&& !(i++%50)*/)
            //    fprintf(gCtrlLogFile, " data[%d]=%d", datacount, data[datacount]);

            if (_datacount == 11)
                _sync = false;           /* Reset sync after channel 12 */

            _datacount++;
        }


        public void ProcessPulse(int sampleRate, int sample)
        {
            var negative = false;

            var pulseLength = CalculatePulseLength(sampleRate, sample, ref negative);

            ProcessPulseTurnigy9XPpm(pulseLength.Normalized, negative);
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
            _formerSync = false;

            //static int i = 0;
            _prevWidth = new int[14]; /* array of previous width values */
        }
    }


}