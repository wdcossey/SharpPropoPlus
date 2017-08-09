using SharpPropoPlus.Decoder.Contracts;

namespace SharpPropoPlus.Decoder.Pcm
{
    public abstract class PcmPulseProcessor : PulseProcessor, IPropoPlusDecoder
    {
        #region PCM Values (General)


        #endregion

        private int _bit;

        private int _bitCount;

        private int _bitStream;

        protected abstract void Process(int width, bool input);

        public abstract string[] Description { get; }

        protected int Bit
        {
            get { return _bit; }
            set { _bit = value; }
        }

        protected int BitCount
        {
            get { return _bitCount; }
            set { _bitCount = value; }
        }

        protected int BitStream
        {
            get { return _bitStream; }
            set { _bitStream = value; }
        }

        public virtual void ProcessPulse(int sampleRate, int sample)
        {
            var negative = false;

            var pulseLength = CalculatePulseLength(sampleRate, sample, ref negative);

            Process(pulseLength.Normalized, negative);
        }

        public virtual void Reset()
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
            PrevWidth = new int[BufferLength]; /* array of previous width values */
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
