using SharpPropoPlus.Contracts.Enums;

namespace SharpPropoPlus.Contracts.Interfaces
{
    public interface IDecoderMetadata : IPropoMetadata
    {
        TransmitterType TransmitterType { get; }
    }
}