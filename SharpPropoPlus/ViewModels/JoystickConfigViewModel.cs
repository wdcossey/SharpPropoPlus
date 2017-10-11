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
        private const int WM_DEVICECHANGE = 0x0219;


        public JoystickConfigViewModel()
        {

            using (var enumerator = new DeviceEnumerator())
            {
                Devices = new ReadOnlyObservableCollection<DeviceInformation>(
                    new ObservableCollection<DeviceInformation>(
                        enumerator.GetDevices(new[]
                        {
                            VjdStat.VJD_STAT_BUSY,
                            VjdStat.VJD_STAT_FREE,
                            VjdStat.VJD_STAT_OWN,
                        }).OrderBy(ob => ob.Id)));

                this.SelectedDeviceItem = Devices.FirstOrDefault();
            }

            //using (var input = new DirectInput())
            //{
            //  var inputDevices = input.GetDevices().Where(w => w.ProductGuid.Equals(DeviceEnumerator.VJoyProductGuid)).ToArray();
            //  foreach (var deviceInstance in inputDevices)
            //  {
            //    var device = new Joystick(input, deviceInstance.ProductGuid);

            //    device.Properties.BufferSize = 128;

            //    device.Acquire();
            //    JoystickState joystickState = new JoystickState();
            //    device.GetCurrentState(ref joystickState);
            //    //Debug.WriteLine(joystickState.Y);
            //    var x = device.GetObjects(DeviceObjectTypeFlags.All).FirstOrDefault(fd => !fd.ReportId.Equals(0));





            //    Debug.WriteLine(device.Properties.JoystickId);
            //    Debug.WriteLine(device.Properties.ProductName);
            //    Debug.WriteLine(device.Properties.ProductId);
            //    //Debug.WriteLine(device.Properties.UserName);
            //    Debug.WriteLine(device.Properties.InstanceName);
            //    Debug.WriteLine(device.Properties.InterfacePath);
            //    //Debug.WriteLine(device.Properties.PortDisplayName);
            //  }

            //  using (var helper = new JoystickHelper())
            //  {
            //    helper.StartCapture(inputDevices[0].ProductGuid);
            //  }


            //}
        }

        private string _vjoyVersion = null;
        private ReadOnlyObservableCollection<DeviceInformation> _devices;
        private DeviceInformation _selectedDeviceItem;

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

        public ReadOnlyObservableCollection<DeviceInformation> Devices
        {
            get { return _devices; }
            private set { _devices = value; }
        }

        public DeviceInformation SelectedDeviceItem
        {
            get { return _selectedDeviceItem; }
            set
            {
                if (_selectedDeviceItem == value)
                    return;

                _selectedDeviceItem = value;

                GlobalEventAggregator.Instance.SendMessage(new JoystickChangedEventArgs(_selectedDeviceItem.Id,
                    _selectedDeviceItem.Name, _selectedDeviceItem.Guid));

                JoystickHelper.Instance.StartCapture(_selectedDeviceItem.Guid);

                OnPropertyChanged();
            }
        }


    }
}