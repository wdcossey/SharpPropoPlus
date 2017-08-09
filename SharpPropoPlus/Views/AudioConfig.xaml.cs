using System.Windows.Controls;
using SharpPropoPlus.Interfaces;

namespace SharpPropoPlus.Views
{
  /// <summary>
  /// Interaction logic for InputConfig.xaml
  /// </summary>
  public partial class AudioConfig : UserControl
  {

      public AudioConfig()
      {
          

      }
    public AudioConfig(IAudioConfigViewModel viewModel)
    {
        InitializeComponent();
        this.DataContext = viewModel;
    }
  }
}
