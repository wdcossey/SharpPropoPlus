using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using SharpPropoPlus.Enums;

namespace SharpPropoPlus.ValueConverters
{
    [ValueConversion(typeof(int), typeof(SolidColorBrush))]
    public class IntegerSolidColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        { 

            if (!(value is int))
                return null;

            try
            {
                var bytes = BitConverter.GetBytes((int)value);
                return new SolidColorBrush(Color.FromArgb(byte.MaxValue, bytes[2], bytes[1], bytes[0]));
            }
            catch (Exception)
            {

                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}