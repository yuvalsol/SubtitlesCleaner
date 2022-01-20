using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SubtitlesCL
{
    class OCRRule
    {
        public Regex Find { get; set; }
        public string ReplaceBy { get; set; }

        public List<IgnoreRule> IgnoreRules { get; set; }

        public override string ToString()
        {
            return
                Find.ToString() +
                " -> " +
                (ReplaceBy.StartsWith(" ") || ReplaceBy.EndsWith(" ") ? "\"" + ReplaceBy + "\"" : ReplaceBy);
        }
    }

    class IgnoreRule
    {
        public Regex IgnoreFind { get; set; }
        public string[] Ignore { get; set; }
        public string[] StartsWithIgnore { get; set; }
        public string[] EndsWithIgnore { get; set; }
        public bool StringComparisonIgnoreCase { get; set; }

        public override string ToString()
        {
            return IgnoreFind.ToString();
        }
    }
}
