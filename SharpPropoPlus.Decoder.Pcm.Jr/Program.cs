using System;
using System.Threading;
using SharpPropoPlus.Contracts.Enums;
using SharpPropoPlus.Contracts.Interfaces;
using SharpPropoPlus.Decoder.Contracts;

namespace SharpPropoPlus.Decoder.Pcm.JrGraupner
{
    [ExportPropoPlusDecoder("{3206CABB-2FBD-4C24-922E-23B5ECD997DB}", "JR/Graupner", "JR/Graupner (PCM) pulse processor", TransmitterType.Pcm)]
    public class Program : PcmPulseProcessor
    {
        /// <summary>
        /// Number of samples normalized to 192K samples per second
        /// </summary>
        private const double PW_JR = 31.95d;


        private int _i;

        private static readonly int[] JrSymbol = {
            -1,  0, 16, -1, 17, 26, -1, -1, 21, 10,  8, -1, -1, -1, -1, -1,
            23, 14, 12, -1,  4, 27, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            19, 15, 13, -1,  5, 25, -1, -1,  7, 29, 31, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            18, 11,  9, -1,  1, 24, -1, -1,  3, 28, 30, -1, -1, -1, -1, -1,
            2, 20, 22, -1,  6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
        };

        #region PCM Values (Jr)

        #endregion

        public override string[] Description => new[]
        {
            "Pulse processor for JR/Graupner PCM"
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
        /// <param name="filterChannels"></param>
        /// <param name="filter"></param>
        protected override void Process(int width, bool input, bool filterChannels, IPropoPlusFilter filter)
        {
            if (Monitor.IsEntered(MonitorLock))
                return;

            //if (gDebugLevel >= 2 && gCtrlLogFile && !(i++ % 50) && !(_strtime_s(tbuffer, 9)))
            //    fprintf(gCtrlLogFile, "\n%s - ProcessPulseJrPcm(%d)", tbuffer, width);


            if (!Sync && Convert.ToInt32(Math.Floor(2.0 * width / PW_JR + 0.5)) == 5)
            {
                Sync = true;
                if (DataCount >= 8)
                {
                    PosUpdateCounter++;
                }

                BitStream = 0;
                BitCount = -1;
                DataCount = 0;
                return;
            }

            if (!Sync)
            {
                return;
            }

            width = Convert.ToInt32(Math.Floor((double) width / PW_JR + 0.5));
            BitStream = ((BitStream << 1) + 1) << (width - 1);
            BitCount += width;

            if (BitCount >= 8)
            {
                BitCount -= 8;
                if ((DataBuffer[DataCount++] = JrSymbol[(BitStream >> BitCount) & 0xFF]) < 0)
                {
                    for (var dt = 0; dt < DataBuffer.Length; dt++)
                    {
                        DataBuffer[dt] = 0;
                    }

                    Sync = false;
                    return;
                }
            }

            switch (DataCount)
            {
                case 3:
                    ChannelData[2] = 1023 - ((DataBuffer[1] << 5) | DataBuffer[2]);
                    break;
                case 6:
                    ChannelData[0] = 1023 - ((DataBuffer[4] << 5) | DataBuffer[5]);
                    break;
                case 11:
                    ChannelData[5] = 1023 - ((DataBuffer[9] << 5) | DataBuffer[10]);
                    break;
                case 14:
                    ChannelData[7] = 1023 - ((DataBuffer[12] << 5) | DataBuffer[13]);
                    break;
                case 18:
                    ChannelData[3] = 1023 - ((DataBuffer[16] << 5) | DataBuffer[17]);
                    break;
                case 21:
                    ChannelData[1] = 1023 - ((DataBuffer[19] << 5) | DataBuffer[20]);
                    break;
                case 26:
                    ChannelData[4] = 1023 - ((DataBuffer[24] << 5) | DataBuffer[25]);
                    break;
                case 29:
                    ChannelData[6] = 1023 - ((DataBuffer[27] << 5) | DataBuffer[28]);
                    break;
                case 30:
                    Sync = false;

                    RawChannelCount = BufferLength; // Fixed number of channels

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

                _i = 0;

                DataBuffer = new int[30];
            }
            finally
            {
                Monitor.Exit(MonitorLock);
            }
        }

        #region JR/Graupner PCM helper functions

        #endregion
    }


}