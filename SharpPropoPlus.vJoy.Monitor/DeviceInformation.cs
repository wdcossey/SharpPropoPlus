using System;

namespace SharpPropoPlus.vJoyMonitor
{
    public class DeviceInformation
    {
        private readonly string _name;

        internal DeviceInformation()
        {

        }

        public DeviceInformation(int deviceId, string name, Guid productGuid, VjdStat status)
        {
            Id = deviceId;
            _name = name;
            Guid = productGuid;
            Status = status;
        }

        public int Id { get; }

        public VjdStat Status { get; }

        public string Name => $"{_name} #{Id}";

        public Guid Guid { get; }
    }
}