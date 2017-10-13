using System;
using System.Collections.Generic;
using System.Linq;
using SharpPropoPlus.Decoder.Contracts;

namespace SharpPropoPlus.Decoder.Ppm
{
    public struct RadrikJitterFilter : IJitterFilter
    {
        private static double _jitterAverage = 0d;

        private static int _jitterPrevValue = 0;

        private int _value;

        public int PreviousValue { get; private set; }

        public int Value
        {
            get => _value;

            private set
            {
                if (_value == value)
                {
                    return;
                }

                PreviousValue = _value;
                _value = value;
            }
        }

        /// <summary>
        /// Advanced jitter filter by: https://github.com/Radrik5
        /// </summary>
        /// <param name="width"></param>
        /// <param name="jitterCeiling"></param>
        /// <param name="alpha"></param> 
        /// <returns></returns>
        public int Filter(int width, double jitterCeiling, double[] alpha = null)
        {
            // Modified Moving Average with differnet ALPHAs
            //
            // Generic formula:
            //   <out> = ((ALPHA - 1) * <out> + <in>) / ALPHA
            //
            // The closer new value to previous value the higher should be ALPHA to remove jitter
            // The farther new value from previous value the lower should be ALPHA to be responsive

            // static const double ALPHA[] = { 8, 6, 4, 3, 2 }; // good but with small jitter
            //double[] alpha = { 16, 12, 6, 3, 2 };

            var diff = Math.Abs(_jitterPrevValue - width);
            var index = Math.Min(Math.Max(diff - 1, 0), alpha.Length - 1);

            _jitterAverage = ((alpha[index] - 1) * _jitterAverage + width) / alpha[index];

            // Go to new value only if the average is farther than 0.75 from the previous value
            // This is done to remove jitter between 10 and 11 when average is around 10.5
            // The average needs to go above 10.75 to switch from 10 to 11
            // and it needs to go below 10.25 to switch from 11 to 10
            var avgDiff = _jitterAverage - _jitterPrevValue;

            var sign = avgDiff < 0 ? -1 : 1;
            _jitterPrevValue += sign * Convert.ToInt32(Math.Abs(avgDiff) + 0.25);

            return Value = _jitterPrevValue;
        }
    }
}