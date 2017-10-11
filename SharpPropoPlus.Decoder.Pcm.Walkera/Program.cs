using SharpPropoPlus.Contracts.Types;
using SharpPropoPlus.Decoder.Contracts;

namespace SharpPropoPlus.Decoder.Pcm.Walkera
{
    [ExportPropoPlusDecoder("Walkera", "Walkera (PCM) pulse processor", TransmitterType.Pcm)]
    public class Program : PcmPulseProcessor
    {

        private int _nPulse; 
        private int[] _cycle;

        #region PCM Values (Walkera)

        #endregion

        public override string[] Description => new[]
        {
            "Pulse processor for Walkera PCM",
            "** Tested with Walkera WK-0701"
        };

        public Program()
        {
            Reset();
            _nPulse = 0;
            _cycle = new int[50];
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
            //const int fixed_n_channel = 8;
            //var m_nChannels = 8;
            int vPulse;
            int[] cs;

            width = (int)(width * 44.1 / 192); // Normalize to 44.1K

            /* Detect Sync pulse - if detected then reset pulse counter and return */
            if (width > 56)
            {
                _nPulse = 1;
                return;
            };


            if (width < 11)
                return;

            /* Even pulses are binary, Odd pulses are Octal */
            if ((_nPulse & 1) != 0)
                vPulse = WalkeraConvert2Bin(width);
            else
                vPulse = WalkeraConvert2Oct(width);
            if (vPulse < 8)
            {
                _cycle[_nPulse] = vPulse;
            }

            _nPulse++;

            /* At the end of the 50-pulse cycle - calculate the channels */
            if (_nPulse == 50)
            {
                /* Channels */
                var Elevator = WalkeraElevator(_cycle);
                var Ailerons = WalkeraAilerons(_cycle);
                var Throttle = WalkeraThrottle(_cycle);
                var Rudder = WalkeraRudder(_cycle);
                var Gear = WalkeraGear(_cycle);
                var Pitch = WalkeraPitch(_cycle);
                var Gyro = WalkeraGyro(_cycle); /* version 3.3.1 */
                var Ch8 = WalkeraChannel8(_cycle); /* version 3.3.1 */

                /* Checksum */
                cs = WalkeraCheckSum(_cycle); /* version 3.3.1 */

                /* Copy data to joystick positions if checksum is valid (ch1-ch4) */
                if (cs[0] == _cycle[21] && cs[1] == _cycle[22])
                {
                    ChannelData[0] = Smooth(ChannelData[0], Elevator);
                    ChannelData[1] = Smooth(ChannelData[1], Ailerons);
                    ChannelData[2] = Smooth(ChannelData[2], Throttle);
                    ChannelData[3] = Smooth(ChannelData[3], Rudder);
                }

                /* Copy data to joystick positions if checksum is valid (ch5-ch8) */
                if (cs[2] == _cycle[47] && cs[3] == _cycle[48])
                {
                    ChannelData[4] = Smooth(ChannelData[4], Gear);
                    ChannelData[5] = Smooth(ChannelData[5], Pitch);
                    ChannelData[6] = Smooth(ChannelData[6], Gyro);
                    ChannelData[7] = Smooth(ChannelData[7], Ch8);
                }

                _nPulse = 0;

                //Send2vJoy(fixed_n_channel, m_Position);
                JoystickInteraction.Instance.Send(RawChannelCount, ChannelData);
            }
        }

        /// <summary>
        /// Resets the static variables.
        /// </summary>
        public sealed override void Reset()
        {
            base.Reset();
        }

        #region Walkera PCM helper functions

        /// <summary>
        /// Convert pulse width to binary value
        /// </summary>
        /// <param name="width"></param>
        /// <returns></returns>
        private static int WalkeraConvert2Bin(int width)
        {
            switch (width)
            {
                case 11:
                case 12:
                case 13:
                case 14:
                    return 0;

                case 19:
                case 20:
                case 21:
                    return 1;

                default:
                    return 0;
            }
        }

        /// <summary>
        /// Convert pulse width to octal value
        /// </summary>
        /// <param name="width"></param>
        /// <returns></returns>
        private static int WalkeraConvert2Oct(int width)
        {
            switch (width)
            {
                case 11:
                case 12:
                case 13:
                case 14:
                    return 0;

                case 17:
                case 18:
                    return 1;

                case 19:
                case 20:
                case 21:
                    return 2;

                case 24:
                case 25:
                    return 3;

                case 27:
                case 28:
                case 29:
                    return 4;

                case 30:
                case 31:
                case 32:
                    return 5;

                case 34:
                case 35:
                case 36:
                    return 6;

                case 38:
                case 39:
                    return 7;

                default:
                    return 8; /* Illegal value */
            }
        }

        /// <summary>
        /// Data: cycle[1:5]
        /// cycle[1]:			0/1 for stick above/below middle point
        /// cycle[2]:			Octal - 0 at middle point(MSB)
        /// cycle[3]:			0/1
        /// cycle[4]:			Octal
        /// cycle[5]:			0/1
        /// MSBit of cycle[6]:	0/1
        /// </summary>
        /// <param name="cycle"></param>
        /// <returns></returns>
        private static int WalkeraElevator(int[] cycle)
        {
            int value;

            value = cycle[2] * 64 + cycle[3] * 32 + cycle[4] * 4 + cycle[5] * 2 + (cycle[6] >> 2);

            /* Mid-point is 511 */
            if (cycle[1] != 0)
                value = 511 - value; /* Below */
            else
                value = 511 + value; /* Above */

            return value;
        }

        /// <summary>
        /// Data: cycle[6:10]
        /// cycle[6]:	Bit[1]: 0/1 for stick Left/Right
        /// cycle[6]:	Bit[0]: (MSB)
        /// cycle[7]:	Binary - 0 at middle point
        /// cycle[8]:	Octal
        /// cycle[9]:	Binary
        /// cycle[10]:	Octal(LSB)
        /// </summary>
        /// <param name="cycle"></param>
        /// <returns></returns>
        private static int WalkeraAilerons(int[] cycle)
        {
            int value, msb;

            msb = cycle[6] & 1;

            /* Offset from mid-point */
            value = msb * 256 + cycle[7] * 128 + cycle[8] * 16 + cycle[9] * 8 + cycle[10];

            /* Mid-point is 511 */
            if ((cycle[6] & 2) != 0)
                value = 511 - value; /* Left */
            else
                value = 511 + value; /* Right */

            return value;
        }


        /// <summary>
        /// Data: cycle[11:15]
        /// cycle[11]:	0/1 for stick above/below middle point
        /// cycle[12]:	Octal - 0 at middle point (MSB)
        /// cycle[13]:	0/1
        /// cycle[14]:	Octal
        /// cycle[15]:	Binary (LSB)
        /// </summary>
        /// <param name="cycle"></param>
        /// <returns></returns>
        private static int WalkeraThrottle(int[] cycle)
        {
            int value;

            value = cycle[12] * 64 + cycle[13] * 32 + cycle[14] * 4 + cycle[15] * 2 + ((cycle[16] & 4) >> 2);

            /* Mid-point is 511 */
            if (cycle[11] != 0)
                value = 511 - value; /* Below */
            else
                value = 511 + value; /* Above */

            return value;
        }

        /// <summary>
        /// Data: cycle[16:21]
        /// cycle[16]:	Bit[1]: 0/1 for stick Left/Right
        /// cycle[16]:	Bit[0]: (MSB)
        /// cycle[17]:	Binary - 0 at middle point
        /// cycle[18]:	Octal
        /// cycle[19]:	Binary
        /// cycle[20]:	Octal(LSB)
        /// </summary>
        /// <param name="cycle"></param>
        /// <returns></returns>
        private static int WalkeraRudder(int[] cycle)
        {
            int value, msb;

            msb = cycle[16] & 1;

            /* Offset from mid-point */
            value = msb * 256 + cycle[17] * 128 + cycle[18] * 16 + cycle[19] * 8 + cycle[20];

            /* Mid-point is 511 */
            if ((cycle[16] & 2) != 0)
                value = 511 - value; /* Left */
            else
                value = 511 + value; /* Right */

            return value;
        }

        /// <summary>
        /// Data: cycle[23]
        /// </summary>
        /// <param name="cycle"></param>
        /// <returns></returns>
        private static int WalkeraGear(int[] cycle)
        {
            int value;

            value = cycle[24] * 64 + cycle[25] * 32 + cycle[26] * 4 + cycle[27] * 2 + ((cycle[28] & 4) >> 2);

            /* Mid-point is 511 */
            if (cycle[23] != 0)
                value = 511 - value; /* Below */
            else
                value = 511 + value; /* Above */

            return value;
        }

        /// <summary>
        /// Data: cycle[29:33]
        /// cycle[28]:	Bit[1]: 0/1 for stick Left/Right
        /// cycle[28]:	Bit[0]: (MSB)
        /// cycle[29]:	Binary - 0 at middle point
        /// cycle[30]:	Octal
        /// cycle[31]:	Binary
        /// cycle[32]:	Octal(LSB)
        /// </summary>
        /// <param name="cycle"></param>
        /// <returns></returns>
        private static int WalkeraPitch(int[] cycle)
        {
            int value, msb;

            msb = cycle[28] & 1;


            /* Offset from mid-point */
            value = msb * 256 + cycle[29] * 128 + cycle[30] * 16 + cycle[31] * 8 + cycle[32];

            /* Mid-point is 511 */
            if ((cycle[28] & 2) != 0)
                value = 511 + value; /* Left */
            else
                value = 511 - value; /* Right */

            return value;
        }

        private static int WalkeraGyro(int[] cycle)
        {
            int value;

            value = cycle[34] * 64 + cycle[35] * 32 + cycle[36] * 4 + cycle[37] * 2 + ((cycle[38] & 4) >> 2);

            /* Mid-point is 511 */
            if (cycle[33] != 0)
                value = 511 - value; /* Below */
            else
                value = 511 + value; /* Above */

            return value;
        }

        private static int WalkeraChannel8(int[] cycle)
        {
            int value, msb;

            msb = cycle[38] & 1;


            /* Offset from mid-point */
            value = msb * 256 + cycle[39] * 128 + cycle[40] * 16 + cycle[41] * 8 + cycle[42];

            /* Mid-point is 511 */
            if ((cycle[38] & 2) != 0)
                value = 511 + value; /* Left */
            else
                value = 511 - value; /* Right */

            return value;
        }

        private static int[] WalkeraCheckSum(int[] cycle)
        {
            int[] checkSum = {-1, -1, -1, -1};

            /* CS2 */
            checkSum[1] = cycle[2] + cycle[4] + cycle[6] + cycle[8] + cycle[10] + cycle[12] + cycle[14] + cycle[16] +
                          cycle[18] + cycle[20];

            /* CS1 */
            checkSum[0] = cycle[1] + cycle[3] + cycle[5] + cycle[7] + cycle[9] + cycle[11] + cycle[13] + cycle[15] +
                          cycle[17] + cycle[19] + (checkSum[1] >> 3);

            /* CS4 */
            checkSum[3] = cycle[24] + cycle[26] + cycle[28] + cycle[30] + cycle[32] + cycle[34] + cycle[36] +
                          cycle[38] +
                          cycle[40] + cycle[42] + cycle[44] + cycle[46];

            /* CS3 */
            checkSum[2] = cycle[23] + cycle[25] + cycle[27] + cycle[29] + cycle[31] + cycle[33] + cycle[35] +
                          cycle[37] +
                          cycle[39] + cycle[41] + cycle[43] + cycle[45] + (checkSum[3] >> 3);

            checkSum[0] &= 0x1;
            checkSum[1] &= 0x7;

            checkSum[2] &= 0x1;
            checkSum[3] &= 0x7;

            return checkSum;
        }

        #endregion
    }


}