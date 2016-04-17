using System;
using SharpPropoPlus.Audio;
using SharpPropoPlus.Audio.Enums;
using SharpPropoPlus.Audio.EventArguments;
using SharpPropoPlus.Decoder;
using SharpPropoPlus.Events;
using SharpPropoPlus.Decoder.EventArguments;

namespace SharpPropoPlus.ViewModels
{
  public class OverviewViewModel: BaseViewModel
  {
    private string _prefferedChannel = string.Empty;
    private string _deviceName = string.Empty;
    private int _rawChannels;
    private string _joystickName = string.Empty;

    public OverviewViewModel()
    {

      PrefferedChannel = AudioChannel.Left.ToString();

      GlobalEventAggregator.Instance.AddListener<PreferredChannelEventArgs>(PrefferedChannelListner);
      GlobalEventAggregator.Instance.AddListener<DeviceInfoEventArgs>(DeviceInfoListner);
      GlobalEventAggregator.Instance.AddListener<PollChannelsEventArgs>(PollChannelListner);
      GlobalEventAggregator.Instance.AddListener<JoystickChangedEventArgs>(JoystickChangedListener);

      DeviceName = AudioHelper.Instance.DeviceName;
      JoystickName = JoystickInteraction.Instance.CurrentDevice.Name;
    }

    private void JoystickChangedListener(JoystickChangedEventArgs args)
    {
      if (args == null)
        return;

      JoystickName = args.Name;
    }

    private void PollChannelListner(PollChannelsEventArgs args)
    {
      if (args == null)
        return;

      RawChannels = args.RawChannels;
    }

    private void DeviceInfoListner(DeviceInfoEventArgs args)
    {
      if (args == null)
        return;

      DeviceName = args.Name;
    }

    private void PrefferedChannelListner(PreferredChannelEventArgs args)
    {
      if (args == null)
        return;

      PrefferedChannel = args.Channel.ToString();
    }

    public string DeviceName
    {
      get { return _deviceName; }
      private set
      {
        if (_deviceName.Equals(value, StringComparison.InvariantCulture))
          return;

        _deviceName = value;
        OnPropertyChanged();
      }
    }

    public string PrefferedChannel
    {
      get { return _prefferedChannel; }
      private set
      {
        if (_prefferedChannel.Equals(value, StringComparison.InvariantCulture))
          return;

        _prefferedChannel = value;
        OnPropertyChanged();
      }
    }

    public int RawChannels
    {
      get { return _rawChannels; }
      private set
      {
        if (_rawChannels.Equals(value))
          return;

        _rawChannels = value;
        OnPropertyChanged();
      }
    }

    public string JoystickName
    {
      get { return _joystickName; }
      private set
      {
        if (_joystickName.Equals(value, StringComparison.InvariantCulture))
          return;

        _joystickName = value;
        OnPropertyChanged();
      }
    }

    public override void Dispose()
    {
      GlobalEventAggregator.Instance.RemoveListener<PreferredChannelEventArgs>(PrefferedChannelListner);
      GlobalEventAggregator.Instance.RemoveListener<DeviceInfoEventArgs>(DeviceInfoListner);
      GlobalEventAggregator.Instance.RemoveListener<PollChannelsEventArgs>(PollChannelListner);
      GlobalEventAggregator.Instance.RemoveListener<JoystickChangedEventArgs>(JoystickChangedListener);

      base.Dispose();
    }
  }
}