using System;
using SharpPropoPlus.Decoder.Contracts;
using SharpPropoPlus.Decoder.Enums;

namespace SharpPropoPlus.Decoder.Pcm.AirtronicsSanwaSecond
{
    [ExportPropoPlusDecoder("Airtronics/Sanwa [2]", "Airtronics/Sanwa [2] (PCM) pulse processor", TransmitterType.Pcm)]
    public class Program : PcmPulseProcessor
    {

        #region PCM Values (Airtronics/Sanwa [2])

        #endregion

        public override string[] Description
        {
            get
            {
                return new[]
                {
                    "Pulse processor for Airtronics/Sanwa [2] PCM"
                };
            }
        }

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
            
        }

        /// <summary>
        /// Resets the static variables.
        /// </summary>
        public sealed override void Reset()
        {
            base.Reset();
            DataBuffer = new int[10];
        }

        #region Airtronics/Sanwa [2] PCM helper functions



        #endregion
    }


}