using System;
using System.Globalization;
using System.Windows.Data;
using SharpPropoPlus.Audio.Enums;
using SharpPropoPlus.ExtensionMethods;

namespace SharpPropoPlus.ValueConverters
{
  [ValueConversion(typeof (AudioBitrate), typeof (string))]
  public class AudioBitrateValueConverter : IValueConverter
  {

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (!(value.GetType() == typeof (AudioBitrate)))
        return null;

      return ((AudioBitrate) value).GetDescription();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return null;
    }
  }
}