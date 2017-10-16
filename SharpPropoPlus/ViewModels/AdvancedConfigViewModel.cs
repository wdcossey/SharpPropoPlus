using Microsoft.Practices.Unity;
using SharpPropoPlus.Interfaces;

namespace SharpPropoPlus.ViewModels
{
    public class AdvancedConfigViewModel : BaseViewModel, IAdvancedConfigViewModel
    {
        private ILoggingTabViewModel _loggingViewModel;

        public AdvancedConfigViewModel([Dependency] ILoggingTabViewModel loggingTabViewModel)
        {
            LoggingViewModel = loggingTabViewModel;
        }

        public ILoggingTabViewModel LoggingViewModel
        {
            get => _loggingViewModel;

            private set
            {
                _loggingViewModel = value;

                OnPropertyChanged();
            }
        }
    }
}