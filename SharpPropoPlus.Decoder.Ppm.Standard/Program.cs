using System;
using System.Threading;
using SharpPropoPlus.Contracts;
using SharpPropoPlus.Contracts.Interfaces;
using SharpPropoPlus.Contracts.Types;
using SharpPropoPlus.Decoder.Contracts;

namespace SharpPropoPlus.Decoder.Ppm.Standard
{
    //[Export(typeof(IPropoPlusDecoder))]
    //[ExportMetadata("Type", TransmitterType.Ppm)]
    [ExportPropoPlusDecoder("Standard", "Standard (PPM) pulse processor", TransmitterType.Ppm)]
    public class Program : PpmPulseProcessor<int>
    {
        /// <summary>
        /// _prev_sep
        /// </summary>
        private static bool _prevSeparator;

        //TODO : inform that a filter has been selected or diselected
        private static int JsChPostProc_selected = -1;

        //private static int _formerSync = 0;

        //static int i = 0;


        #region PPM Values (Standard)

        #endregion 

        public override string[] Description => new[]
        {
            "Pulse processor for Standard PPM",
        };

        public Program()
        {
            Reset();
        }

        /// <summary>
        /// <para>Process Pulse for Standard PPM</para>
        /// <para>A long (over 200 samples) leading high, then a short low, then up to six highs followed by short lows.</para>
        /// <para>The size of a high pulse may vary between 30 to 70 samples, mapped to joystick values of 1024 to 438</para>
        /// <para>where the mid-point of 50 samples is translated to joystick position of 731.</para>
        /// </summary>
        /// <param name="width"></param>
        /// <param name="input"></param>
        protected override void Process(int width, bool input, bool filterChannels, IPropoPlusFilter filter)
        {
            if (Monitor.IsEntered(MonitorLock))
                return;

            if (width < PpmGlitch)
                return;

            //if (gDebugLevel >= 2 && gCtrlLogFile && !(_strtime_s(tbuffer, 10))/*&& !(i++%50)*/)
            //    fprintf(gCtrlLogFile, "\n%s - ProcessPulsePpm(width=%d, input=%d)", tbuffer, width, input);


            /* If pulse is a separator then go to the next one */
            if (width < PpmSeparator || FormerSync)
            {
                _prevSeparator = true;
                FormerSync = false;
                return;
            };

            // Two separators in a row is an error - resseting
            if ((width < PpmSeparator) && _prevSeparator)
            {
                _prevSeparator = true;
                RawChannelCount = 0;
                DataCount = 0;
                return;
            };

            /* sync is detected at the end of a very long pulse (over 200 samples = 4.5mSec) */
            if (/*sync == 0 && */width > PpmTrig)
            {
                Sync = true;
                if (!DataCount.Equals(0))
                {
                    PosUpdateCounter++;
                }
                RawChannelCount = DataCount;
                DataCount = 0;
                FormerSync = true;
                _prevSeparator = false;
                return;
            }

            if (!Sync)
            {
                /* still waiting for sync */
                return; 
            }

            // Two long pulse in a row is an error - resseting
            if (width > PpmSeparator)
            {
                if (!_prevSeparator)
                {
                    RawChannelCount = 0;
                    DataCount = 0;
                    _prevSeparator = false;
                    return;
                }

                _prevSeparator = false;
            };

            // Cancel jitter /* Version 3.3.3 */
            var jitterValue = Math.Abs(PrevWidth[DataCount] - width);
            if (jitterValue < PpmJitter)
            {
                width = PrevWidth[DataCount];
            }

            PrevWidth[DataCount] = width;


            int newdata;

            /* 
             * convert pulse width in samples to joystick position values (newdata)
             * joystick position of 0 correspond to width over 100 samples (2.25mSec)
             * joystick position of 1023 correspond to width under 30 samples (0.68mSec)
             */
            if (input || JsChPostProc_selected != -1)
                newdata = (int)(1024 - (width - PpmMinPulseWidth) / (PpmMaxPulseWidth - PpmMinPulseWidth) * 1024); /* JR */
            else
                newdata = (int)((width - PpmMinPulseWidth) / (PpmMaxPulseWidth - PpmMinPulseWidth) * 1024);		/* Futaba */

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


            //if (input|| m_JsChPostProc_selected!=-1)
            ChannelData[DataCount] = DataBuffer[DataCount];    /* JR - Assign data to joystick channels */
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
            JoystickInteraction.Instance.Send(RawChannelCount, ChannelData, filterChannels, filter);

            //if (gDebugLevel >= 3 && gCtrlLogFile /*&& !(i++%50)*/)
            //    fprintf(gCtrlLogFile, " data[%d]=%d", datacount, data[datacount]);

            if (DataCount == 11)
                Sync = false; /* Reset _sync after channel 12 */

            DataCount++;
        }

        /// <summary>
        /// Resets the static variables.
        /// </summary>
        public sealed override void Reset()
        {
            if (!Monitor.TryEnter(MonitorLock))
                return;

            try
            {
                base.Reset();
                _prevSeparator = false;
            }
            finally
            {
                Monitor.Exit(MonitorLock);
            }
            
        }


    }


}