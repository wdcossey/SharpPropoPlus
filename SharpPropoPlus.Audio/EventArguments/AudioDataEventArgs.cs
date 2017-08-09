using System;

namespace SharpPropoPlus.Audio.EventArguments
{
  public class AudioDataEventArgs : EventArgs
  {
    private readonly int _sampleRate;
    private readonly int _channels;
    private readonly int _bytesRecorded;
    private readonly byte[] _buffer;


    private AudioDataEventArgs()
    {

    }

    public AudioDataEventArgs(int sampleRate, int channels, int bytesRecorded, byte[] buffer)
      : this()
    {
      _sampleRate = sampleRate;
      _channels = channels;
      _bytesRecorded = bytesRecorded;
      _buffer = buffer;
    }

    public int SampleRate
    {
      get { return _sampleRate; }
    }

    public int Channels
    {
      get { return _channels; }
    }

    public int BytesRecorded
    {
      get { return _bytesRecorded; }
    }

    public byte[] Buffer
    {
      get { return _buffer; }
    }
  }
}