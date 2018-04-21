using System.Collections.ObjectModel;
using System.Linq;
using SharpPropoPlus.Decoder.EventArguments;
using SharpPropoPlus.Events;
using SharpPropoPlus.Interfaces;
using SharpPropoPlus.vJoyMonitor;
using vJoyInterfaceWrap;

namespace SharpPropoPlus.ViewModels
{
    public class JoystickConfigViewModel : BaseViewModel, IJoystickConfigViewModel
    {
        public JoystickConfigViewModel()
        {
            Devices = new ReadOnlyObservableCollection<IDeviceInformation>(
                new ObservableCollection<IDeviceInformation>(JoystickHelper.Instance.Devices.OrderBy(ob => ob.Id)));

            SelectedDeviceItem = Devices.FirstOrDefault(fd => fd.Guid == JoystickHelper.Instance.Device.Guid);
        }

        private string _vjoyVersion;

        private IDeviceInformation _selectedDeviceItem;

        public string VJoyVersion
        {
            get
            {
                if (string.IsNullOrEmpty(_vjoyVersion))
                {
                    return _vjoyVersion = new vJoy().GetvJoySerialNumberString();
                }

                return _vjoyVersion;
            }
        }

        public ReadOnlyObservableCollection<IDeviceInformation> Devices { get; }

        public IDeviceInformation SelectedDeviceItem
        {
            get => _selectedDeviceItem;
            set
            {
                if (_selectedDeviceItem == value)
                    return;

                _selectedDeviceItem = value;

                GlobalEventAggregator.Instance.SendMessage(new JoystickChangedEventArgs(_selectedDeviceItem.Id,
                    _selectedDeviceItem.Name, _selectedDeviceItem.Guid));

                JoystickHelper.Instance.StartCapture(_selectedDeviceItem);

                OnPropertyChanged();
            }
        }


    }
}