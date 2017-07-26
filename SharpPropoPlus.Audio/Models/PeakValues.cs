namespace SharpPropoPlus.Audio.Models
{
  public class PeakValues
  {
    private float _left = 0.0f;
    private float? _right = 0.0f;
    private float _master = 0.0f;
    private bool _muted = false;

    public float Left
    {
      get { return _left; }
      internal set { _left = value; }
    }

    public float? Right
    {
      get { return _right; }
      internal set { _right = value; }
    }

    public float Master
    {
      get { return _master; }
      internal set { _master = value; }
    }

    public bool Muted
    {
      get { return _muted; }
      internal set { _muted = value; }
    }


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