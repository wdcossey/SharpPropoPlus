namespace SharpPropoPlus.Decoder.Contracts
{
  public interface IPropoPlusDecoder
  {
    string[] Description { get; }

    /// <summary>
    /// Processes the Pulse data.
    /// </summary>
    void ProcessPulse(int sampleRate, int sample);

    /// <summary>
    /// Resets the static variables.
    /// </summary>
    void Reset();
  }
}