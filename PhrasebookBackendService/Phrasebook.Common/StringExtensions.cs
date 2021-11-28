using System.Text.RegularExpressions;

namespace Phrasebook.Common
{
    public static class StringExtensions
    {
        public static bool IsAlphanumeric(this string s)
        {
            return Regex.IsMatch(s, "^[a-zA-Z0-9]+$");
        }

        public static bool IsValidDisplayName(this string displayName)
        {
            return displayName != null && displayName.IsAlphanumeric() && displayName.Length >= 3;
        }
    }
}
