using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpPropoPlus.Contracts;
using SharpPropoPlus.Contracts.Interfaces;
using SharpPropoPlus.Filter.Contracts;

namespace SharpPropoPlus.Filter.DeltaWing
{
    [ExportPropoPlusFilter("{7409790E-4BC6-42CA-B390-20F24DF672DF}", "Delta Wing (Ch1+Ch2)", "Delta wing un-mixing (mixed channels 1 and 2)")]
    public class Filter : FilterProcessor
    {
        private static readonly int[] InData = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private static readonly int[] OutData = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public override string[] Description => new[]
        {
            "Filter for Delta wing mixed channels 1 and 2"
        };

        /*
	    Delta wing un-mixing
	    Take the data going to both wing servos and extract the orthogonal axes.
	    No rudder.
	    It is assumed that the mixed channels are ch1 and ch2
	    All other channels do not change
        */
        protected override IJoystickData Process(IJoystickData channels, int max, int min)
        {
            // Copy input data to input buffer
            for (var i = 0; i < 6; i++)
            {
                InData[i] = channels.Data[i];
            }

            if (channels.Count >= 2)
            {
                var ailerons = (max - min) / 2 + (InData[0] - InData[1]) / 4;
                var elevator = (InData[0] + InData[1]) / 2;

                OutData[0] = elevator;
                OutData[1] = ailerons;
                OutData[2] = InData[2];
                OutData[3] = InData[3];
                OutData[4] = InData[4];
                OutData[5] = InData[5];
                OutData[6] = InData[6];
                OutData[7] = InData[7];
                OutData[8] = InData[8];
                OutData[9] = InData[9];
                OutData[10] = InData[10];
                OutData[11] = InData[11];
            }

            return new JoystickData(channels.Count, OutData);
        }
    }
}
