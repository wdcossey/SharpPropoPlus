using System;
using SharpPropoPlus.Contracts.Enums;

namespace SharpPropoPlus.Contracts.EventArguments
{
    public class RecordingStateEventArgs : EventArgs
    {
        public RecordingState State { get; }

        public RecordingStateEventArgs(RecordingState state)
        {
            State = state;
        }
    }
}