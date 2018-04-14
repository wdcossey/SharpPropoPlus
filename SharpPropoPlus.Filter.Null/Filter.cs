using SharpPropoPlus.Contracts;
using SharpPropoPlus.Contracts.Interfaces;
using SharpPropoPlus.Filter.Contracts;

namespace SharpPropoPlus.Filter.Null
{
    [ExportPropoPlusFilter("{20E5C6FC-C948-4E21-B14B-DDE2D03632A9}", "NULL Filter (does nothing)", "NULL Filter, 12 channel limit")]
    public class Filter : FilterProcessor
    {
        public override string[] Description => new[]
        {
            "NULL Filter (does nothing), 12 channel limit"
        };

        /*
        NULL Filter
        */
        protected override IJoystickData Process(IJoystickData channels, int max, int min)
        {
            var inData = new[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
            var outData = new[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};

            // Copy input data to input buffer
            var limit = 12;
            if (channels.Count < limit)
            {
                limit = channels.Count;
            }

            for (var i = 0; i < limit; i++)
            {
                outData[i] = channels.Data[i];
            }

            return new JoystickData(channels.Count, outData);
        }
    }
}
