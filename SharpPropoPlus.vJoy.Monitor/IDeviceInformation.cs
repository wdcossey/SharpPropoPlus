using System;

namespace SharpPropoPlus.vJoyMonitor
{
    public interface IDeviceInformation
    {
        int Id { get; }
        VjdStat Status { get; }
        string Name { get; }
        Guid Guid { get; }
    }
}