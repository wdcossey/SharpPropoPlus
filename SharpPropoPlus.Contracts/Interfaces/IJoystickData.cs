namespace SharpPropoPlus.Contracts.Interfaces
{
    public interface IJoystickData
    {
        int[] Data { get; }
        int Count { get; set; }
    }
}