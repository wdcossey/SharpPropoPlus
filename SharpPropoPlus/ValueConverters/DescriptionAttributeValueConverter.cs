using System;
using System.Globalization;
using System.Windows.Data;
using SharpPropoPlus.ExtensionMethods;

namespace SharpPropoPlus.ValueConverters
{
    [ValueConversion(typeof(object), typeof(string))]
    public class DescriptionAttributeValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.GetDescription();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}