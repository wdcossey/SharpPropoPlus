using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using CSCore;
using CSCore.CoreAudioAPI;
using CSCore.SoundIn;
using CSCore.Streams;
using CSCore.Win32;
using SharpPropoPlus.Audio.Enums;
using SharpPropoPlus.Audio.EventArguments;
using SharpPropoPlus.Audio.Models;
using SharpPropoPlus.Events;

namespace SharpPropoPlus.Audio
{
    public class AudioHelper : IDisposable
    {

        private Task _pollingTask;
        //private bool _quitPolling;
        private CancellationTokenSource _pollingCancellationTokenSource;
        private readonly MMDeviceEnumerator _deviceEnumerator;
        private string _deviceId;
        private string _deviceName;
        private readonly PeakValues _lastPeakValues;
        private static readonly object _sync = new object();
        private static volatile AudioHelper _instance;

        private AudioChannel _audioChannel = AudioChannel.Automatic;

        private WasapiCapture _soundIn = null;
        private IList<AudioEndPoint> _devices;
        private IWaveSource _convertedSource;

        public event EventHandler<AudioDataEventArgs> DataAvailable;
        public event EventHandler<AudioEndPointEventArgs> AudioEndPointChanged;

        private AudioHelper()
        {
            //_quitPolling = false;
            _deviceEnumerator = new MMDeviceEnumerator();
            _lastPeakValues = new PeakValues();

            var device = GetDefaultDevice();

            DeviceId = device.DeviceID;
            DeviceName = device.FriendlyName;
        }

        private MMDevice GetDefaultDevice()
        {
            return _deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Console);
        }

        public IList<AudioEndPoint> Devices
        {
            get
            {
                //var x = _deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);

                if (_devices == null)
                {
                    RefreshDevices();
                }

                return _devices;
            }

            private set => _devices = value;
        }

        public void RefreshDevices()
        {
            Devices = _deviceEnumerator.EnumAudioEndpoints(DataFlow.Capture, DeviceState.Active).Select(s => new AudioEndPoint(s.FriendlyName, s.DeviceID, GetDeviceFormat(s).Channels)).ToList();
        }

        private static WaveFormat WaveFormatFromBlob(Blob blob)
        {
            if (blob.Length == 40)
                return (WaveFormat)Marshal.PtrToStructure(blob.Data, typeof(WaveFormatExtensible));
            return (WaveFormat)Marshal.PtrToStructure(blob.Data, typeof(WaveFormat));
        }

        public static AudioHelper Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                lock (_sync)
                {
                    if (_instance == null)
                        _instance = new AudioHelper();
                }

                return _instance;
            }
        }

        public string DeviceName
        {
            get { return _deviceName; }
            internal set { _deviceName = value; }
        }

        public string DeviceId
        {
            get { return _deviceId; }
            internal set { _deviceId = value; }
        }

        public AudioChannel Channel
        {
            get { return _audioChannel; }
            set { _audioChannel = value; }
        }

        public void StartRecording()
        {
            var device = GetDefaultDevice();

            StartRecording(device);
        }

        public void StartRecording(AudioEndPoint audioEndPoint)
        {
            var device = _deviceEnumerator.GetDevice(audioEndPoint.DeviceId);

            StartRecording(device);
        }

        private WaveFormat GetDeviceFormat(MMDevice device)
        {
            return WaveFormatFromBlob(device.PropertyStore[
                new PropertyKey(new Guid(0xf19f064d, 0x82c, 0x4e27, 0xbc, 0x73, 0x68, 0x82, 0xa1, 0xbb, 0x8e, 0x4c), 0)].BlobValue);
        }

        private void StartRecording(MMDevice device)
        {

            StopRecording();

            if (device == null || device.DataFlow != DataFlow.Capture)
                throw new ArgumentNullException(nameof(device), "Selected Audio device is invalid.");

            DeviceId = device.DeviceID;
            DeviceName = device.FriendlyName;

            //for (var i = 0; i < device.Properties.Count; i++)
            //{
            //  object value = null;
            //  try
            //  {
            //    value = device.Properties[i].Value;
            //  }
            //  catch (Exception)
            //  {
            //    value = null;
            //  }

            //  if (value != null && value.GetType() == typeof(byte[]))
            //  {
            //    if (((byte[]) value).Any())
            //    {
            //      value = string.Join(":", ((byte[])value).Select(s => $"{s:X}"));

            //    }
            //    else
            //    {
            //      value = null;
            //    }
            //  }
            //  Debug.WriteLine("{0}: {1}\n\t{2}", i, device.Properties[i].Key.formatId, value);
            //}


            //device.AudioEndpointVolume.OnVolumeNotification += delegate(AudioVolumeNotificationData data)
            //{
            //  GlobalEventAggregator.Instance.SendMessage(new PeakValueEventArgs(new PeakValues(data.Muted, data.MasterVolume, data.ChannelVolume[0], data.ChannelVolume[1])));
            //};


            //var x = _deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.All)
            //  .FirstOrDefault(w => w.ID == _deviceId);



            //deviceFormat = new WaveFormat(192000, 16, deviceFormat.Channels);

            var deviceFormat = GetDeviceFormat(device);

            //var channels = deviceFormat.Channels;

            var audioEndPointArgs = new AudioEndPointEventArgs(device.FriendlyName, device.DeviceID, deviceFormat.Channels);
            AudioEndPointChanged?.Invoke(this, audioEndPointArgs);
            GlobalEventAggregator.Instance.SendMessage(audioEndPointArgs);

            _soundIn =
                new WasapiCapture(false, AudioClientShareMode.Exclusive, 0, deviceFormat,
                    ThreadPriority.Highest)
                {
                    Device = device
                };

            _soundIn.Initialize();

            //var soundInSource = new SoundInSource(_soundIn);
            SoundInSource soundInSource = new SoundInSource(_soundIn)
            {
                FillWithZeros = false
            };

            _convertedSource = soundInSource
                .ChangeSampleRate(192000) // sample rate
                .ToSampleSource()
                .ToWaveSource(16); //bits per sample

            //var singleBlockNotificationStream = new SingleBlockNotificationStream(soundInSource.ToSampleSource());
            //_finalSource = singleBlockNotificationStream.ToWaveSource();

            //byte[] buffer = new byte[_finalSource.WaveFormat.BytesPerSecond / 2];
            soundInSource.DataAvailable += SoundInSourceOnDataAvailable;


            //singleBlockNotificationStream.SingleBlockRead += SingleBlockNotificationStreamOnSingleBlockRead;

            _soundIn.Start();

            PollAudioLevels();
        }

        private void SoundInSourceOnDataAvailable(object sender, DataAvailableEventArgs args)
        {
            var source = sender as IWaveSource;

            if (source == null)
                return;


            byte[] buffer = new byte[_convertedSource.WaveFormat.BytesPerSecond / 2];

            //keep reading as long as we still get some data
            //if you're using such a loop, make sure that soundInSource.FillWithZeros is set to false
            var elementsRead = _convertedSource.Read(buffer, 0, buffer.Length);
            
            var eventArgs = new AudioDataEventArgs(_convertedSource.WaveFormat.SampleRate, _convertedSource.WaveFormat.BitsPerSample,
                _convertedSource.WaveFormat.Channels, elementsRead, buffer.Take(elementsRead).ToArray());
             
            //Raise the event handler
            DataAvailable?.Invoke(this, eventArgs);

            //Publish the message (same as the event handler [above])
            GlobalEventAggregator.Instance.SendMessage(eventArgs);
        }

        public void StopRecording()
        {
            _pollingCancellationTokenSource?.Cancel();

            _pollingTask?.Wait();

            //_quitPolling = true;

            if (_soundIn != null)
            {
                _soundIn.DataAvailable -= SoundInSourceOnDataAvailable;
            }

            _soundIn?.Stop();
            _soundIn?.Dispose();
            _soundIn = null;
            
            _pollingCancellationTokenSource = new CancellationTokenSource();

        }

        private void PollAudioLevels()
        {

            //_quitPolling = false;

            _pollingTask = Task.Factory.StartNew(() =>
            {

                var device = _deviceEnumerator.GetDevice(DeviceId);

                while (!_pollingCancellationTokenSource.IsCancellationRequested)
                {


                    //var peakValues = new PeakValues(device.AudioEndpointVolume.Mute,
                    //    device.AudioMeterInformation.MasterPeakValue,
                    //    device.AudioMeterInformation.PeakValues[0],
                    //    device.AudioMeterInformation.PeakValues.Count == 1 ? (float?)null : device.AudioMeterInformation.PeakValues[1]);

                    //if (!_lastPeakValues.Muted.Equals(peakValues.Muted) ||
                    //    !_lastPeakValues.Master.Equals(peakValues.Master) ||
                    //    !_lastPeakValues.Left.Equals(peakValues.Left) ||
                    //    !_lastPeakValues.Right.Equals(peakValues.Right))
                    //{
                    //    var args = new PeakValueEventArgs(peakValues);
                    //    //OnPeakValuesChanged(args);
                    //    GlobalEventAggregator.Instance.SendMessage(args);

                    //    _lastPeakValues.Update(peakValues);
                    //}

                    //var preferredChannel = Channel == AudioChannel.Automatic
                    //    ? new PreferredChannelEventArgs(peakValues.Left, peakValues.Right)
                    //    : new PreferredChannelEventArgs(Channel);

                    //GlobalEventAggregator.Instance.SendMessage(preferredChannel);

                    ////var deviceInfo = new DeviceInfoEventArgs(device);
                    ////GlobalEventAggregator.Instance.SendMessage(deviceInfo);

                    //Task.Delay(200, _pollingCancellationTokenSource.Token);
                }

                //_quitPolling = false;

            }, _pollingCancellationTokenSource.Token);

        }

        //protected virtual void OnPeakValuesChanged(PeakValueEventArgs e)
        //{
        //  var handler = PeakValuesChanged;
        //  handler?.Invoke(this, e);
        //}

        //protected virtual void OnDeviceChanged(DeviceInfoEventArgs e)
        //{
        //  var handler = DeviceChanged;
        //  handler?.Invoke(this, e);
        //}

        //public event EventHandler<PeakValueEventArgs> PeakValuesChanged;

        //public event EventHandler<DeviceInfoEventArgs> DeviceChanged;

        public void Dispose()
        {
            if (_pollingTask.Status == TaskStatus.Running || !_pollingCancellationTokenSource.IsCancellationRequested)
                StopRecording();

            _soundIn?.Dispose();

            //_device?.Dispose();

            _deviceEnumerator?.Dispose();

        }
    }
}
