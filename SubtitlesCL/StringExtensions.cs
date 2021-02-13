using System.Linq;
using System.Text.RegularExpressions;

namespace System
{
    public static partial class StringExtensions
    {
        public static string Replace(this string input, Regex regex, string replacement)
        {
            return regex.Replace(input, replacement);
        }

        public static string Replace(this string input, Regex regex, string groupName, string replacement)
        {
            if (regex.IsMatch(input))
            {
                foreach (Match match in regex.Matches(input).Cast<Match>().Reverse())
                {
                    if (match.Success)
                    {
                        Group group = match.Groups[groupName];
                        input =
                            input.Substring(0, group.Index) +
                            replacement +
                            input.Substring(group.Index + group.Length);
                    }
                }
            }

            return input;
        }

        public static bool ContainsCI(this string text, string value, StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase)
        {
            return text.IndexOf(value, stringComparison) != -1;
        }
    }
}
