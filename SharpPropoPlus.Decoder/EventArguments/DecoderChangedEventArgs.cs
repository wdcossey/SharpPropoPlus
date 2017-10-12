using System;
using SharpPropoPlus.Contracts.Interfaces;
using SharpPropoPlus.Contracts.Types;
using SharpPropoPlus.Decoder.Contracts;

namespace SharpPropoPlus.Decoder.EventArguments
{
    public class DecoderChangedEventArgs : EventArgs
    {
        private DecoderChangedEventArgs()
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