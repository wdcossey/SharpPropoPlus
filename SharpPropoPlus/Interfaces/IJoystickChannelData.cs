using SharpPropoPlus.Enums;

namespace SharpPropoPlus.Interfaces
{
    public interface IJoystickChannelData
    {
        JoystickChannel Channel { get; set; }
        int Value { get; set; }
        string Title { get; set; }
        string Description { get; set; }

        void SetValue(int value);
    }
}