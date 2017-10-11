using System.Windows.Controls;
using SharpPropoPlus.Interfaces;

namespace SharpPropoPlus.Views
{
    /// <summary>
    ///     Interaction logic for InputConfig.xaml
    /// </summary>
    public partial class AudioConfig : UserControl
    {
        private AudioConfig()
        {
            InitializeComponent();
        }

        public AudioConfig(IAudioConfigViewModel viewModel)
            : this()
        {
            DataContext = viewModel;
        }
    }
}