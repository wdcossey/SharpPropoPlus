using SharpPropoPlus.Interfaces;
using Unity.Attributes;

namespace SharpPropoPlus.ViewModels
{
    public class AdvancedConfigViewModel : BaseViewModel, IAdvancedConfigViewModel
    {


        private ILoggingTabViewModel _loggingViewModel;
        private IAboutTabViewModel _aboutViewModel;

        public AdvancedConfigViewModel([Dependency] ILoggingTabViewModel loggingTabViewModel, [Dependency] IAboutTabViewModel aboutTabViewModel)
        {
            AboutViewModel = aboutTabViewModel;
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

        public IAboutTabViewModel AboutViewModel
        {
            get => _aboutViewModel;
            private set
            {
                _aboutViewModel = value;

                OnPropertyChanged();
            }
        }
    }
}