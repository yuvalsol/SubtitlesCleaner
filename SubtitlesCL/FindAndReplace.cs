using System;
using System.Text.RegularExpressions;

namespace SubtitlesCL
{
    public class FindAndReplace
    {
        public Regex Regex { get; private set; }
        public string GroupName { get; private set; }
        public string Replacement { get; private set; }
        public MatchEvaluator Evaluator { get; private set; }
        public SubtitleError SubtitleError { get; private set; }
        public IgnoreRule[] IgnoreRules { get; private set; }

        #region Constructors

        public FindAndReplace(Regex regex, string replacement, SubtitleError subtitleError, params IgnoreRule[] ignoreRules)
            : this(regex, null, replacement, null, subtitleError, ignoreRules)
        { }

        public FindAndReplace(Regex regex, MatchEvaluator evaluator, SubtitleError subtitleError, params IgnoreRule[] ignoreRules)
            : this(regex, null, null, evaluator, subtitleError, ignoreRules)
        { }

        public FindAndReplace(Regex regex, string groupName, string replacement, SubtitleError subtitleError, params IgnoreRule[] ignoreRules)
            : this(regex, groupName, replacement, null, subtitleError, ignoreRules)
        { }

        public FindAndReplace(Regex regex, string groupName, string replacement, MatchEvaluator evaluator, SubtitleError subtitleError, params IgnoreRule[] ignoreRules)
        {
            Regex = regex;
            GroupName = groupName;
            Replacement = replacement ?? string.Empty;
            Evaluator = evaluator;
            SubtitleError = subtitleError;
            IgnoreRules = ignoreRules;
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

        public class IgnoreRule
        {
            public int ReadPrevCharsFromMatch { get; set; }
            public int ReadNextCharsFromMatch { get; set; }
            public string IgnoreIfEqualsTo { get; set; }
            public string IgnoreIfStartsWith { get; set; }
            public string IgnoreIfEndsWith { get; set; }
            public string IgnoreIfCaseInsensitiveEqualsTo { get; set; }
            public string IgnoreIfCaseInsensitiveStartsWith { get; set; }
            public string IgnoreIfCaseInsensitiveEndsWith { get; set; }
        }

        public string CleanLine(string line, bool cleanHICaseInsensitive = false)
        {
            if (string.IsNullOrEmpty(line))
                return line;

            Regex regex = (cleanHICaseInsensitive && HasRegexCI ? RegexCI : Regex);

            if (string.IsNullOrEmpty(GroupName))
            {
                if (Evaluator != null)
                    return regex.Replace(line, Evaluator);
                else
                    return regex.Replace(line, Replacement);
            }
            else
            {
                if (IgnoreRules == null || IgnoreRules.Length == 0)
                {
                    return regex.ReplaceGroup(line, GroupName, Replacement);
                }
                else
                {
                    bool IsMatch(Match match)
                    {
                        foreach (var rule in IgnoreRules)
                        {
                            int index = match.Index - rule.ReadPrevCharsFromMatch;
                            int length = match.Length + rule.ReadPrevCharsFromMatch + rule.ReadNextCharsFromMatch;
                            if (index < 0)
                                index = 0;
                            if (index + length > line.Length)
                                length = line.Length - index;

                            string value = line.Substring(index, length);

                            if (string.IsNullOrEmpty(rule.IgnoreIfEqualsTo) == false && value == rule.IgnoreIfEqualsTo)
                                return false;
                            if (string.IsNullOrEmpty(rule.IgnoreIfStartsWith) == false && value.StartsWith(rule.IgnoreIfStartsWith))
                                return false;
                            if (string.IsNullOrEmpty(rule.IgnoreIfEndsWith) == false && value.EndsWith(rule.IgnoreIfEndsWith))
                                return false;
                            if (string.IsNullOrEmpty(rule.IgnoreIfCaseInsensitiveEqualsTo) == false && value.ToLowerInvariant() == rule.IgnoreIfCaseInsensitiveEqualsTo.ToLowerInvariant())
                                return false;
                            if (string.IsNullOrEmpty(rule.IgnoreIfCaseInsensitiveStartsWith) == false && value.ToLowerInvariant().StartsWith(rule.IgnoreIfCaseInsensitiveStartsWith.ToLowerInvariant()))
                                return false;
                            if (string.IsNullOrEmpty(rule.IgnoreIfCaseInsensitiveEndsWith) == false && value.ToLowerInvariant().EndsWith(rule.IgnoreIfCaseInsensitiveEndsWith.ToLowerInvariant()))
                                return false;
                        }

                        return true;
                    }

                    return regex.ReplaceGroup(line, GroupName, Replacement, IsMatch);
                }
            }
        }
    }
}
