using System;

namespace SharpPropoPlus.ExtensionMethods
{
    public static class EnumExtensionMethods
    {
        public static string GetDescription(this Enum value)
        {
            return ((object)value).GetDescription();
        }
    }
}