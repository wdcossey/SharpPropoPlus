namespace SharpPropoPlus.Decoder.Contracts
{
    public interface IJitterFilter
    {
        int PreviousValue { get; }

        int Value { get; }
        
        int Filter(int width, double jitterCeiling, double[] alpha = null);
    }
}