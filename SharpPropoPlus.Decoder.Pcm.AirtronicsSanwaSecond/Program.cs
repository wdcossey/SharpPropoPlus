using System.Threading;
using SharpPropoPlus.Contracts.Enums;
using SharpPropoPlus.Contracts.Interfaces;
using SharpPropoPlus.Decoder.Contracts;

namespace SharpPropoPlus.Decoder.Pcm.AirtronicsSanwaSecond
{
    [ExportPropoPlusDecoder("{8CEC7922-8A3A-4CE7-AA70-87B771157C45}", "Airtronics/Sanwa [2]", "Airtronics/Sanwa [2] (PCM) pulse processor", TransmitterType.Pcm)]
    public class Program : PcmPulseProcessor
    {
        private bool? _chunk;

        
        private static readonly int[] Air2Symbol = {
            -1, -1, -1, -1,  0,  0, -1,  2, -1, -1, -1, -1,  1,  1, -1,  3
        };

        #region PCM Values (Airtronics/Sanwa [2])

        #endregion

        public override string[] Description => new[]
        {
            "Pulse processor for Airtronics/Sanwa [2] PCM"
        };

        public Program()
        {
            Reset();
        }

        /// <summary>
        /// <para>Buffer Length/Size<br/></para>
        /// 8
        /// </summary>
        protected override int BufferLength => 6;

        /// <summary>
        /// <para>Process Walkera PCM pulse (Tested with Walkera WK-0701)</para>
        /// <para>ProcessPulseWalPcm</para>
        /// </summary>
        /// <param name="width"></param>
        /// <param name="input"></param>
        /// <param name="filterChannels"></param>
        /// <param name="filter"></param>
        protected override void Process(int width, bool input, bool filterChannels, IPropoPlusFilter filter)
        {
            if (Monitor.IsEntered(MonitorLock))
                return;

            int pulse;
            int shift;

            //if (gDebugLevel >= 2 && gCtrlLogFile && !(i++ % 50) && !(_strtime_s(tbuffer, 9)))
            //    fprintf(gCtrlLogFile, "\n%s - ProcessPulseAirPcm2(Width=%d, input=%d)", tbuffer, width, input);

            if (width < 10)
                return;
            if (width < 70)
                pulse = 1;
            else if (width < 140)
                pulse = 2;
            else
                pulse = 7;

            if (pulse == 7)  // 4-bit pulse marks a biginind of a data chunk
            {
                if (!input)
                {
                    // First data chunk - clear chunnel counter
                    DataCount = 0;
                }
                else
                {   // Second data chunk - get joystick m_Position from channel data
                    ChannelData[0] = Smooth(ChannelData[0], Convert20Bits(DataBuffer[2])); // Elevator	(Ch1)
                    ChannelData[1] = Smooth(ChannelData[1], Convert20Bits(DataBuffer[3])); // Ailerons	(Ch2)
                    ChannelData[2] = Smooth(ChannelData[2], Convert20Bits(DataBuffer[6])); // Throtle	(Ch3)
                    ChannelData[3] = Smooth(ChannelData[3], Convert20Bits(DataBuffer[7])); // Rudder	(Ch4)
                    ChannelData[4] = Smooth(ChannelData[4], Convert20Bits(DataBuffer[1])); // Gear		(Ch5)
                    ChannelData[5] = Smooth(ChannelData[5], Convert20Bits(DataBuffer[5])); // Flaps		(Ch6)

                    RawChannelCount = 6;
                    JoystickInteraction.Instance.Send(RawChannelCount, ChannelData, filterChannels, filter);

                    if (width < 420 && width > 390)
                    {
                        PosUpdateCounter++;
                    }
                }

                Sync = true;       // Sync bit is set for the first 10 bits of the chunk (No channel data here)
                BitStream = 0;
                BitCount = -1;
                _chunk = input;  // Mark chunk polarity - 0: Low channels, 1: High channels
                //return 0;
            }

            if (Sync)
            {
                shift = 7;  // Read the first 10 bits
            }
            else
            {
                shift = 20; // Read a channel data
            }

            BitStream = ((BitStream << 1) + 1) << (pulse - 1);
            BitCount += pulse;

            if (BitCount >= shift)
            {
                BitCount -= shift;
                DataBuffer[DataCount] = (BitStream >> BitCount) & 0xFFFFF; // Put raw 20-bit channel data
                DataCount++;
                Sync = false;
                if (DataCount >= 8)
                {
                    DataCount = 0;
                }
            }
        }

        #region Config

        protected override void LoadConfig()
        {

        }

        protected override void SaveConfig()
        {

        }

        #endregion

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

                _chunk = null;

                DataBuffer = new int[10];
            }
            finally
            {
                Monitor.Exit(MonitorLock);
            }
        }

        #region Airtronics/Sanwa [2] PCM helper functions
        /* Helper function - Airtronic/Sanwa PCM2 data convertor */
        private static int  Convert20Bits(int input)
        {
            int[] quartet = new int[5];

            quartet[4] = Air2Symbol[((input  >> 16)&0xF)];
            if (quartet[4]<0)
                return -1;

            quartet[3] = Air2Symbol[((input >> 12)&0xF)];
            if (quartet[4]<0)
                return -1;

            quartet[2] = Air2Symbol[((input >> 8 )&0xF)];
            if (quartet[4]<0)
                return -1;

            quartet[1] = Air2Symbol[((input >> 4 )&0xF)];
            if (quartet[4]<0)
                return -1;

            quartet[0] = Air2Symbol[((input >> 0 )&0xC)];
            if (quartet[4]<0)
                return -1;

            var value = quartet[4] + (quartet[3]<<2) + (quartet[2]<<4) + (quartet[1]<<6) + (quartet[0]<<8);
            return 1023-2*value;
        }


    #endregion
}


}