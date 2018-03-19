using System.Windows.Controls;
using SharpPropoPlus.Interfaces;

namespace SharpPropoPlus.Views
{
    /// <summary>
    /// Interaction logic for AboutTab.xaml
    /// </summary>
    public partial class AboutTab : UserControl
    {
        public AboutTab()
        {
            InitializeComponent();
        }


        public AboutTab(IAboutTabViewModel viewModel)
            : this()
        {
            DataContext = viewModel;
        }
    }
}
