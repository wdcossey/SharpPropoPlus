using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpPropoPlus.Filter.Contracts;

namespace SharpPropoPlus.Filter.DeltaWing
{
    [ExportPropoPlusFilter("Delta Wing (Ch1+Ch2)", "Delta wing un-mixing (mixed channels 1 and 2)")]
    public class Filter : FilterProcessor
    {
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
        protected override JoyStickChannels Process(JoyStickChannels channels, int max, int min)
        {
            var inData = new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            var outData = new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            // Copy input data to input buffer
            for (var i = 0; i < 6; i++)
            {
                inData[i] = channels.Data[i];
            }

            if (channels.Count >= 2)
            {
                var ailerons = (max - min) / 2 + (inData[0] - inData[1]) / 4;
                var elevator = (inData[0] + inData[1]) / 2;

                outData[0] = elevator;
                outData[1] = ailerons;
                outData[2] = inData[2];
                outData[3] = inData[3];
                outData[4] = inData[4];
                outData[5] = inData[5];
                outData[6] = inData[6];
                outData[7] = inData[7];
                outData[8] = inData[8];
                outData[9] = inData[9];
                outData[10] = inData[10];
                outData[11] = inData[11];
            }

            return new JoyStickChannels(outData, channels.Count);
        }
    }
}
