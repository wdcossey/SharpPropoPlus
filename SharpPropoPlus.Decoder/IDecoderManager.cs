using System;
using System.Collections.Generic;
using SharpPropoPlus.Contracts.Interfaces;
using SharpPropoPlus.Decoder.Contracts;

namespace SharpPropoPlus.Decoder
{
    public interface IDecoderManager : IDisposable
    {
        IEnumerable<Lazy<IPropoPlusDecoder, IDecoderMetadata>> Decoders { get; }

        IPropoPlusDecoder Decoder { get; }

        IDecoderMetadata GetDecoderMetadata(IPropoPlusDecoder decoder);

        void ChangeDecoder(IPropoPlusDecoder decoder);

        void Notify();
    }
}