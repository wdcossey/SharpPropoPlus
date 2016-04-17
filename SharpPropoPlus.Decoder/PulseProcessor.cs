using System;
using SharpPropoPlus.Decoder.EventArguments;
using SharpPropoPlus.Decoder.Structs;
using SharpPropoPlus.Events;

namespace SharpPropoPlus.Decoder
{
  public abstract class PulseProcessor
  {

    private static int _nChannels;

    /// <summary>
    /// Sticky minimum sample value
    /// </summary>
    private static double _minSampleValue;

    /// <summary>
    /// Sticky maximum sample value
    /// </summary>
    private static double _maxSampleValue;

    /// <summary>
    /// Number of contingious above-threshold samples
    /// </summary>
    private static int _high;

    /// <summary>
    /// Number of contingious below-threshold samples
    /// </summary>
    private static int _low;

    public static int RawChannelCount
    {
      get { return _nChannels; }
      protected set
      {
        if (_nChannels.Equals(value))
          return;

        _nChannels = value;

        GlobalEventAggregator.Instance.SendMessage(new PollChannelsEventArgs(_nChannels));
      }
    }

    /// <summary>
    /// UINT CSppProcess::Sample2Pulse(short sample, bool * negative)
    /// </summary>
    /// <param name="sampleRate"></param>
    /// <param name="sample"></param>
    /// <param name="negative"></param>
    /// <returns></returns>
    protected static PulseLength CalculatePulseLength(int sampleRate, int sample, ref bool negative)
    {

      var pulseLength = new PulseLength(0, 0);

      //Initialization of the _minSampleValue/_maxSampleValue vaues
      _maxSampleValue -= 0.1;
      _minSampleValue += 0.1;

      if (_maxSampleValue < _minSampleValue) _maxSampleValue = _minSampleValue + 1;

      if (sample > _maxSampleValue)
      {
        //Update _max value
        _maxSampleValue = sample;
      }
      else if (sample < _minSampleValue)
      {
        //Update _min value
        _minSampleValue = sample;
      }

      ////Update mid-point value
      //threshold = (_minSampleValue + _maxSampleValue) / 2;		

      //calculated mid-point
      var threshold = CalcThreshold(sample);

      //Update the width of the number of the _low/_high samples 
      //If edge, then call ProcessPulse() to process the previous _high/_low level 
      if (sample > threshold)
      {
        _high++;
        if (!_low.Equals(0))
        {
          pulseLength.Raw = _low;
          negative = true;
          _low = 0;
        }
      }
      else
      {
        _low++;
        if (!_high.Equals(0))
        {
          pulseLength.Raw = _high;
          negative = false;
          _high = 0;
        }
      }
      ;

      // Case of very long (20000) pulses
      if (_high >= 20000)
      {
        pulseLength.Raw = Convert.ToInt32(_high);
        negative = false;
        _high = 0;
      }
      else if (_low >= 20000)
      {
        pulseLength.Raw = _low;
        negative = true;
        _low = 0;
      }
      ;


      pulseLength.Normalized = NormalizePulse(sampleRate, pulseLength.Raw);

      return pulseLength;
    }

    private static double _audMaxVal;
    private static double _audMinVal;
    private static int _cAboveThr;
    private static int _cBelowThr;

    /// <summary>
    /// <para>Calculate audio threshold</para><br/>
    /// <para>Based on RCAudio V 3.0 and original Smartpropo</para>
    /// 
    /// <para>Copyright © Philippe G.De Coninck 2007</para>&#160;
    /// <para>Copied from: http://www.rcuniverse.com/forum/m_3413991/tm.htm</para>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static double CalcThreshold(int value)
    {
      // RCAudio V 3.0 : (C) Philippe G.De Coninck 2007

      var deltaMax = Math.Abs(value - _audMaxVal);
      var deltaMin = Math.Abs(value - _audMinVal);

      if (deltaMax > deltaMin) _audMinVal = (4 * _audMinVal + value) / 5;
      else _audMaxVal = (4 * _audMaxVal + value) / 5;

      if (_audMaxVal < _audMinVal + 2)
      {
        _audMaxVal = _audMinVal + 1;
        _audMinVal = _audMinVal - 1;
      }

      var threthold = (_audMaxVal + _audMinVal) / 2;

      //Patch: reset threshold if nothing happens
      if (value > threthold)
      {
        _cAboveThr++;
        _cBelowThr = 0;
      }
      else
      {
        _cAboveThr = 0;
        _cBelowThr++;
      }
      ;
      if (_cAboveThr >= 10000 || _cBelowThr >= 10000)
        _audMaxVal = _audMinVal = 0;

      return (threthold);
    }

    /// <summary>
    /// Normalize pulse length to 192K sampling rate
    /// </summary>
    /// <param name="samplesPerSecond"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    private static int NormalizePulse(int samplesPerSecond, int length)
    {
      switch (samplesPerSecond)
      {
        case 192000:
        case 0:
          return length;
        case 96000:
          return length * 2;
        case 48000:
          return length * 4;
        case 44100:
          return (int)(length * 4.35);
        default:
          return length * 192000 / samplesPerSecond;
      }

    }


  }
}