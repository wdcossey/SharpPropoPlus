namespace SharpPropoPlus.Decoder.Structs
{
  public struct PulseLength
  {
    public int Raw { get; set; }
    public int Normalized { get; set; }

    public PulseLength(int raw, int normalized)
      : this()
    {
      Raw = raw;
      Normalized = normalized;
    }
  }
}