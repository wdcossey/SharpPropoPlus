using SharpPropoPlus.Decoder.Contracts;
using SharpPropoPlus.Decoder.Enums;

namespace SharpPropoPlus.Decoder.EventArguments
{
    public class DecoderChangedEventArgs
    {

        internal DecoderChangedEventArgs()
        {
            
        }

        public DecoderChangedEventArgs(string name, string description, TransmitterType transmitterType)
            : this()
        {
            Name = name;
            Description = description;
            TransmitterType = transmitterType;
        }

        public string Name { get; }

        public string Description { get; }

        public TransmitterType TransmitterType { get; }
    }
}