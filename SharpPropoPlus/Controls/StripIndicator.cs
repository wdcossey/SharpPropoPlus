
/* 
  Source: http://www.codeproject.com/Articles/814902/The-Problem-of-Unwanted-WPF-ProgressBar-Animation
*/

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SharpPropoPlus.Controls
{
  public partial class StripIndicator : ContentControl
  {

    static class DefinitionSet
    {
      internal const double DefaultSize = 20;
      internal const double DefaultMaximumValue = 100;
      internal const double BorderThickness = 0.6;
      internal const double ThresholdThickness = 2.3;
      internal static readonly Brush Border = Brushes.Black;
      //internal static readonly Brush FocusedBorder = Brushes.LightSteelBlue;
      internal static readonly Brush ThresholdFill = Brushes.Black;
      internal static readonly Brush StripFill = Brushes.Blue;
      //it actually should depend on Maximum:
      //internal const uint StepVerySlow = 1;
      //internal const uint StepSlow = 4;
      //internal const uint StepFast = 10;
      //internal const uint StepFastest = 20;
    } //class DefinitionSet

    public StripIndicator()
    {
      Grid grid = new Grid();
      border.VerticalAlignment = VerticalAlignment.Stretch;
      border.HorizontalAlignment = HorizontalAlignment.Stretch;
      border.BorderThickness = new Thickness(DefinitionSet.BorderThickness);
      border.BorderBrush = this.Background;
      border.Child = grid;
      this.Content = border;
      thresholdRectangle.Fill = DefinitionSet.ThresholdFill;
      grid.Children.Add(valueRectangle);
      grid.Children.Add(thresholdRectangle);
      this.Foreground = DefinitionSet.StripFill;
      grid.Background = Brushes.Transparent;
      Focusable = true;
      Threshold = double.PositiveInfinity;
    } //Indicator
    Border border = new Border();

    public double Maximum
    {
      get { return maximum; }
      set
      {
        if (value == maximum) return;
        maximum = value;
        Refresh();
      } //set Maximum
    } //Maximum
    public double Minimum
    {
      get { return minimum; }
      set
      {
        if (value == minimum) return;
        minimum = value;
        Refresh();
      } //set Maximum
    } //Maximum
    public double Value
    {
      get { return currentValue; }
      set
      {
        if (value == currentValue) return;
        currentValue = value;
        Refresh();
      } //set Value
    } //Value
    public double Threshold
    {
      get { return thresholdValue; }
      set
      {
        if (value == thresholdValue) return;
        thresholdValue = value;
        if (thresholdValue >= maximum && !double.IsInfinity(thresholdValue))
          thresholdValue = maximum;
        if (thresholdValue <= minimum)
          thresholdValue = minimum;
        Refresh();
      } //set Value
    } //Threshold
    public Orientation Orientation
    {
      get { return orientation; }
      set
      {
        if (value == orientation) return;
        orientation = value;
        Refresh();
      } //set Value
    } //Orientation

    void Refresh()
    {
      System.Func<double, bool> isNormalValue = (value) => {
        return !(value < 0 || double.IsPositiveInfinity(value) || double.IsNaN(value));
      };
      double fullSize;
      bool usingThreshold = thresholdValue <= maximum;
      if (orientation == Orientation.Horizontal)
        fullSize = this.ActualWidth - 2 * DefinitionSet.BorderThickness;
      else
        fullSize = this.ActualHeight - 2 * DefinitionSet.BorderThickness;
      double newSize, newThresholdLocation;
      if (maximum == 0)
      {
        newSize = 0;
        newThresholdLocation = 0;
      }
      else {
        newSize = fullSize * this.currentValue / (maximum - minimum);
        newThresholdLocation = fullSize * this.thresholdValue / (maximum - minimum);
      } //if
      if (!(isNormalValue(newSize)))
        return;
      if (usingThreshold)
        thresholdRectangle.Visibility = Visibility.Visible;
      else
        thresholdRectangle.Visibility = Visibility.Hidden;
      if (orientation == Orientation.Horizontal)
      {
        this.valueRectangle.HorizontalAlignment = HorizontalAlignment.Left;
        this.valueRectangle.VerticalAlignment = VerticalAlignment.Stretch;
        this.valueRectangle.Width = newSize;
        this.valueRectangle.Height = double.NaN;
        if (newThresholdLocation > ActualWidth - DefinitionSet.ThresholdThickness - DefinitionSet.BorderThickness - 1)
          newThresholdLocation = ActualWidth - DefinitionSet.ThresholdThickness - DefinitionSet.BorderThickness - 1;
        if (usingThreshold)
        {
          this.thresholdRectangle.Width = DefinitionSet.ThresholdThickness;
          this.thresholdRectangle.Height = ActualHeight;
          this.thresholdRectangle.VerticalAlignment = VerticalAlignment.Stretch;
          this.thresholdRectangle.HorizontalAlignment = HorizontalAlignment.Left;
          this.thresholdRectangle.Margin = new Thickness(newThresholdLocation, 0, 0, 0);
        } //if
      }
      else {
        this.valueRectangle.HorizontalAlignment = HorizontalAlignment.Stretch;
        this.valueRectangle.VerticalAlignment = VerticalAlignment.Bottom;
        this.valueRectangle.Height = newSize;
        this.valueRectangle.Width = double.NaN;
        if (newThresholdLocation > ActualHeight - DefinitionSet.ThresholdThickness - DefinitionSet.BorderThickness - 1)
          newThresholdLocation = ActualHeight - DefinitionSet.ThresholdThickness - DefinitionSet.BorderThickness - 1;
        if (usingThreshold)
        {
          this.thresholdRectangle.Height = DefinitionSet.ThresholdThickness;
          this.thresholdRectangle.Width = ActualHeight;
          this.thresholdRectangle.VerticalAlignment = VerticalAlignment.Bottom;
          this.thresholdRectangle.HorizontalAlignment = HorizontalAlignment.Stretch;
          this.thresholdRectangle.Margin = new Thickness(0, 0, 0, newThresholdLocation);
        } //if
      } //if
    } //Refresh

    //protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
    //{
    //  base.OnPreviewMouseLeftButtonDown(e);
    //  Focus();
    //  SetThreasholdByMouse(e);
    //} //OnMouseLeftButtonDown

    //protected override void OnPreviewMouseMove(MouseEventArgs e)
    //{
    //  base.OnPreviewMouseMove(e);
    //  if (!(e.LeftButton == MouseButtonState.Pressed)) return;
    //  SetThreasholdByMouse(e);
    //} //OnPreviewMouseMove

    //void SetThreasholdByMouse(MouseEventArgs e)
    //{
    //  if (orientation == Orientation.Horizontal)
    //    Threshold = Maximum * e.GetPosition(this).X / ActualWidth;
    //  else
    //    Threshold = Maximum * (1d - e.GetPosition(this).Y / ActualHeight);
    //} //SetThreasholdByMouse

    //protected override void OnPreviewKeyDown(KeyEventArgs e)
    //{
    //  e.Handled = e.Key == Key.Right || e.Key == Key.Right;
    //  ulong step = DefinitionSet.StepVerySlow;
    //  if (Keyboard.Modifiers == ModifierKeys.Control)
    //    step = DefinitionSet.StepSlow;
    //  else if (Keyboard.Modifiers == ModifierKeys.Shift)
    //    step = DefinitionSet.StepFast;
    //  else if (Keyboard.Modifiers == (ModifierKeys.Shift | ModifierKeys.Control))
    //    step = DefinitionSet.StepFastest;
    //  if (e.Key == Key.Right)
    //  {
    //    if (!double.IsInfinity(Threshold))
    //    {
    //      if (Threshold >= maximum)
    //        Threshold = double.PositiveInfinity;
    //      else
    //        Threshold += step;
    //    }
    //    else
    //      Threshold = 0;
    //  }
    //  else if (e.Key == Key.Left)
    //  {
    //    if (double.IsInfinity(Threshold))
    //      Threshold = maximum;
    //    else if (Threshold <= 0)
    //      Threshold = double.PositiveInfinity;
    //    else
    //      Threshold -= step;
    //  } //if
    //} //OnPreviewKeyDown

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
      if (orientation == Orientation.Horizontal && double.IsNaN(Height))
        Height = DefinitionSet.DefaultSize;
      if (orientation == Orientation.Vertical && double.IsNaN(Width))
        Width = DefinitionSet.DefaultSize;
      base.OnRenderSizeChanged(sizeInfo);
      Refresh();
    } //OnRenderSizeChanged

    protected override void OnGotFocus(RoutedEventArgs e)
    {
      base.OnGotFocus(e);
      //border.BorderBrush = DefinitionSet.FocusedBorder;
    } //OnGotFocus

    protected override void OnLostFocus(RoutedEventArgs e)
    {
      base.OnLostFocus(e);
      //border.BorderBrush = Brushes.Black;
    } //OnLostFocus

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
      base.OnPropertyChanged(e);
      if (e.Property == ForegroundProperty)
        valueRectangle.Fill = this.Foreground;
    } //OnPropertyChanged

    double currentValue, thresholdValue;
    double maximum = DefinitionSet.DefaultMaximumValue;
    double minimum;

    Orientation orientation = Orientation.Horizontal;
    Rectangle valueRectangle = new Rectangle();
    Rectangle thresholdRectangle = new Rectangle();

  } //class StripIndicator

}