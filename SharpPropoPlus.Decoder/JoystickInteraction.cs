using System;
using System.Linq;
using SharpPropoPlus.Contracts.Interfaces;
using SharpPropoPlus.Decoder.Enums;
using SharpPropoPlus.Decoder.EventArguments;
using SharpPropoPlus.Decoder.Models;
using SharpPropoPlus.Events;
using vJoyInterfaceWrap;

namespace SharpPropoPlus.Decoder
{
    public class JoystickInteraction : IDisposable
    {

        private static JoystickInteraction _instance;
        private static readonly object Sync = new object();

        private JoystickInteraction()
        {
            GlobalEventAggregator.Instance.AddListener<JoystickChangedEventArgs>(JoystickChangedListener);
        }

        private void JoystickChangedListener(JoystickChangedEventArgs args)
        {
            if (args == null)
                return;

            var deviceId = (uint) args.Id;

            if (!deviceId.Equals(Convert.ToUInt32(CurrentDevice.Id)))
            {
                var vJoy = new vJoy();
                vJoy.RelinquishVJD(Convert.ToUInt32(CurrentDevice.Id));

                CurrentDevice = new JoystickInformation(args.Id, args.Name, args.Guid);
            }
        }

        public JoystickInformation CurrentDevice
        {
            get => _currentDevice;

            set
            {
                if (_currentDevice != null && value != null && _currentDevice.Id.Equals(value.Id))
                    return;

                _currentDevice = value;
            }
        }

        // ReSharper disable once InconsistentNaming
        private uint vJoyDeviceId
        {
            get
            {
                if (CurrentDevice == null)
                    return 0;

                return Convert.ToUInt32(CurrentDevice.Id);
            }
        }

        public static JoystickInteraction Instance
        {
            get
            {
                //if (_instance == null)
                //{
                //  lock (Sync)
                //  {
                //    if (_instance == null)
                //      _instance = new JoystickInteraction();
                //  }
                //}

                if (!_initialized)
                    throw new NullReferenceException($"{nameof(JoystickInteraction)} must be initialized before use!");

                return _instance;
            }
        }

        public static void Initialize()
        {
            try
            {

                if (_initialized)
                    return;

                if (_instance == null)
                {
                    lock (Sync)
                    {
                        if (_instance == null)
                            _instance = new JoystickInteraction();
                    }
                }

                _initialized = true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private JoystickInformation _currentDevice = new JoystickInformation();

        //static UINT _count = 0;
        private static uint _count = 0;
        //static std::chrono::system_clock::time_point _tPrev = std::chrono::system_clock::now();
        //private static double _tPrev = 0;

        private static int[] _mBtnMapping = new int[Constants.MAX_BUTTONS];

        private static readonly vJoy VJoy = new vJoy();
        private static vJoy.JoystickState _joystickState;
        private static bool _initialized;

        public void Send(int nChannels, int[] channel, bool filterChannels, IPropoPlusFilter filter)
        {
            bool writeOk;
            //uint rId = 2; //m_vJoyDeviceId;
            int i;
            int k;
            var ch = new int[channel.Length];
            var nCh = 0;

            if (VJoy.GetVJDStatus(vJoyDeviceId) == VjdStat.VJD_STAT_FREE)
            {
                VJoy.AcquireVJD(vJoyDeviceId);
            }


            //if (!m_vJoyReady)
            //  return;

            //Duplicate channel data
            //memcpy(ch, channel, MAX_JS_CH * sizeof(int));
            Array.Copy(channel, ch, ch.Length);

            nCh = nChannels;

            if (filterChannels && filter != null)
            {
                nCh = filter.RunFilter(ref ch, nChannels);
                if (nCh > 0)
                    nCh = nChannels;
            }

            // Create a public duplication of processed data for monitoring
            //memcpy(m_PrcChannel, ch, MAX_JS_CH * sizeof(int));

            // Fill-in the structure to be fed to the vJoy device - Axes the Buttons
            for (i = 0; /*i<=n_ch &&*/ i <= (int) HidUsageFlags.HID_USAGE_SL1 - (int) HidUsageFlags.HID_USAGE_X; i++)
            {

                //iMapped = Map2Nibble(m_Mapping, i); // Prepare mapping re-indexing
                var iMapped = Map2Nibble(0x12345678, i);

                if (iMapped > nChannels)
                    continue;

                // Test value - if (-1) value is illegal
                if (ch[iMapped - 1] < 0)
                    return;

                writeOk = SetAxisDelayed(ref _joystickState, 32 * ch[iMapped - 1],
                    (HidUsageFlags?) ((int) HidUsageFlags.HID_USAGE_X + i));
                // TODO: the normalization to default values should be done in the calling functions
            }

            if (_mBtnMapping == null || !_mBtnMapping.Any() || _mBtnMapping.All(a => a == 0))
            {
                SetDefaultBtnMap(ref _mBtnMapping);
            }

            for (k = 0; k < Constants.MAX_BUTTONS; k++)
            {
                var index = _mBtnMapping[k] - 1;
                var btnValue = int.MinValue;
                if (index < ch.Length)
                    btnValue = ch[index];

                //VJoy.SetBtn(value > 511, rID, (uint) k + 1);

                writeOk = SetBtnDelayed(ref _joystickState, btnValue > 511, k + 1);
                // TODO: Replace 511 with some constant definition
            }

            //// Feed structure to vJoy device rID
            //bool updated = UpdateVJD(rID, &m_vJoyPosition);
            bool updated = VJoy.UpdateVJD(vJoyDeviceId, ref _joystickState);

            // Calculate quality of joystick data

            //std::chrono::system_clock::time_point tNow = std::chrono::system_clock::now();
            //std::chrono::duration<double> delta = std::chrono::system_clock::duration::zero();
            if (updated)
            {
                _count++;
                if (_count >= 10)
                {
                    //    delta = tNow - _tPrev;
                    //    _tPrev = tNow;
                    //    _count = 0;
                    //    double c = delta._count();
                    //    m_JoyQual = (UINT)(100 / c);
                }
            }
            else
            {
                //  m_JoyQual = 0;
            }


        }

        private static int Map2Nibble(int map, int i)
        {
            return ((map & (0xF << (4 * (7 - i)))) >> (4 * (7 - i))) & 0xF;
        }

        private static void SetDefaultBtnMap(ref int[] btnMap)
        {
            var size = btnMap.Length;
            for (var i = 0; i < size; i++)
            {
                if (i < 24)
                    btnMap[i] = i + 9;
                else
                    btnMap[i] = 9;
            }

        }

        /// <summary>
        /// <para>Write value to a given axis defined in the specified VDJ</para>
        /// <para>Limited to the following axes:</para>
        /// <para><see cref="HidUsageFlags.HID_USAGE_X"/></para>
        /// <para><see cref="HidUsageFlags.HID_USAGE_Y"/></para>
        /// <para><see cref="HidUsageFlags.HID_USAGE_Z"/></para>
        /// <para><see cref="HidUsageFlags.HID_USAGE_RX"/></para>
        /// <para><see cref="HidUsageFlags.HID_USAGE_RY"/></para>
        /// <para><see cref="HidUsageFlags.HID_USAGE_RZ"/></para>
        /// <para><see cref="HidUsageFlags.HID_USAGE_SL0"/></para>
        /// <para><see cref="HidUsageFlags.HID_USAGE_SL1"/></para>
        /// <para><see cref="HidUsageFlags.HID_USAGE_WHL"/></para>
        /// </summary>
        /// <param name="joystickState"></param>
        /// <param name="axisValue"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        private static bool SetAxisDelayed(ref vJoy.JoystickState joystickState, int axisValue, HidUsageFlags? axis)
        {
            if (!axis.HasValue || (int) axis < (int) HidUsageFlags.HID_USAGE_X ||
                (int) axis > (int) HidUsageFlags.HID_USAGE_WHL)
                return false;

            switch (axis)
            {
                case HidUsageFlags.HID_USAGE_X:
                    joystickState.AxisX = axisValue;
                    break;
                case HidUsageFlags.HID_USAGE_Y:
                    joystickState.AxisY = axisValue;
                    break;
                case HidUsageFlags.HID_USAGE_Z:
                    joystickState.AxisZ = axisValue;
                    break;
                case HidUsageFlags.HID_USAGE_RX:
                    joystickState.AxisXRot = axisValue;
                    break;
                case HidUsageFlags.HID_USAGE_RY:
                    joystickState.AxisYRot = axisValue;
                    break;
                case HidUsageFlags.HID_USAGE_RZ:
                    joystickState.AxisZRot = axisValue;
                    break;
                case HidUsageFlags.HID_USAGE_SL0:
                    joystickState.Slider = axisValue;
                    break;
                case HidUsageFlags.HID_USAGE_SL1:
                    joystickState.Dial = axisValue;
                    break;
                case HidUsageFlags.HID_USAGE_WHL:
                    joystickState.Wheel = axisValue;
                    break;
                default:
                    return false;
            }

            return true;
        }

        private static bool SetBtnDelayed(ref vJoy.JoystickState joystickState, bool btnValue, int nBtn)
        {
            uint mask = 0x00000001;

            // Write value to a given button defined in the specified VDJ
            if (nBtn < 1 || nBtn > 32)
                return false;

            // If value=TRUE the the given button is set to 1
            if (btnValue)
            {
                mask = mask << (nBtn - 1);
                joystickState.Buttons |= mask;
            }
            else
            {
                mask = mask << (nBtn - 1);
                mask = ~mask;
                joystickState.Buttons &= mask;
            }

            return true;
        }

        public void Dispose()
        {
            GlobalEventAggregator.Instance.RemoveListener<JoystickChangedEventArgs>(JoystickChangedListener);
        }
    }
}