using SharpPropoPlus.Filter.Contracts;

namespace SharpPropoPlus.Filter.Esky.ThreeD
{
    [ExportPropoPlusFilter("CCPM: E-sky (6ch) 3D",
        "E-Sky CCPM 6 channel signal into 4 orthogonal channels and a 5th for blade pitch")]
    public class Filter : FilterProcessor
    {
        public override string[] Description => new[]
        {
            "Filter for E-sky CCPM (6ch) 3D"
        };

        /*
        Contributed by Matthew Morrison

        Convert E-Sky CCPM 6 channel signal into 4 orthogonal channels and a 5th for blade pitch

        CCPM operation:
        Rudder: Independent channel.
        Elevator: Obtained by Front servo going up (or down) while both rear servos going down (or up).
        Roll: Obtained by one rear servo going up and one rear servo going down.
        Power: Throttle stick throw obtained from Power channel.
        Input 6 channels are - 
            * Channel 1 (inData[0]) - Rear servo 1
            * Channel 2 (inData[1]) - Front servo
            * Channel 3 (inData[2]) - Power 
            * Channel 4 (inData[3]) - Rudder
            * Channel 5 (inData[4]) - Not used
            * Channel 6 (inData[6]) - Rear servo 2

        Output 5 channels are - 
            * Channel 1 (outData[0]) - Rudder
            * Channel 2 (outData[1]) - Throttle
            * Channel 3 (outData[2]) - Elevator 
            * Channel 4 (outData[3]) - Ailerons
            * Channel 5 (outData[4]) - Pitch
        
        Input:
            channels Pointer to structure containing CCPM channel data
            channels.Count Number of input channels (only the first 6 are relevant)
            channels.Data[] Array of channel values
            min Minimum possible CCPM channel value
            max Maximum possible CCPM channel value

        Return:
            Pointer to structure containing orthogonal channel data (5ch)

        */
        protected override JoyStickChannels Process(JoyStickChannels channels, int max, int min)
        {
            var inData = new[] {0, 0, 0, 0, 0, 0};
            var outData = new[] {0, 0, 0, 0, 0, 0, 0, 0};

            var servo0 = max - inData[0]; // Servo 0 needs to be inverted

            // Copy input data to input buffer
            for (var i = 0; i < 6; i++)
            {
                inData[i] = channels.Data[i];
            }

            // Rudder
            outData[0] = inData[3];

            // Throttle = Power channel
            outData[1] = inData[2];

            // Pitch: average of all servos compared to the max
            outData[4] = (int) (servo0 + inData[5] + inData[1]) / 3;

            // Roll: the difference between the rear servos compared to the midpoint
            outData[3] = (max / 2 + inData[5] - servo0) / 2;

            // Elevator: the difference between the front servo and the mean of the rear servos
            outData[2] = max / 2 + (inData[1] - (servo0 + inData[5]) / 2) / 2;

            return new JoyStickChannels(outData, 5);
        }
    }
}