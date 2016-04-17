using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Data;
using System.Windows.Media;
using SharpPropoPlus.Audio.Enums;
using SharpPropoPlus.Enums;
using SharpPropoPlus.ExtensionMethods;

namespace SharpPropoPlus.ValueConverters
{

  [ValueConversion(typeof (Color), typeof (Color), ParameterType = typeof(ImmersiveColors))]
  public class SystemColorValueConverter : IValueConverter
  {

    #region Imports

    [DllImport("uxtheme.dll", EntryPoint = "#98", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)
    ]
    private static extern UInt32 GetImmersiveUserColorSetPreference(Boolean forceCheckRegistry, Boolean skipCheckOnFail);

    [DllImport("uxtheme.dll", EntryPoint = "#94", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)
    ]
    private static extern UInt32 GetImmersiveColorSetCount();

    [DllImport("uxtheme.dll", EntryPoint = "#95", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)
    ]
    private static extern UInt32 GetImmersiveColorFromColorSetEx(UInt32 immersiveColorSet, UInt32 immersiveColorType,
      Boolean ignoreHighContrast, UInt32 highContrastCacheMode);

    [DllImport("uxtheme.dll", EntryPoint = "#96", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)
    ]
    private static extern UInt32 GetImmersiveColorTypeFromName(IntPtr name);

    [DllImport("uxtheme.dll", EntryPoint = "#100", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto
      )]
    private static extern IntPtr GetImmersiveColorNamedTypeByIndex(UInt32 index);

    #endregion



    /// <summary>
    /// Gets the color.
    /// https://github.com/RoliSoft/RS-TV-Show-Tracker/commit/54ce20e3b7299038874cf698bfb25ef2bfab9ed6
    /// </summary>
    /// <param name="immersiveColor">Color of the immersive.</param>
    /// <returns>Color.</returns>
    private static Color GetColor(ImmersiveColors immersiveColor)
    {
      //this.AccentColorResultTextBox,	ImmersiveColors.ImmersiveStartSelectionBackground
      //this.MainColorResultTextBox,		ImmersiveColors.ImmersiveStartPrimaryText
      //this.BackgroundColorResultTextBox,ImmersiveColors.ImmersiveStartBackground
      IntPtr pElementName = Marshal.StringToHGlobalUni(immersiveColor.ToString());
      var colourset = GetImmersiveUserColorSetPreference(false, false);
      uint type = GetImmersiveColorTypeFromName(pElementName);
      Marshal.FreeCoTaskMem(pElementName);
      uint colourdword = GetImmersiveColorFromColorSetEx((uint) colourset, type, false, 0);
      byte[] colourbytes = new byte[4];
      colourbytes[0] = (byte) ((0xFF000000 & colourdword) >> 24); // A
      colourbytes[1] = (byte) ((0x00FF0000 & colourdword) >> 16); // B
      colourbytes[2] = (byte) ((0x0000FF00 & colourdword) >> 8); // G
      colourbytes[3] = (byte) (0x000000FF & colourdword); // R
      Color color = Color.FromArgb(colourbytes[0], colourbytes[3], colourbytes[2], colourbytes[1]);
      return color;
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (!(value is Color))
        return null;

      try
      {
        return GetColor((ImmersiveColors)parameter);
        //return GetColor(ImmersiveColors.ImmersiveStartSelectionBackground);
      }
      catch (Exception)
      {

        return value;
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return value;
    }
  }
}