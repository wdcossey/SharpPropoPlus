using SharpPropoPlus.Contracts.Types;

namespace SharpPropoPlus.Contracts.Interfaces
{
    public interface IDecoderMetadata : IPropoMetadata
    {
        TransmitterType TransmitterType { get; }
    }
}