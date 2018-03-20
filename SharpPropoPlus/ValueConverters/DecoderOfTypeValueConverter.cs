using System;
using System.Globalization;
using System.Windows.Data;
using SharpPropoPlus.Contracts.Enums;
using SharpPropoPlus.Contracts.Interfaces;

namespace SharpPropoPlus.ValueConverters
{
    [ValueConversion(typeof(IDecoderMetadata), typeof(bool))]
    public class DecoderOfTypeValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is IDecoderMetadata metaData) || !(parameter is TransmitterType type))
            {
                return false;
            }

            return metaData.TransmitterType == type;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}