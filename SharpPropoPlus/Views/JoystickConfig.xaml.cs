using System.Windows.Controls;
using SharpPropoPlus.Interfaces;

namespace SharpPropoPlus.Views
{
    /// <summary>
    /// Interaction logic for JoystickInformation.xaml
    /// </summary>
    public partial class JoystickConfig : UserControl
    {
        public JoystickConfig(IJoystickConfigViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}
