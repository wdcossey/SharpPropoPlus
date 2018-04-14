using SharpPropoPlus.Enums;

namespace SharpPropoPlus.Interfaces
{
    public interface IJoystickChannelData : IChannelData
    {
        JoystickChannel Channel { get; }
        string Title { get; set; }
        string Description { get; set; }
    }
}