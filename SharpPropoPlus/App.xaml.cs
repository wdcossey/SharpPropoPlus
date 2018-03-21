using System;
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
            try
            {
                base.OnStartup(e);

                Application.Instance.ShowMainWindow();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
                Console.WriteLine(exception);
            }


        }

        protected override void OnExit(ExitEventArgs e)
        {
            Application.Instance.Dispose();
            base.OnExit(e);
        }
    }
}
