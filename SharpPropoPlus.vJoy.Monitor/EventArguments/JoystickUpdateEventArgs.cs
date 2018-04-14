using System;

namespace SharpPropoPlus.vJoyMonitor.EventArguments
{
    public class JoystickUpdateEventArgs : EventArgs
    {
        public int AxisX { get; }
        public int AxisY { get; }
        public int AxisZ { get; }
        public int RotationX { get; }
        public int RotationY { get; }
        public int RotationZ { get; }
        public int Slider0 { get; }
        public int Slider1 { get; }

        private JoystickUpdateEventArgs()
        {

        }

        public JoystickUpdateEventArgs(
            int axisX,
            int axisY,
            int axisZ,
            int rotationX,
            int rotationY,
            int rotationZ,
            int slider0,
            int slider1)
            : this()
        {
            AxisX = axisX;
            AxisY = axisY;
            AxisZ = axisZ;
            RotationX = rotationX;
            RotationY = rotationY;
            RotationZ = rotationZ;
            Slider0 = slider0;
            Slider1 = slider1;
        }
    }
}