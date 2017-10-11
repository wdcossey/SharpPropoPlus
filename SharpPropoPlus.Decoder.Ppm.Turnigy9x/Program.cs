using System;
using System.Threading;
using SharpPropoPlus.Contracts.Types;
using SharpPropoPlus.Decoder.Contracts;

namespace SharpPropoPlus.Decoder.Ppm.Turnigy9x
{
    //[Export(typeof(IPropoPlusDecoder))]
    //[ExportMetadata("Type", TransmitterType.Ppm)]
    [ExportPropoPlusDecoder("Turnigy 9X", "Turnigy 9X (PPM) pulse processor", TransmitterType.Ppm)]
    public class Program : PpmPulseProcessor<int>
    {
        static bool _prevSep = false;

        private int _jsChPostProcSelected = -1;

        /// <summary>
        /// Added to sypport PPM for Turngy 9x 
        /// </summary>
        static int _lastSeparatorWidth = 0;

        //static int i = 0;


        #region  PPM Values (Turnigy)

        public override double PpmSeparatorDefault => 61.44;

        public override double PpmMinPulseWidthDefault => 130.56;

        public override double PpmMaxPulseWidthDefault => 322.56;
    

        #endregion PPM Values (Turnigy)

        public override string[] Description => new[]
        {
            "Pulse processor for Turnigy 9X PPM"
        };

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
        protected override void Process(int width, bool input)
        {
            if (Monitor.IsEntered(MonitorLock))
                return;

            //var tbuffer = new char[9];

            if (width < PpmGlitch)
                return;

            //if (gDebugLevel >= 2 && gCtrlLogFile && !(_strtime_s(tbuffer, 10))/*&& !(i++%50)*/)
            //    fprintf(gCtrlLogFile, "\n%s - ProcessPulseTurnigy9XPpm(width=%d, input=%d)", tbuffer, width, input);

            /* If pulse is a separator then go to the next one */
            if (width < PpmSeparator || FormerSync)
            {
                _prevSep = true;
                FormerSync = false;
                return;
                _lastSeparatorWidth = width; /* Added to sypport PPM for Turngy 9x */
            }
            ;


            // Two separators in a row is an error - resseting
            if ((width < PpmSeparator) && _prevSep)
            {
                _prevSep = true;
                RawChannelCount = 0;
                DataCount = 0;
                return;
            }
            ;


            /* sync is detected at the end of a very long pulse (over 200 samples = 4.5mSec) */
            if ( /*sync == 0 && */width > PpmTrig)
            {
                Sync = true;
                if (!DataCount.Equals(0))
                {
                    PosUpdateCounter++;
                }

                RawChannelCount = DataCount;
                DataCount = 0;
                FormerSync = true;
                _prevSep = false;
                return;
            }

            if (!Sync)
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
                    DataCount = 0;
                    _prevSep = false;
                    return;
                }
                else
                {
                    _prevSep = false;
                }
            }
            ;



            // Cancel jitter
            if (Math.Abs(PrevWidth[DataCount] - width) < PpmJitter)
            {
                width = PrevWidth[DataCount];
            }

            PrevWidth[DataCount] = width;

            int newdata;

            /* convert pulse width in samples to joystick position values (newdata)  */
            if (input || _jsChPostProcSelected != -1)
                newdata = (int) (1024 - (width - PpmMinPulseWidth) /
                                 (PpmMaxPulseWidth - PpmMinPulseWidth) * 1024); /* JR */
            else
                newdata = (int) ((width - PpmMinPulseWidth) /
                                 (PpmMaxPulseWidth - PpmMinPulseWidth) * 1024); /* Futaba */


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
            ChannelData[DataCount] = DataBuffer[DataCount]; /* JR - Assign data to joystick channels */
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
            JoystickInteraction.Instance.Send(RawChannelCount, ChannelData);

            //if (gDebugLevel >= 3 && gCtrlLogFile /*&& !(i++%50)*/)
            //    fprintf(gCtrlLogFile, " data[%d]=%d", datacount, data[datacount]);

            if (DataCount == 11)
                Sync = false; /* Reset sync after channel 12 */

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
            }
            finally
            {
                Monitor.Exit(MonitorLock);
            }
        }
    }


}