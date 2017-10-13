using System;
using SharpPropoPlus.Decoder.Contracts;

namespace SharpPropoPlus.Decoder.Structs
{
    public struct JitterFilter : IJitterFilter
    {
        private int _value;

        public int PreviousValue { get; private set; }

        public int Value
        {
            get => _value;

            private set
            {
                if (_value == value)
                    return;

                PreviousValue = _value;
                _value = value;
            }
        }

        public int Filter(int width, double jitterCeiling, double[] alpha = null)
        {
            var jitterValue = Math.Abs(Value - width);

            if (jitterValue < jitterCeiling)
                return Value;

            return Value = width;
        }
    }
}