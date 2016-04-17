namespace SharpPropoPlus.Decoder.Structs
{
  public struct PulseResult
  {
    private int _channels;

    public int Channels => _channels;

    public PulseResult(int channels)
      :this()
    {
      _channels = channels;
    }

  }
}