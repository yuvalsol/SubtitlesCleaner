using System;
using System.Text.RegularExpressions;

namespace SubtitlesCL
{
    public class FindAndReplace
    {
        public Regex Regex { get; private set; }
        public string GroupName { get; private set; }
        public string Replacement { get; private set; }
        public SubtitleError SubtitleError { get; private set; }

        #region Constructors

        public FindAndReplace(Regex regex, string replacement, SubtitleError subtitleError)
            : this(regex, null, replacement, subtitleError)
        {
        }

        public FindAndReplace(Regex regex, string groupName, string replacement, SubtitleError subtitleError)
        {
            Regex = regex;
            GroupName = groupName;
            Replacement = replacement;
            SubtitleError = subtitleError;
        }

        #endregion

        #region Case Insensitive

        public Regex RegexCI { get; private set; }
        private bool HasRegexCI;

        public FindAndReplace SetRegexCI(Regex regexCI)
        {
            RegexCI = regexCI;
            HasRegexCI = true;
            return this;
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            return ToString(false);
        }

        public string ToStringCI()
        {
            return ToString(true);
        }

        private string ToString(bool cleanHICaseInsensitive)
        {
            return
                (cleanHICaseInsensitive && HasRegexCI ? RegexCI : Regex).ToString() + " -> " +
                (string.IsNullOrEmpty(GroupName) ? string.Empty : GroupName + " -> ") +
                (Replacement.StartsWith(" ") || Replacement.EndsWith(" ") ? "\"" + Replacement + "\"" : Replacement);
        }

        #endregion

        public string CleanLine(string line, bool cleanHICaseInsensitive = false)
        {
            if (string.IsNullOrEmpty(line))
                return line;
            else if (string.IsNullOrEmpty(GroupName))
                return (cleanHICaseInsensitive && HasRegexCI ? RegexCI : Regex).Replace(line, Replacement);
            else
                return (cleanHICaseInsensitive && HasRegexCI ? RegexCI : Regex).ReplaceGroup(line, GroupName, Replacement);
        }
    }
}
