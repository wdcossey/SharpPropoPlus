using System.Configuration;
using SharpPropoPlus.Decoder.Contracts;

namespace SharpPropoPlus.Decoder.Ppm
{
    public abstract class PpmSettings : ApplicationSettingsBase, IPropoPlusPpmSettings
    {
        [UserScopedSetting]
        [DefaultSettingValue("0")]
        public double PpmMinPulseWidth
        {
            get => (double)this["PpmMinPulseWidth"];
            set => this["PpmMinPulseWidth"] = value;
        }

        public virtual double PpmMinPulseWidthDefault => 96d;

        [UserScopedSetting]
        [DefaultSettingValue("0")]
        public double PpmMaxPulseWidth
        {
            get => (double)this["PpmMaxPulseWidth"];
            set => this["PpmMaxPulseWidth"] = value;
        }

        public virtual double PpmMaxPulseWidthDefault => 288d;

        [UserScopedSetting]
        [DefaultSettingValue("0")]
        public double PpmTrig
        {
            get => (double)this["PpmTrig"];
            set => this["PpmTrig"] = value;
        }

        public virtual double PpmTrigDefault => 870d;

        [UserScopedSetting]
        [DefaultSettingValue("0")]
        public double PpmSeparator
        {
            get => (double)this["PpmSeparator"];
            set => this["PpmSeparator"] = value;
        }

        public virtual double PpmSeparatorDefault => 95d;

        [UserScopedSetting]
        [DefaultSettingValue("0")]
        public double PpmGlitch
        {
            get => (double)this["PpmGlitch"];
            set => this["PpmGlitch"] = value;
        }

        public virtual double PpmGlitchDefault => 21d;

        [UserScopedSetting]
        [DefaultSettingValue("0")]
        public double PpmJitter
        {
            get => (double)this["PpmJitter"];
            set => this["PpmJitter"] = value;
        }

        public virtual double PpmJitterDefault => 5d;
        
        public void RestoreDefaults()
        {
            PpmMinPulseWidth = PpmMinPulseWidthDefault;
            PpmMaxPulseWidth = PpmMaxPulseWidthDefault;
            PpmTrig = PpmTrigDefault;
            PpmSeparator = PpmSeparatorDefault;
            PpmGlitch = PpmGlitchDefault;
            PpmJitter = PpmJitterDefault;
        }


        public override void Save()
        {
            base.Save();
        }
    }
}