using System;
using SharpPropoPlus.Decoder.Contracts;
using SharpPropoPlus.Decoder.Enums;

namespace SharpPropoPlus.Decoder.EventArguments
{
    public class DecoderChangedEventArgs : EventArgs
    {
        internal DecoderChangedEventArgs()
        {

        }

        public DecoderChangedEventArgs(string name, string description, TransmitterType transmitterType, IPropoPlusDecoder decoder)
            : this()
        {
            Decoder = decoder;
            Name = name;
            Description = description;
            TransmitterType = transmitterType;
        }

        public string Name { get; }

        public string Description { get; }

        public TransmitterType TransmitterType { get; }

        public IPropoPlusDecoder Decoder { get; }
    }
}