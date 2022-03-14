using System;
using System.Text.RegularExpressions;

namespace SubtitlesCL
{
    class FindAndReplace
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
            Regex = new Regex(Pattern, IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
        }

        public FindAndReplace(string pattern, string groupName, string replacement, bool ignoreCase = false) : this(pattern, replacement, ignoreCase)
        {
            GroupName = groupName;
        }

        public override string ToString()
        {
            return
                Pattern +
                " -> " +
                (Replacement.StartsWith(" ") || Replacement.EndsWith(" ") ? "\"" + Replacement + "\"" : Replacement);
        }

        public string CleanSubtitleLine(string line)
        {
            if (string.IsNullOrEmpty(line))
                return null;
            else if (string.IsNullOrEmpty(GroupName))
                return Regex.Replace(line, Replacement);
            else
                return Regex.ReplaceGroup(line, GroupName, Replacement);
        }
    }
}
