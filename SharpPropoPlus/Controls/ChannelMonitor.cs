using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using SharpPropoPlus.Interfaces;

namespace SharpPropoPlus.Controls
{
    public class ChannelMonitor : UserControl
    {
        public ObservableCollection<IChannelData> Data
        {
            get => (ObservableCollection<IChannelData>) this.GetValue(DataProperty);
            set => this.SetValue(DataProperty, value);
        }

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
            "Data", typeof(ObservableCollection<IChannelData>), typeof(ChannelMonitor), new PropertyMetadata(null));
    }
}