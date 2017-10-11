using SharpPropoPlus.Contracts;
using SharpPropoPlus.Filter.Contracts;

namespace SharpPropoPlus.Filter
{
    public abstract class FilterProcessor : IPropoPlusFilter
    {

        public abstract string[] Description { get; }

        public virtual int RunFilter(ref int[] channelData, int channelCount)
        {
            var jsData = new JoyStickChannels(channelData, channelCount);
            var result = 0;
            
            var jsFilterOut = Process(jsData, 1023, 0);

            if (jsFilterOut == null || jsFilterOut.Count <= 0 ||
                jsFilterOut.Data.Length > Constants.MAX_JS_CH)
            {
                return result;
            }

            for (var i = 0; i < jsFilterOut.Data.Length; i++)
            {
                channelData[i] = jsFilterOut.Data[i];
                jsFilterOut.Data[i] = 0;
            }

            result = jsFilterOut.Count;

            jsFilterOut.Count = 0;

            return result;
        }

        protected abstract JoyStickChannels Process(JoyStickChannels channels, int max, int min);
    }
}