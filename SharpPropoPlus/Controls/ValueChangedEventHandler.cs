using System.Windows;

namespace SharpPropoPlus.Controls
{
    public delegate void ValueChangedEventHandler(object sender, ValueChangedEventArgs e);

    public class ValueChangedEventArgs : RoutedEventArgs
    {
        public ValueChangedEventArgs(RoutedEvent routedEvent, object source, double oldValue, double newValue)
            : base(routedEvent, source)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        public double OldValue { get; }
        public double NewValue { get; }
    }
}
