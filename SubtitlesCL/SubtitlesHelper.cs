using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SubtitlesCL
{
    public static class SubtitlesHelper
    {
        #region Time Parsing

        private const string showTimeFormat = @"(?<Show_HH>\d{2}):(?<Show_MM>\d{2}):(?<Show_SS>\d{2}),(?<Show_MS>\d{3})";
        private const string hideTimeFormat = @"(?<Hide_HH>\d{2}):(?<Hide_MM>\d{2}):(?<Hide_SS>\d{2}),(?<Hide_MS>\d{3})";
        private const string fullTimeFormat = showTimeFormat + " --> " + hideTimeFormat;

        public static readonly Regex regexTime = new Regex(@"^" + fullTimeFormat + "$", RegexOptions.Compiled);
        public static readonly Regex regexSubtitleNumber = new Regex(@"^\d+$", RegexOptions.Compiled);

        public static readonly Regex regexShowTime = new Regex(@"^" + showTimeFormat + "$", RegexOptions.Compiled);
        public static readonly Regex regexShiftTime = new Regex(@"^(?<Shift_Sign>-|\+)?(?:(?:(?:(?<Shift_HH>\d{1,2}):)?(?<Shift_MM>\d{1,2}):)?(?<Shift_SS>\d{1,2})(?:,|:|\.))?(?<Shift_MS>\d{1,3})$", RegexOptions.Compiled);

        private const string showTimeFormatAlternate = @"(?:(?<Show_HH>\d{2}):)?(?<Show_MM>\d{2}):(?<Show_SS>\d{2})(?:[.,](?<Show_MS>\d{3}))?";
        public static readonly Regex regexShowTimeAlternate = new Regex(@"^" + showTimeFormatAlternate + "$", RegexOptions.Compiled);

        public static DateTime ParseShowTime(string showTime)
        {
            if (string.IsNullOrEmpty(showTime))
                return DateTime.MinValue;

            Match match = regexShowTime.Match(showTime);
            if (match.Success)
            {
                var show = new DateTime(
                    1900, 1, 1,
                    int.Parse(match.Groups["Show_HH"].Value),
                    int.Parse(match.Groups["Show_MM"].Value),
                    int.Parse(match.Groups["Show_SS"].Value),
                    int.Parse(match.Groups["Show_MS"].Value)
                );

                return show;
            }
            else
            {
                match = regexShowTimeAlternate.Match(showTime);
                if (match.Success)
                {
                    var show = new DateTime(
                        1900, 1, 1,
                        match.Groups["Show_HH"].Success ? int.Parse(match.Groups["Show_HH"].Value) : 0,
                        int.Parse(match.Groups["Show_MM"].Value),
                        int.Parse(match.Groups["Show_SS"].Value),
                        match.Groups["Show_MS"].Success ? int.Parse(match.Groups["Show_MS"].Value) : 0
                    );

                    return show;
                }
                else
                {
                    return DateTime.MinValue;
                }
            }
        }

        public static TimeSpan ParseShiftTime(string shiftTime)
        {
            if (string.IsNullOrEmpty(shiftTime))
                return TimeSpan.Zero;

            if (regexShiftTime.IsMatch(shiftTime))
            {
                Match match = regexShiftTime.Match(shiftTime);
                var span = new TimeSpan(
                    0,
                    match.Groups["Shift_HH"].Success ? int.Parse(match.Groups["Shift_HH"].Value) : 0,
                    match.Groups["Shift_MM"].Success ? int.Parse(match.Groups["Shift_MM"].Value) : 0,
                    match.Groups["Shift_SS"].Success ? int.Parse(match.Groups["Shift_SS"].Value) : 0,
                    match.Groups["Shift_MS"].Success ? int.Parse(match.Groups["Shift_MS"].Value) : 0
                );

                if (match.Groups["Shift_Sign"].Success && match.Groups["Shift_Sign"].Value == "-")
                    return span.Negate();
                else
                    return span;
            }
            else
            {
                return TimeSpan.Zero;
            }
        }

        #endregion

        #region Get

        /*public static readonly Encoding Windows1252 = Encoding.GetEncoding("Windows-1252");
        public static readonly Regex regexAccentedCharacters = new Regex(@"[á-úÁ-Ú]", RegexOptions.Compiled);

        public static bool HasAccentedCharacters(string filePath)
        {
            return regexAccentedCharacters.IsMatch(File.ReadAllText(filePath, Windows1252));
        }*/

        public static List<Subtitle> GetSubtitles(string filePath, int? firstSubtitlesCount = null)
        {
            List<string> lines = new List<string>(File.ReadAllLines(filePath, Encoding.UTF8));

            for (int i = 0; i < lines.Count; i++)
                lines[i] = (lines[i] ?? string.Empty).Trim();

            for (int i = lines.Count - 1; i >= 0; i--)
            {
                string line = lines[i];

                if (string.IsNullOrEmpty(line))
                {
                    lines.RemoveAt(i);
                }
                else if (regexTime.IsMatch(line))
                {
                    if (i - 1 >= 0)
                    {
                        string prevLine = lines[i - 1];
                        if (regexSubtitleNumber.IsMatch(prevLine))
                        {
                            lines.RemoveAt(i - 1);
                        }
                    }
                }
            }

            List<Subtitle> subtitles = new List<Subtitle>();
            Subtitle subtitle = null;
            foreach (var line in lines)
            {
                if (regexTime.IsMatch(line))
                {
                    if (subtitle != null)
                        subtitles.Add(subtitle);

                    subtitle = new Subtitle();

                    Match match = regexTime.Match(line);

                    subtitle.Show = new DateTime(
                        1900, 1, 1,
                        int.Parse(match.Groups["Show_HH"].Value),
                        int.Parse(match.Groups["Show_MM"].Value),
                        int.Parse(match.Groups["Show_SS"].Value),
                        int.Parse(match.Groups["Show_MS"].Value)
                    );

                    subtitle.Hide = new DateTime(
                        1900, 1, 1,
                        int.Parse(match.Groups["Hide_HH"].Value),
                        int.Parse(match.Groups["Hide_MM"].Value),
                        int.Parse(match.Groups["Hide_SS"].Value),
                        int.Parse(match.Groups["Hide_MS"].Value)
                    );
                }
                else if (subtitle != null)
                {
                    subtitle.Lines.Add(line);
                }
            }

            if (subtitle != null)
                subtitles.Add(subtitle);

            if (firstSubtitlesCount != null && firstSubtitlesCount > 0)
                return subtitles.Take(firstSubtitlesCount.Value).ToList();

            return subtitles;
        }

        #endregion

        #region ToLines

        public static string[] ToLines(this List<Subtitle> subtitles)
        {
            if (subtitles == null || subtitles.Count == 0)
                return new string[0];

            return subtitles.SelectMany((subtitle, index) => subtitle.ToLines(index)).ToArray();
        }

        public static string[] ToLines(this Subtitle subtitle, int index)
        {
            string[] lines = new string[subtitle.Lines.Count + 3];
            lines[0] = (index + 1).ToString();
            lines[1] = subtitle.TimeToString();
            subtitle.Lines.CopyTo(lines, 2);
            lines[lines.Length - 1] = string.Empty;
            return lines;
        }

        public static List<Subtitle> Clone(this List<Subtitle> subtitles)
        {
            return subtitles.Select(subtitle => subtitle.Clone()).Cast<Subtitle>().ToList();
        }

        #endregion

        #region Clean & Check

        public static List<Subtitle> CleanSubtitles(this List<Subtitle> subtitles, bool cleanHICaseInsensitive, bool isPrint)
        {
            subtitles = IterateSubtitlesPre(subtitles, cleanHICaseInsensitive, isPrint);

            bool subtitlesChanged = false;
            do
            {
                subtitlesChanged = false;
                subtitles = IterateSubtitles(subtitles, cleanHICaseInsensitive, isPrint, ref subtitlesChanged);
            } while (subtitlesChanged);

            subtitles = IterateSubtitlesPost(subtitles, isPrint);
            return subtitles;
        }

        private static List<Subtitle> IterateSubtitlesPre(List<Subtitle> subtitles, bool cleanHICaseInsensitive, bool isPrint)
        {
            if (subtitles == null)
                return new List<Subtitle>();

            if (subtitles.Count == 0)
                return subtitles;

            for (int k = subtitles.Count - 1; k >= 0; k--)
            {
                Subtitle subtitle = subtitles[k];

                for (int i = subtitle.Lines.Count - 1; i >= 0; i--)
                {
                    string line = subtitle.Lines[i];

                    if (IsEmptyLine(line, isPrint))
                    {
                        subtitle.Lines.RemoveAt(i);
                    }
                    else if (IsNotSubtitle(line, isPrint))
                    {
                        subtitle.Lines = null;
                        break;
                    }
                }

                subtitle.Lines = CleanSubtitleLinesPre(subtitle.Lines, cleanHICaseInsensitive);

                if (subtitle.Lines == null || subtitle.Lines.Count == 0)
                    subtitles.RemoveAt(k);
            }

            return subtitles;
        }

        private static List<Subtitle> IterateSubtitles(List<Subtitle> subtitles, bool cleanHICaseInsensitive, bool isPrint, ref bool subtitlesChanged)
        {
            if (subtitles == null)
                return new List<Subtitle>();

            if (subtitles.Count == 0)
                return subtitles;

            for (int k = subtitles.Count - 1; k >= 0; k--)
            {
                Subtitle subtitle = subtitles[k];

                for (int i = subtitle.Lines.Count - 1; i >= 0; i--)
                {
                    string line = subtitle.Lines[i];

                    if (IsEmptyLine(line, isPrint))
                    {
                        subtitle.Lines.RemoveAt(i);
                        subtitlesChanged = true;
                    }
                    else if (IsNotSubtitle(line, isPrint))
                    {
                        subtitle.Lines = null;
                        subtitlesChanged = true;
                        break;
                    }
                    else
                    {
                        string cleanLine = (CleanSubtitleLine(line, cleanHICaseInsensitive, isPrint) ?? string.Empty).Trim();

                        if (IsEmptyLine(cleanLine, isPrint))
                        {
                            subtitle.Lines.RemoveAt(i);
                            subtitlesChanged = true;
                        }
                        else
                        {
                            subtitlesChanged = subtitlesChanged || (subtitle.Lines[i] != cleanLine);
                            subtitle.Lines[i] = cleanLine;
                        }
                    }
                }

                List<string> cleanLines = subtitle.Lines.GetRange(0, subtitle.Lines.Count);
                cleanLines = CleanSubtitleLines(cleanLines, cleanHICaseInsensitive);

                if (cleanLines == null || cleanLines.Count == 0)
                {
                    subtitles.RemoveAt(k);
                    subtitlesChanged = true;
                }
                else
                {
                    subtitlesChanged =
                        subtitlesChanged ||
                        subtitle.Lines.Count != cleanLines.Count ||
                        subtitle.Lines.Zip(cleanLines, (l1, l2) => l1 != l2).Any(isLineChanged => isLineChanged);

                    subtitle.Lines = cleanLines;
                }
            }

            return subtitles;
        }

        private static List<Subtitle> IterateSubtitlesPost(List<Subtitle> subtitles, bool isPrint)
        {
            if (subtitles == null)
                return new List<Subtitle>();

            if (subtitles.Count == 0)
                return subtitles;

            for (int k = subtitles.Count - 1; k >= 0; k--)
            {
                Subtitle subtitle = subtitles[k];

                for (int i = subtitle.Lines.Count - 1; i >= 0; i--)
                {
                    string cleanLine = (CleanSubtitleLinePost(subtitle.Lines[i], isPrint) ?? string.Empty).Trim();

                    if (IsEmptyLine(cleanLine, isPrint))
                        subtitle.Lines.RemoveAt(i);
                    else
                        subtitle.Lines[i] = cleanLine;
                }

                subtitle.Lines = CleanSubtitleLinesPost(subtitle.Lines);
            }

            return subtitles;
        }

        public static List<Subtitle> SetSubtitlesOrder(this List<Subtitle> subtitles)
        {
            if (subtitles == null)
                return new List<Subtitle>();

            if (subtitles.Count == 0)
                return subtitles;

            for (int k = subtitles.Count - 1; k >= 0; k--)
            {
                Subtitle subtitle = subtitles[k];

                for (int i = subtitle.Lines.Count - 1; i >= 0; i--)
                {
                    string line = subtitle.Lines[i];

                    if (IsEmptyLine(line, false))
                    {
                        subtitle.Lines.RemoveAt(i);
                    }
                    else if (IsNotSubtitle(line, false))
                    {
                        subtitle.Lines = null;
                        break;
                    }
                }

                if (subtitle.Lines == null || subtitle.Lines.Count == 0)
                    subtitles.RemoveAt(k);
            }

            subtitles.Sort();

            return subtitles;
        }

        public static void CheckSubtitles(this List<Subtitle> subtitles, bool cleanHICaseInsensitive, bool isPrint)
        {
            if (subtitles == null)
                return;

            if (subtitles.Count == 0)
                return;

            foreach (Subtitle subtitle in subtitles)
                CheckSubtitle(subtitle, cleanHICaseInsensitive, isPrint);
        }

        public static void CheckSubtitle(this Subtitle subtitle, bool cleanHICaseInsensitive, bool isPrint)
        {
            subtitle.SubtitleError = SubtitleError.None;

            foreach (string line in subtitle.Lines)
            {
                subtitle.SubtitleError |= CheckSubtitleLine(line, cleanHICaseInsensitive, isPrint);
                subtitle.SubtitleError |= CheckSubtitleLinePost(line, isPrint);
            }

            subtitle.SubtitleError |= CheckSubtitleLinesPre(subtitle.Lines, cleanHICaseInsensitive);
            subtitle.SubtitleError |= CheckSubtitleLines(subtitle.Lines, cleanHICaseInsensitive);
            subtitle.SubtitleError |= CheckSubtitleLinesPost(subtitle.Lines);

            if ((subtitle.SubtitleError & SubtitleError.Not_Subtitle) == SubtitleError.Not_Subtitle)
                subtitle.SubtitleError = SubtitleError.Not_Subtitle;
        }

        #region Clean Multiple Lines Pre

        private static List<string> CleanSubtitleLinesPre(List<string> lines, bool cleanHICaseInsensitive)
        {
            if (lines == null || lines.Count == 0)
                return null;

            if (lines.Count > 1)
            {
                var resultsHIPrefix = lines.Select((line, index) => new
                {
                    line,
                    index,
                    isMatchHIPrefix = (cleanHICaseInsensitive ? regexHIPrefixWithoutDialogDashCI : regexHIPrefixWithoutDialogDash).IsMatch(line)
                }).ToArray();

                if (resultsHIPrefix.Count(x => x.isMatchHIPrefix) > 1)
                {
                    foreach (var item in resultsHIPrefix)
                    {
                        if (item.isMatchHIPrefix)
                        {
                            Match match = (cleanHICaseInsensitive ? regexHIPrefixWithoutDialogDashCI : regexHIPrefixWithoutDialogDash).Match(lines[item.index]);
                            lines[item.index] = (match.Groups["Prefix"].Value + " - " + match.Groups["Subtitle"].Value).Trim();
                        }
                    }
                }
            }

            return lines;
        }

        private static SubtitleError CheckSubtitleLinesPre(List<string> lines, bool cleanHICaseInsensitive)
        {
            if (lines == null || lines.Count == 0)
                return SubtitleError.None;

            SubtitleError subtitleError = SubtitleError.None;

            if (lines.Count > 1)
            {
                var resultsHIPrefix = lines.Select((line, index) => new
                {
                    line,
                    index,
                    isMatchHIPrefix = (cleanHICaseInsensitive ? regexHIPrefixWithoutDialogDashCI : regexHIPrefixWithoutDialogDash).IsMatch(line)
                }).ToArray();

                if (resultsHIPrefix.Count(x => x.isMatchHIPrefix) > 1)
                {
                    subtitleError |= SubtitleError.Hearing_Impaired;
                }
            }

            return subtitleError;
        }

        #endregion

        #region Clean Single Line

        private static string CleanSubtitleLine(string line, bool cleanHICaseInsensitive, bool isPrint)
        {
            return CleanLine(line, FindAndReplaceRules, cleanHICaseInsensitive, isPrint);
        }

        private static SubtitleError CheckSubtitleLine(string line, bool cleanHICaseInsensitive, bool isPrint)
        {
            SubtitleError subtitleError = SubtitleError.None;
            CleanLine(line, FindAndReplaceRules, cleanHICaseInsensitive, isPrint, ref subtitleError);
            return subtitleError;
        }

        #endregion

        #region Clean Multiple Lines

        public static readonly Regex regexCapitalLetter = new Regex(@"[A-ZÁ-Ú]", RegexOptions.Compiled);
        public static readonly Regex regexDialog = new Regex(@"^(?<Italic><i>)?-\s*(?<Subtitle>.*?)$", RegexOptions.Compiled);
        public static readonly Regex regexContainsDialog = new Regex(@" - [A-ZÁ-Ú]", RegexOptions.Compiled);
        public static readonly Regex regexEndsWithLowerCaseLetter = new Regex(@"[a-zá-ú]$", RegexOptions.Compiled);

        private static List<string> CleanSubtitleLines(List<string> lines, bool cleanHICaseInsensitive)
        {
            if (lines == null || lines.Count == 0)
                return null;

            if (lines.Count > 1)
            {
                string firstLine = lines[0];
                string lastLine = lines[lines.Count - 1];

                if (IsHearingImpairedMultipleLines_RoundBrackets(firstLine, lastLine))
                {
                    if (lines.Skip(1).Take(lines.Count - 2).Any(line => line.Contains("(") || line.Contains(")")) == false)
                    {
                        return null;
                    }
                }

                if (IsHearingImpairedMultipleLines_SquareBrackets(firstLine, lastLine))
                {
                    if (lines.Skip(1).Take(lines.Count - 2).Any(line => line.Contains("[") || line.Contains("]")) == false)
                    {
                        return null;
                    }
                }

                for (int i = 1; i < lines.Count; i++)
                {
                    string line1 = lines[i - 1];
                    string line2 = lines[i];

                    if (IsHearingImpairedMultipleLines(line1, line2))
                    {
                        lines.RemoveAt(i);
                        lines.RemoveAt(i - 1);
                        i--;
                    }
                }
            }

            if (lines.Count == 2)
            {
                string line1 = lines[0];
                string line2 = lines[1];
                if (line1 == "<i>-" && line2.StartsWith("- ") && line2.EndsWith("</i>"))
                {
                    lines[1] = "<i>" + line2.Substring(2);
                    lines.RemoveAt(0);
                }
                else if (line1 == "<i>" && line2.StartsWith("<i>") == false && line2.EndsWith("</i>"))
                {
                    lines[1] = "<i>" + line2;
                    lines.RemoveAt(0);
                }
                else if (line1 == "- <i>" && line2.StartsWith("- ") && line2.EndsWith("</i>"))
                {
                    lines[1] = "<i>" + line2.Substring(2);
                    lines.RemoveAt(0);
                }
                else if (line1.StartsWith("<i>") && line1.IndexOf("</i>") == -1 && line2.StartsWith("</i>"))
                {
                    lines[0] = lines[0] + "</i>";
                    lines[1] = lines[1].Substring("</i>".Length);
                }
            }

            if (lines.Count == 1)
            {
                if (regexDialog.IsMatch(lines[0]))
                {
                    Match match = regexDialog.Match(lines[0]);
                    lines[0] = match.Groups["Italic"].Value + match.Groups["Subtitle"].Value;
                }

                if ((cleanHICaseInsensitive ? regexHIPrefixCI : regexHIPrefix).IsMatch(lines[0]))
                {
                    Match match = (cleanHICaseInsensitive ? regexHIPrefixCI : regexHIPrefix).Match(lines[0]);
                    lines[0] = match.Groups["Prefix"].Value + match.Groups["Subtitle"].Value;
                }

                if (lines[0] == "-")
                    return null;

                if (lines[0] == "<i>")
                    return null;

                if (lines[0] == "</i>")
                    return null;
            }
            else if (lines.Count > 1)
            {
                var resultsHIPrefix = lines.Select((line, index) => new
                {
                    line,
                    index,
                    isMatchHIPrefix = (cleanHICaseInsensitive ? regexHIPrefixCI : regexHIPrefix).IsMatch(line)
                }).ToArray();

                if (resultsHIPrefix[0].isMatchHIPrefix && resultsHIPrefix.Skip(1).All(x => x.isMatchHIPrefix == false))
                {
                    Match match = (cleanHICaseInsensitive ? regexHIPrefixCI : regexHIPrefix).Match(lines[0]);
                    lines[0] = match.Groups["Subtitle"].Value;
                }
                else if (resultsHIPrefix.Count(x => x.isMatchHIPrefix) > 1)
                {
                    foreach (var item in resultsHIPrefix)
                    {
                        if (item.isMatchHIPrefix)
                        {
                            Match match = (cleanHICaseInsensitive ? regexHIPrefixCI : regexHIPrefix).Match(lines[item.index]);
                            lines[item.index] = match.Groups["Prefix"].Value + match.Groups["Subtitle"].Value;
                        }
                    }
                }

                var resultsDialog = lines.Select((line, index) => new
                {
                    line,
                    index,
                    isMatchDialog = regexDialog.IsMatch(line),
                    isContainsDialog_CapitalLetter = regexContainsDialog.IsMatch(line),
                    isStartsWithDots = line.StartsWith("..."),
                    isStartsWithDotsAndItalics = line.StartsWith("<i>..."),
                    isStartsWithI = line.StartsWith("I "),
                    isStartsWithContractionI = line.StartsWith("I'"),
                    isEndsWithDots = line.EndsWith("..."),
                    isEndsWithComma = line.EndsWith(","),
                    isEndsWithLowerCaseLetter = regexEndsWithLowerCaseLetter.IsMatch(line)
                }).ToArray();

                if (resultsDialog[0].isStartsWithDots && resultsDialog.Skip(1).All(x => x.isMatchDialog))
                {
                    // ...line 1
                    // - Line 2
                    lines[0] = "- " + lines[0];
                    // - ...line 1
                    // - Line 2
                }
                else if (resultsDialog[0].isStartsWithDotsAndItalics && resultsDialog.Skip(1).All(x => x.isMatchDialog))
                {
                    // <i>...line 1
                    // - Line 2
                    lines[0] = "<i>- " + lines[0].Substring(3);
                    // <i>- ...line 1
                    // - Line 2
                }
                else if (resultsDialog[0].isMatchDialog && resultsDialog.Skip(1).All(x => x.isStartsWithDots || x.isStartsWithDotsAndItalics))
                {
                    // - Line 1
                    // ...line 2
                    for (int i = 1; i < lines.Count; i++)
                        lines[i] = "- " + lines[i];
                    // - Line 1
                    // - ...line 2
                }
                else if (resultsDialog[0].isMatchDialog && resultsDialog.Skip(1).All(x => x.isMatchDialog == false) && resultsDialog.Skip(1).All(x => x.isContainsDialog_CapitalLetter == false))
                {
                    string firstCharSecondLine = (lines[1][0]).ToString();

                    if (regexCapitalLetter.IsMatch(firstCharSecondLine))
                    {
                        // - Line 1 - Dialog
                        // I am line 2
                        if (resultsDialog[0].isMatchDialog &&
                            resultsDialog[0].isContainsDialog_CapitalLetter &&
                            (resultsDialog[1].isStartsWithI || resultsDialog[1].isStartsWithContractionI))
                        {
                            // don't do anything
                        }
                        // - Line 1 - Dialog...
                        // Line 2
                        else if (resultsDialog[0].isMatchDialog &&
                            resultsDialog[0].isContainsDialog_CapitalLetter &&
                            resultsDialog[0].isEndsWithDots)
                        {
                            // don't do anything
                        }
                        else if (resultsDialog[0].isMatchDialog &&
                            resultsDialog[0].isEndsWithComma &&
                            (resultsDialog[1].isStartsWithI || resultsDialog[1].isStartsWithContractionI))
                        {
                            // - Line 1,
                            // I'll Line 2
                            Match match = regexDialog.Match(lines[0]);
                            lines[0] = match.Groups["Italic"].Value + match.Groups["Subtitle"].Value;
                            // Line 1,
                            // I'll Line 2
                        }
                        else if (resultsDialog[0].isMatchDialog &&
                            resultsDialog[0].isEndsWithLowerCaseLetter &&
                            (resultsDialog[1].isStartsWithI || resultsDialog[1].isStartsWithContractionI))
                        {
                            // - Line 1 end with lower case letter
                            // I'll Line 2
                            Match match = regexDialog.Match(lines[0]);
                            lines[0] = match.Groups["Italic"].Value + match.Groups["Subtitle"].Value;
                            // Line 1 end with lower case letter
                            // I'll Line 2
                        }
                        else
                        {
                            // - Line 1
                            // Line 2
                            lines[1] = "- " + lines[1];
                            // - Line 1
                            // - Line 2
                        }
                    }
                    else
                    {
                        // - Line 1 - Dialog
                        // line 2
                        if (resultsDialog[0].isMatchDialog &&
                            resultsDialog[0].isContainsDialog_CapitalLetter &&
                            resultsDialog[1].isMatchDialog == false)
                        {
                            // don't do anything
                        }
                        else
                        {
                            // - Line 1
                            // line 2
                            Match match = regexDialog.Match(lines[0]);
                            lines[0] = match.Groups["Italic"].Value + match.Groups["Subtitle"].Value;
                            // Line 1
                            // line 2
                        }
                    }
                }
                else if (resultsDialog[0].isMatchDialog == false && resultsDialog.Skip(1).All(x => x.isMatchDialog))
                {
                    string firstCharFirstLine = (lines[0][0]).ToString();

                    if (regexCapitalLetter.IsMatch(firstCharFirstLine))
                    {
                        // Line 1
                        // - Line 2
                        lines[0] = "- " + lines[0];
                        // - Line 1
                        // - Line 2
                    }
                    else
                    {
                        // Line 1
                        // - line 2
                        Match match = regexDialog.Match(lines[1]);
                        lines[1] = match.Groups["Italic"].Value + match.Groups["Subtitle"].Value;
                        // Line 1
                        // line 2
                    }
                }
                else if (resultsDialog.Count(x => x.isMatchDialog) > 1)
                {
                    foreach (var item in resultsDialog)
                    {
                        if (item.isMatchDialog)
                        {
                            Match match = regexDialog.Match(lines[item.index]);
                            lines[item.index] = match.Groups["Italic"].Value + "- " + match.Groups["Subtitle"].Value;
                        }
                    }
                }
            }

            if (lines.Count == 1)
            {
                string line = lines[0];
                if (line.StartsWith("- ") == false)
                {
                    int index = -1;
                    if ((index = line.IndexOf(". - ")) != -1)
                    {
                        lines.Clear();
                        lines.Add("- " + line.Substring(0, index + 1));
                        lines.Add(line.Substring(index + 2));
                    }
                    else if ((index = line.IndexOf("? - ")) != -1)
                    {
                        lines.Clear();
                        lines.Add("- " + line.Substring(0, index + 1));
                        lines.Add(line.Substring(index + 2));
                    }
                    else if ((index = line.IndexOf("! - ")) != -1)
                    {
                        lines.Clear();
                        lines.Add("- " + line.Substring(0, index + 1));
                        lines.Add(line.Substring(index + 2));
                    }
                }
            }

            return lines;
        }

        private static SubtitleError CheckSubtitleLines(List<string> lines, bool cleanHICaseInsensitive)
        {
            if (lines == null || lines.Count == 0)
                return SubtitleError.None;

            SubtitleError subtitleError = SubtitleError.None;

            if (lines.Count > 1)
            {
                for (int i = 1; i < lines.Count; i++)
                {
                    string line1 = lines[i - 1];
                    string line2 = lines[i];

                    if (IsHearingImpairedMultipleLines(line1, line2))
                    {
                        subtitleError |= SubtitleError.Hearing_Impaired;
                        break;
                    }
                }
            }

            if (lines.Count == 1)
            {
                if (regexDialog.IsMatch(lines[0]))
                    subtitleError |= SubtitleError.Dialog_Error;

                if ((cleanHICaseInsensitive ? regexHIPrefixCI : regexHIPrefix).IsMatch(lines[0]))
                    subtitleError |= SubtitleError.Hearing_Impaired;

                if (lines[0] == "-")
                    return SubtitleError.Empty_Line;

                if (lines[0] == "<i>")
                    return SubtitleError.Empty_Line;

                if (lines[0] == "</i>")
                    return SubtitleError.Empty_Line;
            }
            else if (lines.Count > 1)
            {
                var resultsHIPrefix = lines.Select((line, index) => new
                {
                    line,
                    index,
                    isMatchHIPrefix = (cleanHICaseInsensitive ? regexHIPrefixCI : regexHIPrefix).IsMatch(line)
                }).ToArray();

                if (resultsHIPrefix[0].isMatchHIPrefix && resultsHIPrefix.Skip(1).All(x => x.isMatchHIPrefix == false))
                {
                    Match match = (cleanHICaseInsensitive ? regexHIPrefixCI : regexHIPrefix).Match(lines[0]);
                    if (lines[0] != match.Groups["Subtitle"].Value)
                        subtitleError |= SubtitleError.Hearing_Impaired;
                }
                else if (resultsHIPrefix.Count(x => x.isMatchHIPrefix) > 1)
                {
                    foreach (var item in resultsHIPrefix)
                    {
                        if (item.isMatchHIPrefix)
                        {
                            Match match = (cleanHICaseInsensitive ? regexHIPrefixCI : regexHIPrefix).Match(lines[item.index]);
                            if (lines[item.index] != match.Groups["Prefix"].Value + match.Groups["Subtitle"].Value)
                            {
                                subtitleError |= SubtitleError.Hearing_Impaired;
                                break;
                            }
                        }
                    }
                }

                var resultsDialog = lines.Select((line, index) => new
                {
                    line,
                    index,
                    isMatchDialog = regexDialog.IsMatch(line),
                    isContainsDialog_CapitalLetter = regexContainsDialog.IsMatch(line),
                    isStartsWithDots = line.StartsWith("..."),
                    isStartsWithDotsAndItalics = line.StartsWith("<i>..."),
                    isStartsWithI = line.StartsWith("I "),
                    isStartsWithContractionI = line.StartsWith("I'"),
                    isEndsWithDots = line.EndsWith("..."),
                    isEndsWithComma = line.EndsWith(","),
                    isEndsWithLowerCaseLetter = regexEndsWithLowerCaseLetter.IsMatch(line)
                }).ToArray();

                if (resultsDialog[0].isStartsWithDots && resultsDialog.Skip(1).All(x => x.isMatchDialog))
                {
                    subtitleError |= SubtitleError.Dialog_Error;
                }
                else if (resultsDialog[0].isStartsWithDotsAndItalics && resultsDialog.Skip(1).All(x => x.isMatchDialog))
                {
                    subtitleError |= SubtitleError.Dialog_Error;
                }
                else if (resultsDialog[0].isMatchDialog && resultsDialog.Skip(1).All(x => x.isStartsWithDots || x.isStartsWithDotsAndItalics))
                {
                    subtitleError |= SubtitleError.Dialog_Error;
                }
                else if (resultsDialog[0].isMatchDialog && resultsDialog.Skip(1).All(x => x.isMatchDialog == false) && resultsDialog.Skip(1).All(x => x.isContainsDialog_CapitalLetter == false))
                {
                    string firstCharSecondLine = (lines[1][0]).ToString();

                    if (regexCapitalLetter.IsMatch(firstCharSecondLine))
                    {
                        if (resultsDialog[0].isMatchDialog &&
                            resultsDialog[0].isContainsDialog_CapitalLetter &&
                            (resultsDialog[1].isStartsWithI || resultsDialog[1].isStartsWithContractionI))
                        {
                            // don't do anything
                        }
                        else if (resultsDialog[0].isMatchDialog &&
                            resultsDialog[0].isContainsDialog_CapitalLetter &&
                            resultsDialog[0].isEndsWithDots)
                        {
                            // don't do anything
                        }
                        else if (resultsDialog[0].isMatchDialog &&
                            resultsDialog[0].isEndsWithComma &&
                            (resultsDialog[1].isStartsWithI || resultsDialog[1].isStartsWithContractionI))
                        {
                            Match match = regexDialog.Match(lines[0]);
                            if (lines[0] != match.Groups["Italic"].Value + match.Groups["Subtitle"].Value)
                                subtitleError |= SubtitleError.Dialog_Error;
                        }
                        else if (resultsDialog[0].isMatchDialog &&
                            resultsDialog[0].isEndsWithLowerCaseLetter &&
                            (resultsDialog[1].isStartsWithI || resultsDialog[1].isStartsWithContractionI))
                        {
                            Match match = regexDialog.Match(lines[0]);
                            if (lines[0] != match.Groups["Italic"].Value + match.Groups["Subtitle"].Value)
                                subtitleError |= SubtitleError.Dialog_Error;
                        }
                        else
                        {
                            subtitleError |= SubtitleError.Dialog_Error;
                        }
                    }
                    else
                    {
                        if (resultsDialog[0].isMatchDialog &&
                            resultsDialog[0].isContainsDialog_CapitalLetter &&
                            resultsDialog[1].isMatchDialog == false)
                        {
                            // don't do anything
                        }
                        else
                        {
                            Match match = regexDialog.Match(lines[0]);
                            if (lines[0] != match.Groups["Italic"].Value + match.Groups["Subtitle"].Value)
                                subtitleError |= SubtitleError.Dialog_Error;
                        }
                    }
                }
                else if (resultsDialog[0].isMatchDialog == false && resultsDialog.Skip(1).All(x => x.isMatchDialog))
                {
                    string firstCharFirstLine = (lines[0][0]).ToString();

                    if (regexCapitalLetter.IsMatch(firstCharFirstLine))
                    {
                        subtitleError |= SubtitleError.Dialog_Error;
                    }
                    else
                    {
                        Match match = regexDialog.Match(lines[1]);
                        if (lines[1] != match.Groups["Italic"].Value + match.Groups["Subtitle"].Value)
                            subtitleError |= SubtitleError.Dialog_Error;
                    }
                }
                else if (resultsDialog.Count(x => x.isMatchDialog) > 1)
                {
                    foreach (var item in resultsDialog)
                    {
                        if (item.isMatchDialog)
                        {
                            Match match = regexDialog.Match(lines[item.index]);
                            if (lines[item.index] != match.Groups["Italic"].Value + "- " + match.Groups["Subtitle"].Value)
                            {
                                subtitleError |= SubtitleError.Dialog_Error;
                                break;
                            }
                        }
                    }
                }
            }

            return subtitleError;
        }

        #endregion

        #region Clean Single Line Post

        private static string CleanSubtitleLinePost(string line, bool isPrint)
        {
            if (IsEmptyLine(line, isPrint))
                return null;

            return line;
        }

        private static SubtitleError CheckSubtitleLinePost(string line, bool isPrint)
        {
            if (IsEmptyLine(line, isPrint))
                return SubtitleError.Empty_Line;

            SubtitleError subtitleError = SubtitleError.None;

            return subtitleError;
        }

        #endregion

        #region Clean Multiple Lines Post

        private static List<string> CleanSubtitleLinesPost(List<string> lines)
        {
            // Line 1 - Dialog
            bool isMatchDialog = regexDialog.IsMatch(lines[0]);
            bool isContainsDialog = regexContainsDialog.IsMatch(lines[0]);
            if (isMatchDialog == false && isContainsDialog)
            {
                if (lines[0].StartsWith("<i>"))
                    lines[0] = "<i>- " + lines[0].Substring(3);
                else
                    lines[0] = "- " + lines[0];
            }

            if (lines.Count > 1)
            {
                for (int i = 1; i < lines.Count; i++)
                {
                    string prevLine = lines[i - 1];
                    string line = lines[i];

                    if (IsRedundantItalics(prevLine, line))
                    {
                        lines[i - 1] = prevLine.Substring(0, prevLine.Length - "</i>".Length);
                        lines[i] = line.Substring("<i>".Length);
                    }
                }

                string line0 = lines[0];
                if ((line0.StartsWith("- ") || line0.StartsWith("<i>- ") || line0.StartsWith("- <i>")) == false)
                {
                    for (int i = 1; i < lines.Count; i++)
                    {
                        string line = lines[i];
                        if (line.Contains(" - "))
                        {
                            if (line0.StartsWith("<i>"))
                                line0 = "<i>- " + line0.Substring(3);
                            else
                                line0 = "- " + line0;
                            lines[0] = line0;
                            break;
                        }
                    }
                }
            }

            // ends with '?
            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];
                if (line.EndsWith("'?") && line.EndsWith("in'?") == false)
                {
                    string str = string.Join(" ", lines.Take(i + 1))
                        .Replace("Can't", string.Empty).Replace("can't", string.Empty)
                        .Replace("Didn't", string.Empty).Replace("didn't", string.Empty)
                        .Replace("Doesn't", string.Empty).Replace("doesn't", string.Empty)
                        .Replace("Don't", string.Empty).Replace("don't", string.Empty)
                        .Replace("Hadn't", string.Empty).Replace("hadn't", string.Empty)
                        .Replace("Isn't", string.Empty).Replace("isn't", string.Empty)
                        .Replace("Won't", string.Empty).Replace("won't", string.Empty)
                        .Replace("Wouldn't", string.Empty).Replace("wouldn't", string.Empty)
                        .Replace("in'", string.Empty);

                    if (str.IndexOf("'") == str.Length - 2) // there is no ' before '?
                        lines[i] = line.Replace("'?", "?");
                }
            }

            return lines;
        }

        private static SubtitleError CheckSubtitleLinesPost(List<string> lines)
        {
            if (lines == null || lines.Count == 0)
                return SubtitleError.None;

            SubtitleError subtitleError = SubtitleError.None;

            // Line 1 - Dialog
            bool isMatchDialog = regexDialog.IsMatch(lines[0]);
            bool isContainsDialog = regexContainsDialog.IsMatch(lines[0]);
            if (isMatchDialog == false && isContainsDialog)
                subtitleError |= SubtitleError.Missing_Dash;

            if (lines.Count > 1)
            {
                for (int i = 1; i < lines.Count; i++)
                {
                    string prevLine = lines[i - 1];
                    string line = lines[i];

                    if (IsRedundantItalics(prevLine, line))
                    {
                        subtitleError |= SubtitleError.Redundant_Italics;
                        break;
                    }
                }

                string line0 = lines[0];
                if ((line0.StartsWith("- ") || line0.StartsWith("<i>- ") || line0.StartsWith("- <i>")) == false)
                {
                    for (int i = 1; i < lines.Count; i++)
                    {
                        string line = lines[i];
                        if (line.Contains(" - "))
                        {
                            subtitleError |= SubtitleError.Missing_Dash;
                            break;
                        }
                    }
                }
            }

            // ends with '?
            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];
                if (line.EndsWith("'?") && line.EndsWith("in'?") == false)
                {
                    string str = string.Join(" ", lines.Take(i + 1))
                        .Replace("Can't", string.Empty).Replace("can't", string.Empty)
                        .Replace("Didn't", string.Empty).Replace("didn't", string.Empty)
                        .Replace("Doesn't", string.Empty).Replace("doesn't", string.Empty)
                        .Replace("Don't", string.Empty).Replace("don't", string.Empty)
                        .Replace("Hadn't", string.Empty).Replace("hadn't", string.Empty)
                        .Replace("Isn't", string.Empty).Replace("isn't", string.Empty)
                        .Replace("Won't", string.Empty).Replace("won't", string.Empty)
                        .Replace("Wouldn't", string.Empty).Replace("wouldn't", string.Empty)
                        .Replace("in'", string.Empty);

                    if (str.IndexOf("'") == str.Length - 2) // there is no ' before '?
                    {
                        subtitleError |= SubtitleError.Punctuation_Error;
                        break;
                    }
                }
            }

            return subtitleError;
        }

        #endregion

        #endregion

        #region Clean & Validation

        private const string HI_CHARS = @"A-ZÁ-Ú0-9 #\-'.";
        private const string HI_CHARS_CI = @"A-ZÁ-Úa-zá-ú0-9 #\-'.";

        #region Empty Line

        public static readonly FindAndReplace[] EmptyLine = new FindAndReplace[] {
            new FindAndReplace(new Regex(@"^[-!?:_#.*♪♫¶ ]*$", RegexOptions.Compiled), "", SubtitleError.Empty_Line)
            ,new FindAndReplace(new Regex(@"^<i>[-!?:_#.*♪♫¶ ]*</i>$", RegexOptions.Compiled), "", SubtitleError.Empty_Line)
        };

        #endregion

        #region Not Subtitle

        public static readonly FindAndReplace[] NotSubtitle = new FindAndReplace[] {
            new FindAndReplace(new Regex(@"AllSubs\.org", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"Best watched using", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"Captioned by", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"Captioning by", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"Captioning made possible by", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"Captioning performed by", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"Captioning sponsored by", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"Captions by", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"Captions copyright", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"Captions made possible by", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"Captions performed by", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"Captions sponsored by", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"Captions, Inc\.", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"Closed Caption", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"Closed-Caption", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"Contain Strong Language", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"Contains Strong Language", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"Copyright Australian", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"Corrected by", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"DVDRIP by", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"ENGLISH - SDH", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"ENGLISH - US - SDH", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"ENGLISH SDH", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"Eng subs", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"Eng subtitles", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"ExplosiveSkull", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"HighCode", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"MKV Player", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"NETFLIX PRESENTS", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"OCR by", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"Open Subtitles", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"OpenSubtitles", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"Proofread by", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"Rip by", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"Ripped by", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"SUBTITLES EDITED BY", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"SharePirate\.com", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"Subs by", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"Subscene", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"Subtitled By", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"Subtitles by", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"Subtitles:", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"Subtitletools\.com", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"Subtitling", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"Sync by", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"Synced & corrected", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"Synced and corrected", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"Synchronization by", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"Synchronized by", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"ThePirateBay", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"Translated by", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"Translation by", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"Translations by", RegexOptions.Compiled | RegexOptions.IgnoreCase), "", SubtitleError.Not_Subtitle)

            ,new FindAndReplace(new Regex(@"DIRECTED BY", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"PRODUCED BY", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"WRITTEN BY", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)

            ,new FindAndReplace(new Regex(@"\\fad", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"\\move", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)

            ,new FindAndReplace(new Regex(@"<font color=", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"^-?\s*<font>.*?</\s*font>$", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"^-?\s*<font\s+.*?</\s*font>$", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
        };

        #endregion

        #region Punctuations

        public static readonly FindAndReplace[] Punctuations = new FindAndReplace[] {
            new FindAndReplace(new Regex(@"'’", RegexOptions.Compiled), "'", SubtitleError.Punctuation_Error)
            ,new FindAndReplace(new Regex(@"[´`‘’]", RegexOptions.Compiled), "'", SubtitleError.Punctuation_Error)
            ,new FindAndReplace(new Regex(@"^'{3}", RegexOptions.Compiled), "\"'", SubtitleError.Punctuation_Error)
            ,new FindAndReplace(new Regex(@"'{3}$", RegexOptions.Compiled), "'\"", SubtitleError.Punctuation_Error)
            ,new FindAndReplace(new Regex(@"[“”]", RegexOptions.Compiled), "\"", SubtitleError.Punctuation_Error)
            ,new FindAndReplace(new Regex(@"(\\x22)+", RegexOptions.Compiled), "\"", SubtitleError.Punctuation_Error)
            ,new FindAndReplace(new Regex(@"'{2}", RegexOptions.Compiled), "\"", SubtitleError.Punctuation_Error)
            ,new FindAndReplace(new Regex(@"""{2,}", RegexOptions.Compiled), "\"", SubtitleError.Punctuation_Error)
            ,new FindAndReplace(new Regex(@"[?!:](?<Dot>\.)(\s|\b|$)", RegexOptions.Compiled), "Dot", string.Empty, SubtitleError.Punctuation_Error)
            ,new FindAndReplace(new Regex(@"\s\?", RegexOptions.Compiled), "?", SubtitleError.Punctuation_Error)
            ,new FindAndReplace(new Regex(@"\s!", RegexOptions.Compiled), "!", SubtitleError.Punctuation_Error)
            ,new FindAndReplace(new Regex(@"\s:", RegexOptions.Compiled), ":", SubtitleError.Punctuation_Error)
            ,new FindAndReplace(new Regex(@"[‐=]", RegexOptions.Compiled), "-", SubtitleError.Punctuation_Error)
            ,new FindAndReplace(new Regex(@"-\s-", RegexOptions.Compiled), "-", SubtitleError.Punctuation_Error)
            ,new FindAndReplace(new Regex(@"[…—–―‒]", RegexOptions.Compiled), "...", SubtitleError.Punctuation_Error)
            ,new FindAndReplace(new Regex(@"\.\s\.\.", RegexOptions.Compiled), "...", SubtitleError.Punctuation_Error)
            ,new FindAndReplace(new Regex(@"\.\.\s\.", RegexOptions.Compiled), "...", SubtitleError.Punctuation_Error)
            ,new FindAndReplace(new Regex(@"\.\s\.\s\.", RegexOptions.Compiled), "...", SubtitleError.Punctuation_Error)
            ,new FindAndReplace(new Regex(@"-{2,}", RegexOptions.Compiled), "...", SubtitleError.Punctuation_Error)
            ,new FindAndReplace(new Regex(@",{2,}", RegexOptions.Compiled), "...", SubtitleError.Punctuation_Error)
            ,new FindAndReplace(new Regex(@"\.{4,}", RegexOptions.Compiled), "...", SubtitleError.Punctuation_Error)
            ,new FindAndReplace(new Regex(@"[;，]", RegexOptions.Compiled), ",", SubtitleError.Punctuation_Error)
            ,new FindAndReplace(new Regex(@"[♫¶*]", RegexOptions.Compiled), "♪", SubtitleError.Punctuation_Error)
            ,new FindAndReplace(new Regex(@"\#(?![0-9])", RegexOptions.Compiled), "♪", SubtitleError.Punctuation_Error)
            ,new FindAndReplace(new Regex(@"♪{2,}", RegexOptions.Compiled), "♪", SubtitleError.Punctuation_Error)
        };

        #endregion

        #region Redundant Italics

        public static readonly FindAndReplace[] RedundantItalics = new FindAndReplace[] {
            new FindAndReplace(new Regex(@"<i>\.</i>", RegexOptions.Compiled), ".", SubtitleError.Redundant_Italics)
            ,new FindAndReplace(new Regex(@"<i>\.{2,}</i>", RegexOptions.Compiled), "...", SubtitleError.Redundant_Italics)
        };

        #endregion

        #region Hearing Impaired Full Line

        public static readonly FindAndReplace[] HearingImpairedFullLine = new FindAndReplace[] {
            // ^(HI)$
            // ^- (HI)$
            new FindAndReplace(new Regex(@"^\s*-?\s*\(.*?\)\s*$", RegexOptions.Compiled), "", SubtitleError.Hearing_Impaired)
            ,new FindAndReplace(new Regex(@"^\s*-?\s*\[.*?\]\s*$", RegexOptions.Compiled), "", SubtitleError.Hearing_Impaired)

            // ^<i>(HI)</i>$
            // ^- <i>(HI)</i>$
            // ^<i>- (HI)</i>$
            // ^♪ <i>(HI)</i>$
            // ^<i>♪ (HI)</i>$
            ,new FindAndReplace(new Regex(@"^\s*-?\s*♪?\s*<i>\s*-?\s*♪?\s*\(.*?\)\s*</i>\s*$", RegexOptions.Compiled), "", SubtitleError.Hearing_Impaired)
            ,new FindAndReplace(new Regex(@"^\s*-?\s*♪?\s*<i>\s*-?\s*♪?\s*\[.*?\]\s*</i>\s*$", RegexOptions.Compiled), "", SubtitleError.Hearing_Impaired)

            ,new FindAndReplace(new Regex(@"^[" + HI_CHARS + @"\[\]]+:\s*$", RegexOptions.Compiled), "", SubtitleError.Hearing_Impaired)
                    .SetRegexCI(new Regex(@"^[" + HI_CHARS_CI + @"\[\]]+:\s*$", RegexOptions.Compiled))
            ,new FindAndReplace(new Regex(@"^[" + HI_CHARS + @"]+\[.*?\]:\s*$", RegexOptions.Compiled), "", SubtitleError.Hearing_Impaired)
                    .SetRegexCI(new Regex(@"^[" + HI_CHARS_CI + @"]+\[.*?\]:\s*$", RegexOptions.Compiled))
        };

        #endregion

        #region Screen Position

        public static readonly FindAndReplace[] ScreenPosition = new FindAndReplace[] {
            // {\a1}
            new FindAndReplace(new Regex(@"\{\\a\d+}", RegexOptions.Compiled), "", SubtitleError.Screen_Position)
            // {\an1}
            ,new FindAndReplace(new Regex(@"\{\\an\d+}", RegexOptions.Compiled), "", SubtitleError.Screen_Position)
            // {\pos(250,270)}
            ,new FindAndReplace(new Regex(@"\{\\pos\(\d+,\d+\)}", RegexOptions.Compiled), "", SubtitleError.Screen_Position)
        };

        #endregion

        #region Redundant Spaces

        public static readonly FindAndReplace[] RedundantSpaces = new FindAndReplace[] {
            new FindAndReplace(new Regex(@"<i/>", RegexOptions.Compiled), "</i>", SubtitleError.Redundant_Spaces)
            ,new FindAndReplace(new Regex(@"</ i>", RegexOptions.Compiled), "</i>", SubtitleError.Redundant_Spaces)
            ,new FindAndReplace(new Regex(@"<i>\s*</i>", RegexOptions.Compiled), "", SubtitleError.Redundant_Spaces)

            ,new FindAndReplace(new Regex(@"<u/>", RegexOptions.Compiled), "</u>", SubtitleError.Redundant_Spaces)
            ,new FindAndReplace(new Regex(@"</ u>", RegexOptions.Compiled), "</u>", SubtitleError.Redundant_Spaces)
            ,new FindAndReplace(new Regex(@"<u>\s*</u>", RegexOptions.Compiled), "", SubtitleError.Redundant_Spaces)

            ,new FindAndReplace(new Regex(@"^(?<Dash>-)(?<Space>\s*)(?<Italic></i>)", RegexOptions.Compiled), "${Italic}${Space}${Dash}", SubtitleError.Redundant_Spaces)
            ,new FindAndReplace(new Regex(@"<i>-\s+</i>", RegexOptions.Compiled), "- ", SubtitleError.Redundant_Spaces)

            // a<i> b </i>c => a <i>b</i> c
            ,new FindAndReplace(new Regex(@"([^ ])<i>[ ]", RegexOptions.Compiled), "$1 <i>", SubtitleError.Redundant_Spaces)
            ,new FindAndReplace(new Regex(@"[ ]</i>([^ ])", RegexOptions.Compiled), "</i> $1", SubtitleError.Redundant_Spaces)

            ,new FindAndReplace(new Regex(@"\.<i>\s+", RegexOptions.Compiled), ". <i>", SubtitleError.Redundant_Spaces)
            ,new FindAndReplace(new Regex(@",<i>\s+", RegexOptions.Compiled), ", <i>", SubtitleError.Redundant_Spaces)
            ,new FindAndReplace(new Regex(@"<i>\s+", RegexOptions.Compiled), "<i>", SubtitleError.Redundant_Spaces)
            ,new FindAndReplace(new Regex(@"\s+</i>", RegexOptions.Compiled), "</i>", SubtitleError.Redundant_Spaces)

            // 1 987 => 1987 (Positive lookahead)
            // 1 1/2 => 1 1/2 (Negative lookahead)
            ,new FindAndReplace(new Regex(@"1\s+(?=[0-9.,/])(?!1/2|1 /2)", RegexOptions.Compiled), "1", SubtitleError.Redundant_Spaces)
        };

        #endregion

        #region Hearing Impaired

        public static readonly FindAndReplace[] HearingImpaired = new FindAndReplace[] {
            // - (MAN): Text => - Text
            new FindAndReplace(new Regex(@"^(?<Prefix>- )?\(.*?\)(:\s*)?(?<Subtitle>.+)$", RegexOptions.Compiled), "${Prefix}${Subtitle}", SubtitleError.Hearing_Impaired)
            ,new FindAndReplace(new Regex(@"^(?<Prefix>- )?\[.*?\](:\s*)?(?<Subtitle>.+)$", RegexOptions.Compiled), "${Prefix}${Subtitle}", SubtitleError.Hearing_Impaired)

            // <i>- MAN (laughting): Text</i> => <i>- Text</i>
            ,new FindAndReplace(new Regex(@"^(?<Prefix><i>\s*-?\s*)[A-Z]*\s*\(.*?\)(:\s*)?(?<Subtitle>.+?)(?<Suffix></i>)$", RegexOptions.Compiled), "${Prefix}${Subtitle}${Suffix}", SubtitleError.Hearing_Impaired)
            ,new FindAndReplace(new Regex(@"^(?<Prefix><i>\s*-?\s*)[A-Z]*\s*\[.*?\](:\s*)?(?<Subtitle>.+?)(?<Suffix></i>)$", RegexOptions.Compiled), "${Prefix}${Subtitle}${Suffix}", SubtitleError.Hearing_Impaired)

            // debug
            // this doesn't match
            // - MAN (laughting): Text

            // <i>MAN (laughting): => <i>
            ,new FindAndReplace(new Regex(@"^(?<Prefix><i>)[A-Z]*\s*\(.*?\):$", RegexOptions.Compiled), "${Prefix}", SubtitleError.Hearing_Impaired)
            ,new FindAndReplace(new Regex(@"^(?<Prefix><i>)[A-Z]*\s*\[.*?\]:$", RegexOptions.Compiled), "${Prefix}", SubtitleError.Hearing_Impaired)

            // Text (laughting) => Text
            ,new FindAndReplace(new Regex(@"^(?<Subtitle>.+?)\(.*?\)$", RegexOptions.Compiled), "${Subtitle}", SubtitleError.Hearing_Impaired)
            ,new FindAndReplace(new Regex(@"^(?<Subtitle>.+?)\[.*?\]$", RegexOptions.Compiled), "${Subtitle}", SubtitleError.Hearing_Impaired)

            // MAN #1: Text => Text
            ,new FindAndReplace(new Regex(@"^[0-9A-Z #\-'\[\]]*[A-Z#'\[\]][0-9A-Z #\-'\[\]]*:\s*(?<Subtitle>.+?)$", RegexOptions.Compiled), "${Subtitle}", SubtitleError.Hearing_Impaired)

            // Some (laughting) text => Some text
            ,new FindAndReplace(new Regex(@"\s+\(.*?\)\s+", RegexOptions.Compiled), " ", SubtitleError.Hearing_Impaired)
            ,new FindAndReplace(new Regex(@"\s+\[.*?\]\s+", RegexOptions.Compiled), " ", SubtitleError.Hearing_Impaired)

            // Text <i>(laughting)</i> => Text
            ,new FindAndReplace(new Regex(@"(?:<i>\s*)\(.*?\)(?:\s*</i>)", RegexOptions.Compiled), "", SubtitleError.Hearing_Impaired)
            ,new FindAndReplace(new Regex(@"(?:<i>\s*)\[.*?\](?:\s*</i>)", RegexOptions.Compiled), "", SubtitleError.Hearing_Impaired)

            // MAN [laughting]: Text => Text
            ,new FindAndReplace(new Regex(@"^[" + HI_CHARS.Replace("-'", "'") + @"]+\[.*?\]:\s*", RegexOptions.Compiled), "", SubtitleError.Hearing_Impaired)
                    .SetRegexCI(new Regex(@"^[" + HI_CHARS_CI.Replace("-'", "'") + @"]+\[.*?\]:\s*", RegexOptions.Compiled))

            // - MAN [laughting]: Text => - Text
            ,new FindAndReplace(new Regex(@"^-\s*[" + HI_CHARS + @"]+\[.*?\]:\s*", RegexOptions.Compiled), "- ", SubtitleError.Hearing_Impaired)
                    .SetRegexCI(new Regex(@"^-\s*[" + HI_CHARS_CI + @"]+\[.*?\]:\s*", RegexOptions.Compiled))

            // <i>- MAN LAUGHTING: Text => <i>- Text
            // - <i>MAN LAUGHTING: Text => - <i>Text
            ,new FindAndReplace(new Regex(@"^(?<Prefix>(?:<i>)?-?\s*|-?\s*(?:<i>)?\s*)[" + HI_CHARS + @"]*[A-Z]+[" + HI_CHARS + @"]*:\s*(?<Subtitle>.*?)$", RegexOptions.Compiled), "${Prefix}${Subtitle}", SubtitleError.Hearing_Impaired)
                    .SetRegexCI(new Regex(@"^(?<Prefix>(?:<i>)?-?\s*|-?\s*(?:<i>)?\s*)[" + HI_CHARS_CI + @"]*[A-Z]+[" + HI_CHARS_CI + @"]*:\s*(?<Subtitle>.*?)$", RegexOptions.Compiled))

            // <i>- : Text => Text
            ,new FindAndReplace(new Regex(@"^(?:\s*<i>)?\s*-\s*:\s*", RegexOptions.Compiled), "", SubtitleError.Hearing_Impaired)

            // <i>: Text => Text
            ,new FindAndReplace(new Regex(@"^(?:\s*<i>)?\s*:\s*", RegexOptions.Compiled), "", SubtitleError.Hearing_Impaired)

            // {MAN} {MAN) {MAN] (MAN} [MAN}
            ,new FindAndReplace(new Regex(@"\{[^\{\[\(\)\]\}]+[\)\]\}]", RegexOptions.Compiled), "", SubtitleError.Hearing_Impaired)
            ,new FindAndReplace(new Regex(@"[\{\[\(][^\{\[\(\)\]\}]+\}", RegexOptions.Compiled), "", SubtitleError.Hearing_Impaired)
        };

        #endregion

        #region Missing Spaces

        public static readonly FindAndReplace[] MissingSpaces = new FindAndReplace[] {
            // <i>♪Lyrics => <i>♪ Lyrics
            new FindAndReplace(new Regex(@"^(?<Prefix>(?:<i>)?♪+)(?<Lyrics>[^ ♪])", RegexOptions.Compiled), "${Prefix} ${Lyrics}", SubtitleError.Missing_Spaces)
            // ♪<i>Lyrics => ♪ <i>Lyrics
            ,new FindAndReplace(new Regex(@"^(?<Prefix>♪+)(?<Italic><i>)?(?<Lyrics>[^ ♪])", RegexOptions.Compiled), "${Prefix} ${Italic}${Lyrics}", SubtitleError.Missing_Spaces)

            // Lyrics</i>♪ => Lyrics</i> ♪
            ,new FindAndReplace(new Regex(@"(?<Lyrics>[^ ♪])(?<Italic></i>)?(?<Suffix>♪+)$", RegexOptions.Compiled), "${Lyrics}${Italic} ${Suffix}", SubtitleError.Missing_Spaces)
            // Lyrics♪</i> => Lyrics ♪</i>
            ,new FindAndReplace(new Regex(@"(?<Lyrics>[^ ♪])(?<Suffix>♪+(?:</i>)?)$", RegexOptions.Compiled), "${Lyrics} ${Suffix}", SubtitleError.Missing_Spaces)

            //  -Text =>  - Text
            ,new FindAndReplace(new Regex(@"(?<Prefix>\s+)(?<Dash>-)(?<Suffix>[A-ZÁ-Úa-zá-ú])", RegexOptions.Compiled), "${Prefix}${Dash} ${Suffix}", SubtitleError.Missing_Spaces)
        };

        #endregion

        #region Trim Spaces

        public static readonly FindAndReplace[] TrimSpaces = new FindAndReplace[] {
            new FindAndReplace(new Regex(@"\s{2,}", RegexOptions.Compiled), " ", SubtitleError.Redundant_Spaces)
            ,new FindAndReplace(new Regex(@"^\s+", RegexOptions.Compiled), "", SubtitleError.Redundant_Spaces)
            ,new FindAndReplace(new Regex(@"\s+$", RegexOptions.Compiled), "", SubtitleError.Redundant_Spaces)
        };

        #endregion

        #region OCR Error - Non-Ansi Chars

        public static readonly FindAndReplace[] OCRError_NonAnsiChars = new FindAndReplace[] {
            new FindAndReplace(new Regex(@"ﬁ", RegexOptions.Compiled), "fi", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"Α", RegexOptions.Compiled), "A", SubtitleError.OCR_Error) // 913 -> A
            ,new FindAndReplace(new Regex(@"Β", RegexOptions.Compiled), "B", SubtitleError.OCR_Error) // 914 -> B
            ,new FindAndReplace(new Regex(@"Ε", RegexOptions.Compiled), "E", SubtitleError.OCR_Error) // 917 -> E
            ,new FindAndReplace(new Regex(@"Ζ", RegexOptions.Compiled), "Z", SubtitleError.OCR_Error) // 918 -> Z
            ,new FindAndReplace(new Regex(@"Η", RegexOptions.Compiled), "H", SubtitleError.OCR_Error) // 919 -> H
            ,new FindAndReplace(new Regex(@"Ι", RegexOptions.Compiled), "I", SubtitleError.OCR_Error) // 921 -> I
            ,new FindAndReplace(new Regex(@"Κ", RegexOptions.Compiled), "K", SubtitleError.OCR_Error) // 922 -> K
            ,new FindAndReplace(new Regex(@"Μ", RegexOptions.Compiled), "M", SubtitleError.OCR_Error) // 924 -> M
            ,new FindAndReplace(new Regex(@"Ν", RegexOptions.Compiled), "N", SubtitleError.OCR_Error) // 925 -> N
            ,new FindAndReplace(new Regex(@"Ο", RegexOptions.Compiled), "O", SubtitleError.OCR_Error) // 927 -> O
            ,new FindAndReplace(new Regex(@"Ρ", RegexOptions.Compiled), "P", SubtitleError.OCR_Error) // 929 -> P
            ,new FindAndReplace(new Regex(@"Τ", RegexOptions.Compiled), "T", SubtitleError.OCR_Error) // 932 -> T
            ,new FindAndReplace(new Regex(@"Υ", RegexOptions.Compiled), "Y", SubtitleError.OCR_Error) // 933 -> Y
            ,new FindAndReplace(new Regex(@"Χ", RegexOptions.Compiled), "X", SubtitleError.OCR_Error) // 935 -> X
            ,new FindAndReplace(new Regex(@"ϲ", RegexOptions.Compiled), "c", SubtitleError.OCR_Error) // 1010 -> c
            ,new FindAndReplace(new Regex(@"ϳ", RegexOptions.Compiled), "j", SubtitleError.OCR_Error) // 1011 -> j
            ,new FindAndReplace(new Regex(@"Ϲ", RegexOptions.Compiled), "C", SubtitleError.OCR_Error) // 1017 -> C
            ,new FindAndReplace(new Regex(@"Ϻ", RegexOptions.Compiled), "M", SubtitleError.OCR_Error) // 1018 -> M
            ,new FindAndReplace(new Regex(@"Ѕ", RegexOptions.Compiled), "S", SubtitleError.OCR_Error) // 1029 -> S
            ,new FindAndReplace(new Regex(@"І", RegexOptions.Compiled), "I", SubtitleError.OCR_Error) // 1030 -> I
            ,new FindAndReplace(new Regex(@"Ј", RegexOptions.Compiled), "J", SubtitleError.OCR_Error) // 1032 -> J
            ,new FindAndReplace(new Regex(@"А", RegexOptions.Compiled), "A", SubtitleError.OCR_Error) // 1040 -> A
            ,new FindAndReplace(new Regex(@"В", RegexOptions.Compiled), "B", SubtitleError.OCR_Error) // 1042 -> B
            ,new FindAndReplace(new Regex(@"Е", RegexOptions.Compiled), "E", SubtitleError.OCR_Error) // 1045 -> E
            ,new FindAndReplace(new Regex(@"К", RegexOptions.Compiled), "K", SubtitleError.OCR_Error) // 1050 -> K
            ,new FindAndReplace(new Regex(@"М", RegexOptions.Compiled), "M", SubtitleError.OCR_Error) // 1052 -> M
            ,new FindAndReplace(new Regex(@"Н", RegexOptions.Compiled), "H", SubtitleError.OCR_Error) // 1053 -> H
            ,new FindAndReplace(new Regex(@"О", RegexOptions.Compiled), "O", SubtitleError.OCR_Error) // 1054 -> O
            ,new FindAndReplace(new Regex(@"Р", RegexOptions.Compiled), "P", SubtitleError.OCR_Error) // 1056 -> P
            ,new FindAndReplace(new Regex(@"С", RegexOptions.Compiled), "C", SubtitleError.OCR_Error) // 1057 -> C
            ,new FindAndReplace(new Regex(@"Т", RegexOptions.Compiled), "T", SubtitleError.OCR_Error) // 1058 -> T
            ,new FindAndReplace(new Regex(@"У", RegexOptions.Compiled), "y", SubtitleError.OCR_Error) // 1059 -> y
            ,new FindAndReplace(new Regex(@"Х", RegexOptions.Compiled), "X", SubtitleError.OCR_Error) // 1061 -> X
            ,new FindAndReplace(new Regex(@"а", RegexOptions.Compiled), "a", SubtitleError.OCR_Error) // 1072 -> a
            ,new FindAndReplace(new Regex(@"е", RegexOptions.Compiled), "e", SubtitleError.OCR_Error) // 1077 -> e
            ,new FindAndReplace(new Regex(@"о", RegexOptions.Compiled), "o", SubtitleError.OCR_Error) // 1086 -> o
            ,new FindAndReplace(new Regex(@"р", RegexOptions.Compiled), "p", SubtitleError.OCR_Error) // 1088 -> p
            ,new FindAndReplace(new Regex(@"с", RegexOptions.Compiled), "c", SubtitleError.OCR_Error) // 1089 -> c
            ,new FindAndReplace(new Regex(@"у", RegexOptions.Compiled), "y", SubtitleError.OCR_Error) // 1091 -> y
            ,new FindAndReplace(new Regex(@"х", RegexOptions.Compiled), "x", SubtitleError.OCR_Error) // 1093 -> x
        };

        #endregion

        #region OCR Error - Mangled Letters

        public static readonly FindAndReplace[] OCRError_MangledLetters = new FindAndReplace[] {
            new FindAndReplace(new Regex(@"\b(?<OCR>I-l)", RegexOptions.Compiled), "OCR", "H", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?<OCR>I- l)", RegexOptions.Compiled), "OCR", "H", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?<OCR>L\\/l)", RegexOptions.Compiled), "OCR", "M", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?<OCR>I\\/l)", RegexOptions.Compiled), "OCR", "M", SubtitleError.OCR_Error)
        };

        #endregion

        #region OCR Error - Contractions

        public static readonly FindAndReplace[] OCRError_Contractions = new FindAndReplace[] {
            new FindAndReplace(new Regex(@"\b(?i:I)(?<OCR>""|''|'’| ""|"" )d\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:I)(?<OCR>""|''|'’| ""|"" )ll\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:I)(?<OCR>""|''|'’| ""|"" )m\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:I)(?<OCR>""|''|'’| ""|"" )ve\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:a)ren(?<OCR>""|''|'’| ""|"" )t\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:c)ouldn(?<OCR>""|''|'’| ""|"" )t\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:d)on(?<OCR>""|''|'’| ""|"" )t\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:h)e(?<OCR>""|''|'’| ""|"" )ll\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:h)e(?<OCR>""|''|'’| ""|"" )s\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:i)sn(?<OCR>""|''|'’| ""|"" )t\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:i)t(?<OCR>""|''|'’| ""|"" )s\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:l)et(?<OCR>""|''|'’| ""|"" )s\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:s)he(?<OCR>""|''|'’| ""|"" )ll\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:s)he(?<OCR>""|''|'’| ""|"" )s\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:t)hat(?<OCR>""|''|'’| ""|"" )s\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:t)here(?<OCR>""|''|'’| ""|"" )ll\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:t)here(?<OCR>""|''|'’| ""|"" )s\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:t)hey(?<OCR>""|''|'’| ""|"" )re\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:w)e(?<OCR>""|''|'’| ""|"" )re\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:w)eren(?<OCR>""|''|'’| ""|"" )t\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:w)hat(?<OCR>""|''|'’| ""|"" )s\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:w)ould(?<OCR>""|''|'’| ""|"" )ve\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:w)ouldn(?<OCR>""|''|'’| ""|"" )t\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:y)ou(?<OCR>""|''|'’| ""|"" )ll\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:y)ou(?<OCR>""|''|'’| ""|"" )re\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:y)ou(?<OCR>""|''|'’| ""|"" )ve\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:o)(?<OCR>""|''|'’| ""|"" )er\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.OCR_Error)
            // 'tweren't
            ,new FindAndReplace(new Regex(@"(?i:t)weren(?<OCR>""|''|'’| ""|"" )t\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\s?(?<OCR>""|''|'’| ""|"" )(?i:t)weren't\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.OCR_Error)
            // 'em
            ,new FindAndReplace(new Regex(@"\b\s(?<OCR>""|''|'’| ""|"" )em\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.OCR_Error)
            // 's
            ,new FindAndReplace(new Regex(@"[a-z](?<OCR>""|''|'’| ""|"" )s\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.OCR_Error)
        };

        #endregion

        #region OCR Error - Accent Letters

        public static readonly FindAndReplace[] OCRError_AccentLetters = new FindAndReplace[] {
            new FindAndReplace(new Regex(@"\b(?i:F)ianc(?<OCR>'e)\b", RegexOptions.Compiled), "OCR", "é", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:C)af(?<OCR>'e)\b", RegexOptions.Compiled), "OCR", "é", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:C)ach(?<OCR>'e)\b", RegexOptions.Compiled), "OCR", "é", SubtitleError.OCR_Error)
        };

        #endregion

        #region OCR Error - I and L

        public static readonly FindAndReplace[] OCRError_I_And_L = new FindAndReplace[] {
            // The most common OCR error - I (uppercase i) and l (lowercase L) mistakes

            // Roman numerals
            new FindAndReplace(new Regex(@"\b[VXLCDM]*(?<OCR>lll)\b", RegexOptions.Compiled), "OCR", "III", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"[^.?!—–―‒-][^']\b[IVXLCDM]*(?<OCR>ll)I{0,1}\b", RegexOptions.Compiled), "OCR", "II", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"^(?<OCR>ll)\b", RegexOptions.Compiled), "OCR", "II", SubtitleError.OCR_Error)

            ,new FindAndReplace(new Regex(@"\b[IVXLCDM]*(?<OCR>l)[IVX]*\b", RegexOptions.Compiled), "OCR", "I", SubtitleError.OCR_Error,
                new FindAndReplace.IgnoreRule() { ReadNextCharsFromMatch = 8, IgnoreIfEqualsTo = "Il y avait" }
                , new FindAndReplace.IgnoreRule() { ReadNextCharsFromMatch = 5, IgnoreIfEqualsTo = "Il y a " }
                , new FindAndReplace.IgnoreRule() { ReadNextCharsFromMatch = 5, IgnoreIfEqualsTo = "Il faut" }
            )

			// Replace "II" with "ll" at the end of a lowercase word
			,new FindAndReplace(new Regex(@"[a-zá-ú](?<OCR>II)", RegexOptions.Compiled), "OCR", "ll", SubtitleError.OCR_Error)
			// Replace "II" with "ll" at the beginning of a lowercase word
			,new FindAndReplace(new Regex(@"(?<OCR>II)[a-zá-ú]", RegexOptions.Compiled), "OCR", "ll", SubtitleError.OCR_Error)
			// Replace "I" with "l" in the middle of a lowercase word
			,new FindAndReplace(new Regex(@"[a-zá-ú](?<OCR>I)[a-zá-ú]", RegexOptions.Compiled), "OCR", "l", SubtitleError.OCR_Error)
			// Replace "I" with "l" at the end of a lowercase word
			,new FindAndReplace(new Regex(@"[a-zá-ú](?<OCR>I)\b", RegexOptions.Compiled), "OCR", "l", SubtitleError.OCR_Error)
			// Replace "l" with "I" in the middle of an uppercase word
			,new FindAndReplace(new Regex(@"[A-ZÁ-Ú](?<OCR>l)[A-ZÁ-Ú]", RegexOptions.Compiled), "OCR", "I", SubtitleError.OCR_Error)
			// Replace "l" with "I" at the end of an uppercase word
			,new FindAndReplace(new Regex(@"[A-ZÁ-Ú]{2,}(?<OCR>l)", RegexOptions.Compiled), "OCR", "I", SubtitleError.OCR_Error)

			// Replace a single "l" with "I"
			,new FindAndReplace(new Regex(@"\b(?<OCR>l)\b", RegexOptions.Compiled), "OCR", "I", SubtitleError.OCR_Error)

			// Replace a single "i" with "I", but not <i> or </i>
            ,new FindAndReplace(new Regex(@"\b(?<OCR>(?<!<|/)i(?!>))\b", RegexOptions.Compiled), "OCR", "I", SubtitleError.OCR_Error)

			// Replace "I'II"/"you'II" etc. with "I'll"/"you'll" etc.
			,new FindAndReplace(new Regex(@"[A-ZÁ-Úa-zá-ú]'(?<OCR>II)\b", RegexOptions.Compiled), "OCR", "ll", SubtitleError.OCR_Error)
			// Replace "I 'II" with "I'll" or "I' II" with "I'll" - rare cases with a space before or after the apostrophe
			,new FindAndReplace(new Regex(@"[A-ZÁ-Úa-zá-ú](?<OCR>'\sII|\s'II)\b", RegexOptions.Compiled), "OCR", "'ll", SubtitleError.OCR_Error)

            // All
            ,new FindAndReplace(new Regex(@"\bA(?<OCR>II)\b", RegexOptions.Compiled), "OCR", "ll", SubtitleError.OCR_Error)

            // Live, Living
            ,new FindAndReplace(new Regex(@"^(?<OCR>I)(?:ive|iving)\b", RegexOptions.Compiled), "OCR", "L", SubtitleError.OCR_Error)
            // live, living
            ,new FindAndReplace(new Regex(@"\s+(?<OCR>I)(?:ive|iving)\b", RegexOptions.Compiled), "OCR", "l", SubtitleError.OCR_Error)

            // "I" at the beginning of a word before lowercase vowels is most likely an "l"
            ,new FindAndReplace(new Regex(@"\b(?<OCR>I)[oaeiuyá-ú]", RegexOptions.Compiled), "OCR", "l", SubtitleError.OCR_Error,
                new FindAndReplace.IgnoreRule() { ReadNextCharsFromMatch = 2, IgnoreIfEqualsTo = "Iago" }
            )

            // "I" after an uppercase letter at the beginning and before a lowercase letter is most likely an "l"
            ,new FindAndReplace(new Regex(@"\b[A-ZÁ-Ú](?<OCR>I)[a-zá-ú]", RegexOptions.Compiled), "OCR", "l", SubtitleError.OCR_Error)

            // "l" at the beginning before a consonant different from "l" is most likely an "I"
            ,new FindAndReplace(new Regex(@"\b(?<OCR>l)[^aeiouyàá-úl]", RegexOptions.Compiled), "OCR", "I", SubtitleError.OCR_Error,
                new FindAndReplace.IgnoreRule() { ReadNextCharsFromMatch = 1, IgnoreIfEqualsTo = "lbs" }
            )

			// Fixes for "I" at the beginning of the word before lowercase vowels
			// The name "Ian"
			,new FindAndReplace(new Regex(@"\b(?<OCR>lan)\b", RegexOptions.Compiled), "OCR", "Ian", SubtitleError.OCR_Error)
			// The name "Iowa"
			,new FindAndReplace(new Regex(@"\b(?<OCR>lowa)\b", RegexOptions.Compiled), "OCR", "Iowa", SubtitleError.OCR_Error)
			// The word "Ill"
			,new FindAndReplace(new Regex(@"[.?!-]\s?(?<OCR>III)\b", RegexOptions.Compiled), "OCR", "Ill", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"^(?<OCR>III)\b.", RegexOptions.Compiled), "OCR", "Ill", SubtitleError.OCR_Error)
			// The word "Ion" and its derivatives
			,new FindAndReplace(new Regex(@"\b(?<OCR>l)on\b.", RegexOptions.Compiled), "OCR", "I", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?<OCR>l)oni", RegexOptions.Compiled), "OCR", "I", SubtitleError.OCR_Error)
			// The word "Iodine" and its derivatives
			,new FindAndReplace(new Regex(@"\b(?<OCR>l)odi", RegexOptions.Compiled), "OCR", "I", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?<OCR>l)odo", RegexOptions.Compiled), "OCR", "I", SubtitleError.OCR_Error)

            ,new FindAndReplace(new Regex(@"\b(?<OCR>L)\b", RegexOptions.Compiled), "OCR", "I", SubtitleError.OCR_Error,
                new FindAndReplace.IgnoreRule() { ReadPrevCharsFromMatch = 1, ReadNextCharsFromMatch = 1, IgnoreIfEqualsTo = "-L-" }
                , new FindAndReplace.IgnoreRule() { ReadPrevCharsFromMatch = 1, ReadNextCharsFromMatch = 1, IgnoreIfEqualsTo = "-L." }
                , new FindAndReplace.IgnoreRule() { ReadPrevCharsFromMatch = 1, ReadNextCharsFromMatch = 1, IgnoreIfEqualsTo = "-L." }
                , new FindAndReplace.IgnoreRule() { ReadPrevCharsFromMatch = 1, ReadNextCharsFromMatch = 1, IgnoreIfEqualsTo = ".L." }
                , new FindAndReplace.IgnoreRule() { ReadNextCharsFromMatch = 3, IgnoreIfEqualsTo = "L.A." }
                , new FindAndReplace.IgnoreRule() { ReadNextCharsFromMatch = 6, IgnoreIfEqualsTo = "L'chaim" }
            )

            ,new FindAndReplace(new Regex(@"\b(?<OCR>L)'m\b", RegexOptions.Compiled), "OCR", "I", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?<OCR>L)'d\b", RegexOptions.Compiled), "OCR", "I", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?<OCR>L)t's\b", RegexOptions.Compiled), "OCR", "I", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?<OCR>L)ts\b", RegexOptions.Compiled), "OCR", "I", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?<OCR>L)n\b", RegexOptions.Compiled), "OCR", "I", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?<OCR>L)s\b", RegexOptions.Compiled), "OCR", "I", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?<OCR>L)f\b", RegexOptions.Compiled), "OCR", "I", SubtitleError.OCR_Error)
        };

        #endregion

        #region OCR Error - Other

        public static readonly FindAndReplace[] OCRError_Other = new FindAndReplace[] {

			// Merged Words
            new FindAndReplace(new Regex(@"\b(?<OCR>ofthe)\b", RegexOptions.Compiled), "OCR", "of the", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?<OCR>onthe)\b", RegexOptions.Compiled), "OCR", "on the", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?<OCR>fora)\b", RegexOptions.Compiled), "OCR", "for a", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?<OCR>numberi)\b", RegexOptions.Compiled), "OCR", "number one", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:time)(?<OCR>to)\b", RegexOptions.Compiled), "OCR", " to", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:don't)(?<OCR>do)\b", RegexOptions.Compiled), "OCR", " do", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:don't)(?<OCR>just)\b", RegexOptions.Compiled), "OCR", " just", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:for)(?<OCR>just)\b", RegexOptions.Compiled), "OCR", " just", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:after)(?<OCR>just)\b", RegexOptions.Compiled), "OCR", " just", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:off)(?<OCR>too)\b", RegexOptions.Compiled), "OCR", " too", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:off)(?<OCR>first)\b", RegexOptions.Compiled), "OCR", " first", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:if)(?<OCR>this)\b", RegexOptions.Compiled), "OCR", " this", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:if)(?<OCR>they)\b", RegexOptions.Compiled), "OCR", " they", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:of)(?<OCR>this)\b", RegexOptions.Compiled), "OCR", " this", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:that)(?<OCR>jerk)\b", RegexOptions.Compiled), "OCR", " jerk", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:this)(?<OCR>jerk)\b", RegexOptions.Compiled), "OCR", " jerk", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:of)(?<OCR>them)\b", RegexOptions.Compiled), "OCR", " them", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:of)(?<OCR>thing)\b", RegexOptions.Compiled), "OCR", " thing", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:of)(?<OCR>things)\b", RegexOptions.Compiled), "OCR", " things", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:of)(?<OCR>too)\b", RegexOptions.Compiled), "OCR", " too", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:if)(?<OCR>we)\b", RegexOptions.Compiled), "OCR", " we", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:if)(?<OCR>the)\b", RegexOptions.Compiled), "OCR", " the", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:if)(?<OCR>those)\b", RegexOptions.Compiled), "OCR", " those", SubtitleError.OCR_Error)

            // rn => m
            ,new FindAndReplace(new Regex(@"\b(?<OCR>Morn)\b", RegexOptions.Compiled), "OCR", "Mom", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?<OCR>morn)\b", RegexOptions.Compiled), "OCR", "mom", SubtitleError.OCR_Error)

            ,new FindAndReplace(new Regex(@"\b(?<OCR>FBl)\b", RegexOptions.Compiled), "OCR", "FBI", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?<OCR>F\.B\.l)\b", RegexOptions.Compiled), "OCR", "F.B.I", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?<OCR>SHIEID)\b", RegexOptions.Compiled), "OCR", "SHIELD", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?<OCR>S\.H\.I\.E\.I\.D)\b", RegexOptions.Compiled), "OCR", "S.H.I.E.L.D", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?<OCR>I.A.)\b", RegexOptions.Compiled), "OCR", "L.A.", SubtitleError.OCR_Error)

            ,new FindAndReplace(new Regex(@"\b(?:Mr|Mrs|Dr|St)(?<OCR>\s+)\b", RegexOptions.Compiled), "OCR", ". ", SubtitleError.OCR_Error)

			// Fix zero and capital 'o' ripping mistakes
			,new FindAndReplace(new Regex(@"[0-9](?<OCR>O)", RegexOptions.Compiled), "OCR", "0", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"[0-9](?<OCR>\.O)", RegexOptions.Compiled), "OCR", ".0", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"[0-9](?<OCR>,O)", RegexOptions.Compiled), "OCR", ",0", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"[A-Z](?<OCR>0)", RegexOptions.Compiled), "OCR", "O", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b(?<OCR>0)[A-ZÁ-Úa-zá-ú]", RegexOptions.Compiled), "OCR", "O", SubtitleError.OCR_Error)

            // 9: 55
			,new FindAndReplace(new Regex(@"[0-2]?\d(?<OCR>: )[0-5]\d", RegexOptions.Compiled), "OCR", ":", SubtitleError.OCR_Error)

            // "1 :", "2 :" ... "n :" to "n:"
			,new FindAndReplace(new Regex(@"\d(?<OCR> :)", RegexOptions.Compiled), "OCR", ":", SubtitleError.OCR_Error)

            // Spaces after aphostrophes, eg. "I' d" to "I'd", "I' LL" to "I'LL", "Hasn 't" and "Hasn' t", etc.
			,new FindAndReplace(new Regex(@"(?i)[A-ZÁ-Úa-zá-ú](?<OCR>'\s|\s')(?:ll|ve|s|m|d|t|re)\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.OCR_Error)

			// Gun Calibre
            // Derringer.22
            ,new FindAndReplace(new Regex(@"[A-ZÁ-Úa-zá-ú](?<OCR>\.)\d+\b", RegexOptions.Compiled), "OCR", " .", SubtitleError.OCR_Error)

            // Smart space after dot(s)
            // Add space after a single dot
            ,new FindAndReplace(new Regex(@"[a-zá-úñä-ü](?<OCR>\.)[^(\s\n\'\.\?\!<"")\,]", RegexOptions.Compiled), "OCR", ". ", SubtitleError.OCR_Error,
                new FindAndReplace.IgnoreRule() { ReadPrevCharsFromMatch = 1, IgnoreIfEqualsTo = "Ph.D" }
                , new FindAndReplace.IgnoreRule() { ReadNextCharsFromMatch = 1, IgnoreIfCaseInsensitiveEqualsTo = "a.m." }
                , new FindAndReplace.IgnoreRule() { ReadNextCharsFromMatch = 1, IgnoreIfCaseInsensitiveEqualsTo = "p.m." }
                , new FindAndReplace.IgnoreRule() { ReadPrevCharsFromMatch = 2, IgnoreIfCaseInsensitiveStartsWith = "www." }
                , new FindAndReplace.IgnoreRule() { ReadNextCharsFromMatch = 2, IgnoreIfCaseInsensitiveEndsWith = ".com" }
                , new FindAndReplace.IgnoreRule() { ReadNextCharsFromMatch = 2, IgnoreIfCaseInsensitiveEndsWith = "a.k.a" }
            )

            // a.m., p.m.
            ,new FindAndReplace(new Regex(@"(?<OCR>a\.\sm\.)", RegexOptions.Compiled), "OCR", "a.m.", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"(?<OCR>p\.\sm\.)", RegexOptions.Compiled), "OCR", "p.m.", SubtitleError.OCR_Error)

            // Remove space after two or more consecutive dots (e.g. "...") at the beginning of the line
            ,new FindAndReplace(new Regex(@"^(?:<i>)?\.{2,}(?<OCR>\s+)", RegexOptions.Compiled), "OCR", "", SubtitleError.OCR_Error)
            // Add space after comma
            ,new FindAndReplace(new Regex(@"(?<OCR>\,)[A-ZÁ-Úa-zá-ú]", RegexOptions.Compiled), "OCR", ", ", SubtitleError.OCR_Error)
            
            // x ...
            ,new FindAndReplace(new Regex(@"(?<OCR>\s+)\.{3}(?:</i>)?$", RegexOptions.Compiled), "OCR", "", SubtitleError.OCR_Error)
            // a ... a
            ,new FindAndReplace(new Regex(@"[A-ZÁ-Úa-zá-ú](?<OCR>\s+)\.{3}\s+[A-ZÁ-Úa-zá-ú]", RegexOptions.Compiled), "OCR", "", SubtitleError.OCR_Error)

            // 1, 000
            ,new FindAndReplace(new Regex(@"\b\d+(?<OCR>, | ,)0{3}\b", RegexOptions.Compiled), "OCR", ",", SubtitleError.OCR_Error)

            // /t => It
            ,new FindAndReplace(new Regex(@"(?<OCR>/t)", RegexOptions.Compiled), "OCR", "It", SubtitleError.OCR_Error)

            // I-I-I, I-I
            //,new FindAndReplace(new Regex(@"(?<OCR>I- I- I)", RegexOptions.Compiled), "OCR", "I... I... I", SubtitleError.OCR_Error)
            //,new FindAndReplace(new Regex(@"(?<OCR>I-I-I)", RegexOptions.Compiled), "OCR", "I... I... I", SubtitleError.OCR_Error)
            //,new FindAndReplace(new Regex(@"(?<OCR>I- I)", RegexOptions.Compiled), "OCR", "I... I", SubtitleError.OCR_Error)
            //,new FindAndReplace(new Regex(@"(?<OCR>I-I)", RegexOptions.Compiled), "OCR", "I... I", SubtitleError.OCR_Error)

            // -</i> => ...</i>
            ,new FindAndReplace(new Regex(@"(?<OCR>\s*-\s*)</i>$", RegexOptions.Compiled), "OCR", "...", SubtitleError.OCR_Error)
            // Text - $ => Text...$
            // doesn't match un-fuckin-$reasonable
            ,new FindAndReplace(new Regex(@"(?<![A-ZÁ-Úa-zá-ú]+-[A-ZÁ-Úa-zá-ú]+)(?<OCR>\s*-\s*)$", RegexOptions.Compiled), "OCR", "...", SubtitleError.OCR_Error)
            // text - text => text... text
            ,new FindAndReplace(new Regex(@"[a-zá-ú](?<OCR> - )[a-zá-ú]", RegexOptions.Compiled), "OCR", "... ", SubtitleError.OCR_Error)

            // text - text => text... text
            ,new FindAndReplace(new Regex(@"[Ia-zá-ú](?<OCR>-)\s[A-ZÁ-Ú]", RegexOptions.Compiled), "OCR", "...", SubtitleError.OCR_Error)

            // Text..
            ,new FindAndReplace(new Regex(@"[A-ZÁ-Úa-zá-ú](?<OCR>\.{2})(?:\s|♪|</i>|$)", RegexOptions.Compiled), "OCR", "...", SubtitleError.OCR_Error)
            // ..Text
            ,new FindAndReplace(new Regex(@"(?:\s|♪|<i>|^)(?<OCR>\.{2})[A-ZÁ-Úa-zá-ú]", RegexOptions.Compiled), "OCR", "...", SubtitleError.OCR_Error)

            // from iterate post
            //,new FindAndReplace(new Regex(@"[A-ZÁ-Úa-zá-ú0-9](?:(?<OCR>\.{2,})[A-ZÁ-Úa-zá-ú0-9])+", RegexOptions.Compiled), "OCR", "... ", SubtitleError.OCR_Error)
            
            // Text . Next text
            ,new FindAndReplace(new Regex(@"[A-ZÁ-Úa-zá-ú](?<OCR>\s\.)\s[A-ZÁ-Úa-zá-ú]", RegexOptions.Compiled), "OCR", ". ", SubtitleError.OCR_Error)

            // I6 => 16
            ,new FindAndReplace(new Regex(@"\b(?<OCR>I)\d+\b", RegexOptions.Compiled), "OCR", "1", SubtitleError.OCR_Error)

            // ♪
            ,new FindAndReplace(new Regex(@"^(?<OCR>Jj)$", RegexOptions.Compiled | RegexOptions.IgnoreCase), "OCR", "♪", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"^(?<OCR>J['""&!]?|j['""&!]?)\s", RegexOptions.Compiled), "OCR", "♪", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\s(?<OCR>['""&!]?J|['""&!]?j)$", RegexOptions.Compiled), "OCR", "♪", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"^(?<OCR>['""&!]?J|['""&!]?j)\s", RegexOptions.Compiled), "OCR", "♪", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\s(?<OCR>J['""&!]?|j['""&!]?)$", RegexOptions.Compiled), "OCR", "♪", SubtitleError.OCR_Error)

            // ♪ Text &
            ,new FindAndReplace(new Regex(@"^[♪&].*?(?<OCR>&)$", RegexOptions.Compiled), "OCR", "♪", SubtitleError.OCR_Error)
            // & Text ♪
            ,new FindAndReplace(new Regex(@"^(?<OCR>&).*?[♪&]$", RegexOptions.Compiled), "OCR", "♪", SubtitleError.OCR_Error)

            // Ordinal Numbers
            ,new FindAndReplace(new Regex(@"\b\d*1(?<OCR>\s+)st\b", RegexOptions.Compiled), "OCR", "", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b\d*2(?<OCR>\s+)nd\b", RegexOptions.Compiled), "OCR", "", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b\d*3(?<OCR>\s+)rd\b", RegexOptions.Compiled), "OCR", "", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"\b\d*[4-9](?<OCR>\s+)th\b", RegexOptions.Compiled), "OCR", "", SubtitleError.OCR_Error)

            // 1 => I
            // 1 can
            ,new FindAndReplace(new Regex(@"\b(?<OCR>1)\s+(?:can|can't|did|didn't|do|don't|had|hadn't|am|ain't|will|won't|would|wouldn't)\b", RegexOptions.Compiled | RegexOptions.IgnoreCase), "OCR", "I", SubtitleError.OCR_Error)
            // can 1
            ,new FindAndReplace(new Regex(@"\b(?:can|can't|did|didn't|do|don't|had|hadn't|am|ain't|will|won't|would|wouldn't)\s+(?<OCR>1)\b", RegexOptions.Compiled | RegexOptions.IgnoreCase), "OCR", "I", SubtitleError.OCR_Error)
        };

        #endregion

        #region Find And Replace Rules

        public static readonly FindAndReplace[] FindAndReplaceRules =
            EmptyLine
            .Concat(NotSubtitle)
            .Concat(Punctuations)
            .Concat(RedundantItalics)
            .Concat(HearingImpairedFullLine)
            .Concat(ScreenPosition)
            .Concat(RedundantSpaces)
            .Concat(HearingImpaired)
            .Concat(MissingSpaces)
            .Concat(TrimSpaces)
            .Concat(OCRError_NonAnsiChars)
            .Concat(OCRError_MangledLetters)
            .Concat(OCRError_Contractions)
            .Concat(OCRError_AccentLetters)
            .Concat(OCRError_I_And_L)
            .Concat(OCRError_Other)
            .ToArray();

        #endregion

        private static string CleanLine(string line, FindAndReplace[] rules, bool cleanHICaseInsensitive, bool isPrint)
        {
            SubtitleError subtitleError = SubtitleError.None;
            return CleanLine(line, rules, cleanHICaseInsensitive, isPrint, ref subtitleError);
        }

        private static string CleanLine(string line, FindAndReplace[] rules, bool cleanHICaseInsensitive, bool isPrint, ref SubtitleError subtitleError)
        {
            if (string.IsNullOrEmpty(line))
            {
                subtitleError = SubtitleError.Empty_Line;
                return null;
            }

            int ruleCounter = 0;

            foreach (var rule in rules)
            {
                string newLine = rule.CleanLine(line, cleanHICaseInsensitive);

                if (line != newLine)
                {
                    if (isPrint)
                    {
                        Console.WriteLine(ruleCounter);
                        Console.WriteLine("OCR:    " + (cleanHICaseInsensitive ? rule.ToStringCI() : rule.ToString()));
                        Console.WriteLine("Before: " + line);
                        Console.WriteLine("After:  " + newLine);
                    }

                    line = newLine;
                    subtitleError |= rule.SubtitleError;

                    if (subtitleError.HasFlag(SubtitleError.Empty_Line))
                    {
                        subtitleError = SubtitleError.Empty_Line;
                        return null;
                    }
                    else if (subtitleError.HasFlag(SubtitleError.Not_Subtitle))
                    {
                        subtitleError = SubtitleError.Not_Subtitle;
                        return null;
                    }
                }
            }

            if (subtitleError.HasFlag(SubtitleError.Hearing_Impaired))
                subtitleError = SubtitleError.Hearing_Impaired;

            if (isPrint && ruleCounter > 0)
                Console.WriteLine();

            return line;
        }

        /************************************************************************/

        private static bool IsEmptyLine(string line, bool isPrint)
        {
            return string.IsNullOrEmpty(CleanLine(line, EmptyLine, false, isPrint));
        }

        private static bool IsNotSubtitle(string line, bool isPrint)
        {
            return string.IsNullOrEmpty(CleanLine(line, NotSubtitle, false, isPrint));
        }

        public static readonly Regex regexHIPrefix = new Regex(@"^(?<Prefix>(?:<i>)?-?\s*|-?\s*(?:<i>)?\s*)[" + HI_CHARS + @"]*[A-Z]+[" + HI_CHARS + @"]*:\s*(?<Subtitle>.*?)$", RegexOptions.Compiled);
        public static readonly Regex regexHIPrefixCI = new Regex(@"^(?<Prefix>(?:<i>)?-?\s*|-?\s*(?:<i>)?\s*)[" + HI_CHARS_CI + @"]*[A-Z]+[" + HI_CHARS_CI + @"]*:\s*(?<Subtitle>.*?)$", RegexOptions.Compiled);
        public static readonly Regex regexHIPrefixWithoutDialogDash = new Regex(regexHIPrefix.ToString().Replace("-?", string.Empty), RegexOptions.Compiled);
        public static readonly Regex regexHIPrefixWithoutDialogDashCI = new Regex(regexHIPrefixCI.ToString().Replace("-?", string.Empty), RegexOptions.Compiled);

        // start (: ^ <i>? ♪? ( anything except () $
        //   end ): ^ anything except () ) ♪? <i>? $
        public static readonly Regex regexHI1Start = new Regex(@"^(?:\s*<i>)?(?:\s*♪)?\s*\([^\(\)]*?$", RegexOptions.Compiled);
        public static readonly Regex regexHI1End = new Regex(@"^[^\(\)]*?\)\s*(?:\s*♪)?(?:\s*</i>)?$", RegexOptions.Compiled);
        public static readonly Regex regexHI2Start = new Regex(@"^(?:\s*<i>)?(?:\s*♪)?\s*\[[^\[\]]*?$", RegexOptions.Compiled);
        public static readonly Regex regexHI2End = new Regex(@"^[^\[\]]*?\]\s*(?:\s*♪)?(?:\s*</i>)?$", RegexOptions.Compiled);

        private static bool IsHearingImpairedMultipleLines_RoundBrackets(string line1, string line2)
        {
            return regexHI1Start.IsMatch(line1) && regexHI1End.IsMatch(line2);
        }

        private static bool IsHearingImpairedMultipleLines_SquareBrackets(string line1, string line2)
        {
            return regexHI2Start.IsMatch(line1) && regexHI2End.IsMatch(line2);
        }

        private static bool IsHearingImpairedMultipleLines(string line1, string line2)
        {
            return
                IsHearingImpairedMultipleLines_RoundBrackets(line1, line2) ||
                IsHearingImpairedMultipleLines_SquareBrackets(line1, line2);
        }

        private static bool IsRedundantItalics(string line1, string line2)
        {
            return (line1.EndsWith("</i>") && line2.StartsWith("<i>"));
        }

        #endregion

        #region Shift

        public static void Shift(this List<Subtitle> subtitles, string shiftTime, int? subtitleNumber = null)
        {
            subtitles.Shift(ParseShiftTime(shiftTime), subtitleNumber);
        }

        public static void Shift(this List<Subtitle> subtitles, TimeSpan span, int? subtitleNumber = null)
        {
            if (subtitles == null || subtitles.Count == 0)
                return;

            if (span == TimeSpan.Zero)
                return;

            if (subtitleNumber != null && subtitleNumber.Value >= 1)
            {
                foreach (var subtitle in subtitles.Skip(subtitleNumber.Value - 1))
                {
                    subtitle.Show += span;
                    subtitle.Hide += span;
                }
            }
            else
            {
                foreach (var subtitle in subtitles)
                {
                    subtitle.Show += span;
                    subtitle.Hide += span;
                }
            }
        }

        #endregion

        #region MoveTo

        public static void MoveTo(this List<Subtitle> subtitles, string showTime, int? subtitleNumber = 1)
        {
            subtitles.MoveTo(ParseShowTime(showTime), subtitleNumber);
        }

        public static void MoveTo(this List<Subtitle> subtitles, DateTime show, int? subtitleNumber = 1)
        {
            if (subtitles == null || subtitles.Count == 0)
                return;

            if (show == DateTime.MinValue)
                return;

            int index = (subtitleNumber ?? 1) - 1;
            if (0 <= index && index <= subtitles.Count - 1)
            {
                TimeSpan span = show - subtitles[index].Show;
                foreach (var subtitle in subtitles.Skip(index))
                {
                    subtitle.Show += span;
                    subtitle.Hide += span;
                }
            }
        }

        #endregion

        #region Adjust

        public static void Adjust(this List<Subtitle> subtitles, string showStart, string showEnd)
        {
            subtitles.Adjust(ParseShowTime(showStart), ParseShowTime(showEnd));
        }

        public static void Adjust(this List<Subtitle> subtitles, DateTime showStart, DateTime showEnd)
        {
            subtitles.Adjust(
                subtitles[0].Show,
                showStart,
                subtitles[subtitles.Count - 1].Show,
                showEnd
            );
        }

        public static void Adjust(this List<Subtitle> subtitles, DateTime x1Show, DateTime x2Show, DateTime y1Show, DateTime y2Show)
        {
            if (subtitles == null || subtitles.Count == 0)
                return;

            if (x1Show == DateTime.MinValue || x2Show == DateTime.MinValue || y1Show == DateTime.MinValue || y2Show == DateTime.MinValue)
                return;

            var x1 = x1Show.ToMilliseconds();
            var x2 = x2Show.ToMilliseconds();

            var y1 = y1Show.ToMilliseconds();
            var y2 = y2Show.ToMilliseconds();

            // y = v1 * x + v2
            // (x2,y2) = v1 * (x1,y1) + v2
            // x2 = v1*x1 + v2
            // y2 = v1*y1 + v2
            double v1 = 1.0 * (x2 - y2) / (x1 - y1);
            double v2 = x2 - (v1 * x1); // = y2 - (v1 * y1)

            foreach (Subtitle subtitle in subtitles)
            {
                subtitle.Show = new DateTime(1900, 1, 1).AddMilliseconds((v1 * subtitle.Show.ToMilliseconds()) + v2);
                subtitle.Hide = new DateTime(1900, 1, 1).AddMilliseconds((v1 * subtitle.Hide.ToMilliseconds()) + v2);
            }
        }

        #endregion

        #region Errors

        public static readonly Regex regexBrackets = new Regex(@"[\({\[~\]}\)]", RegexOptions.Compiled);
        public static readonly Regex regexAngleBracketLeft = new Regex(@"<(?!/?i>)", RegexOptions.Compiled);
        public static readonly Regex regexAngleBracketRight = new Regex(@"(?<!</?i)>", RegexOptions.Compiled);
        public static readonly Regex regexColonStartLine = new Regex(@"^[A-ZÁ-Úa-zá-ú0-9#\-'.]+:", RegexOptions.Compiled);
        // ^10:30
        public static readonly Regex regexColonStartLineExclude = new Regex(@"^\d{1,2}:\d{2}", RegexOptions.Compiled);
        public static readonly Regex regexColon = new Regex(@"[A-ZÁ-Úa-zá-ú0-9#\-'.]+:\s", RegexOptions.Compiled);
        // Course 1 can
        public static readonly Regex regexOneInsteadOfI = new Regex(@"[A-ZÁ-Úa-zá-ú]\s+(1)\s+[A-ZÁ-Úa-zá-ú]", RegexOptions.Compiled);
        // a/b
        public static readonly Regex regexSlash = new Regex(@"[A-ZÁ-Úa-zá-ú]/[A-ZÁ-Úa-zá-ú]", RegexOptions.Compiled);
        // " / " -> " I "
        public static readonly Regex regexSlashInsteadOfI = new Regex(@"\s+/\s+", RegexOptions.Compiled);
        // replace with new line
        public static readonly Regex regexMissingSpace = new Regex(@"[!?][A-ZÁ-Úa-zá-ú]", RegexOptions.Compiled);

        public static readonly Regex regexHIWithoutBracket = new Regex(@"^[A-ZÁ-Ú]+$", RegexOptions.Compiled);

        public static readonly Regex regexHIFullLineWithoutBrackets = new Regex(@"^[" + HI_CHARS + @"]+$", RegexOptions.Compiled);
        // A... I... OK. 100. 123.45.
        public static readonly Regex regexHIFullLineWithoutBracketsExclude = new Regex(@"^(-\s)?(A[A. ]*|I[I. ]*|OK|O\.K\.|L\.A\.|F\.B\.I\.|\d+(\.\d+)+|\d+(-\d+)+|\d+)\.*$", RegexOptions.Compiled);

        public static readonly Regex regexDoubleQuateAndQuestionMark = new Regex(@"(?<!""[A-ZÁ-Úa-zá-ú0-9 #\-'.]+)(""\?)(\s|$)", RegexOptions.Compiled);

        //public static readonly Regex regexSpeachStartsWithLowerLetter = new Regex(@"^-\s+[a-zá-ú]", RegexOptions.Compiled);

        public static readonly Regex regexDuplicateOpenItalic = new Regex(@"<i><i>", RegexOptions.Compiled);
        public static readonly Regex regexDuplicateCloseItalic = new Regex(@"</i></i>", RegexOptions.Compiled);

        public static bool HasErrors(this Subtitle subtitle)
        {
            return subtitle.Lines.Any(HasErrors);
        }

        public static bool HasErrors(string line)
        {
            return
                regexBrackets.IsMatch(line) ||
                regexAngleBracketLeft.IsMatch(line) ||
                regexAngleBracketRight.IsMatch(line) ||
                (regexColonStartLine.IsMatch(line) && regexColonStartLineExclude.IsMatch(line) == false) ||
                regexColon.IsMatch(line) ||
                regexOneInsteadOfI.IsMatch(line) ||
                regexSlash.IsMatch(line) ||
                regexSlashInsteadOfI.IsMatch(line) ||
                regexMissingSpace.IsMatch(line) ||
                regexHIWithoutBracket.IsMatch(line) ||
                (regexHIFullLineWithoutBrackets.IsMatch(line) && regexHIFullLineWithoutBracketsExclude.IsMatch(line) == false) ||
                regexDoubleQuateAndQuestionMark.IsMatch(line) ||
                //regexSpeachStartsWithLowerLetter.IsMatch(line) ||
                regexDuplicateOpenItalic.IsMatch(line) ||
                regexDuplicateCloseItalic.IsMatch(line) ||
                (line.EndsWith("'?") && line.EndsWith("in'?") == false);
        }

        #endregion
    }
}
