using System.Windows.Controls;
using SharpPropoPlus.Interfaces;

namespace SharpPropoPlus.Views
{
    /// <summary>
    ///     Interaction logic for AdvancedConfig.xaml
    /// </summary>
    public partial class AdvancedConfig : UserControl
    {
        private AdvancedConfig()
        {
            InitializeComponent();
        }

        public AdvancedConfig(IAdvancedConfigViewModel viewModel)
            : this()
        { 
            DataContext = viewModel;
        }
    }
}