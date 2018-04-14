using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using CSCore;
using CSCore.CoreAudioAPI;
using CSCore.DeviceTopology.ExtensionMethods;
using CSCore.SoundIn;
using CSCore.Streams;
using CSCore.Win32;
using SharpPropoPlus.Audio.Enums;
using SharpPropoPlus.Audio.EventArguments;
using SharpPropoPlus.Audio.Interfaces;
using SharpPropoPlus.Audio.Models;
using SharpPropoPlus.Contracts.EventArguments;
using SharpPropoPlus.Events;
using DeviceStateChangedEventArgs = SharpPropoPlus.Audio.EventArguments.DeviceStateChangedEventArgs;
using RecordingState = SharpPropoPlus.Contracts.Enums.RecordingState;

namespace SharpPropoPlus.Audio
{
    public class AudioHelper : IDisposable
    {

        private Task _pollingTask;
        private CancellationTokenSource _pollingCancellationTokenSource;
        private readonly MMDeviceEnumerator _deviceEnumerator;
        private readonly PeakValues _lastPeakValues;
        private static readonly object _sync = new object();
        private static volatile AudioHelper _instance;

        private AudioChannel _audioChannel = AudioChannel.Automatic;
        private AudioBitrate _audioBitrate = AudioBitrate.Automatic;

        private WasapiCapture _soundIn = null;
        private IList<AudioEndPoint> _devices;
        private IWaveSource _convertedSource;

        public event EventHandler<AudioDataEventArgs> DataAvailable;
        public event EventHandler<AudioEndPointEventArgs> AudioEndPointChanged;

        private MMDevice _currentDevice = null;
        private bool _isRecording;
        private readonly bool _isStartUp;

        private AudioHelper()
        {
            _isStartUp = true;

            GlobalEventAggregator.Instance.AddListener<SleepStateEventArgs>(SleepStateListner);

            _deviceEnumerator = new MMDeviceEnumerator();

            _deviceEnumerator.DeviceAdded += DeviceAdded;
            _deviceEnumerator.DeviceRemoved += DeviceRemoved;
            _deviceEnumerator.DeviceStateChanged += DeviceStateChanged;
            _deviceEnumerator.DevicePropertyChanged += DevicePropertyChanged;
            _lastPeakValues = new PeakValues();

            SetBitrate((AudioBitrate)Settings.Default.Bitrate);
            SetChannel((AudioChannel)Settings.Default.Channel);

            _currentDevice = GetDefaultDevice();

            Device = new AudioEndPoint(_currentDevice.FriendlyName, _currentDevice.DeviceID, GetDeviceFormat(_currentDevice).Channels, _currentDevice.DeviceState != DeviceState.Active, (int?)((_currentDevice.GetJackDescriptions()?.FirstOrDefault())?.Color)?.Value); ;

            _isStartUp = false;
        }

        private void DevicePropertyChanged(object sender, DevicePropertyChangedEventArgs args)
        {

        }

        private void DeviceStateChanged(object sender, CSCore.CoreAudioAPI.DeviceStateChangedEventArgs args)
        {
            GlobalEventAggregator.Instance.SendMessage(new DeviceStateChangedEventArgs(args.DeviceId, (AudioDeviceState)(int)args.DeviceState));

            var device = _deviceEnumerator.GetDevice(args.DeviceId);

            if (device == null || device.DeviceState != DeviceState.Active)
            {
                //do something with the disabled device!
            }

            RefreshDevices();
        }

        private void DeviceRemoved(object sender, DeviceNotificationEventArgs args)
        {

        }

        private void DeviceAdded(object sender, DeviceNotificationEventArgs args)
        {

        }

        private void SleepStateListner(SleepStateEventArgs args)
        {
            if (args == null || args.State == _isRecording)
                return;

            if (args.State)
            {
                StartRecording();
            }
            else
            {
                StopRecording();
            }
        }

        private MMDevice GetDefaultDevice()
        {
            if (!string.IsNullOrWhiteSpace(Settings.Default.InputDevice))
            {
                try
                {
                    var device = _deviceEnumerator.GetDevice(Settings.Default.InputDevice);

                    if (device != null && device.DeviceState == DeviceState.Active)
                        return device;
                }
                catch (CoreAudioAPIException ex)
                {

                }
                catch (Exception ex)
                {

                }

            }

            return _deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Console);
        }

        public IList<AudioEndPoint> Devices
        {
            get
            {
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
            Devices = _deviceEnumerator.EnumAudioEndpoints(DataFlow.Capture, DeviceState.Active | DeviceState.UnPlugged).Select(s => new AudioEndPoint(s.FriendlyName, s.DeviceID, GetDeviceFormat(s).Channels, s.DeviceState != DeviceState.Active, (int?)((s.GetJackDescriptions()?.FirstOrDefault())?.Color)?.Value)).OrderBy(ob => ob.Disabled).ThenBy(tb => tb.DeviceName).ToList();
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

        public IAudioEndPoint Device { get; private set; }


        public AudioChannel Channel
        {
            get => _audioChannel;
            private set
            {
                if (_audioChannel == value)
                {
                    return;
                }

                _audioChannel = value;

                if (!_isStartUp)
                {
                    
                }
            }
        }

        private AudioChannel PreferredChannel { get; set; }

        public AudioHelper SetChannel(AudioChannel audioChannel)
        {
            Channel = audioChannel;

            Settings.Default.Channel = (int)audioChannel;
            Settings.Default.Save();

            return this;
        }

        public AudioBitrate Bitrate
        {
            get => _audioBitrate;
            private set
            {
                if (_audioBitrate == value)
                {
                    return;
                }

                _audioBitrate = value;

                if (!_isStartUp)
                {
                    StartRecording(_currentDevice);
                }
            }
        }

        public AudioHelper SetBitrate(AudioBitrate audioBitrate)
        {
            Bitrate = audioBitrate;

            Settings.Default.Bitrate = (int)audioBitrate;
            Settings.Default.Save();

            return this;
        }

        public void StartRecording()
        {
            _currentDevice = GetDefaultDevice();

            StartRecording(_currentDevice);
        }

        public void StartRecording(IAudioEndPoint audioEndPoint)
        {
            _currentDevice = _deviceEnumerator.GetDevice(audioEndPoint.DeviceId);

            if (_currentDevice.DeviceState != DeviceState.Active)
            {
                return;
            }

            StartRecording(_currentDevice);

            Settings.Default.InputDevice = audioEndPoint.DeviceId;
            Settings.Default.Save();
        }

        private WaveFormat GetDeviceFormat(MMDevice device)
        {
            if (device.DeviceState == DeviceState.Active)
            {
                return WaveFormatFromBlob(device.PropertyStore[
                    new PropertyKey(new Guid(0xf19f064d, 0x82c, 0x4e27, 0xbc, 0x73, 0x68, 0x82, 0xa1, 0xbb, 0x8e, 0x4c),
                        0)].BlobValue);
            }

            return new WaveFormat();
        }

        private void StartRecording(MMDevice mmDevice)
        {
            StopRecording();

            if (mmDevice == null || mmDevice.DataFlow != DataFlow.Capture)
                throw new ArgumentNullException(nameof(mmDevice), "Selected Audio device is invalid.");

            if (mmDevice.DeviceState != DeviceState.Active)
            {
                return;
            }

            Device = new AudioEndPoint(mmDevice.FriendlyName, mmDevice.DeviceID, GetDeviceFormat(mmDevice).Channels, mmDevice.DeviceState != DeviceState.Active, (int?)((mmDevice.GetJackDescriptions()?.FirstOrDefault())?.Color)?.Value);

            var deviceFormat = GetDeviceFormat(mmDevice);

            var audioEndPointArgs = new AudioEndPointEventArgs(Device.DeviceName, Device.DeviceId, deviceFormat.Channels, Device.Disabled, Device.JackColor);
            AudioEndPointChanged?.Invoke(this, audioEndPointArgs);
            GlobalEventAggregator.Instance.SendMessage(audioEndPointArgs);

            _soundIn =
                new WasapiCapture(false, AudioClientShareMode.Shared, 0, deviceFormat,
                    ThreadPriority.Highest)
                {
                    Device = mmDevice
                };

            _soundIn.Initialize();

            SoundInSource soundInSource = new SoundInSource(_soundIn)
            {
                FillWithZeros = false
            };

            _convertedSource = soundInSource
                .ChangeSampleRate(192000) // sample rate
                .ToSampleSource()
                .ToWaveSource(/*_audioBitrate == AudioBitrate.EightBit ? 8 :*/ 16); //bits per sample

            //var singleBlockNotificationStream = new SingleBlockNotificationStream(soundInSource.ToSampleSource());
            //_finalSource = singleBlockNotificationStream.ToWaveSource();

            //byte[] buffer = new byte[_finalSource.WaveFormat.BytesPerSecond / 2];
            soundInSource.DataAvailable += SoundInSourceOnDataAvailable;

            _soundIn.Start();

            PollAudioLevels();

            _isRecording = true;

            GlobalEventAggregator.Instance.SendMessage(new RecordingStateEventArgs(RecordingState.Started));
        }

        private void SoundInSourceOnDataAvailable(object sender, DataAvailableEventArgs args)
        {
            if (!(sender is IWaveSource source))
                return;
            
            var bufferArray = new byte[_convertedSource.WaveFormat.BytesPerSecond / 2];

            //keep reading as long as we still get some data
            //if you're using such a loop, make sure that soundInSource.FillWithZeros is set to false
            var elementsRead = _convertedSource.Read(bufferArray, 0, bufferArray.Length);
            
            var eventArgs = new AudioDataEventArgs(_convertedSource.WaveFormat.SampleRate, _convertedSource.WaveFormat.BitsPerSample,
                _convertedSource.WaveFormat.Channels, elementsRead, bufferArray.Take(elementsRead).ToArray(), PreferredChannel, _audioBitrate);
             
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

            _isRecording = false;

            _pollingCancellationTokenSource = new CancellationTokenSource();

            GlobalEventAggregator.Instance.SendMessage(new RecordingStateEventArgs(RecordingState.Stopped));
        }

        private void PollAudioLevels()
        {
            _pollingTask = Task.Factory.StartNew(() =>
            {

                var audioEndpointVolume = AudioEndpointVolume.FromDevice(_currentDevice);
                var audioMeterInformation = AudioMeterInformation.FromDevice(_currentDevice);

                while (!_pollingCancellationTokenSource.IsCancellationRequested)
                {
                    var channelPeaks = audioMeterInformation.GetChannelsPeakValues();

                    var peakValues = new PeakValues(audioEndpointVolume.IsMuted,
                        audioEndpointVolume.MasterVolumeLevel,
                        channelPeaks[0],
                        audioMeterInformation.MeteringChannelCount == 1 ? (float?)null : channelPeaks[1]);

                    if (!_lastPeakValues.Muted.Equals(peakValues.Muted) ||
                        !_lastPeakValues.Master.Equals(peakValues.Master) ||
                        !_lastPeakValues.Left.Equals(peakValues.Left) ||
                        !_lastPeakValues.Right.Equals(peakValues.Right))
                    {
                        var args = new PeakValueEventArgs(peakValues);
                        GlobalEventAggregator.Instance.SendMessage(args);

                        _lastPeakValues.Update(peakValues);
                    }

                    var preferredChannel = Channel == AudioChannel.Automatic
                        ? new PreferredChannelEventArgs(peakValues.Left, peakValues.Right)
                        : new PreferredChannelEventArgs(Channel);

                    PreferredChannel = preferredChannel.Channel;

                    GlobalEventAggregator.Instance.SendMessage(preferredChannel);
                }

                audioEndpointVolume?.Dispose();
                audioMeterInformation?.Dispose();

            }, _pollingCancellationTokenSource.Token);
        }

        public void Dispose()
        {
            if (_pollingTask?.Status == TaskStatus.Running || _pollingCancellationTokenSource?.IsCancellationRequested != true)
                StopRecording();

            _soundIn?.Dispose();

            _deviceEnumerator?.Dispose();

        }
    }
}
