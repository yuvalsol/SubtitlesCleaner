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
        public SubtitleError SubtitleError { get; private set; }
        public Regex Regex { get; private set; }

        #region Constructors

        public FindAndReplace(string pattern, string replacement, SubtitleError subtitleError)
            : this(pattern, null, replacement, false, subtitleError)
        {
        }

        public FindAndReplace(string pattern, string replacement, bool ignoreCase, SubtitleError subtitleError)
            : this(pattern, null, replacement, ignoreCase, subtitleError)
        {
        }

        public FindAndReplace(string pattern, string groupName, string replacement, SubtitleError subtitleError)
            : this(pattern, groupName, replacement, false, subtitleError)
        {
        }

        public FindAndReplace(string pattern, string groupName, string replacement, bool ignoreCase, SubtitleError subtitleError)
        {
            Pattern = pattern;
            GroupName = groupName;
            Replacement = replacement;
            IgnoreCase = ignoreCase;
            SubtitleError = subtitleError;

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

        public FindAndReplace(Regex regex, string replacement, SubtitleError subtitleError)
            : this(regex, null, replacement, subtitleError)
        {
        }

        public FindAndReplace(Regex regex, string groupName, string replacement, SubtitleError subtitleError)
        {
            Regex = regex;
            Pattern = Regex.ToString();
            GroupName = groupName;
            Replacement = replacement;
            IgnoreCase = Regex.Options.HasFlag(RegexOptions.IgnoreCase);
            SubtitleError = subtitleError;
        }

        #endregion

        #region Case Insensitive

        public bool HasRegexCI { get; private set; }
        public string PatternCI { get; private set; }
        public Regex RegexCI { get; private set; }

        public FindAndReplace SetRegexCI(string patternCI)
        {
            try
            {
                RegexCI = new Regex(PatternCI);
                PatternCI = RegexCI.ToString();
                HasRegexCI = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(this.ToStringCI());
                Console.WriteLine(ex.Message);
                throw;
            }

            return this;
        }

        public FindAndReplace SetRegexCI(Regex regexCI)
        {
            RegexCI = regexCI;
            PatternCI = RegexCI.ToString();
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

        private string ToString(bool isCaseInsensitive)
        {
            return
                (isCaseInsensitive && HasRegexCI ? PatternCI : Pattern) + " -> " +
                (string.IsNullOrEmpty(GroupName) ? string.Empty : GroupName + " -> ") +
                (Replacement.StartsWith(" ") || Replacement.EndsWith(" ") ? "\"" + Replacement + "\"" : Replacement);
        }

        #endregion

        public string CleanLine(string line, bool isCaseInsensitive = false)
        {
            if (string.IsNullOrEmpty(line))
                return line;
            else if (string.IsNullOrEmpty(GroupName))
                return (isCaseInsensitive && HasRegexCI ? RegexCI : Regex).Replace(line, Replacement);
            else
                return (isCaseInsensitive && HasRegexCI ? RegexCI : Regex).ReplaceGroup(line, GroupName, Replacement);
        }
    }
}
