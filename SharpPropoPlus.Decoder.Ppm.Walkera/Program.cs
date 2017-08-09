using System;
using SharpPropoPlus.Decoder.Contracts;
using SharpPropoPlus.Decoder.Enums;

namespace SharpPropoPlus.Decoder.Ppm.Walkera
{
    //[Export(typeof(IPropoPlusDecoder))]
    //[ExportMetadata("Type", TransmitterType.Ppm)]
    [ExportPropoPlusDecoder("Walkera", "Walkera WK-2401 (PPM) pulse processor", TransmitterType.Ppm)]
    public class Program : PpmPulseProcessor
    {
        /// <summary>
        /// Polarity ('input') of the Sync pulse is the polarity of the following data pulses
        /// </summary>
        private static bool _polarity;

        //private static int _formerSync = 0;

        //static int i = 0;


        #region PPM Values (Walkera)

        protected override double PpmJitter()
        {
            return 4.25;
        }

        protected override double PpmMinPulseWidth()
        {
            return 78.4;
        }

        protected override double PpmMaxPulseWidth()
        {
            return 304.8;
        }

        protected override double PpmSeparator()
        {
            return 65.3;
        }

        #endregion PPM Values (Walkera)




        public override string[] Description
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
        protected override void Process(int width, bool input)
        {
            //var tbuffer = new char[9];

            if (width < 5)
                return;

            //if (gDebugLevel >= 2 && gCtrlLogFile && !(_strtime_s(tbuffer, 9)))
            //  fprintf(gCtrlLogFile, "\n%s - ProcessPulseWk2401Ppm(width=%d, input=%d)", tbuffer, width, input);

            //_sync is detected at the end of a very long pulse (over  4.5mSec)
            if (width > PpmTrig())
            {
                Sync = true;
                if (!DataCount.Equals(0))
                {
                    PosUpdateCounter++;
                }

                //m_nChannels = _datacount;
                RawChannelCount = DataCount;

                DataCount = 0;
                _polarity = input;
                return;
            }

            if (!Sync)
                return; /* still waiting for _sync */

            // If this pulse is a separator - read the next pulse
            if (_polarity != input)
                return;

            // Cancel jitter /* Version 3.3.3 */
            var jitterValue = Math.Abs(PrevWidth[DataCount] - width);
            if (jitterValue < PpmJitter())
            {
                width = PrevWidth[DataCount];
            }

            PrevWidth[DataCount] = width;


            /* convert pulse width in samples to joystick Position values (newdata)
            joystick Position of 0 correspond to width over 100 samples (2.25mSec)
            joystick Position of 1023 correspond to width under 30 samples (0.68mSec)*/
            var newdata = (int) ((width - PpmMinPulseWidth()) /
                                 (PpmMaxPulseWidth() - PpmMinPulseWidth()) * 1024);

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
            if (DataBuffer[DataCount] - newdata > 100)
            {
                DataBuffer[DataCount] -= 100;
            }
            else if (newdata - DataBuffer[DataCount] > 100)
            {
                DataBuffer[DataCount] += 100;
            }
            else
            {
                DataBuffer[DataCount] = (DataBuffer[DataCount] + newdata) / 2;
            }


            //Assign _data to joystick channels
            ChannelData[DataCount] = DataBuffer[DataCount];

            // Send Position and number of channels to the virtual joystick
            ////SendPPJoy(11, _mPosition);
            JoystickInteraction.Instance.Send(11, ChannelData);

            //if (gDebugLevel >= 3 && gCtrlLogFile /*&& !(i++%50)*/)
            //  fprintf(gCtrlLogFile, " _data[%d]=%d", _datacount, _data[_datacount]);

            //Debug.WriteLine($"_data[{_datacount}]={_data[_datacount]}");

            if (DataCount == 11)
                Sync = false; /* Reset _sync after channel 12 */

            DataCount++;
        }

        /// <summary>
        /// Resets the static variables.
        /// </summary>
        public sealed override void Reset()
        {
            base.Reset();

            _polarity = false;
        }

    }


}