using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using SharpPropoPlus.Interfaces;

namespace SharpPropoPlus.Controls
{
    public class JoystickMonitor : UserControl
    {
        public ObservableCollection<IJoystickChannelData> Data
        {
            get => (ObservableCollection<IJoystickChannelData>) this.GetValue(DataProperty);
            set => this.SetValue(DataProperty, value);
        }

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
            "Data", typeof(ObservableCollection<IJoystickChannelData>), typeof(JoystickMonitor), new PropertyMetadata(null));
    }
}