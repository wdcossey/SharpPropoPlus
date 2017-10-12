using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using Microsoft.Practices.Unity;
using SharpPropoPlus.Audio;
using SharpPropoPlus.Audio.EventArguments;
using SharpPropoPlus.Decoder;
using SharpPropoPlus.Decoder.EventArguments;
using SharpPropoPlus.Events;
using SharpPropoPlus.Helpers;
using SharpPropoPlus.Interfaces;
using SharpPropoPlus.vJoyMonitor;
using SharpPropoPlus.ViewModels;
using SharpPropoPlus.Filter;

namespace SharpPropoPlus
{
    public class Application : IDisposable
    {
        private static Application _instance;
        private static readonly object Sync = new object();
        private IUnityContainer _container;

        private Application()
        {
            //_decoderManager = new DecoderManager();

            DecoderManager = Container.Resolve<DecoderManager>();
            FilterManager = Container.Resolve<FilterManager>();

            JoystickHelper.Initialize();
            JoystickInteraction.Initialize();

            //TODO: Remove, this is only for testing...
            //GlobalEventAggregator.Instance.AddListener<AudioDataEventArgs>(AudioDataAction);
            //TODO: Remove, this is only for testing...
            AudioHelper.Instance.DataAvailable += AudioDataAvailable;
        }

        //TODO: Remove, this is only for testing...
        private void AudioDataAvailable(object o, AudioDataEventArgs args)
        {
            var samplesDesired = args.BytesRecorded / args.Channels;
            var sampleRate = args.SampleRate;
            var bitsPerSample = args.BitsPerSample;
            var channels = args.Channels;

            var left = new int[samplesDesired];
            var right = new int[samplesDesired];
            var index = 0;

            for (var sample = 0; sample < args.BytesRecorded / 4; sample++)
            {

                left[sample] = BitConverter.ToInt16(args.Buffer, index);
                index += 2;
                right[sample] = BitConverter.ToInt16(args.Buffer, index);
                index += 2;

                DecoderManager.Decoder.ProcessPulse(args.SampleRate, right[sample], FilterManager.IsEnabled, FilterManager.Filter); /*data =>
                {
                    var filterData = data.Data;
                    var channelCount = data.Count;

                    if (FilterManager.IsEnabled)
                    {
                        channelCount = (FilterManager.Filter?.RunFilter(ref filterData, channelCount + 1)).GetValueOrDefault(0);
                    }

                    if (channelCount > 0)
                    {
                        JoystickInteraction.Instance.Send(channelCount, filterData);
                    }

                });*/

                //
            }
        }

        //TODO: Remove, this is only for testing...
        //private void AudioDataAction(AudioDataEventArgs args)
        //{
        //    //var bitsPerSample = source.WaveFormat.BitsPerSample;
        //    //var sampleRate = source.WaveFormat.SampleRate;
        //    //var channels = source.WaveFormat.Channels;

        //    var samplesDesired = args.BytesRecorded / args.Channels;

        //    var left = new int[samplesDesired];
        //    var right = new int[samplesDesired];
        //    var index = 0;

        //    for (var sample = 0; sample < args.BytesRecorded / 4; sample++)
        //    {

        //        //left[sample] = BitConverter.ToInt16(args.Buffer, index);
        //        index += 2;
        //        right[sample] = BitConverter.ToInt16(args.Buffer, index);
        //        index += 2;

        //        _decoder.Value.ProcessPulse(args.SampleRate, right[sample]);
        //    }

        //}


        public static Application Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (Sync)
                    {
                        if (_instance == null)
                            _instance = new Application();
                    }
                }

                return _instance;
            }
        }

        public IDecoderManager DecoderManager { get; }

        public IFilterManager FilterManager { get; }

        public IUnityContainer Container
        {
            get
            {
                if (_container == null)
                {
                    lock (Sync)
                    {
                        if (_container == null)
                            _container = new UnityContainer();
                    }
                }

                return _container;
            }
        }

        public void ShowMainWindow()
        {

            Container.RegisterType<IDecoderManager, DecoderManager>(new ContainerControlledLifetimeManager());
            //Container.RegisterInstance<IDecoderManager>(new DecoderManager());

            Container.RegisterInstance<IAudioConfigViewModel>(new AudioConfigViewModel());
            Container.RegisterInstance<IJoystickConfigViewModel>(new JoystickConfigViewModel());
            Container.RegisterInstance<ITransmitterConfigViewModel>(new TransmitterConfigViewModel());
            Container.RegisterInstance<IFilterConfigViewModel>(new FilterConfigViewModel());

            var mainWindow = Container.Resolve<Shell>(); // Creating Main window

            mainWindow.Loaded += (sender, args) =>
            {
                var windowHandle = new WindowInteropHelper(mainWindow).Handle;

                var source = HwndSource.FromHwnd(windowHandle);
                source?.AddHook(WndProc);

                DeviceNotification.RegisterDeviceNotification(windowHandle, true);
            };


            //System.Windows.Application.Current.MainWindow = mainWindow;
            mainWindow.Show();

            //System.Windows.Application.Current.MainWindow.Show();
        }

        private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            //  do stuff
            if (msg == DeviceNotification.WM_DEVICECHANGE)
            {
                var action = (int) wParam;

                if (lParam.Equals(IntPtr.Zero) && !wParam.Equals(IntPtr.Zero))
                {

                }

                else if (!lParam.Equals(IntPtr.Zero) && !wParam.Equals(IntPtr.Zero))
                {
                    var pHdr = Marshal.PtrToStructure<DeviceNotification.DEV_BROADCAST_HDR>(lParam);

                    switch (pHdr.dbch_DeviceType)
                    {
                        case DeviceNotification.DbtDevtypDeviceinterface:

                            var pDevInf =
                                Marshal.PtrToStructure<DeviceNotification.DEV_BROADCAST_DEVICEINTERFACE>(lParam);

                            switch (action)
                            {
                                case DeviceNotification.DBT_DEVICEARRIVAL:
                                    // do something...
                                    break;
                                case DeviceNotification.DBT_DEVICEREMOVECOMPLETE:
                                    // do something...
                                    break;
                            }
                            break;

                        //case DBT_DEVTYP_HANDLE:
                        //    PDEV_BROADCAST_HANDLE pDevHnd = (PDEV_BROADCAST_HANDLE)pHdr;
                        //    // do something...
                        //    break;

                        //case DBT_DEVTYP_OEM:
                        //    PDEV_BROADCAST_OEM pDevOem = (PDEV_BROADCAST_OEM)pHdr;
                        //    // do something...
                        //    break;

                        //case DBT_DEVTYP_PORT:
                        //    PDEV_BROADCAST_PORT pDevPort = (PDEV_BROADCAST_PORT)pHdr;
                        //    // do something...
                        //    break;

                        //case DBT_DEVTYP_VOLUME:
                        //    PDEV_BROADCAST_VOLUME pDevVolume = (PDEV_BROADCAST_VOLUME)pHdr;
                        //    // do something...
                        //    break;
                    }
                }

            }
            return IntPtr.Zero;
        }

        public void Dispose()
        {
            AudioHelper.Instance.DataAvailable -= AudioDataAvailable;
            AudioHelper.Instance.Dispose();
            //GlobalEventAggregator.Instance.AddListener<AudioDataEventArgs>(AudioDataAction);
        }
    }
}