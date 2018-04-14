using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace SharpPropoPlus.Controls
{

    [TemplatePart(Name = "PART_TextBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_ButtonUp", Type = typeof(ButtonBase))]
    [TemplatePart(Name = "PART_ButtonDown", Type = typeof(ButtonBase))]
    public class NumericUpDown : Control
    {

        private TextBox _partTextBox = new TextBox();

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (GetTemplateChild("PART_TextBox") is TextBox textBox)
            {
                _partTextBox = textBox;
                _partTextBox.PreviewKeyDown += TextBox_PreviewKeyDown;
                _partTextBox.TextChanged += TextBox_TextChanged;
                _partTextBox.Text = Value.ToString(CultureInfo.InvariantCulture);
            }

            if (GetTemplateChild("PART_ButtonUp") is ButtonBase partButtonUp)
            {
                partButtonUp.Click += buttonUp_Click;
            }

            if (GetTemplateChild("PART_ButtonDown") is ButtonBase partButtonDown)
            {
                partButtonDown.Click += buttonDown_Click;
            }
        }

        static NumericUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericUpDown),
                new FrameworkPropertyMetadata(typeof(NumericUpDown)));
        }

        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent(
            "ValueChanged", RoutingStrategy.Direct,
            typeof(ValueChangedEventHandler), typeof(NumericUpDown));

        public event ValueChangedEventHandler ValueChanged
        {
            add => AddHandler(ValueChangedEvent, value);
            remove => RemoveHandler(ValueChangedEvent, value);
        }

        public double MaxValue
        {
            get => (double) GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }

        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(double), typeof(NumericUpDown),
                new FrameworkPropertyMetadata(100D, MaxValueChangedCallback, CoerceMaxValueCallback));

        private static object CoerceMaxValueCallback(DependencyObject d, object value)
        {
            var minValue = ((NumericUpDown) d).MinValue;
            if ((double) value < minValue)
                return minValue;

            return value;
        }

        private static void MaxValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var numericUpDown = ((NumericUpDown) d);
            numericUpDown.CoerceValue(MinValueProperty);
            numericUpDown.CoerceValue(ValueProperty);
        }

        public double MinValue
        {
            get => (double) GetValue(MinValueProperty);
            set => SetValue(MinValueProperty, value);
        }

        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(double), typeof(NumericUpDown),
                new FrameworkPropertyMetadata(0D, MinValueChangedCallback, CoerceMinValueCallback));

        private static object CoerceMinValueCallback(DependencyObject d, object value)
        {
            var maxValue = ((NumericUpDown) d).MaxValue;
            if ((double) value > maxValue)
                return maxValue;

            return value;
        }

        private static void MinValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var numericUpDown = ((NumericUpDown) d);
            numericUpDown.CoerceValue(MaxValueProperty);
            numericUpDown.CoerceValue(ValueProperty);
        }

        public double Increment
        {
            get => (double) GetValue(IncrementProperty);
            set => SetValue(IncrementProperty, value);
        }

        public static readonly DependencyProperty IncrementProperty =
            DependencyProperty.Register("Increment", typeof(double), typeof(NumericUpDown),
                new FrameworkPropertyMetadata(1D, null, CoerceIncrementCallback));

        private static object CoerceIncrementCallback(DependencyObject d, object value)
        {
            var numericUpDown = ((NumericUpDown) d);
            var i = numericUpDown.MaxValue - numericUpDown.MinValue;
            if ((double) value > i)
                return i;

            return value;
        }

        public double Value
        {
            get => (double) GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(NumericUpDown),
                new FrameworkPropertyMetadata(0D, ValueChangedCallback, CoerceValueCallback), ValidateValueCallback);

        private static void ValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var numericUpDown = (NumericUpDown) d;
            var ea =
                new ValueChangedEventArgs(ValueChangedEvent, d, (double) e.OldValue, (double) e.NewValue);
            numericUpDown.RaiseEvent(ea);
            //if (ea.Handled) numericUpDown.Value = (double)e.OldValue;
            //else 
            numericUpDown._partTextBox.Text = e.NewValue.ToString();
        }

        private static bool ValidateValueCallback(object value)
        {
            var val = (double) value;

            if (val > double.MinValue && val < double.MaxValue)
                return true;

            return false;
        }

        private static object CoerceValueCallback(DependencyObject d, object value)
        {
            var val = (double) value;
            var minValue = ((NumericUpDown) d).MinValue;
            var maxValue = ((NumericUpDown) d).MaxValue;
            double result;
            if (val < minValue)
                result = minValue;
            else if (val > maxValue)
                result = maxValue;
            else
                result = (double) value;

            return result;
        }

        private void buttonUp_Click(object sender, RoutedEventArgs e)
        {
            Value += Increment;
        }

        private void buttonDown_Click(object sender, RoutedEventArgs e)
        {
            Value -= Increment;
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var index = _partTextBox.CaretIndex;
            if (!double.TryParse(_partTextBox.Text, out var result))
            {
                var changes = e.Changes.FirstOrDefault();
                _partTextBox.Text = _partTextBox.Text.Remove(changes.Offset, changes.AddedLength);
                _partTextBox.CaretIndex = index > 0 ? index - changes.AddedLength : 0;
            }
            else if (result < MaxValue && result > MinValue)
                Value = result;
            else
            {
                _partTextBox.Text = Value.ToString(CultureInfo.InvariantCulture);
                _partTextBox.CaretIndex = index > 0 ? index - 1 : 0;
            }
        }
    }
}
