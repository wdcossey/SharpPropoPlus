namespace SharpPropoPlus.Audio.Models
{
    public class PeakValues
    {
        public float Left { get; internal set; } = 0.0f;

        public float? Right { get; internal set; } = 0.0f;

        public float Master { get; internal set; } = 0.0f;

        public bool Muted { get; internal set; } = false;


        internal PeakValues()
        {

        }

        public PeakValues(bool muted, float master, float left, float? right)
            : this()
        {
            Update(muted, master, left, right);
        }

        private void Update(bool muted, float master, float left, float? right)
        {
            Muted = muted;
            Master = master;
            Left = left;
            Right = right;
        }

        internal void Update(PeakValues values)
        {
            Update(values.Muted, values.Master, values.Left, values.Right);
        }
    }
}