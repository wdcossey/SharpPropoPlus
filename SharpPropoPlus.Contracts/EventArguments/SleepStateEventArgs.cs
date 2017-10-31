using System;

namespace SharpPropoPlus.Contracts.EventArguments
{
    public class SleepStateEventArgs : EventArgs
    {
        public bool State { get; }

        public SleepStateEventArgs(bool state)
        {
            State = state;
        }
    }
}