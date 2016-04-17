using System;
using System.Globalization;
using System.Windows.Data;
using SharpPropoPlus.Audio.Enums;
using SharpPropoPlus.ExtensionMethods;

namespace SharpPropoPlus.ValueConverters
{
  [ValueConversion(typeof (AudioChannel), typeof (string))]
  public class AudioChannelValueConverter : IValueConverter
  {

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (!(value.GetType() == typeof (AudioChannel)))
        return null;

      return ((AudioChannel) value).GetDescription();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return null;
    }
  }
}