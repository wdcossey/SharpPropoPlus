namespace SharpPropoPlus.Audio.Interfaces
{
    public interface IAudioEndPoint
    {
        string DeviceName { get; }
        string DeviceId { get; }
        int Channels { get; }
        bool Disabled { get; }
        int? JackColor { get; }
    }
}