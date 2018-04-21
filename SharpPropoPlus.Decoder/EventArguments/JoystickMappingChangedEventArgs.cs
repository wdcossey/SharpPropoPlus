namespace SharpPropoPlus.Decoder.EventArguments
{
    public class JoystickMappingChangedEventArgs
    {
        internal JoystickMappingChangedEventArgs()
        {

        }

        public JoystickMappingChangedEventArgs(int[] mapping)
            : this()
        {
            Mapping = mapping;
        }

        public int[] Mapping { get; }

    }
}