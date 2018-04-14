using SharpPropoPlus.Contracts.Interfaces;
using SharpPropoPlus.Decoder.Structs;

namespace SharpPropoPlus.Decoder.Pcm
{
    public abstract class PcmPulseProcessor : PulseProcessor<JitterFilter>
    {
        #region PCM Values (General)


        #endregion

        private int _bit;

        private int _bitCount;

        private int _bitStream;

        protected abstract override void Process(int width, bool input, bool filterChannels, IPropoPlusFilter filter);

        public abstract override string[] Description { get; }

        protected int Bit
        {
            get => _bit;
            set => _bit = value;
        }

        protected int BitCount
        {
            get => _bitCount;
            set => _bitCount = value;
        }

        protected int BitStream
        {
            get => _bitStream;
            set => _bitStream = value;
        }

        public override void Reset()
        {
            RawChannelCount = BufferLength;
            Sync = false;
            DataBuffer = new int[BufferLength]; /* Array of pulse widthes in joystick values */
            DataCount = 0; /* pulse index (corresponds to channel index) */
            FormerSync = false;
            ChannelData = new int[BufferLength];

            #region PCM
            Bit = 0;
            BitCount = 0;
            BitStream = 0;
            #endregion

            PosUpdateCounter = 0;

            //static int i = 0;

            //PrevWidth not required for PCM
            //PrevWidth = new JitterFilter[BufferLength]; /* array of previous width values */
        }

        /// <summary>
        /// smooth
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="newval"></param>
        /// <returns></returns>
        protected int Smooth(int orig, int newval)
        {
            if ((orig - newval > 100) || (newval - orig > 100))
                return (newval + orig) / 2;

            return newval;

        }

        

    }
}
