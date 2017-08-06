using System;
using SharpPropoPlus.Decoder.Contracts;
using SharpPropoPlus.Decoder.Enums;

namespace SharpPropoPlus.Decoder.Pcm.Walkera
{
    [ExportPropoPlusDecoder("Walkera", "Walkera (PCM) pulse processor", TransmitterType.Pcm)]
    public class Program : PcmPulseProcessor
    {

        #region PCM Values (Walkera)

        #endregion 

        public override string[] Description
        {
            get
            {
                return new[]
                {
                    "Pulse processor for Walkera PCM",
                    "** Tested with Walkera WK-0701"
                };
            }
        }

        public Program()
        {
            Reset();
        }

        /// <summary>
        /// <para>Buffer Length/Size</para>
        /// <para>Default: 14</para>
        /// </summary>
        protected override int BufferLength => 8;

        /// <summary>
        /// <para>Process Walkera PCM pulse (Tested with Walkera WK-0701)</para>
        /// <para></para>
        /// </summary>
        /// <param name="width"></param>
        /// <param name="input"></param>
        protected override void Process(int width, bool input)
        {
            
        }

        /// <summary>
        /// Resets the static variables.
        /// </summary>
        public sealed override void Reset()
        {
            Sync = false;
            DataBuffer = new int[BufferLength]; /* Array of pulse widthes in joystick values */
            DataCount = 0; /* pulse index (corresponds to channel index) */
            FormerSync = false;
            ChannelData = new int[BufferLength];

            //static int i = 0;
            PrevWidth = new int[BufferLength]; /* array of previous width values */
        }


    }


}