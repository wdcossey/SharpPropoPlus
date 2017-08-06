using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using SharpPropoPlus.Events;

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
