namespace SharpPropoPlus.Decoder.Contracts
{
    public interface IPropoPlusPpmSettings
    {
        [System.Configuration.UserScopedSetting]
        [System.Diagnostics.DebuggerNonUserCode]
        [System.Configuration.DefaultSettingValue("0")]
        double PpmMinPulseWidth { get; set; }

        double PpmMinPulseWidthDefault { get; }

        [System.Configuration.UserScopedSetting]
        [System.Diagnostics.DebuggerNonUserCode]
        [System.Configuration.DefaultSettingValue("0")]
        double PpmMaxPulseWidth { get; set; }

        double PpmMaxPulseWidthDefault { get; }

        [System.Configuration.UserScopedSetting]
        [System.Diagnostics.DebuggerNonUserCode]
        [System.Configuration.DefaultSettingValue("0")]
        double PpmTrig { get; set; }

        double PpmTrigDefault { get; }

        [System.Configuration.UserScopedSetting]
        [System.Diagnostics.DebuggerNonUserCode]
        [System.Configuration.DefaultSettingValue("0")]
        double PpmSeparator { get; set; }

        double PpmSeparatorDefault { get; }

        [System.Configuration.UserScopedSetting]
        [System.Diagnostics.DebuggerNonUserCode]
        [System.Configuration.DefaultSettingValue("0")]
        double PpmGlitch { get; set; }

        double PpmGlitchDefault { get; }

        [System.Configuration.UserScopedSetting]
        [System.Diagnostics.DebuggerNonUserCode]
        [System.Configuration.DefaultSettingValue("0")]
        double PpmJitter { get; set; }

        double PpmJitterDefault { get; }

        void Save();
    }
}