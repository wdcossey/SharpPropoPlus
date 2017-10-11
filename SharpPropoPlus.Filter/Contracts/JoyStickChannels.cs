namespace SharpPropoPlus.Filter.Contracts
{
    public class JoyStickChannels
    {
        public int[] Data { get; set; }
        public int Count { get; set; }

        public JoyStickChannels()
        {
            
        }

        public JoyStickChannels(int[] data, int count)
            :this()
        {
            Data = data;
            Count = count;
        }
    }
}