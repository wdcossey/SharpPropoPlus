using System;
using SharpPropoPlus.Audio.Models;

namespace SharpPropoPlus.Audio.EventArguments
{
  public class PeakValueEventArgs : EventArgs
  {
    private PeakValues _values;

    public PeakValues Values
    {
      get { return _values; }
      internal set { _values = value; }
    }

    private PeakValueEventArgs()
    {

    }

    public PeakValueEventArgs(PeakValues values)
      : this()
    {
      Values = values;
    }
  }
}