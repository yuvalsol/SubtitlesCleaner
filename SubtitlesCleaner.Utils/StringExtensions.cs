using System.Collections.Generic;
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
                        if (group.Success)
                        {
                            foreach (Capture capture in group.Captures.Cast<Capture>().Reverse())
                            {
                                input =
                                    input.Substring(0, capture.Index) +
                                    replacement +
                                    input.Substring(capture.Index + capture.Length);
                            }
                        }
                    }
                }
            }

            return input;
        }

        public static bool ContainsCI(this string text, string value, StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase)
        {
            return text.IndexOf(value, stringComparison) != -1;
        }

        public static string EscapeString(this string text)
        {
            return text.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }

        public static string EscapeVerbatim(this string text)
        {
            return text.Replace("\"", "\"\"");
        }

        private static readonly Regex regexCamelCaseIndexes = new Regex("[a-z][A-Z]|[A-Za-z][0-9]", RegexOptions.Compiled);

        public static IEnumerable<string> CamelCaseWords(this string name)
        {
            foreach (string word in name.Split(new char[] { ' ', '_', '-' }, StringSplitOptions.RemoveEmptyEntries))
            {
                int start = 0;
                foreach (int index in regexCamelCaseIndexes.Matches(word).Cast<Match>().Select(m => m.Index))
                {
                    yield return word.Substring(start, index + 1 - start).ToCamelCase();
                    start = index + 1;
                }

                yield return word.Substring(start).ToCamelCase();
            }
        }

        public static string ToCamelCase(this string name)
        {
            if (name.Length > 0 && '0' <= name[0] && name[0] <= '9')
                return name.ToLower();
            else
                return name.Substring(0, 1).ToUpper() + name.Substring(1).ToLower();
        }
    }
}
