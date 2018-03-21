using System.Diagnostics;
using System.Reflection;
using System.Windows.Input;
using SharpPropoPlus.Commands;
using SharpPropoPlus.Interfaces;

namespace SharpPropoPlus.ViewModels
{
    public class AboutTabViewModel : BaseViewModel, IAboutTabViewModel
    {
        private string _fileVersion;

        public AboutTabViewModel()
        {
            FileVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location)?.FileVersion;
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

        public string FileVersion
        {
            get => _fileVersion;

            set
            {
                if (_fileVersion == value)
                    return;

                _fileVersion = value;
                OnPropertyChanged();
            }
        }
    }
}