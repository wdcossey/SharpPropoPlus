namespace SharpPropoPlus.Audio.Enums
{
    public enum AudioDeviceState
    {

        /// <summary>
        /// The audio endpoint device is active.
        /// </summary>
        Active = 1,

        /// <summary>
        /// The audio endpoint device is disabled.
        /// </summary>
        Disabled = 2,

        /// <summary>
        /// The audio endpoint device is not present.
        /// </summary>
        NotPresent = 4,

        /// <summary>
        /// The audio endpoint device is unplugged.
        /// </summary>
        UnPlugged = 8,

        /// <summary>
        /// Includes audio endpoint devices in all states—active, 
        /// disabled, not present, and unplugged.
        /// </summary>
        All = 15
    }
}