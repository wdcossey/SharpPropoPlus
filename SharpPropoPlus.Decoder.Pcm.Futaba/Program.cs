using System;
using SharpPropoPlus.Decoder.Contracts;
using SharpPropoPlus.Decoder.Enums;

namespace SharpPropoPlus.Decoder.Pcm.Futaba
{
    [ExportPropoPlusDecoder("Futaba", "Futaba (PCM) pulse processor", TransmitterType.Pcm)]
    public class Program : PcmPulseProcessor
    {

        #region PCM Values (Futaba)

        #endregion

        public override string[] Description
        {
            get
            {
                return new[]
                {
                    "Pulse processor for Futaba PCM"
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
        }

        #region Futaba PCM helper functions

        #endregion
    }


}