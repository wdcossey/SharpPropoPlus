using System.Windows.Controls;
using SharpPropoPlus.Interfaces;

namespace SharpPropoPlus.Views
{
    /// <summary>
    /// Interaction logic for JoystickInformation.xaml
    /// </summary>
    public partial class JoystickConfig : UserControl
    {
        private JoystickConfig()
        {
            InitializeComponent();
        }

        public JoystickConfig(IJoystickConfigViewModel viewModel)
            : this()
        {
            this.DataContext = viewModel;
        }
    }
}
