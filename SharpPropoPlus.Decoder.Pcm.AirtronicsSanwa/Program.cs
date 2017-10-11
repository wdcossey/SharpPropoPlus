using System.Threading;
using SharpPropoPlus.Contracts.Types;
using SharpPropoPlus.Decoder.Contracts;

namespace SharpPropoPlus.Decoder.Pcm.AirtronicsSanwa
{
    [ExportPropoPlusDecoder("Airtronics/Sanwa [1]", "Airtronics/Sanwa [1] (PCM) pulse processor", TransmitterType.Pcm)]
    public class Program : PcmPulseProcessor
    {
        private int _chunk;

        private int _pulse;

        #region PCM Values (Airtronics/Sanwa [1])

        #endregion

        public override string[] Description => new[]
        {
            "Pulse processor for Airtronics/Sanwa [1] PCM"
        };

        public Program()
        {
            Reset();
        }

        /// <summary>
        /// <para>Buffer Length/Size<br/></para>
        /// 8
        /// </summary>
        protected override int BufferLength => 8;

        /// <summary>
        /// <para>Process Walkera PCM pulse (Tested with Walkera WK-0701)</para>
        /// <para>ProcessPulseWalPcm</para>
        /// </summary>
        /// <param name="width"></param>
        /// <param name="input"></param>
        protected override void Process(int width, bool input)
        {

            if (Monitor.IsEntered(MonitorLock))
                return;

            //if (gDebugLevel >= 2 && gCtrlLogFile && i++ % 10 && !(_strtime_s(tbuffer, 9)))
            //    fprintf(gCtrlLogFile, "\n%s - ProcessPulseAirPcm1(%d)", tbuffer, width);


            if (width < 25)
                return;

            if (width < 50)
                _pulse = 1;
            else if (width < 90)
                _pulse = 2;
            else if (width < 130)
                _pulse = 3;
            else
                _pulse = 4;

            // 4-bit pulse marks a bigining of a data chunk
            if (_pulse == 4)  
            {
                if (!input)
                {
                    // First data chunk - clear chunnel counter
                    DataCount = 0;
                }
                else
                {   // Second data chunk - get joystick m_Position from channel data
                    ChannelData[0] = Smooth(ChannelData[0], Convert15Bits(DataBuffer[8])); // Elevator (Ch1)
                    ChannelData[1] = Smooth(ChannelData[1], Convert15Bits(DataBuffer[7])); // Ailron (Ch2)
                    ChannelData[2] = Smooth(ChannelData[2], Convert15Bits(DataBuffer[6])); // Throttle (Ch3)
                    ChannelData[3] = Smooth(ChannelData[3], Convert15Bits(DataBuffer[9])); // Rudder (Ch4)
                    ChannelData[4] = Smooth(ChannelData[4], Convert15Bits(DataBuffer[1])); // Gear (Ch5)
                    ChannelData[5] = Smooth(ChannelData[5], Convert15Bits(DataBuffer[2])); // Flaps (Ch6)
                    ChannelData[6] = Smooth(ChannelData[6], Convert15Bits(DataBuffer[3])); // Aux1 (Ch7)
                    ChannelData[7] = Smooth(ChannelData[7], Convert15Bits(DataBuffer[4])); // Aux2 (Ch8)

                    JoystickInteraction.Instance.Send(RawChannelCount, ChannelData);
                };


                // Sync bit is set for the first 10 bits of the chunk (No channel data here)
                Sync = true;     
                BitStream = 0;
                BitCount = -1;

                // Mark chunk polarity - 0: Low channels, 1: High channels
                _chunk = input ? 1 : 0;  
                //return 0;

                if (DataCount == 5 && width < 160)
                {
                    PosUpdateCounter++;
                }
            };


            //  9: Read the first 10 bits
            // 15: Read a channel data
            var shift = Sync ? 9 : 15;

            BitStream = ((BitStream << 1) + 1) << (_pulse - 1);
            BitCount += _pulse;

            if (BitCount >= shift)
            {
                BitCount -= shift;
                DataBuffer[DataCount] = (BitStream >> BitCount) & 0x7FFF; // Put raw 15-bit channel data
                DataCount++;
                Sync = false;
                if (DataCount >= BufferLength + 2)
                    DataCount = 0;
            }

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

                _chunk = -1;

                DataBuffer = new int[10];
            }
            finally
            {
                Monitor.Exit(MonitorLock);
            }
        }

        #region Airtronics/Sanwa [1] PCM helper functions

        private static readonly int[] AirMsb = {
            -1, -1, -1, -1, -1, -1, -1, -1, -1,  0,  0,  2, -1,  0,  0,  2,
            -1, -1,  3,  2, -1,  3,  3,  2, -1,  1,  1,  3, -1,  1,  1, -1,
        };

        private static readonly int[] AirSymbol =
        {
            -1, -1, -1, -1, -1, -1, -1, -1, -1, 2, 10, 8, -1, 6, 14, 12,
            -1, -1, 9, 0, -1, 5, 13, 4, -1, 3, 11, 1, -1, 7, 15, -1,
        };

        
        private static int Convert15Bits(int input)
        {
            int[] quintet = new int[3];

            /* Convert the upper 5-bits to value 0-3 */
            quintet[2] = AirMsb[(input >> 10) & 0x1F];
            if (quintet[2] < 0)
                return -1;

            /* Convert the mid 5-bits to value 0-15 */
            quintet[1] = AirSymbol[(input >> 5) & 0x1F];
            if (quintet[1] < 0)
                return -1;

            /* Convert the low 5-bits to value 0-15 */
            quintet[0] = AirSymbol[input & 0x1F];
            if (quintet[0] < 0)
                return -1;

            /* Return the calculated (inverted) channel value of 0-1023 */
            return 1023 - (quintet[2] * 256 + quintet[1] * 16 + quintet[0]);

        }

        #endregion
}


}