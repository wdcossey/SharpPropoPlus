using System.Windows.Controls;
using SharpPropoPlus.Interfaces;

namespace SharpPropoPlus.Views
{
    /// <summary>
    /// Interaction logic for TransmitterConfig.xaml
    /// </summary>
    public partial class TransmitterConfig : UserControl
    {

        public TransmitterConfig()
        {


        }

        public TransmitterConfig(ITransmitterConfigViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}
