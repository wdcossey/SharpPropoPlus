using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.CoreAudioApi;
using NAudio.Wave;
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
    private readonly MMDeviceEnumerator _deviceEnumerator;
    private string _deviceId;
    private string _deviceName;
    private readonly PeakValues _lastPeakValues;
    private static readonly object _sync = new object();
    private static volatile AudioHelper _instance;

    private AudioChannel _audioChannel = AudioChannel.Automatic;

    private WaveIn _waveIn = null;


    private AudioHelper()
    {
      _quitPolling = false;
      _deviceEnumerator = new MMDeviceEnumerator();
      _lastPeakValues = new PeakValues();

      var device = GetDefaultDevice();

      DeviceId = device.ID;
      DeviceName = device.FriendlyName;
    }

    private MMDevice GetDefaultDevice()
    {
      return _deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Console);
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


    public void StartRecording(string deviceId)
    {
      var device = _deviceEnumerator.GetDevice(deviceId);

      StartRecording(device);
    }

    private void StartRecording(MMDevice device)
    {

      StopRecording();

      if (device == null || device.DataFlow != DataFlow.Capture)
        throw new ArgumentNullException(nameof(device), "Selected Audio device is invalid.");

      DeviceId = device.ID;
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

      var args = new DeviceInfoEventArgs(device);
      //OnDeviceChanged(args);
      GlobalEventAggregator.Instance.SendMessage(args);

      //var x = _deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.All)
      //  .FirstOrDefault(w => w.ID == _deviceId);

      var channels = device.AudioEndpointVolume.Channels.Count;

      _waveIn = new WaveIn
      {
        WaveFormat = new WaveFormat(192000, 16, channels),
        //NumberOfBuffers = 1,
        BufferMilliseconds = 5,
      };

      _waveIn.DataAvailable += WaveInOnDataAvailable;
      _waveIn.RecordingStopped += WaveInOnRecordingStopped;
      _waveIn.StartRecording();

      PollAudioLevels();
    }


    private void WaveInOnRecordingStopped(object sender, StoppedEventArgs args)
    {
      //
    }

    private void WaveInOnDataAvailable(object sender, WaveInEventArgs args)
    {
      var source = sender as IWaveIn;

      if (source == null)
        return;

      var bitsPerSample = source.WaveFormat.BitsPerSample;
      var sampleRate = source.WaveFormat.SampleRate;
      var channels = source.WaveFormat.Channels;

      int samplesDesired = args.BytesRecorded / channels;

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

      var message = new AudioDataEventArgs(sampleRate, channels, args.BytesRecorded, args.Buffer);
      GlobalEventAggregator.Instance.SendMessage(message);


    }

    public void StopRecording()
    {
      _waveIn?.StopRecording();
      _quitPolling = true;
      _pollingTask?.Wait();
      _quitPolling = false;
    }

    private void PollAudioLevels()
    {

      _pollingTask = Task.Factory.StartNew(() =>
      {

        var device = _deviceEnumerator.GetDevice(DeviceId);

        while (!_quitPolling)
        {

          var peakValues = new PeakValues(device.AudioEndpointVolume.Mute, device.AudioMeterInformation.MasterPeakValue,
            device.AudioMeterInformation.PeakValues[0], device.AudioMeterInformation.PeakValues[1]);

          if (!_lastPeakValues.Muted.Equals(peakValues.Muted) || !_lastPeakValues.Master.Equals(peakValues.Master) || !_lastPeakValues.Left.Equals(peakValues.Left) ||
              !_lastPeakValues.Right.Equals(peakValues.Right))
          {
            var args = new PeakValueEventArgs(peakValues);
            //OnPeakValuesChanged(args);
            GlobalEventAggregator.Instance.SendMessage(args);

            _lastPeakValues.Update(peakValues);
          }

          var preferredChannel = Channel == AudioChannel.Automatic ? new PreferredChannelEventArgs(peakValues.Left, peakValues.Right) : new PreferredChannelEventArgs(Channel);
          GlobalEventAggregator.Instance.SendMessage(preferredChannel);

          //var deviceInfo = new DeviceInfoEventArgs(device);
          //GlobalEventAggregator.Instance.SendMessage(deviceInfo);

          Task.Delay(200);
        }

        _quitPolling = false;

      });

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

      _waveIn?.Dispose();
      //_device?.Dispose();
      //_deviceEnumerator?.Dispose();

    }
  }

  

  

  




}
