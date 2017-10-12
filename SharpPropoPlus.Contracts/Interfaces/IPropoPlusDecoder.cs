namespace SharpPropoPlus.Contracts.Interfaces
{
    public interface IPropoPlusDecoder
    {
        string[] Description { get; }

        /// <summary>
        /// Processes the Pulse data.
        /// </summary>
        void ProcessPulse(int sampleRate, int sample, bool filterChannels, IPropoPlusFilter filter);

        /// <summary>
        /// Resets the static variables.
        /// </summary>
        void Reset();
    }
}