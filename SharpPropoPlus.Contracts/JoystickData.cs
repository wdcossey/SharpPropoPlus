using SharpPropoPlus.Contracts.Interfaces;

namespace SharpPropoPlus.Contracts
{
    public class JoystickData : IJoystickData
    {
        public int[] Data { get; }
        public int Count { get; set; }

        private JoystickData()
        {

        }

        public JoystickData(int count, int[] data)
            :this()
        {
            Data = data;
            Count = count;
        }
    }
}