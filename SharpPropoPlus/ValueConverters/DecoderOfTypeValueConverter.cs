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
            var metaData = value as IDecoderMetadata;
            var type = parameter as TransmitterType?;

            if (metaData == null || !type.HasValue)
            {
                return false;
            }

            return metaData.TransmitterType == type.Value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}