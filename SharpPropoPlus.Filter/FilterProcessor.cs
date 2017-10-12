using SharpPropoPlus.Contracts;
using SharpPropoPlus.Contracts.Constants;
using SharpPropoPlus.Contracts.Interfaces;
using SharpPropoPlus.Filter.Contracts;

namespace SharpPropoPlus.Filter
{
    public abstract class FilterProcessor : IPropoPlusFilter
    {

        public abstract string[] Description { get; }

        public virtual int RunFilter(ref int[] channelData, int channelCount)
        {
            var jsData = new JoystickData(channelCount, channelData) as IJoystickData;
            var result = 0;
            
            var jsFilterOut = Process(jsData, 1023, 0);

            if (jsFilterOut != null && jsFilterOut.Count > 0 &&
                jsFilterOut.Count <= Constants.MAX_JS_CH)
            {
                for (var i = 0; i < jsFilterOut.Count; i++)
                {
                    channelData[i] = jsFilterOut.Data[i];
                    jsFilterOut.Data[i] = 0;
                }

                result = jsFilterOut.Count;
                jsFilterOut.Count = 0;
            }

            return result;
        }

        protected abstract IJoystickData Process(IJoystickData channels, int max, int min);
    }
}