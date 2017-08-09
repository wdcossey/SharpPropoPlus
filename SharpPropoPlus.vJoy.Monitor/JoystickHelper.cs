using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharpDX.DirectInput;

namespace SharpPropoPlus.vJoyMonitor
{
    public class JoystickHelper : IDisposable
    {
        private readonly DirectInput _directInput;
        private int startButtonOffset = -1;
        private int lapButtonOffset = -1;

        private Task _pollingTask;
        private bool _quitPolling;

        private static JoystickHelper _instance;
        private static bool _initialized;
        private static readonly object Sync = new object();

        private Guid _deviceGuid;
        //private Joystick _joystick;

        public JoystickHelper()
        {
            _quitPolling = false;
            _directInput = new DirectInput();
        }

        public static JoystickHelper Instance
        {
            get
            {
                //if (_instance == null)
                //{
                //  lock (Sync)
                //  {
                //    if (_instance == null)
                //      _instance = new JoystickInteraction();
                //  }
                //}

                if (!_initialized)
                    throw new NullReferenceException($"{nameof(JoystickHelper)} must be initialized before use!");

                return _instance;
            }
        }

        public static void Initialize()
        {
            try
            {

                if (_initialized)
                    return;

                if (_instance == null)
                {
                    lock (Sync)
                    {
                        if (_instance == null)
                            _instance = new JoystickHelper();
                    }
                }

                _initialized = true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //public List<JoystickDescriptor> DetectDevices()
        //{
        //  List<JoystickDescriptor> joystickDescriptors = new List<JoystickDescriptor>();

        //  // check for gamepads
        //  foreach (var deviceInstance in _directInput.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AllDevices))
        //  {
        //    joystickDescriptors.Add(new JoystickDescriptor(deviceInstance.InstanceGuid, deviceInstance.InstanceName));
        //  }

        //  // check for joysticks
        //  foreach (var deviceInstance in _directInput.GetDevices(DeviceType.Joystick, DeviceEnumerationFlags.AllDevices))
        //  {
        //    joystickDescriptors.Add(new JoystickDescriptor(deviceInstance.InstanceGuid, deviceInstance.InstanceName));
        //  }

        //  return joystickDescriptors;
        //}


        //public void StartCapture(Guid joystickGuid, int startButtonOffset, int lapButtonOffset)
        //{
        //  this.startButtonOffset = startButtonOffset;
        //  this.lapButtonOffset = lapButtonOffset;
        //  StartCapture(joystickGuid);
        //}

        public void StartCapture(Guid deviceGuid)
        {
            StopCapture();

            _deviceGuid = deviceGuid;

            //_joystick?.Dispose();

            //_joystick = new Joystick(_directInput, deviceGuid)
            //{
            //  Properties =
            //  {
            //    BufferSize = 128
            //  }
            //};

            //_joystick.Acquire();

            _quitPolling = false;

            PollJoystick();

            // Spin for a while waiting for the started thread to become alive
            //while (!pollingThread.IsAlive) ;
        }

        public void StopCapture()
        {
            _quitPolling = true;
            _pollingTask?.Wait(); // you can give this Wait a timeout
        }

        private void PollJoystick()
        {

            _pollingTask = Task.Factory.StartNew(() =>
            {

                var lastSate = new List<JoystickUpdate>();

                var joystick = new Joystick(_directInput, _deviceGuid)
                {

                    Properties =
                    {
                        BufferSize = 0,
                        AxisMode = DeviceAxisMode.Absolute,
                        Range = new InputRange(0, Int32.MaxValue)
                    }

                };

                joystick.SetCooperativeLevel(IntPtr.Zero, CooperativeLevel.Background | CooperativeLevel.NonExclusive);

                joystick.Acquire();

                while (!_quitPolling)
                {

                    try
                    {
                        joystick.Poll();

                        //var range = joystick.Properties.Range;

                        var data = new JoystickState();

                        joystick.GetCurrentState(ref data);
                        //Debug.WriteLine("X: {0}, Y: {1}, Z: {2}", data.X, data.Y, data.Z);


                        //var data = joystick.GetBufferedData();

                        //foreach (var state in data)
                        //{

                        //  var lastUpdate = lastSate.FirstOrDefault(f => f.Offset == state.Offset);

                        //  if (lastUpdate.Timestamp == 0)
                        //  {
                        //    lastSate.Add(state);
                        //  }

                        //  if (state.Offset == JoystickOffset.X && state.Value != lastUpdate.Value)
                        //  {
                        //    ////PostAxisValue(iDevice, HID_USAGE_X, state.lX);
                        //    Debug.WriteLine("{0}: {1}", state.Offset, state.Value);
                        //  }

                        //  lastUpdate = state;

                        //}


                        //Task.Delay(10);
                    }
                    catch (SharpDX.SharpDXException e)
                    {
                        StopCapture();
                    }

                }

                joystick.Dispose();

            });

        }

        protected virtual void OnJoystickButtonPressed(JoystickButtonPressedEventArgs e)
        {
            EventHandler<JoystickButtonPressedEventArgs> handler = JoystickButtonPressed;
            handler?.Invoke(this, e);
        }

        public event EventHandler<JoystickButtonPressedEventArgs> JoystickButtonPressed;

        protected virtual void OnJoystickLapButtonPressed(JoystickButtonPressedEventArgs e)
        {
            EventHandler<JoystickButtonPressedEventArgs> handler = JoystickLapButtonPressed;
            handler?.Invoke(this, e);
        }

        public event EventHandler<JoystickButtonPressedEventArgs> JoystickLapButtonPressed;

        protected virtual void OnJoystickStartButtonPressed(JoystickButtonPressedEventArgs e)
        {
            EventHandler<JoystickButtonPressedEventArgs> handler = JoystickStartButtonPressed;
            handler?.Invoke(this, e);
        }

        public event EventHandler<JoystickButtonPressedEventArgs> JoystickStartButtonPressed;

        public void Dispose()
        {
            if (_pollingTask.Status == TaskStatus.Running)
                StopCapture();

            _directInput?.Dispose();
            //_joystick?.Dispose();

        }

    }

    public class JoystickButtonPressedEventArgs : EventArgs
    {
        public int ButtonOffset { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}