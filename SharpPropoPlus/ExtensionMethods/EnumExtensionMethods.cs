using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

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