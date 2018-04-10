using System;
using System.Globalization;
using System.Windows.Data;

namespace SharpPropoPlus.ValueConverters
{
    public class FontSizeValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                double v = (double)value;
                return v * 0.6;
            }

            return Binding.DoNothing;
            //double v = (double)value;
            //v = v - 7;
            //if (v > 0)
            //    return v;
            //else
            //    return v;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
