using System.Linq;
using System.Text.RegularExpressions;

namespace Phrasebook.Common
{
    public static class StringExtensions
    {
        private static readonly Regex NonWordCharactersOnlyRegex = new Regex(@"\W+", RegexOptions.Compiled);

        public static bool IsAlphanumeric(this string s)
        {
            return Regex.IsMatch(s, @"^[a-zA-Z0-9_]+$");
        }

        public static bool IsValidDisplayName(this string displayName)
        {
            return displayName != null && displayName.IsAlphanumeric() && displayName.Length >= 3;
        }

        public static string Sanitize(this string input)
        {
            // TODO: remove all symbols except punctuation and parenthesis/brackets
            return string.Join(" ", input.Split(" ").Where(s => s != string.Empty && !NonWordCharactersOnlyRegex.IsMatch(s)));
        }
    }
}
