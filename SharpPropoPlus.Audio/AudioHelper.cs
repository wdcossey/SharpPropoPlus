using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
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
        private bool _quitPolling;
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


        private AudioHelper()
        {
            _quitPolling = false;
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
            Devices = _deviceEnumerator.EnumAudioEndpoints(DataFlow.Capture, DeviceState.Active).Select(s => new AudioEndPoint(s.FriendlyName, s.DeviceID)).ToList();
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

            var args = new AudioEndPointEventArgs(device.FriendlyName, device.DeviceID);
            //OnDeviceChanged(args);
            GlobalEventAggregator.Instance.SendMessage(args);

            //var x = _deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.All)
            //  .FirstOrDefault(w => w.ID == _deviceId);

            var deviceFormat = WaveFormatFromBlob(device.PropertyStore[
                new PropertyKey(new Guid(0xf19f064d, 0x82c, 0x4e27, 0xbc, 0x73, 0x68, 0x82, 0xa1, 0xbb, 0x8e, 0x4c), 0)].BlobValue);

            //deviceFormat = new WaveFormat(192000, 16, deviceFormat.Channels);

            //var channels = deviceFormat.Channels;


            _soundIn = new WasapiCapture();

            _soundIn.Device = device;
            _soundIn.Initialize();

            //var soundInSource = new SoundInSource(_soundIn);
            SoundInSource soundInSource = new SoundInSource(_soundIn) { FillWithZeros = false };

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

            var bitsPerSample = args.Format.BitsPerSample;
            var sampleRate = args.Format.SampleRate;
            var channels = args.Format.Channels;

            int samplesDesired = args.ByteCount / channels;

            //int[] left = new int[samplesDesired];
            //int[] right = new int[samplesDesired];
            //int index = 0;

            //for (var sample = 0; sample < args.BytesRecorded / 4; sample++)
            //{

            //  //left[sample] = BitConverter.ToInt16(args.Buffer, index);
            //  index += 2;
            //  right[sample] = BitConverter.ToInt16(args.Buffer, index);
            //  index += 2;
            //}

            byte[] buffer = new byte[_convertedSource.WaveFormat.BytesPerSecond / 2];
            int read;

            //keep reading as long as we still get some data
            //if you're using such a loop, make sure that soundInSource.FillWithZeros is set to false
            while ((read = _convertedSource.Read(buffer, 0, buffer.Length)) > 0)
            {
                //write the read data to a file
                // ReSharper disable once AccessToDisposedClosure
                //waveWriter.Write(buffer, 0, read);

                var message = new AudioDataEventArgs(_convertedSource.WaveFormat.SampleRate, _convertedSource.WaveFormat.Channels, buffer.Length, buffer);
                GlobalEventAggregator.Instance.SendMessage(message);
            }

           
        }

        public void StopRecording()
        {
            _quitPolling = true;

            if (_soundIn != null)
            {
                _soundIn.DataAvailable -= SoundInSourceOnDataAvailable;
                _soundIn.Stop();
                _soundIn.Dispose();
                _soundIn = null;
            }
            
            _pollingCancellationTokenSource?.Cancel();
            _pollingTask?.Wait();
            _pollingCancellationTokenSource = new CancellationTokenSource();

        }

        private void PollAudioLevels()
        {

            _quitPolling = false;

            _pollingTask = Task.Factory.StartNew(() =>
            {

                var device = _deviceEnumerator.GetDevice(DeviceId);

                while (!_quitPolling)
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

                _quitPolling = false;

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
            if (_pollingTask.Status == TaskStatus.Running)
                StopRecording();

            _soundIn?.Dispose();

            //_device?.Dispose();

            _deviceEnumerator?.Dispose();

        }
    }
}
