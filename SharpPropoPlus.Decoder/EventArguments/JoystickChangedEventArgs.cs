using System;

namespace SharpPropoPlus.Decoder.EventArguments
{
    public class JoystickChangedEventArgs : EventArgs
    {
        internal JoystickChangedEventArgs()
        {

        }

        public JoystickChangedEventArgs(int deviceId, string name, Guid productGuid)
            : this()
        {
            Id = deviceId;
            Name = name;
            Guid = productGuid;
        }

        public int Id { get; }

        public string Name { get; }

        public Guid Guid { get; }
    }
}