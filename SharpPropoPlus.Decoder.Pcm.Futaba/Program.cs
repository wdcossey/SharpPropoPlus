using System;
using System.Threading;
using SharpPropoPlus.Contracts;
using SharpPropoPlus.Contracts.Enums;
using SharpPropoPlus.Contracts.Interfaces;
using SharpPropoPlus.Decoder.Contracts;

namespace SharpPropoPlus.Decoder.Pcm.Futaba
{
    [ExportPropoPlusDecoder("{9D66DB4A-8B76-4CB5-AC26-715D233806A4}", "Futaba", "Futaba (PCM) pulse processor", TransmitterType.Pcm)]
    public class Program : PcmPulseProcessor
    {
        /// <summary>
        /// Number of samples normalized to 192K samples per second
        /// </summary>
        private const decimal PwFutaba = 27.5m;

        private static readonly int[] FutabaSymbol = {
            -1, -1, -1, -1, -1, -1, -1, 63, -1, -1, -1, -1, 62, -1, -1, 39,
            -1, -1, -1, -1, -1, -1, -1, -1, 60, -1, -1, -1, 61, -1, -1, 38,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            58, -1, -1, 43, -1, -1, -1, -1, 59, -1, -1, -1, 48, -1, -1, 10,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            56, -1, -1, 42, -1, -1, -1, 34, -1, -1, -1, -1, -1, -1, -1, -1,
            57, -1, -1, 33, -1, -1, -1, -1, 49, -1, -1, -1, 37, -1, -1,  9,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            52, -1, -1, 41, -1, -1, -1, 32, -1, -1, -1, -1, 40, -1, -1, 19,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            51, -1, -1, 35, -1, -1, -1, 18, -1, -1, -1, -1, -1, -1, -1, -1,
            50, -1, -1, 17, -1, -1, -1, -1, 36, -1, -1, -1, 16, -1, -1,  8,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            55, -1, -1, 47, -1, -1, -1, 27, -1, -1, -1, -1, 46, -1, -1, 13,
            -1, -1, -1, -1, -1, -1, -1, -1, 45, -1, -1, -1, 28, -1, -1, 12,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            44, -1, -1, 23, -1, -1, -1, -1, 31, -1, -1, -1, 22, -1, -1, 11,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            54, -1, -1, 26, -1, -1, -1, 14, -1, -1, -1, -1, 30, -1, -1,  6,
            -1, -1, -1, -1, -1, -1, -1, -1, 29, -1, -1, -1, 21, -1, -1,  7,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            53, -1, -1, 15, -1, -1, -1,  4, -1, -1, -1, -1, 20, -1, -1,  5,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            25, -1, -1,  2, -1, -1, -1,  3, -1, -1, -1, -1, -1, -1, -1, -1,
            24, -1, -1,  1, -1, -1, -1, -1,  0, -1, -1, -1, -1, -1, -1, -1
        };

        private int _sync;

        private int _i;

        #region PCM Values (Futaba)

        #endregion

        public override string[] Description => new[]
        {
            "Pulse processor for Futaba PCM"
        };

        public Program()
        {
            Reset();
        }

        /// <summary>
        /// <para>Buffer Length/Size</para>
        /// 9
        /// </summary>
        protected override int BufferLength => 9;

        private new int Sync
        {
            get { return _sync; }
            set { _sync = value; }
        }

        /// <summary>
        /// <para>Process Walkera PCM pulse (Tested with Walkera WK-0701)</para>
        /// <para>ProcessPulseWalPcm</para>
        /// </summary>
        /// <param name="width"></param>
        /// <param name="input"></param>
        protected override void Process(int width, bool input, bool filterChannels, IPropoPlusFilter filter)
        {
            if (Monitor.IsEntered(MonitorLock))
                return;

            width = Convert.ToInt32(Math.Floor(width / PwFutaba));

            /* 
                Sync is determined as 18-bit wide pulse 
                If detected, move state machine to sync=1 (wait for parity marker) 
                and initialize all static params
            */

            if (Sync == 0 && width == 18)
            {
                Sync = 1;
                if (DataCount != 0)
                {
                    PosUpdateCounter++;
                }

                Bit = 0;
                BitStream = 0;
                BitCount = 0;
                DataCount = 0;

                //if (gDebugLevel>=2 && gCtrlLogFile &&  !( _strtime_s( tbuffer, 9 ))/*!(i++%50)*/)
                //    fprintf(gCtrlLogFile,"\n%s - ProcessPulseFutabaPcm(%d) %02d %02d %02d %02d ; %02d %02d %02d %02d ; %02d %02d %02d %02d ; %02d %02d %02d %02d - %02d %02d %02d %02d ; %02d %02d %02d %02d ; %02d %02d %02d %02d ; %02d %02d %02d %02d", tbuffer, width,\
                //    data[0], data[1], data[2], data[3], data[4], data[5], data[6], data[7],\
                //    data[8], data[9], data[10], data[11], data[12], data[13], data[14], data[15],\
                //    data[16], data[17], data[18], data[19], data[20], data[21], data[22], data[23],\
                //    data[24], data[25], data[26], data[27], data[28], data[29], data[30], data[31]\
                //    );

                return;
            }

            if (Sync == 0)
                return;

            BitStream = (BitStream << width) | (Bit >> (32 - width));
            Bit ^= Convert.ToInt32(0xFFFFFFFF);
            BitCount += width;

            /* 
                Parity marker must follow the sync pulse (sync==1)
                It might take one of two forms:
                - EVEN parity marker is 000011.
                - ODD parity marker is 00000011.
                If either is found mode state machine to sync=2 (read raw data)
            */

            if (Sync == 0)
            {
                if (BitCount >= 6)
                {
                    BitCount -= 6;
                    if (((BitStream >> BitCount) & 0x3F) == 0x03)
                    {
                        /* Even? */
                        Sync = 2;
                        DataCount = 0; /* Even: Reset sextet counter to read the first (even) 16 ones */
                    }
                    else if (((BitStream >> BitCount) & 0x3F) == 0x00)
                    {
                        /* Odd? */
                        Sync = 2;
                        DataCount = 16; /* Odd: Set sextet counter to 16  to read the last (odd) 16 ones */
                        BitCount -= 2;
                    }
                    else
                    {
                        Sync = 0;
                    }
                }
                return;
            }

            /* 
                Read the next ten bits of raw data 
                Convert then into a sextet
                Increment sextet counter
                If data is illegal reset state machine to sync=0
            */
            if (BitCount >= 10)
            {
                BitCount -= 10;
                if ((DataBuffer[DataCount++] = FutabaSymbol[(BitStream >> BitCount) & 0x3FF]) < 0)
                {
                    Sync = 0;
                    return;
                }
            }

            /* 
        Convert sextet data into channel (m_Position) data
        Every channel is 10-bit, copied from POS in the corresponding packet.
        Every channel is calculated only after the corresponding packet is ready (forth sextet is ready).
    */
            switch (DataCount)
            {
                /* Even frame */
                case 3: /* 4th sextet of first packet is ready */
                    if (DataBuffer[0] >> 4 != 0)
                        ChannelData[2] =
                            (DataBuffer[1] << 4) | (DataBuffer[2] >> 2); /* Ch3: m_Position: packet[6:15] */
                    break;

                case 7: /* 4th sextet of second packet is ready */
                    if (DataBuffer[0] >> 4 != 0)
                        ChannelData[3] =
                            (DataBuffer[5] << 4) | (DataBuffer[6] >> 2); /* Ch4: m_Position: packet[6:15] */
                    ChannelData[9] = ((DataBuffer[20] >> 4) & 1) * 512; /* Ch 10: One of the auxilliary bits */
                    break;

                case 11: /* 4th sextet of 3rd packet is ready */
                    if (DataBuffer[0] >> 4 != 0)
                        ChannelData[4] =
                            (DataBuffer[9] << 4) | (DataBuffer[10] >> 2); /* Ch5: m_Position: packet[6:15] */
                    break;

                case 15: /* 4th sextet of 4th packet is ready */
                    if (DataBuffer[0] >> 4 != 0)
                        ChannelData[6] =
                            (DataBuffer[13] << 4) | (DataBuffer[14] >> 2); /* Ch7: m_Position: packet[6:15] */
                    ChannelData[8] = ((DataBuffer[12] >> 4) & 1) * 512; /* Ch 9: One of the auxilliary bits */
                    Sync = 0; /* End of even frame. Wait for sync */
                    break;

                /* Odd frame */
                case 19: /* 4th sextet of 4th packet is ready */
                    if (DataBuffer[16] >> 4 != 1)
                        ChannelData[1] =
                            (DataBuffer[17] << 4) | (DataBuffer[18] >> 2); /* Ch2: m_Position: packet[6:15] */
                    break;
                case 23: /* 4th sextet of 4th packet is ready */
                    if (DataBuffer[16] >> 4 != 1)
                        ChannelData[0] =
                            (DataBuffer[21] << 4) | (DataBuffer[22] >> 2); /* Ch1: m_Position: packet[6:15] */
                    ChannelData[9] = ((DataBuffer[20] >> 4) & 1) * 512; /* Ch 10: One of the auxilliary bits */
                    break;
                case 27: /* 4th sextet of 4th packet is ready */
                    if (DataBuffer[16] >> 4 != 1)
                        ChannelData[5] =
                            (DataBuffer[25] << 4) | (DataBuffer[26] >> 2); /* Ch6: m_Position: packet[6:15] */
                    break;
                case 31: /* 4th sextet of 4th packet is ready */
                    if (DataBuffer[16] >> 4 != 1)
                        ChannelData[7] =
                            (DataBuffer[29] << 4) | (DataBuffer[30] >> 2); /* Ch8: m_Position: packet[6:15] */
                    ChannelData[8] = ((DataBuffer[28] >> 4) & 1) * 512; /* Ch 9: One of the auxilliary bits */
                    break;
                case 32:
                    Sync = 0; /* End of odd frame. Wait for sync */

                    RawChannelCount = 10; // Fixed number of channels
                    JoystickInteraction.Instance.Send(RawChannelCount, ChannelData, filterChannels, filter);
                    break;
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

                Sync = 0;
                _i = 0;

                DataBuffer = new int[32];
            }
            finally
            {
                Monitor.Exit(MonitorLock);
            }
        }

        #region Futaba PCM helper functions

        #endregion
    }


}