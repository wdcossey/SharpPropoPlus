using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpPropoPlus.Filter.Contracts;

namespace SharpPropoPlus.Filter.VTail
{
    [ExportPropoPlusFilter("V-Tail (Ch1+Ch4)", "V-Tail un-mixing (mixed channels 1 and 4)")]
    public class Filter : FilterProcessor
    {
        public override string[] Description => new[]
        {
            "Filter for V-Tail mixed channels 1 and 4"
        };


        /*
        V-Tail un-mixing
        Take the data going to both tail servos and extract the orthogonal axes.
        It is assumed that the mixed channels are ch1 and ch4
        All other channels do not change
        */
        protected override JoyStickChannels Process(JoyStickChannels channels, int max, int min)
        {
            var inData = new[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
            var outData = new[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};

            // Copy input data to input buffer
            for (var i = 0; i < 6; i++)
            {
                inData[i] = channels.Data[i];
            }

            if (channels.Count >= 4)
            {
                var rudder = (max - min) / 2 + (inData[0] - inData[3]) / 4;
                var elevator = (inData[0] + inData[3]) / 2;

                outData[0] = elevator;
                outData[1] = inData[1];
                outData[2] = inData[2];
                outData[3] = rudder;
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
