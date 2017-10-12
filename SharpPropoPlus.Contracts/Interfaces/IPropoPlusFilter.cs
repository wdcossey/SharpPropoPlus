﻿namespace SharpPropoPlus.Contracts.Interfaces
{
  public interface IPropoPlusFilter  
  {
    string[] Description { get; }

    /// <summary>
    /// Runs the Filter.
    /// </summary>
    int RunFilter(ref int[] channelData, int channelCount);
  }
}