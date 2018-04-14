namespace SharpPropoPlus.Interfaces
{
    public interface IChannelData
    {
        int Value { get; }
        string ToolTip { get; }
        void SetValue(int value);
    }
}