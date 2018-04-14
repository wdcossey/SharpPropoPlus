using System.Configuration;
using System.Diagnostics;

namespace SharpPropoPlus.Decoder.Contracts
{
    public interface IPropoPlusPpmSettings : IPropoPlusSettings
    {
        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("0")]
        double PpmMinPulseWidth { get; set; }

        double PpmMinPulseWidthDefault { get; }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("0")]
        double PpmMaxPulseWidth { get; set; }

        double PpmMaxPulseWidthDefault { get; }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("0")]
        double PpmTrig { get; set; }

        double PpmTrigDefault { get; }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("0")]
        double PpmSeparator { get; set; }

        double PpmSeparatorDefault { get; }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("0")]
        double PpmGlitch { get; set; }

        double PpmGlitchDefault { get; }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("0")]
        double PpmJitter { get; set; }

        double PpmJitterDefault { get; }

    }
}