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

        private static readonly Regex regexTime = new Regex(@"^" + fullTimeFormat + "$", RegexOptions.Compiled);
        private static readonly Regex regexSubtitleNumber = new Regex(@"^\d+$", RegexOptions.Compiled);

        private static readonly Regex regexShowTime = new Regex(@"^" + showTimeFormat + "$", RegexOptions.Compiled);
        private static readonly Regex regexShiftTime = new Regex(@"^(?<Shift_Sign>-|\+)?(?:(?:(?:(?<Shift_HH>\d{1,2}):)?(?<Shift_MM>\d{1,2}):)?(?<Shift_SS>\d{1,2})(?:,|:|\.))?(?<Shift_MS>\d{1,3})$", RegexOptions.Compiled);

        private const string showTimeFormatAlternate = @"(?:(?<Show_HH>\d{2}):)?(?<Show_MM>\d{2}):(?<Show_SS>\d{2})(?:[.,](?<Show_MS>\d{3}))?";
        private static readonly Regex regexShowTimeAlternate = new Regex(@"^" + showTimeFormatAlternate + "$", RegexOptions.Compiled);

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
        private static readonly Regex regexAccentedCharacters = new Regex(@"[á-úÁ-Ú]", RegexOptions.Compiled);

        public static bool HasAccentedCharacters(string filePath)
        {
            return regexAccentedCharacters.IsMatch(File.ReadAllText(filePath, Windows1252));
        }*/

        public static List<Subtitle> GetSubtitles(string filePath)
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

        public static List<Subtitle> CleanSubtitles(this List<Subtitle> subtitles)
        {
            return IterateSubtitles(
                IterateSubtitlesPost(
                IterateSubtitlesOCR(
                IterateSubtitles(
                IterateSubtitlesOCR(
                IterateSubtitles(
                IterateSubtitlesPre(subtitles)))))));
        }

        private static List<Subtitle> IterateSubtitlesPre(List<Subtitle> subtitles)
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

                    if (IsEmptyLine(line))
                    {
                        subtitle.Lines.RemoveAt(i);
                    }
                    else if (IsNotSubtitle(line))
                    {
                        subtitle.Lines = null;
                        break;
                    }
                }

                subtitle.Lines = CleanSubtitleLinesPre(subtitle.Lines);

                if (subtitle.Lines == null || subtitle.Lines.Count == 0)
                    subtitles.RemoveAt(k);
            }

            return subtitles;
        }

        private static List<Subtitle> IterateSubtitles(List<Subtitle> subtitles)
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

                    if (IsEmptyLine(line))
                    {
                        subtitle.Lines.RemoveAt(i);
                    }
                    else if (IsNotSubtitle(line))
                    {
                        subtitle.Lines = null;
                        break;
                    }
                    else
                    {
                        string cleanLine = (CleanSubtitleLine(line) ?? string.Empty).Trim();

                        if (string.IsNullOrEmpty(cleanLine))
                            subtitle.Lines.RemoveAt(i);
                        else
                            subtitle.Lines[i] = cleanLine;
                    }
                }

                subtitle.Lines = CleanSubtitleLines(subtitle.Lines);

                if (subtitle.Lines == null || subtitle.Lines.Count == 0)
                    subtitles.RemoveAt(k);
            }

            return subtitles;
        }

        private static List<Subtitle> IterateSubtitlesOCR(List<Subtitle> subtitles)
        {
            foreach (var subtitle in subtitles)
            {
                for (int i = 0; i < subtitle.Lines.Count; i++)
                    subtitle.Lines[i] = CleanSubtitleOCR(subtitle.Lines[i]);
            }

            return subtitles;
        }

        private static List<Subtitle> IterateSubtitlesPost(List<Subtitle> subtitles)
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
                    string cleanLine = (CleanSubtitleLinePost(subtitle.Lines[i]) ?? string.Empty).Trim();

                    if (string.IsNullOrEmpty(cleanLine))
                        subtitle.Lines.RemoveAt(i);
                    else
                        subtitle.Lines[i] = cleanLine;
                }

                subtitle.Lines = CleanSubtitleLinesPost(subtitle.Lines);
            }

            return subtitles;
        }

        public static List<Subtitle> CleanHICaseInsensitive(this List<Subtitle> subtitles)
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

                    if (string.IsNullOrEmpty(line) == false)
                    {
                        string cleanLine = (CleanHearingImpairedCaseInsensitive(line) ?? string.Empty).Trim();

                        if (string.IsNullOrEmpty(cleanLine))
                            subtitle.Lines.RemoveAt(i);
                        else
                            subtitle.Lines[i] = cleanLine;
                    }
                }

                if (subtitle.Lines == null || subtitle.Lines.Count == 0)
                    subtitles.RemoveAt(k);
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

                    if (IsEmptyLine(line))
                    {
                        subtitle.Lines.RemoveAt(i);
                    }
                    else if (IsNotSubtitle(line))
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

        public static void CheckSubtitles(this List<Subtitle> subtitles)
        {
            if (subtitles == null)
                return;

            if (subtitles.Count == 0)
                return;

            foreach (Subtitle subtitle in subtitles)
                CheckSubtitle(subtitle);
        }

        public static void CheckSubtitle(this Subtitle subtitle)
        {
            subtitle.SubtitleError = SubtitleError.None;

            foreach (string line in subtitle.Lines)
            {
                subtitle.SubtitleError |= CheckSubtitleLine(line);
                subtitle.SubtitleError |= CheckSubtitleLinePost(line);
            }

            subtitle.SubtitleError |= CheckSubtitleLinesPre(subtitle.Lines);
            subtitle.SubtitleError |= CheckSubtitleLines(subtitle.Lines);
            subtitle.SubtitleError |= CheckSubtitleLinesPost(subtitle.Lines);

            foreach (string line in subtitle.Lines)
                subtitle.SubtitleError |= CheckSubtitleOCR(line);

            if ((subtitle.SubtitleError & SubtitleError.Not_Subtitle) == SubtitleError.Not_Subtitle)
                subtitle.SubtitleError = SubtitleError.Not_Subtitle;
        }

        #region Clean Single Line

        private static string CleanSubtitleLine(string line)
        {
            line = CleanPunctuations(line);

            if (IsEighthNotes(line))
                return null;

            if (IsHearingImpairedFullLine(line))
                return null;

            line = CleanScreenPosition(line);

            line = CleanItalics(line);

            line = CleanOnes(line);

            line = CleanHearingImpaired(line);

            line = CleanMissingSpaces(line);

            line = CleanSpaces(line);

            return line;
        }

        private static SubtitleError CheckSubtitleLine(string line)
        {
            if (IsEmptyLine(line))
                return SubtitleError.Empty_Line;

            if (IsNotSubtitle(line))
                return SubtitleError.Not_Subtitle;

            SubtitleError subtitleError = SubtitleError.None;

            if (line != CleanPunctuations(line))
                subtitleError |= SubtitleError.Punctuation_Error;

            if (IsEighthNotes(line))
                return SubtitleError.Empty_Line;

            if (IsHearingImpairedFullLine(line))
                return SubtitleError.Hearing_Impaired;

            if (line != CleanScreenPosition(line))
                subtitleError |= SubtitleError.Screen_Position;

            if (line != CleanItalics(line))
                subtitleError |= SubtitleError.Redundant_Spaces;

            if (line != CleanOnes(line))
                subtitleError |= SubtitleError.Redundant_Spaces;

            if (line != CleanHearingImpaired(line))
                subtitleError |= SubtitleError.Hearing_Impaired;

            if (line != CleanMissingSpaces(line))
                subtitleError |= SubtitleError.Missing_Spaces;

            if (line != CleanSpaces(line))
                subtitleError |= SubtitleError.Redundant_Spaces;

            return subtitleError;
        }

        #endregion

        #region Clean Multiple Lines

        private static readonly Regex regexCapitalLetter = new Regex(@"[A-ZÁ-Ú]", RegexOptions.Compiled);
        private static readonly Regex regexDialog = new Regex(@"^(?<Italic>\<i\>)?-\s*(?<Subtitle>.*?)$", RegexOptions.Compiled);
        private static readonly Regex regexContainsDialog = new Regex(@" - [A-ZÁ-Ú]", RegexOptions.Compiled);

        private static List<string> CleanSubtitleLines(List<string> lines)
        {
            if (lines == null || lines.Count == 0)
                return null;

            if (lines.Count > 1)
            {
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
            }

            if (lines.Count == 1)
            {
                if (regexDialog.IsMatch(lines[0]))
                {
                    Match match = regexDialog.Match(lines[0]);
                    lines[0] = match.Groups["Italic"].Value + match.Groups["Subtitle"].Value;
                }

                if (regexHIPrefix.IsMatch(lines[0]))
                {
                    Match match = regexHIPrefix.Match(lines[0]);
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
                    isMatchHIPrefix = regexHIPrefix.IsMatch(line)
                }).ToArray();

                if (resultsHIPrefix[0].isMatchHIPrefix && resultsHIPrefix.Skip(1).All(x => x.isMatchHIPrefix == false))
                {
                    Match match = regexHIPrefix.Match(lines[0]);
                    lines[0] = match.Groups["Subtitle"].Value;
                }
                else if (resultsHIPrefix.Count(x => x.isMatchHIPrefix) > 1)
                {
                    foreach (var item in resultsHIPrefix)
                    {
                        if (item.isMatchHIPrefix)
                        {
                            Match match = regexHIPrefix.Match(lines[item.index]);
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
                    isEndsWithDots = line.EndsWith("...")
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
                            resultsDialog[1].isStartsWithI)
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
                }
            }

            return lines;
        }

        private static SubtitleError CheckSubtitleLines(List<string> lines)
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

                if (regexHIPrefix.IsMatch(lines[0]))
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
                    isMatchHIPrefix = regexHIPrefix.IsMatch(line)
                }).ToArray();

                if (resultsHIPrefix[0].isMatchHIPrefix && resultsHIPrefix.Skip(1).All(x => x.isMatchHIPrefix == false))
                {
                    Match match = regexHIPrefix.Match(lines[0]);
                    if (lines[0] != match.Groups["Subtitle"].Value)
                        subtitleError |= SubtitleError.Hearing_Impaired;
                }
                else if (resultsHIPrefix.Count(x => x.isMatchHIPrefix) > 1)
                {
                    foreach (var item in resultsHIPrefix)
                    {
                        if (item.isMatchHIPrefix)
                        {
                            Match match = regexHIPrefix.Match(lines[item.index]);
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
                    isEndsWithDots = line.EndsWith("...")
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
                            resultsDialog[1].isStartsWithI)
                        {
                            // don't do anything
                        }
                        else if (resultsDialog[0].isMatchDialog &&
                            resultsDialog[0].isContainsDialog_CapitalLetter &&
                            resultsDialog[0].isEndsWithDots)
                        {
                            // don't do anything
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

        #region Clean Multiple Lines Pre

        private static List<string> CleanSubtitleLinesPre(List<string> lines)
        {
            if (lines == null || lines.Count == 0)
                return null;

            if (lines.Count > 1)
            {
                var resultsHIPrefix = lines.Select((line, index) => new
                {
                    line,
                    index,
                    isMatchHIPrefix = regexHIPrefixWithoutDialogDash.IsMatch(line)
                }).ToArray();

                if (resultsHIPrefix.Count(x => x.isMatchHIPrefix) > 1)
                {
                    foreach (var item in resultsHIPrefix)
                    {
                        if (item.isMatchHIPrefix)
                        {
                            Match match = regexHIPrefixWithoutDialogDash.Match(lines[item.index]);
                            lines[item.index] = (match.Groups["Prefix"].Value + " - " + match.Groups["Subtitle"].Value).Trim();
                        }
                    }
                }
            }

            return lines;
        }

        private static SubtitleError CheckSubtitleLinesPre(List<string> lines)
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
                    isMatchHIPrefix = regexHIPrefixWithoutDialogDash.IsMatch(line)
                }).ToArray();

                if (resultsHIPrefix.Count(x => x.isMatchHIPrefix) > 1)
                {
                    subtitleError |= SubtitleError.Hearing_Impaired;
                }
            }

            return subtitleError;
        }

        #endregion

        #region Clean Single Line Post

        private static string CleanSubtitleLinePost(string line)
        {
            if (string.IsNullOrEmpty(line))
                return line;

            line = CleanDash(line);

            line = CleanDots(line);

            return line;
        }

        private static SubtitleError CheckSubtitleLinePost(string line)
        {
            if (IsEmptyLine(line))
                return SubtitleError.Empty_Line;

            SubtitleError subtitleError = SubtitleError.None;

            if (line != CleanDash(line))
                subtitleError |= SubtitleError.Punctuation_Error;

            if (line != CleanDots(line))
                subtitleError |= SubtitleError.Punctuation_Error;

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

            return lines;
        }

        private static SubtitleError CheckSubtitleLinesPost(List<string> lines)
        {
            if (lines == null || lines.Count == 0)
                return SubtitleError.None;

            SubtitleError subtitleError = SubtitleError.None;

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

            return subtitleError;
        }

        #endregion

        #region Clean & Validation

        private static bool IsEmptyLine(string line)
        {
            return
                string.IsNullOrEmpty(line) ||
                line == "-" ||
                line == "!" ||
                line == "?" ||
                line == ":" ||
                line == "_" ||
                line == "#" ||
                line == "##" ||
                line == "♪" ||
                line == "♪♪" ||
                line == "♪ ♪" ||
                line == "♪  ♪" ||
                line == "♪ - ♪" ||
                line == "- ♪" ||
                line == "- ♪♪" ||
                line == "<i>♪</i>" ||
                line == "<i>♪♪</i>";
        }

        private static bool IsNotSubtitle(string line)
        {
            return
                line.ContainsCI("subtitles by") ||
                line.ContainsCI("Subtitled By") ||
                line.ContainsCI("Subtitles:") ||
                line.ContainsCI("Subtitling") ||
                line.ContainsCI("SUBTITLES EDITED BY") ||
                line.ContainsCI("sync by") ||
                line.ContainsCI("OpenSubtitles") ||
                line.ContainsCI("www.AllSubs.org") ||
                line.ContainsCI("thepiratebay") ||
                line.ContainsCI("Best watched using Open Subtitles MKV Player") ||
                line.ContainsCI("ENGLISH SDH") ||
                line.ContainsCI("Closed-Caption") ||
                line.ContainsCI("Closed Caption") ||
                line.ContainsCI("Captions, Inc.") ||
                line.ContainsCI("Captions by") ||
                line.ContainsCI("Captioned by") ||
                line.ContainsCI("Captioning by") ||
                line.ContainsCI("Captioning made possible by") ||
                line.ContainsCI("Captions made possible by") ||
                line.ContainsCI("Captioning performed by") ||
                line.ContainsCI("Captions performed by") ||
                line.ContainsCI("Captioning sponsored by") ||
                line.ContainsCI("Captions sponsored by") ||
                line.ContainsCI("DVDRIP by") ||
                line.ContainsCI("Translated by") ||
                line.ContainsCI("Translation by") ||
                line.ContainsCI("Translations by") ||
                line.ContainsCI("Synced & corrected") ||
                line.ContainsCI("Copyright Australian") ||

                line.Contains("DIRECTED BY") ||
                line.Contains("WRITTEN BY") ||
                line.Contains("PRODUCED BY") ||
                line.Contains("Proofread by") ||
                line.Contains(@"\fad") ||
                line.Contains(@"\move") ||

                line.Contains("<font color=") ||
                (
                    (line.EndsWith("</font>") || line.EndsWith("</ font>")) && (
                        line.StartsWith("<font>") ||
                        line.StartsWith("<font ") ||
                        line.StartsWith("- <font>") ||
                        line.StartsWith("- <font ")
                    )
                );
        }

        private static readonly Regex regexX22 = new Regex(@"\x22{2,}", RegexOptions.Compiled);
        private static readonly Regex regexQuotes = new Regex(@"""{2,}", RegexOptions.Compiled);
        private static readonly Regex regexRedundantDot = new Regex(@"[?!:](?<Dot>\.)(\s|\b|$)", RegexOptions.Compiled);
        private static readonly Regex regexCommas = new Regex(@",{2,}", RegexOptions.Compiled);
        private static readonly Regex regexDots = new Regex(@"\.{4,}", RegexOptions.Compiled);
        private static readonly Regex regexNumberSign = new Regex(@"#(?![0-9])", RegexOptions.Compiled);
        private static readonly Regex regexMultipleEighthNotes = new Regex(@"♪{2,}", RegexOptions.Compiled);

        private static string CleanPunctuations(string line)
        {
            return line
                .Replace("`e", "é")
                .Replace("'’", "'").Replace("´", "'").Replace("`", "'").Replace("‘", "'").Replace("’", "'")
                .Replace("“", "\"").Replace("”", "\"").Replace(regexX22, "\"").Replace(@"\x22", "\"").Replace("''", "\"").Replace(regexQuotes, "\"")
                .Replace(regexRedundantDot, "Dot", string.Empty)
                .Replace(" ?", "?").Replace(" !", "!").Replace(" :", ":")
                .Replace("…", "...").Replace("--", "...").Replace(". ..", "...").Replace(".. .", "...").Replace(". . .", "...").Replace(regexCommas, "...").Replace(regexDots, "...")
                .Replace("—", "...").Replace("–", "...").Replace("―", "...").Replace("‒", "...")
                .Replace("‐", "-")
                .Replace("- -", "-")
                .Replace(";", ",")
                .Replace("♫", "♪").Replace("¶", "♪").Replace("*", "♪").Replace(regexNumberSign, "♪").Replace(regexMultipleEighthNotes, "♪");
        }

        private static readonly Regex regexEighthNotes = new Regex(@"^(?:♪|\s)+$", RegexOptions.Compiled);

        private static bool IsEighthNotes(string line)
        {
            return regexEighthNotes.IsMatch(line);
        }

        private const string HIChars = @"A-ZÁ-Ú0-9 #\-'.";
        private static readonly Regex regexHIFullLine1 = new Regex(@"^(?:-\s*)?(?:<i>\s*)?\(.*?\)(?:\s*</i>)?$", RegexOptions.Compiled);
        private static readonly Regex regexHIFullLine2 = new Regex(@"^(?:-\s*)?(?:<i>\s*)?\[.*?\](?:\s*</i>)?$", RegexOptions.Compiled);
        private static readonly Regex regexHIFullLine3 = new Regex(@"^[" + HIChars + @"\[\]]+\:\s*$", RegexOptions.Compiled);
        private static readonly Regex regexHIFullLine4 = new Regex(@"^[" + HIChars + @"]+\[.*?\]\:\s*$", RegexOptions.Compiled);

        private static bool IsHearingImpairedFullLine(string line)
        {
            return
                regexHIFullLine1.IsMatch(line) ||
                regexHIFullLine2.IsMatch(line) ||
                regexHIFullLine3.IsMatch(line) ||
                regexHIFullLine4.IsMatch(line);
        }

        private static readonly Regex regexScreenPosition = new Regex(@"{\\an\d*}", RegexOptions.Compiled);

        private static string CleanScreenPosition(string line)
        {
            return line.Replace(regexScreenPosition, string.Empty);
        }

        private static readonly Regex regexEmptyItalics = new Regex(@"<i>\s*</i>", RegexOptions.Compiled);
        private static readonly Regex regexItalicsAndHI = new Regex(@"<i>\-\s+</i>", RegexOptions.Compiled);
        private static readonly Regex regexItalic1 = new Regex(@"(?<Prefix>[^ ])<i>[ ]", RegexOptions.Compiled);
        private static readonly Regex regexItalic2 = new Regex(@"[ ]</i>(?<Suffix>[^ ])", RegexOptions.Compiled);
        private static readonly Regex regexItalic3 = new Regex(@"\.<i>\s+", RegexOptions.Compiled);
        private static readonly Regex regexItalic4 = new Regex(@",<i>\s+", RegexOptions.Compiled);
        private static readonly Regex regexItalic5 = new Regex(@"<i>\s+", RegexOptions.Compiled);
        private static readonly Regex regexItalic6 = new Regex(@"\s+</i>", RegexOptions.Compiled);

        private static string CleanItalics(string line)
        {
            if (regexItalic1.IsMatch(line))
            {
                Match match = regexItalic1.Match(line);
                line = line.Remove(match.Index, match.Length).Insert(match.Index, match.Groups["Prefix"] + " <i>");
            }

            if (regexItalic2.IsMatch(line))
            {
                Match match = regexItalic2.Match(line);
                line = line.Remove(match.Index, match.Length).Insert(match.Index, "</i> " + match.Groups["Suffix"]);
            }

            return line
                .Replace("<i/>", "</i>").Replace("</ i>", "</i>")
                .Replace(regexEmptyItalics, string.Empty)
                .Replace(regexItalicsAndHI, "- ")
                .Replace(regexItalic3, ". <i>")
                .Replace(regexItalic4, ", <i>")
                .Replace(regexItalic5, "<i>")
                .Replace(regexItalic6, "</i>");
        }

        private static readonly Regex regexOne = new Regex(@"1\s+(?=[0-9.,/])(?!1/2|1 /2)", RegexOptions.Compiled);

        private static string CleanOnes(string line)
        {
            return line.Replace(regexOne, "1");
        }

        private static readonly Regex regexHI1 = new Regex(@"^(?<Prefix>- )?\(.*?\)(\:\s*)?(?<Subtitle>.+)$", RegexOptions.Compiled);
        private static readonly Regex regexHI2 = new Regex(@"^(?<Prefix>- )?\[.*?\](\:\s*)?(?<Subtitle>.+)$", RegexOptions.Compiled);
        private static readonly Regex regexHI3 = new Regex(@"^(?<Subtitle>.+?)\(.*?\)$", RegexOptions.Compiled);
        private static readonly Regex regexHI4 = new Regex(@"^(?<Subtitle>.+?)\[.*?\]$", RegexOptions.Compiled);
        private static readonly Regex regexHI5 = new Regex(@"^[0-9A-Z #\-'\[\]]*[A-Z#'\[\]][0-9A-Z #\-'\[\]]*\:\s*(?<Subtitle>.+?)$", RegexOptions.Compiled);
        private static readonly Regex regexHI6 = new Regex(@"\s+\(.*?\)\s+", RegexOptions.Compiled);
        private static readonly Regex regexHI7 = new Regex(@"\s+\[.*?\]\s+", RegexOptions.Compiled);
        private static readonly Regex regexHI8 = new Regex(@"(?:<i>\s*)\(.*?\)(?:\s*</i>)", RegexOptions.Compiled);
        private static readonly Regex regexHI9 = new Regex(@"(?:<i>\s*)\[.*?\](?:\s*</i>)", RegexOptions.Compiled);
        private static readonly Regex regexHIPrefix = new Regex(@"^(?<Prefix>(?:\<i\>)?-?\s*|-?\s*(?:\<i\>)?\s*)[" + HIChars + @"]*[A-Z]+[" + HIChars + @"]*:\s*(?<Subtitle>.*?)$", RegexOptions.Compiled);
        private static readonly Regex regexHIPrefix_Dash = new Regex(@"^(?:\s*<i>)?\s*-\s*:\s*", RegexOptions.Compiled);
        private static readonly Regex regexHIPrefix_Colon = new Regex(@"^(?:\s*<i>)?\s*:\s*", RegexOptions.Compiled);
        private static readonly Regex regexHIPrefixWithoutDialogDash = new Regex(regexHIPrefix.ToString().Replace("-?", string.Empty), RegexOptions.Compiled);

        private static string CleanHearingImpaired(string line)
        {
            if (regexHI1.IsMatch(line))
            {
                Match match = regexHI1.Match(line);
                line = match.Groups["Prefix"].Value + match.Groups["Subtitle"].Value;
            }

            if (regexHI2.IsMatch(line))
            {
                Match match = regexHI2.Match(line);
                line = match.Groups["Prefix"].Value + match.Groups["Subtitle"].Value;
            }

            if (regexHI3.IsMatch(line))
            {
                Match match = regexHI3.Match(line);
                line = match.Groups["Subtitle"].Value;
            }

            if (regexHI4.IsMatch(line))
            {
                Match match = regexHI4.Match(line);
                line = match.Groups["Subtitle"].Value;
            }

            if (regexHI5.IsMatch(line))
            {
                Match match = regexHI5.Match(line);
                line = match.Groups["Subtitle"].Value;
            }

            line = line
                .Replace(regexHI6, " ").Replace(regexHI7, " ")
                .Replace(regexHI8, string.Empty).Replace(regexHI9, string.Empty);

            if (regexHIPrefix.IsMatch(line))
            {
                Match match = regexHIPrefix.Match(line);
                line = match.Groups["Prefix"].Value + match.Groups["Subtitle"].Value;
            }

            line = line
                .Replace(regexHIPrefix_Dash, string.Empty)
                .Replace(regexHIPrefix_Colon, string.Empty);

            return line;
        }

        private static readonly Regex regexHICaseInsensitive1 = new Regex(@"\b[A-ZÁ-Úa-zá-ú0-9 #\-']+:\s*", RegexOptions.Compiled);
        private static readonly Regex regexHICaseInsensitive2 = new Regex(@"\[.*?\]", RegexOptions.Compiled);
        private static readonly Regex regexHICaseInsensitive3 = new Regex(@"\(.*?\)", RegexOptions.Compiled);

        private static string CleanHearingImpairedCaseInsensitive(string line)
        {
            line = regexHICaseInsensitive1.Replace(line, string.Empty);
            line = regexHICaseInsensitive2.Replace(line, string.Empty);
            line = regexHICaseInsensitive3.Replace(line, string.Empty);
            return line;
        }

        private static readonly Regex regexMissingSpaces1 = new Regex(@"^(?<Prefix>♪+)[^ ♪]", RegexOptions.Compiled);
        private static readonly Regex regexMissingSpaces2 = new Regex(@"[^ ♪](?<Suffix>♪+)$", RegexOptions.Compiled);
        private static readonly Regex regexMissingSpaces3 = new Regex(@"\s+(?<Dash>-)[A-ZÁ-Úa-zá-ú]", RegexOptions.Compiled);

        private static string CleanMissingSpaces(string line)
        {
            if (regexMissingSpaces1.IsMatch(line))
            {
                Match match = regexMissingSpaces1.Match(line);
                line = line.Insert(match.Groups["Prefix"].Index + match.Groups["Prefix"].Length, " ");
            }

            if (regexMissingSpaces2.IsMatch(line))
            {
                Match match = regexMissingSpaces2.Match(line);
                line = line.Insert(match.Groups["Suffix"].Index, " ");
            }

            if (regexMissingSpaces3.IsMatch(line))
            {
                Match match = regexMissingSpaces3.Match(line);
                line = line.Insert(match.Groups["Dash"].Index + match.Groups["Dash"].Length, " ");
            }

            return line;
        }

        private static readonly Regex regexSpaces = new Regex(@"\s{2,}", RegexOptions.Compiled);

        private static string CleanSpaces(string line)
        {
            return line
                .Replace(regexSpaces, " ")
                .Trim();
        }

        private static readonly Regex regexHI1Start = new Regex(@"^\([^\(\)]*?$", RegexOptions.Compiled);
        private static readonly Regex regexHI1End = new Regex(@"^[^\(\)]*?\)$", RegexOptions.Compiled);
        private static readonly Regex regexHI2Start = new Regex(@"^\[[^\[\]]*?$", RegexOptions.Compiled);
        private static readonly Regex regexHI2End = new Regex(@"^[^\[\]]*?\]$", RegexOptions.Compiled);

        private static bool IsHearingImpairedMultipleLines(string line1, string line2)
        {
            return
                (regexHI1Start.IsMatch(line1) && regexHI1End.IsMatch(line2)) ||
                (regexHI2Start.IsMatch(line1) && regexHI2End.IsMatch(line2));
        }

        private static readonly Regex regexDash1 = new Regex(@"\s*-\s*</i>$", RegexOptions.Compiled);
        private static readonly Regex regexDash2 = new Regex(@"(?<![A-ZÁ-Úa-zá-ú]+-[A-ZÁ-Úa-zá-ú]+)\s*-\s*$", RegexOptions.Compiled); // doesn't match un-fuckin-$reasonable
        private static readonly Regex regexDash3 = new Regex(@"[a-zá-ú](?<Dash> - )[a-zá-ú]", RegexOptions.Compiled);

        private static string CleanDash(string line)
        {
            return line
                .Replace(regexDash1, "...</i>")
                .Replace(regexDash2, "...")
                .Replace(regexDash3, "Dash", "... ");
        }

        private static readonly Regex regexDotsWithNoSpace = new Regex(@"[A-ZÁ-Úa-zá-ú](?<Dots>\.{2,})[A-ZÁ-Úa-zá-ú]", RegexOptions.Compiled);

        private static string CleanDots(string line)
        {
            return line
                .Replace(regexDotsWithNoSpace, "Dots", "... ");
        }

        private static bool IsRedundantItalics(string line1, string line2)
        {
            return (line1.EndsWith("</i>") && line2.StartsWith("<i>"));
        }

        #endregion

        #region OCR Errors

        private static readonly bool IsPrintOCR = false;

        private static string CleanSubtitleOCR(string line)
        {
            foreach (var rule in ocrRules)
            {
                if (rule.Find.IsMatch(line))
                {
                    bool isIgnore = IsIgnore(line, rule);
                    if (isIgnore)
                        continue;

                    if (IsPrintOCR)
                    {
                        Console.WriteLine(rule);
                        Console.WriteLine(line);
                    }

                    Match match = rule.Find.Match(line);
                    Group group = match.Groups[1];
                    line = line.Remove(group.Index, group.Length).Insert(group.Index, rule.ReplaceBy).Trim();

                    if (IsPrintOCR)
                    {
                        Console.WriteLine(line);
                    }
                }
            }

            return line;
        }

        private static SubtitleError CheckSubtitleOCR(string line)
        {
            foreach (var rule in ocrRules)
            {
                if (rule.Find.IsMatch(line))
                {
                    bool isIgnore = IsIgnore(line, rule);
                    if (isIgnore)
                        continue;

                    return SubtitleError.OCR_Error;
                }
            }

            return SubtitleError.None;
        }

        private static bool IsIgnore(string line, OCRRule rule)
        {
            if (rule.IgnoreRules != null && rule.IgnoreRules.Count > 0)
            {
                foreach (var ignoreRule in rule.IgnoreRules)
                {
                    Match match = null;

                    if (ignoreRule.IgnoreFind != null)
                        match = ignoreRule.IgnoreFind.Match(line);
                    else
                        match = rule.Find.Match(line);

                    if (match.Success)
                    {
                        string ignoreValue = match.Value;
                        if (string.IsNullOrEmpty(ignoreValue) == false)
                        {
                            if (ignoreRule.StartsWith)
                            {
                                if (ignoreValue.StartsWith(ignoreRule.Ignore))
                                    return true;
                            }
                            else if (ignoreRule.EndsWith)
                            {
                                if (ignoreValue.EndsWith(ignoreRule.Ignore))
                                    return true;
                            }
                            else if (ignoreValue == ignoreRule.Ignore)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private static readonly OCRRule[] ocrRules = new OCRRule[] {
			// The most common OCR error - I (uppercase i) and l (lowercase L) mistakes

			// Roman numerals
			 new OCRRule() { Find = new Regex(@"\b[VXLCDM]*(lll)\b", RegexOptions.Compiled), ReplaceBy = "III" }
            ,new OCRRule() { Find = new Regex(@"[^.?!—–―‒-][^']\b[IVXLCDM]*(ll)I{0,1}\b", RegexOptions.Compiled), ReplaceBy = "II" }
            ,new OCRRule() { Find = new Regex(@"^(ll)\b", RegexOptions.Compiled), ReplaceBy = "II" }
            ,new OCRRule() { Find = new Regex(@"\b[IVXLCDM]*(l)[IVX]*\b", RegexOptions.Compiled), ReplaceBy = "I" }

			// Replace "II" with "ll" at the end of a lowercase word
			,new OCRRule() { Find = new Regex(@"[a-zá-ú](II)", RegexOptions.Compiled), ReplaceBy = "ll" }
			// Replace "II" with "ll" at the beginning of a lowercase word
			,new OCRRule() { Find = new Regex(@"(II)[a-zá-ú]", RegexOptions.Compiled), ReplaceBy = "ll" }
			// Replace "I" with "l" in the middle of a lowercase word
			,new OCRRule() { Find = new Regex(@"[a-zá-ú](I)[a-zá-ú]", RegexOptions.Compiled), ReplaceBy = "l" }
			// Replace "I" with "l" at the end of a lowercase word
			,new OCRRule() { Find = new Regex(@"[a-zá-ú](I)\b", RegexOptions.Compiled), ReplaceBy = "l" }
			// Replace "l" with "I" in the middle of an uppercase word
			,new OCRRule() { Find = new Regex(@"[A-ZÁ-Ú](l)[A-ZÁ-Ú]", RegexOptions.Compiled), ReplaceBy = "I" }
			// Replace "l" with "I" at the end of an uppercase word
			,new OCRRule() { Find = new Regex(@"[A-ZÁ-Ú]{2,}(l)", RegexOptions.Compiled), ReplaceBy = "I" }

			// Replace a single "l" with "I"
			,new OCRRule() { Find = new Regex(@"\b(l)\b", RegexOptions.Compiled), ReplaceBy = "I" }

			// Custom
            ,new OCRRule() { Find = new Regex(@"\b((?<!\<|/)i(?!\>))\b", RegexOptions.Compiled), ReplaceBy = "I" }

			// Replace "I'II"/"you'II" etc. with "I'll"/"you'll" etc.
			,new OCRRule() { Find = new Regex(@"[A-ZÁ-Úa-zá-ú]'(II)\b", RegexOptions.Compiled), ReplaceBy = "ll" }
			// Replace "I 'II" with "I'll" or "I' II" with "I'll" - rare cases with a space before or after the apostrophe
			,new OCRRule() { Find = new Regex(@"[A-ZÁ-Úa-zá-ú]('\sII|\s'II)\b", RegexOptions.Compiled), ReplaceBy = "'ll" }

			// Custom
            // Replace "AII" with "All"
			,new OCRRule() { Find = new Regex(@"A(II)\b", RegexOptions.Compiled), ReplaceBy = "ll" }

			// Custom
			,new OCRRule() { Find = new Regex(@"^(Iive)\b", RegexOptions.Compiled), ReplaceBy = "Live" }
            ,new OCRRule() { Find = new Regex(@"^(Iiving)\b", RegexOptions.Compiled), ReplaceBy = "Living" }
            ,new OCRRule() { Find = new Regex(@"\b(Iive)\b", RegexOptions.Compiled), ReplaceBy = "live" }
            ,new OCRRule() { Find = new Regex(@"\b(Iiving)\b", RegexOptions.Compiled), ReplaceBy = "living" }

            // Custom
			,new OCRRule() { Find = new Regex(@"\b(fianc'e)\b", RegexOptions.Compiled), ReplaceBy = "fiancé" }
            ,new OCRRule() { Find = new Regex(@"\b(Fianc'e)\b", RegexOptions.Compiled), ReplaceBy = "Fiancé" }
            ,new OCRRule() { Find = new Regex(@"\b(caf'e)\b", RegexOptions.Compiled), ReplaceBy = "café" }
            ,new OCRRule() { Find = new Regex(@"\b(Caf'e)\b", RegexOptions.Compiled), ReplaceBy = "Café" }
            
            // "I" at the beginning of a word before lowercase vowels is most likely an "l"
			,new OCRRule() { Find = new Regex(@"\b(I)[oaeiuyá-ú]", RegexOptions.Compiled), ReplaceBy = "l",
                IgnoreRules = new List<IgnoreRule>() {
                    new IgnoreRule() { IgnoreFind = new Regex(@"\b(I)[oaeiuyá-ú].{2}", RegexOptions.Compiled), Ignore="Iago" }
                }
            }
			// "I" after an uppercase letter at the beginning and before a lowercase letter is most likely an "l"
			,new OCRRule() { Find = new Regex(@"\b[A-ZÁ-Ú](I)[a-zá-ú]", RegexOptions.Compiled), ReplaceBy = "l" }
			// "l" at the beginning before a consonant different from "l" is most likely an "I"
			,new OCRRule() { Find = new Regex(@"\b(l)[^aeiouyàá-úl]", RegexOptions.Compiled), ReplaceBy = "I",
                IgnoreRules = new List<IgnoreRule>() {
                    new IgnoreRule() { IgnoreFind = new Regex(@"\b(l)[^aeiouyàá-úl].{1}", RegexOptions.Compiled), Ignore="lbs" }
                }
            }

			// Fixes for "I" at the beginning of the word before lowercase vowels
			// The name "Ian"
			,new OCRRule() { Find = new Regex(@"\b(lan)\b", RegexOptions.Compiled), ReplaceBy = "Ian" }
			// The name "Iowa"
			,new OCRRule() { Find = new Regex(@"\b(lowa)\b", RegexOptions.Compiled), ReplaceBy = "Iowa" }
			// The word "Ill"
			,new OCRRule() { Find = new Regex(@"[.?!-]\s?(III)\b", RegexOptions.Compiled), ReplaceBy = "Ill" }
            ,new OCRRule() { Find = new Regex(@"^(III)\b.", RegexOptions.Compiled), ReplaceBy = "Ill" }
			// The word "Ion" and its derivatives
			,new OCRRule() { Find = new Regex(@"\b(l)on\b.", RegexOptions.Compiled), ReplaceBy = "I" }
            ,new OCRRule() { Find = new Regex(@"\b(l)oni", RegexOptions.Compiled), ReplaceBy = "I" }
			// The word "Iodine" and its derivatives
			,new OCRRule() { Find = new Regex(@"\b(l)odi", RegexOptions.Compiled), ReplaceBy = "I" }
            ,new OCRRule() { Find = new Regex(@"\b(l)odo", RegexOptions.Compiled), ReplaceBy = "I" }

			// Custom
            ,new OCRRule() { Find = new Regex(@"\b(L)\b", RegexOptions.Compiled), ReplaceBy = "I",
                IgnoreRules = new List<IgnoreRule>() {
                    new IgnoreRule() { IgnoreFind = new Regex(@"\b(L)\.A\.", RegexOptions.Compiled), Ignore="L.A." },
                    new IgnoreRule() { IgnoreFind = new Regex(@"\b(L)'chaim", RegexOptions.Compiled), Ignore="L'chaim" }
                }
            }

			// Custom
            ,new OCRRule() { Find = new Regex(@"\b(L'm)\b", RegexOptions.Compiled), ReplaceBy = "I'm" }
            ,new OCRRule() { Find = new Regex(@"\b(L'd)\b", RegexOptions.Compiled), ReplaceBy = "I'd" }
            ,new OCRRule() { Find = new Regex(@"\b(Lt's)\b", RegexOptions.Compiled), ReplaceBy = "It's" }
            ,new OCRRule() { Find = new Regex(@"\b(Ln)\b", RegexOptions.Compiled), ReplaceBy = "In" }
            ,new OCRRule() { Find = new Regex(@"\b(Ls)\b", RegexOptions.Compiled), ReplaceBy = "Is" }
            ,new OCRRule() { Find = new Regex(@"\b(Lf)\b", RegexOptions.Compiled), ReplaceBy = "If" }
            ,new OCRRule() { Find = new Regex(@"\b(ofthe)\b", RegexOptions.Compiled), ReplaceBy = "of the" }

            // Custom
			,new OCRRule() { Find = new Regex(@"\b(FBl)\b", RegexOptions.Compiled), ReplaceBy = "FBI" }
            ,new OCRRule() { Find = new Regex(@"\b(F\.B\.l)\b", RegexOptions.Compiled), ReplaceBy = "F.B.I" }
            ,new OCRRule() { Find = new Regex(@"\b(SHIEID)\b", RegexOptions.Compiled), ReplaceBy = "SHIELD" }
            ,new OCRRule() { Find = new Regex(@"\b(S\.H\.I\.E\.I\.D)\b", RegexOptions.Compiled), ReplaceBy = "S.H.I.E.L.D" }
            ,new OCRRule() { Find = new Regex(@"\b(I.A.)\b", RegexOptions.Compiled), ReplaceBy = "L.A." }


			// Other OCR errors

			// Fix zero and capital 'o' ripping mistakes
			,new OCRRule() { Find = new Regex(@"[0-9](O)", RegexOptions.Compiled), ReplaceBy = "0" }
            ,new OCRRule() { Find = new Regex(@"[0-9](\.O)", RegexOptions.Compiled), ReplaceBy = ".0" }
            ,new OCRRule() { Find = new Regex(@"[0-9](,O)", RegexOptions.Compiled), ReplaceBy = ",0" }
            ,new OCRRule() { Find = new Regex(@"[A-Z](0)", RegexOptions.Compiled), ReplaceBy = "O" }
            ,new OCRRule() { Find = new Regex(@"\b(0)[A-ZÁ-Úa-zá-ú]", RegexOptions.Compiled), ReplaceBy = "O" }

            // Spaces fixes

			// Custom
            // 9: 55
			,new OCRRule() { Find = new Regex(@"[0-2]?\d(: )[0-5]\d", RegexOptions.Compiled), ReplaceBy = ":" }

            // "1 :", "2 :" ... "n :" to "n:"
			,new OCRRule() { Find = new Regex(@"\d( :)", RegexOptions.Compiled), ReplaceBy = ":" }

            // Spaces after aphostrophes, eg. "I' d" to "I'd", "I' LL" to "I'LL", "Hasn 't" and "Hasn' t", etc.
			,new OCRRule() { Find = new Regex(@"(?i)[A-ZÁ-Úa-zá-ú]('\s|\s')(ll|ve|s|m|d|t)\b", RegexOptions.Compiled), ReplaceBy = "'" }

            // Smart space after dot(s)
            // Add space after a single dot
            ,new OCRRule() { Find = new Regex(@"[a-zá-úñä-ü](\.)[^(\s\n\'\.\?\!\<"")\,]", RegexOptions.Compiled), ReplaceBy = ". ",
                IgnoreRules = new List<IgnoreRule>() {
                    new IgnoreRule() { IgnoreFind = new Regex(@".{1}[a-zá-úñä-ü](\.)[^(\s\n\'\.\?\!\<"")]", RegexOptions.Compiled), Ignore="Ph.D" },
                    new IgnoreRule() { IgnoreFind = new Regex(@"[a-zá-úñä-ü](\.)[^(\s\n\'\.\?\!\<"")].{1}", RegexOptions.Compiled), Ignore="a.m." },
                    new IgnoreRule() { IgnoreFind = new Regex(@"[a-zá-úñä-ü](\.)[^(\s\n\'\.\?\!\<"")].{1}", RegexOptions.Compiled), Ignore="p.m." },
                    new IgnoreRule() { IgnoreFind = new Regex(@"[a-zá-úñä-ü](\.)[^(\s\n\'\.\?\!\<"")].{2}", RegexOptions.Compiled), Ignore="a.k.a" },
                    new IgnoreRule() { IgnoreFind = new Regex(@"[a-zá-úñä-ü](\.)[^(\s\n\'\.\?\!\<"")].{2}", RegexOptions.Compiled), Ignore="A.K.A" },
                    new IgnoreRule() { IgnoreFind = new Regex(@"[a-zá-úñä-ü](\.)[^(\s\n\'\.\?\!\<"")].{2}", RegexOptions.Compiled), Ignore=".com", EndsWith = true }
                }
            }

			// Custom
            ,new OCRRule() { Find = new Regex(@"(a\.\sm\.)", RegexOptions.Compiled), ReplaceBy = "a.m." }
            ,new OCRRule() { Find = new Regex(@"(p\.\sm\.)", RegexOptions.Compiled), ReplaceBy = "p.m." }

            // Add space after the last of two or more consecutive dots (e.g. "...")
            //,new OCRRule() { Find = new Regex(@"(\.\.)[^(\s\n\'\.\?\!\<"")]", RegexOptions.Compiled), ReplaceBy = ".. " }
            // Remove space after two or more consecutive dots (e.g. "...") at the beginning of the line
            ,new OCRRule() { Find = new Regex(@"^\.{2,}(\s+)", RegexOptions.Compiled), ReplaceBy = "" }
            // Add space after comma
            ,new OCRRule() { Find = new Regex(@"(\,)[A-ZÁ-Úa-zá-ú]", RegexOptions.Compiled), ReplaceBy = ", " }

            
            // Custom
            // x ...
            ,new OCRRule() { Find = new Regex(@"(\s+)\.{3}(?:</i>)?$", RegexOptions.Compiled), ReplaceBy = "" }
             // a ... a
            ,new OCRRule() { Find = new Regex(@"[A-ZÁ-Úa-zá-ú](\s+)\.{3}\s+[A-ZÁ-Úa-zá-ú]", RegexOptions.Compiled), ReplaceBy = "" }
             // 1, 000
            ,new OCRRule() { Find = new Regex(@"\b\d(, )\d{3}\b", RegexOptions.Compiled), ReplaceBy = "," }

            // Text..
            ,new OCRRule() { Find = new Regex(@"[A-ZÁ-Úa-zá-ú](\.{2})(?:\s|</i>|$)", RegexOptions.Compiled), ReplaceBy = "..." }
            //,new OCRRule() { Find = new Regex(@"(I-I-I)", RegexOptions.Compiled), ReplaceBy = "I... I... I..." }
            //,new OCRRule() { Find = new Regex(@"(I-I)", RegexOptions.Compiled), ReplaceBy = "I... I..." }

            ,new OCRRule() { Find = new Regex(@"^(j|J)\b", RegexOptions.Compiled), ReplaceBy = "♪" }
            ,new OCRRule() { Find = new Regex(@"\b(j|J)$", RegexOptions.Compiled), ReplaceBy = "♪" }
        };

        #endregion

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

        private static readonly Regex regexBrackets = new Regex(@"[\({\[\]}\)]", RegexOptions.Compiled);
        private static readonly Regex regexColon = new Regex(@"^[A-ZÁ-Úa-zá-ú0-9#\-'.]+:", RegexOptions.Compiled);
        private static readonly Regex regexOneInsteadOfI = new Regex(@"[A-ZÁ-Úa-zá-ú]\s+(1)\s+[A-ZÁ-Úa-zá-ú]", RegexOptions.Compiled); // Course 1 can
        private static readonly Regex regexSlashInsteadOfI = new Regex(@"\s+/\s+", RegexOptions.Compiled); // " / " -> " I "

        public static bool HasErrors(this Subtitle subtitle)
        {
            return subtitle.Lines.Any(line =>
                regexBrackets.IsMatch(line) ||
                regexColon.IsMatch(line) ||
                regexOneInsteadOfI.IsMatch(line) ||
                regexSlashInsteadOfI.IsMatch(line)
            );
        }

        #endregion
    }
}
