using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using SharpPropoPlus.Annotations;
using SharpPropoPlus.Audio;
using SharpPropoPlus.Audio.Enums;
using SharpPropoPlus.Audio.EventArguments;
using SharpPropoPlus.Events;
using SharpPropoPlus.Events.Events;
using SharpPropoPlus.vJoyMonitor;

namespace SharpPropoPlus.ViewModels
{
  public class AudioConfigViewModel: BaseViewModel
  {
    private ReadOnlyObservableCollection<AudioBitrate> _bitrateCollection;
    private ReadOnlyObservableCollection<AudioChannel> _channelCollection;
    private AudioBitrate _selectedBitrateItem;
    private AudioChannel _selectedChannelItem;
    private int _leftChannelPeak;
    private int _rightChannelPeak;
    private bool _muted;

    public AudioConfigViewModel()
    {

      BitrateCollection = new ReadOnlyObservableCollection<AudioBitrate>(new ObservableCollection<AudioBitrate>(Enum.GetValues(typeof(AudioBitrate)).Cast<AudioBitrate>()));
      ChannelCollection = new ReadOnlyObservableCollection<AudioChannel>(new ObservableCollection<AudioChannel>(Enum.GetValues(typeof(AudioChannel)).Cast<AudioChannel>()));

      GlobalEventAggregator.Instance.AddListener<PeakValueEventArgs>(PeakValueChangedListner);

      AudioHelper.Instance.StartRecording();
    }

    private void PeakValueChangedListner(PeakValueEventArgs args)
    {
      LeftChannelPeak = (int)Math.Ceiling(args.Values.Left * 100);
      RightChannelPeak = (int)Math.Ceiling(args.Values.Right * 100);
      Muted = args.Values.Muted;
    }

    public ReadOnlyObservableCollection<AudioBitrate> BitrateCollection
    {
      get { return _bitrateCollection; }
      private set
      {
        _bitrateCollection = value;
        OnPropertyChanged();
      }
    }

    public AudioBitrate SelectedBitrateItem
    {
      get { return _selectedBitrateItem; }
      set
      {
        if (Equals(_selectedBitrateItem, value))
          return;

        _selectedBitrateItem = value;
        OnPropertyChanged();
      }
    }

    public int LeftChannelPeak
    {
      get { return _leftChannelPeak; }
      private set
      {
        if (Equals(_leftChannelPeak, value))
          return;

        _leftChannelPeak = value;
        OnPropertyChanged();
      }
    }

    public int RightChannelPeak
    {
      get { return _rightChannelPeak; }
      private set
      {
        if (Equals(_rightChannelPeak, value))
          return;

        _rightChannelPeak = value;
        OnPropertyChanged();
      }
    }

    public bool Muted
    {
      get { return _muted; }
      private set
      {
        if (Equals(_muted, value))
          return;

        _muted = value;
        OnPropertyChanged();
      }
    }

    public ReadOnlyObservableCollection<AudioChannel> ChannelCollection
    {
      get { return _channelCollection; }
      private set
      {
        _channelCollection = value;
        OnPropertyChanged();
      }
    }

    public AudioChannel SelectedChannelItem
    {
      get { return _selectedChannelItem; }
      set
      {
        if (Equals(_selectedChannelItem, value))
          return;

        AudioHelper.Instance.Channel = _selectedChannelItem = value;

        OnPropertyChanged();
      }
    }

    public override void Dispose()
    {
      GlobalEventAggregator.Instance.RemoveListener<PeakValueEventArgs>(PeakValueChangedListner);

      base.Dispose();
    }


  }

}