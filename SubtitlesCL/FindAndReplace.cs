using System;
using System.Text.RegularExpressions;

namespace SubtitlesCL
{
    public class FindAndReplace
    {
        public string Pattern { get; private set; }
        public string GroupName { get; private set; }
        public string Replacement { get; private set; }
        public bool IgnoreCase { get; private set; }
        public Regex Regex { get; private set; }

        public FindAndReplace(string pattern, string replacement, bool ignoreCase = false)
        {
            Pattern = pattern;
            Replacement = replacement;
            IgnoreCase = ignoreCase;

            try
            {
                Regex = new Regex(Pattern, IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine(this.ToString());
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public FindAndReplace(string pattern, string groupName, string replacement, bool ignoreCase = false) : this(pattern, replacement, ignoreCase)
        {
            GroupName = groupName;
        }

        public override string ToString()
        {
            return
                Pattern + " -> " +
                (string.IsNullOrEmpty(GroupName) ? string.Empty : GroupName + " -> ") +
                (Replacement.StartsWith(" ") || Replacement.EndsWith(" ") ? "\"" + Replacement + "\"" : Replacement);
        }

        public string Replace(string line)
        {
            if (string.IsNullOrEmpty(line))
                return line;
            else if (string.IsNullOrEmpty(GroupName))
                return Regex.Replace(line, Replacement);
            else
                return Regex.ReplaceGroup(line, GroupName, Replacement);
        }
    }
}
