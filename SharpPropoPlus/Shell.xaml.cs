using SharpPropoPlus.Controls;
using SharpPropoPlus.Interfaces;

namespace SharpPropoPlus
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Shell : CustomWindow
    {
        private Shell()
        {
            InitializeComponent();
        }

        public Shell(IShellViewModel viewModel)
            : this()
        {
            DataContext = viewModel;
        }
    }
}
