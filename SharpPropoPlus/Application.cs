using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using SharpPropoPlus.Audio.EventArguments;
using SharpPropoPlus.Decoder;
using SharpPropoPlus.Decoder.Contracts;
using SharpPropoPlus.Events;
using SharpPropoPlus.vJoyMonitor;
using SharpPropoPlus.Views;

namespace SharpPropoPlus
{
  public class Application: IDisposable
  {
    private static Application _instance;
    private static readonly object Sync = new object();
    private readonly DecoderManager  _decoderManager;
    private Lazy<IPropoPlusDecoder, IDecoderMetadata> _decoder;


    private Application()
    {
      _decoderManager = new DecoderManager();
      _decoder = DecoderManager.Decoders.First();
      JoystickHelper.Initialize();
      JoystickInteraction.Initialize();

      GlobalEventAggregator.Instance.AddListener<AudioDataEventArgs>(AudioDataAction);

      System.Windows.Application.Current.MainWindow = (Window)new Shell();
    }

    private void AudioDataAction(AudioDataEventArgs args)
    {
      //var bitsPerSample = source.WaveFormat.BitsPerSample;
      //var sampleRate = source.WaveFormat.SampleRate;
      //var channels = source.WaveFormat.Channels;

      var samplesDesired = args.BytesRecorded / args.Channels;

      var left = new int[samplesDesired];
      var right = new int[samplesDesired];
      var index = 0;

      for (var sample = 0; sample < args.BytesRecorded / 4; sample++)
      {

        //left[sample] = BitConverter.ToInt16(args.Buffer, index);
        index += 2;
        right[sample] = BitConverter.ToInt16(args.Buffer, index);
        index += 2;

        _decoder.Value.ProcessPulse(args.SampleRate, right[sample]);
      }

    }


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

    public DecoderManager DecoderManager
    {
      get { return _decoderManager; }
    }

    public void ShowMainWindow()
    {
      System.Windows.Application.Current.MainWindow.Show();
    }

    public void Dispose()
    {
      GlobalEventAggregator.Instance.AddListener<AudioDataEventArgs>(AudioDataAction);
    }
  }
}