using System;

namespace CSCore.DeviceTopology.ExtensionMethods
{
    public static class JackDescriptionExtensions
    {
        public static uint GetJackCount(this IKsJackDescription ksJackDescription)
        {
            ksJackDescription.GetJackCount(out var count);

            return count;
        }

        public static KSJACK_DESCRIPTION GetJackDescription(this IKsJackDescription ksJackDescription, uint index)
        {
            ksJackDescription.GetJackDescription(index, out var ksjackDescription);
            return ksjackDescription;
        }
    }
}