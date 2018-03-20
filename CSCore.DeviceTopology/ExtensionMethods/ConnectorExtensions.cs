namespace CSCore.DeviceTopology.ExtensionMethods
{
    public static class ConnectorExtensions
    {
        public static IConnector GetConnectedTo(this IConnector connector)
        {
            connector.GetConnectedTo(out var resultConnector);
            return resultConnector;
        }
        public static IPart GetConnectedToAsPart(this IConnector connector)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            return connector.GetConnectedTo() as IPart;
        }
    }
}