using SharpPropoPlus.Contracts;
using SharpPropoPlus.Contracts.Interfaces;
using SharpPropoPlus.Filter.Contracts;

namespace SharpPropoPlus.Filter.Esky.Type2
{
    [ExportPropoPlusFilter("CCPM: E-sky (6ch) Type 2", "E-Sky CCPM 6 channel signal into 4 orthogonal channels")]
    public class Filter : FilterProcessor
    {
        public override string[] Description => new[]
        {
            "Filter for E-sky CCPM (6ch)"
        };


        /*
        Contributed by by Joseph Congiundi

        Convert E-Sky CCPM 6 channel signal into 4 orthogonal channels

        CCPM operation:
        Rudder:	Independent channel.
        Pitch:	Obtained by Front servo going up (or down) while both rear servos going down (or up).
	        Rear servos travel at 50% rate of the front servo.
	        Elevator stick throw obtained from Front servo channel.
        Roll:	Obtained by one rear servo going up and one rear servo going down.
	        Aileron throw obtained from a Rear servo channel with the reduction of the Pitch element.
        Power:	Throttle stick throw obtained from Power channel.
	        When power channel in in the Mid to Max range, it also a Rear servo and the Front servo			
	
        Input 6 channels are - 
            * Channel 1 (inData[0]) - Rear servo 1
            * Channel 2 (inData[1]) - Front servo
            * Channel 3 (inData[2]) - Power 
            * Channel 4 (inData[3]) - Rudder
            * Channel 5 (inData[4]) - Not used
            * Channel 6 (inData[6]) - Rear servo 2

        Output 4 channes are - 
            * Channel 1 (outData[0]) - Rudder
            * Channel 2 (outData[1]) - Throttle
            * Channel 3 (outData[2]) - Elevator 
            * Channel 4 (outData[3]) - Ailerons

        Input:
            channels		Pointer to structure containing CCPM channel data
            channels.Count	Number of input channels (only the first 6 are relevant)
            channels.Data[]	Array of channel values
            min				Minimum possible CCPM channel value
            max				Maximum possible CCPM channel value

        Return:
            Pointer to structure containing orthogonal channel data (4ch)

        */
        protected override IJoystickData Process(IJoystickData channels, int max, int min)
        {
            var inData = new[] {0, 0, 0, 0, 0, 0};
            var outData = new[] {0, 0, 0, 0, 0, 0, 0, 0};

            // Copy input data to input buffer
            for (var i = 0; i < 6; i++)
            {
                inData[i] = channels.Data[i];
            }

            // Rudder
            outData[0] = inData[3];

            // Throttle = Power channel
            outData[1] = inData[2];

            // Elevator: Front servo (Pitch correction when power above mid-point)
            outData[2] = inData[1] - (int) (.35 * inData[2]);

            // Ailerons: Rear servo minus pitch effect (roll correction when power above mid-point)
            outData[3] = (int) (max / 2 + inData[0] + .5 * inData[1] - .53 * inData[2]);

            return new JoystickData(4, outData);
        }
    }
}