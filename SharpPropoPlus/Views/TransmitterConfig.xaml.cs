using System.Windows.Controls;
using SharpPropoPlus.Interfaces;

namespace SharpPropoPlus.Views
{
    /// <summary>
    ///     Interaction logic for TransmitterConfig.xaml
    /// </summary>
    public partial class TransmitterConfig : UserControl
    {
        private TransmitterConfig()
        {
            InitializeComponent();
        }

        public TransmitterConfig(ITransmitterConfigViewModel viewModel)
            : this()
        {
            DataContext = viewModel;
        }
    }
}