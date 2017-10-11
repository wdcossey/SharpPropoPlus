using System.Windows.Controls;
using SharpPropoPlus.Interfaces;

namespace SharpPropoPlus.Views
{
    /// <summary>
    ///     Interaction logic for FilterConfig.xaml
    /// </summary>
    public partial class FilterConfig : UserControl
    {
        public FilterConfig()
        {
            InitializeComponent();
        }

        public FilterConfig(IFilterConfigViewModel viewModel)
            : this()
        {
            DataContext = viewModel;
        }
    }
}