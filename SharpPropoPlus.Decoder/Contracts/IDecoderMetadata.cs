using SharpPropoPlus.Decoder.Enums;

namespace SharpPropoPlus.Decoder.Contracts
{
  public interface IDecoderMetadata
  {
    string Name { get; }

    string Description { get; }

    TransmitterType Type { get; }
  }
}