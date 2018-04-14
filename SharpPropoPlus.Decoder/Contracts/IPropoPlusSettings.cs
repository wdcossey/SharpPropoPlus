using System.Configuration;
using System.Diagnostics;

namespace SharpPropoPlus.Decoder.Contracts
{
    public interface IPropoPlusSettings
    {
        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("true")]
        bool UpgradeRequired { get; set; }

        void Save();

        void Upgrade();

        void RestoreDefaults();

        bool IsDefaultSettings { get; }
    }
}