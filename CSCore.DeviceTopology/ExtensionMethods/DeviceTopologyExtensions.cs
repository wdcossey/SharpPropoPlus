namespace CSCore.DeviceTopology.ExtensionMethods
{
    public static class DeviceTopologyExtensions
    {
        public static IConnector GetConnector(this IDeviceTopology deviceTopology)
        {
            deviceTopology.GetConnector(0, out var resultConnector);
            return resultConnector;
        }
    }
}