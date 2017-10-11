using System.Windows;

namespace SharpPropoPlus
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Application.Instance.ShowMainWindow();

        }

        protected override void OnExit(ExitEventArgs e)
        {
            Application.Instance.Dispose();
            base.OnExit(e);
        }
    }
}
