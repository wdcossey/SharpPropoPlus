using System.Diagnostics;
using System.Windows.Input;
using SharpPropoPlus.Commands;
using SharpPropoPlus.Interfaces;

namespace SharpPropoPlus.ViewModels
{
    public class AboutTabViewModel : BaseViewModel, IAboutTabViewModel
    {

        public AboutTabViewModel()
        {

        }

        public ICommand HyperlinkCommand
        {
            get { return new RelayCommand<string>(url =>
            {
                var p = new Process
                { 
                    StartInfo = new ProcessStartInfo
                    {
                        UseShellExecute = true,
                        FileName = url
                    }
                };
                p.Start();
            });}
        }
    }
}