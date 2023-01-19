using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SubtitlesCleanerLibrary
{
    public static class SubtitlesHelper
    {
        #region Time Parsing

        private const string showTimeFormat = @"(?<Show_HH>\d{2}):(?<Show_MM>\d{2}):(?<Show_SS>\d{2}),(?<Show_MS>\d{3})";
        private const string hideTimeFormat = @"(?<Hide_HH>\d{2}):(?<Hide_MM>\d{2}):(?<Hide_SS>\d{2}),(?<Hide_MS>\d{3})";
        private const string showTimeFormatAlternate = @"(?:(?<Show_HH>\d{2}):)?(?<Show_MM>\d{2}):(?<Show_SS>\d{2})(?:[.,](?<Show_MS>\d{3}))?";
        private const string fullTimeFormat = showTimeFormat + " --> " + hideTimeFormat;
        private const string diffTimeFormat = @"(?<Diff_Sign>-|\+)?(?:(?:(?:(?<Diff_HH>\d{1,2}):)?(?<Diff_MM>\d{1,2}):)?(?<Diff_SS>\d{1,2})(?:,|:|\.))?(?<Diff_MS>\d{1,3})";

        public static readonly Regex regexSubtitleNumber = new Regex(@"^\d+$", RegexOptions.Compiled);
        public static readonly Regex regexShowTime = new Regex(@"^" + showTimeFormat + "$", RegexOptions.Compiled);
        public static readonly Regex regexShowTimeAlternate = new Regex(@"^" + showTimeFormatAlternate + "$", RegexOptions.Compiled);
        public static readonly Regex regexTime = new Regex(@"^" + fullTimeFormat + "$", RegexOptions.Compiled);
        public static readonly Regex regexDiffTime = new Regex(@"^" + diffTimeFormat + "$", RegexOptions.Compiled);

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

        public static TimeSpan ParseDiffTime(string diffTime)
        {
            if (string.IsNullOrEmpty(diffTime))
                return TimeSpan.Zero;

            if (regexDiffTime.IsMatch(diffTime))
            {
                Match match = regexDiffTime.Match(diffTime);
                var span = new TimeSpan(
                    0,
                    match.Groups["Diff_HH"].Success ? int.Parse(match.Groups["Diff_HH"].Value) : 0,
                    match.Groups["Diff_MM"].Success ? int.Parse(match.Groups["Diff_MM"].Value) : 0,
                    match.Groups["Diff_SS"].Success ? int.Parse(match.Groups["Diff_SS"].Value) : 0,
                    match.Groups["Diff_MS"].Success ? int.Parse(match.Groups["Diff_MS"].Value) : 0
                );

                if (match.Groups["Diff_Sign"].Success && match.Groups["Diff_Sign"].Value == "-")
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

        #region File Encoding

        // https://stackoverflow.com/questions/3825390/effective-way-to-find-any-files-encoding
        public static Encoding GetEncoding(string filePath)
        {
            using (var reader = new StreamReader(filePath, Encoding.UTF8 /* defaultEncodingIfNoBom*/, true))
            {
                reader.Peek();
                return reader.CurrentEncoding;
            }
        }

        //public static readonly Encoding ISO_8859_1 = Encoding.GetEncoding("ISO-8859-1");
        //public static readonly Regex regexAccentedCharacters = new Regex(@"[Á-Úá-ú]", RegexOptions.Compiled);

        //public static bool HasAccentedCharacters(string filePath)
        //{
        //    return regexAccentedCharacters.IsMatch(File.ReadAllText(filePath, ISO_8859_1));
        //}

        public static Encoding GetFileEncoding(string filePath)
        {
            return GetEncoding(filePath);
            //return HasAccentedCharacters(filePath) ? ISO_8859_1 : Encoding.UTF8;
        }

        #endregion

        #region Get Subtitles

        public static List<Subtitle> GetSubtitles(string filePath, ref Encoding encoding, int? firstSubtitlesCount = null)
        {
            encoding = GetFileEncoding(filePath);
            //encoding = Encoding.UTF8;
            List<string> lines = new List<string>(File.ReadAllLines(filePath, encoding));

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

        #region To Lines

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

        public static Subtitle CleanSubtitles(this Subtitle subtitles, bool cleanHICaseInsensitive, bool isPrintCleaning)
        {
            return new List<Subtitle>() { subtitles }.CleanSubtitles(cleanHICaseInsensitive, isPrintCleaning).FirstOrDefault();
        }

        public static List<Subtitle> CleanSubtitles(this List<Subtitle> subtitles, bool cleanHICaseInsensitive, bool isPrintCleaning)
        {
            subtitles = IterateSubtitlesPre(subtitles, cleanHICaseInsensitive, isPrintCleaning);

            bool subtitlesChanged = false;
            int loopCount = 0;
            int loopThresh = 6;
            do
            {
                subtitlesChanged = false;
                subtitles = IterateSubtitles(subtitles, cleanHICaseInsensitive, isPrintCleaning, ref subtitlesChanged);
                loopCount++;

                if (subtitlesChanged && loopCount == loopThresh - 1)
                {
                    Console.WriteLine("Infinite Loop");
                    Console.WriteLine();
                    isPrintCleaning = true;
                }
                else if (subtitlesChanged && loopCount == loopThresh)
                {
                    Console.ReadKey(true);
                    throw new Exception("Infinite Loop");
                }
            } while (subtitlesChanged);

            subtitles = IterateSubtitlesPost(subtitles);
            return subtitles;
        }

        private static List<Subtitle> IterateSubtitlesPre(List<Subtitle> subtitles, bool cleanHICaseInsensitive, bool isPrintCleaning)
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
                        string cleanLine = (CleanSubtitleLinePre(line, cleanHICaseInsensitive, isPrintCleaning) ?? string.Empty).Trim();

                        if (IsEmptyLine(cleanLine))
                            subtitle.Lines.RemoveAt(i);
                        else
                            subtitle.Lines[i] = cleanLine;
                    }
                }

                subtitle.Lines = CleanSubtitleMultipleLinesPre(subtitle.Lines, cleanHICaseInsensitive);

                if (subtitle.Lines == null || subtitle.Lines.Count == 0)
                    subtitles.RemoveAt(k);
            }

            return subtitles;
        }

        private static List<Subtitle> IterateSubtitles(List<Subtitle> subtitles, bool cleanHICaseInsensitive, bool isPrintCleaning, ref bool subtitlesChanged)
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
                        subtitlesChanged = true;
                    }
                    else if (IsNotSubtitle(line))
                    {
                        subtitle.Lines = null;
                        subtitlesChanged = true;
                        break;
                    }
                    else
                    {
                        string cleanLine = (CleanSubtitleLine(line, cleanHICaseInsensitive, isPrintCleaning) ?? string.Empty).Trim();

                        if (IsEmptyLine(cleanLine))
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
                cleanLines = CleanSubtitleMultipleLines(cleanLines, cleanHICaseInsensitive);

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

                    if (IsEmptyLine(cleanLine))
                        subtitle.Lines.RemoveAt(i);
                    else
                        subtitle.Lines[i] = cleanLine;
                }

                subtitle.Lines = CleanSubtitleMultipleLinesPost(subtitle.Lines);
            }

            return subtitles;
        }

        public static void CheckSubtitles(this List<Subtitle> subtitles, bool cleanHICaseInsensitive)
        {
            if (subtitles == null)
                return;

            if (subtitles.Count == 0)
                return;

            foreach (Subtitle subtitle in subtitles)
                CheckSubtitle(subtitle, cleanHICaseInsensitive);
        }

        public static void CheckSubtitle(this Subtitle subtitle, bool cleanHICaseInsensitive)
        {
            subtitle.SubtitleError = SubtitleError.None;

            if (subtitle.Lines == null || subtitle.Lines.Count == 0)
            {
                subtitle.SubtitleError = SubtitleError.Empty_Line;
                return;
            }

            foreach (string line in subtitle.Lines)
            {
                if (IsNotSubtitle(line))
                {
                    subtitle.SubtitleError |= SubtitleError.Not_Subtitle;
                }
                else if (IsEmptyLine(line))
                {
                    subtitle.SubtitleError |= SubtitleError.Empty_Line;
                }
                else
                {
                    subtitle.SubtitleError |= CheckSubtitleLinePre(line, cleanHICaseInsensitive);
                    subtitle.SubtitleError |= CheckSubtitleLine(line, cleanHICaseInsensitive);
                    subtitle.SubtitleError |= CheckSubtitleLinePost(line);
                }
            }

            if (subtitle.SubtitleError.IsSet(SubtitleError.Not_Subtitle))
            {
                subtitle.SubtitleError = SubtitleError.Not_Subtitle;
            }
            else if (subtitle.SubtitleError.IsSet(SubtitleError.Empty_Line))
            {
                subtitle.SubtitleError = SubtitleError.Empty_Line;
            }
            else
            {
                subtitle.SubtitleError |= CheckSubtitleMultipleLinesPre(subtitle.Lines, cleanHICaseInsensitive);
                subtitle.SubtitleError |= CheckSubtitleMultipleLines(subtitle.Lines, cleanHICaseInsensitive);
                subtitle.SubtitleError |= CheckSubtitleMultipleLinesPost(subtitle.Lines);

                if (subtitle.SubtitleError.IsSet(SubtitleError.Not_Subtitle))
                    subtitle.SubtitleError = SubtitleError.Not_Subtitle;
                else if (subtitle.SubtitleError.IsSet(SubtitleError.Empty_Line))
                    subtitle.SubtitleError = SubtitleError.Empty_Line;
            }
        }

        #region Clean Single Line Pre

        private static string CleanSubtitleLinePre(string line, bool cleanHICaseInsensitive, bool isPrintCleaning)
        {
            return CleanLine(line, FindAndReplaceRulesPre, cleanHICaseInsensitive, isPrintCleaning);
        }

        private static SubtitleError CheckSubtitleLinePre(string line, bool cleanHICaseInsensitive)
        {
            SubtitleError subtitleError = SubtitleError.None;
            CleanLine(line, FindAndReplaceRulesPre, cleanHICaseInsensitive, false, ref subtitleError);
            return subtitleError;
        }

        #endregion

        #region Clean Multiple Lines Pre

        public static readonly Regex regexMissingNewLine = new Regex(@"[!?][A-ZÁ-Úa-zá-ú]", RegexOptions.Compiled);
        public static readonly Regex regexNoteStart = new Regex(@"^(?:-\s*)?(?<Note>♪+)", RegexOptions.Compiled);
        public static readonly Regex regexNoteEnd = new Regex(@"\s+(?<Note>♪+)$", RegexOptions.Compiled);
        public static readonly Regex regexQMStart = new Regex(@"^(?:-\s*)?(?<QM>\?+)", RegexOptions.Compiled);
        public static readonly Regex regexQMEnd = new Regex(@"\s+(?<QM>\?+)$", RegexOptions.Compiled);

        private static List<string> CleanSubtitleMultipleLinesPre(List<string> lines, bool cleanHICaseInsensitive)
        {
            if (lines == null || lines.Count == 0)
                return null;

            #region Missing New Line

            for (int i = lines.Count - 1; i >= 0; i--)
            {
                string line = lines[i];
                if (regexMissingNewLine.IsMatch(line))
                {
                    int[] splitIndexes = regexMissingNewLine.Matches(line)
                        .Cast<Match>()
                        .Select(m => m.Index)
                        .Distinct()
                        .OrderByDescending(x => x)
                        .ToArray();

                    List<string> splitLines = new List<string>();
                    foreach (int splitIndex in splitIndexes)
                    {
                        splitLines.Insert(0, "- " + line.Substring(splitIndex + 1));
                        line = line.Substring(0, splitIndex + 1);
                    }

                    if (line.StartsWith("-") || line.StartsWith("<i>-"))
                        splitLines.Insert(0, line);
                    else
                        splitLines.Insert(0, "- " + line);

                    lines.RemoveAt(i);
                    lines.InsertRange(i, splitLines);
                }
            }

            #endregion

            if (lines.Count > 1)
            {
                var results = lines.Select((line, index) => new
                {
                    line,
                    index,
                    isStartsWithNote = regexNoteStart.IsMatch(line),
                    isEndsWithNote = regexNoteEnd.IsMatch(line),
                    isStartsWithQM = regexQMStart.IsMatch(line),
                    isEndsWithQM = regexQMEnd.IsMatch(line),
                    isStartsWithDash = line.StartsWith("-"),
                    isMatchHIPrefix = (cleanHICaseInsensitive ? regexHIPrefixWithoutDialogDashCI : regexHIPrefixWithoutDialogDash).IsMatch(line)
                }).ToArray();

                #region Lyrics Multiple Lines

                // ? start lyrics => ♪ start lyrics
                // between lyrics
                // end lyrics ? => end lyrics ♪
                var startItem = results.FirstOrDefault(item => (item.isStartsWithNote || item.isStartsWithQM) && (item.isEndsWithNote == false && item.isEndsWithQM == false));
                while (startItem != null)
                {
                    bool isStartItemStartsWithDash = startItem.isStartsWithDash;
                    var endItem = results.Skip(startItem.index + 1).FirstOrDefault(item =>
                        (item.isStartsWithNote == false && item.isStartsWithQM == false) &&
                        (item.isEndsWithNote || item.isEndsWithQM) &&
                        item.isStartsWithDash == isStartItemStartsWithDash);
                    if (endItem != null)
                    {
                        var itemsBetween = results.Skip(startItem.index + 1).Take(endItem.index - startItem.index - 1);
                        if (itemsBetween.All(item => item.isStartsWithNote == false && item.isStartsWithQM == false && item.isEndsWithNote == false && item.isEndsWithQM == false))
                        {
                            if (startItem.isStartsWithQM)
                                lines[startItem.index] = regexQMStart.ReplaceGroup(startItem.line, "QM", "♪");
                            if (endItem.isEndsWithQM)
                                lines[endItem.index] = regexQMEnd.ReplaceGroup(endItem.line, "QM", "♪");

                            if (startItem.isStartsWithDash &&
                                itemsBetween.All(item => item.isStartsWithDash == false) &&
                                endItem.isStartsWithDash == false)
                            {
                                lines[startItem.index] = lines[startItem.index].TrimStart('-');
                            }

                            startItem = results
                                .Skip(endItem.index + 1)
                                .FirstOrDefault(item => (item.isStartsWithNote || item.isStartsWithQM) && (item.isEndsWithNote == false && item.isEndsWithQM == false));
                        }
                        else
                        {
                            startItem = results
                                .Skip(endItem.index + 1)
                                .FirstOrDefault(item => (item.isStartsWithNote || item.isStartsWithQM) && (item.isEndsWithNote == false && item.isEndsWithQM == false));
                        }
                    }
                    else
                    {
                        startItem = null;
                    }
                }

                #endregion

                #region HI Prefix

                if (results.Count(x => x.isMatchHIPrefix) > 1)
                {
                    foreach (var item in results)
                    {
                        if (item.isMatchHIPrefix)
                        {
                            Match match = (cleanHICaseInsensitive ? regexHIPrefixWithoutDialogDashCI : regexHIPrefixWithoutDialogDash).Match(lines[item.index]);
                            lines[item.index] = (match.Groups["Prefix"].Value + " - " + match.Groups["Subtitle"].Value).Trim();
                        }
                    }
                }

                #endregion
            }

            return lines;
        }

        private static SubtitleError CheckSubtitleMultipleLinesPre(List<string> lines, bool cleanHICaseInsensitive)
        {
            if (lines == null || lines.Count == 0)
                return SubtitleError.Empty_Line;

            SubtitleError subtitleError = SubtitleError.None;

            if (lines.Count > 1)
            {
                var results = lines.Select((line, index) => new
                {
                    line,
                    index,
                    isMatchHIPrefix = (cleanHICaseInsensitive ? regexHIPrefixWithoutDialogDashCI : regexHIPrefixWithoutDialogDash).IsMatch(line)
                }).ToArray();

                if (results.Count(x => x.isMatchHIPrefix) > 1)
                {
                    subtitleError |= SubtitleError.Hearing_Impaired;
                }
            }

            return subtitleError;
        }

        #endregion

        #region Clean Single Line

        private static string CleanSubtitleLine(string line, bool cleanHICaseInsensitive, bool isPrintCleaning)
        {
            return CleanLine(line, FindAndReplaceRules, cleanHICaseInsensitive, isPrintCleaning);
        }

        private static SubtitleError CheckSubtitleLine(string line, bool cleanHICaseInsensitive)
        {
            SubtitleError subtitleError = SubtitleError.None;
            CleanLine(line, FindAndReplaceRules, cleanHICaseInsensitive, false, ref subtitleError);
            return subtitleError;
        }

        #endregion

        #region Clean Multiple Lines

        public static readonly Regex regexMergedLinesWithHI = new Regex(@"(?<Line1>^-[^-]+)(?<Line2><i>-.*?</i>$)");
        public static readonly Regex regexCapitalLetter = new Regex(@"[A-ZÁ-Ú]", RegexOptions.Compiled);
        public static readonly Regex regexLowerLetter = new Regex(@"[a-zá-ú]", RegexOptions.Compiled);
        public static readonly Regex regexDialog = new Regex(@"^(?<Italic><i>)?-\s*(?<Subtitle>.*?)$", RegexOptions.Compiled);
        public static readonly Regex regexContainsDialog = new Regex(@" - [A-ZÁ-Ú]", RegexOptions.Compiled);
        public static readonly Regex regexEndsWithLowerCaseLetter = new Regex(@"[a-zá-ú]$", RegexOptions.Compiled);

        private static List<string> CleanSubtitleMultipleLines(List<string> lines, bool cleanHICaseInsensitive)
        {
            if (lines == null || lines.Count == 0)
                return null;

            if (lines.Count == 1)
            {
                // - Line 1 <i>- Line 2</i>
                //
                // - Line 1
                // <i>- Line 2</i>
                if (regexMergedLinesWithHI.IsMatch(lines[0]))
                {
                    Match match = regexMergedLinesWithHI.Match(lines[0]);
                    string line1 = match.Groups["Line1"].Value;
                    string line2 = match.Groups["Line2"].Value;
                    lines[0] = line1;
                    lines.Add(line2);
                }
            }

            if (lines.Count > 1)
            {
                string firstLine = lines[0];
                string lastLine = lines[lines.Count - 1];

                // first line starts with (
                // last line ends with )
                // lines between don't have ()
                if (IsHearingImpairedMultipleLines_RoundBrackets(firstLine, lastLine))
                {
                    if (lines.Skip(1).Take(lines.Count - 2).Any(line => line.Contains("(") || line.Contains(")")) == false)
                    {
                        return null;
                    }
                }

                // first line starts with [
                // last line ends with ]
                // lines between don't have []
                if (IsHearingImpairedMultipleLines_SquareBrackets(firstLine, lastLine))
                {
                    if (lines.Skip(1).Take(lines.Count - 2).Any(line => line.Contains("[") || line.Contains("]")) == false)
                    {
                        return null;
                    }
                }

                // consecutive lines have parentheses or brackets
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

            if (lines.Count == 3)
            {
                string line1 = lines[0];
                string line2 = lines[1];
                string line3 = lines[2];

                // Line 1: <i>-
                // Line 2: Text 2
                // Line 3: - Text 3</i>
                //
                // Line 2: <i>- Text 2
                // Line 3: - Text 3</i>
                if (line1 == "<i>-" &&
                    line2.StartsWith("- ") == false && line2.StartsWith("<i>") == false && line2.EndsWith("</i>") == false &&
                    line3.StartsWith("- ") && line3.EndsWith("</i>"))
                {
                    lines[1] = "<i>- " + line2;
                    lines.RemoveAt(0);
                }
                // Line 1: <i>
                // Line 2: Text 2
                // Line 3: Text 3</i>
                //
                // Line 2: <i>Text 2
                // Line 3: Text 3</i>
                else if (line1 == "<i>" &&
                    line2.StartsWith("- ") == false && line2.StartsWith("<i>") == false && line2.EndsWith("</i>") == false &&
                    line3.StartsWith("<i>") == false && line3.EndsWith("</i>"))
                {
                    lines[1] = "<i>" + line2;
                    lines.RemoveAt(0);
                }
                // Line 1: - <i>
                // Line 2: - Text 2
                // Line 3: - Text 3</i>
                //
                // Line 2: <i>- Text 2
                // Line 3: - Text 3</i>
                else if (line1 == "- <i>" &&
                    line2.StartsWith("- ") && line2.StartsWith("<i>") == false && line2.EndsWith("</i>") == false &&
                    line3.StartsWith("- ") && line3.EndsWith("</i>"))
                {
                    lines[1] = "<i>" + line2;
                    lines.RemoveAt(0);
                }
                // Line 1: <i>Text 1
                // Line 2: Text 2
                // Line 3: </i>Text 3
                //
                // Line 1: <i>Text 1
                // Line 2: Text 2</i>
                // Line 3: Text 3
                else if (line1.StartsWith("<i>") && line1.EndsWith("</i>") == false &&
                    line2.StartsWith("- ") == false && line2.StartsWith("<i>") == false && line2.EndsWith("</i>") == false &&
                    line3.StartsWith("</i>"))
                {
                    lines[1] = lines[1] + "</i>";
                    lines[2] = lines[2].Substring(4);
                }
            }

            if (lines.Count == 2)
            {
                string line1 = lines[0];
                string line2 = lines[1];

                // Line 1: <i>-
                // Line 2: - Text</i>
                //
                // Line 2: <i>- Text</i>
                if (line1 == "<i>-" && line2.StartsWith("- ") && line2.EndsWith("</i>"))
                {
                    lines[1] = "<i>" + line2.Substring(2);
                    lines.RemoveAt(0);
                }
                // Line 1: <i>
                // Line 2: Text</i>
                //
                // Line 2: <i>Text</i>
                else if (line1 == "<i>" && line2.StartsWith("<i>") == false && line2.EndsWith("</i>"))
                {
                    lines[1] = "<i>" + line2;
                    lines.RemoveAt(0);
                }
                // Line 1: - <i>
                // Line 2: - Text</i>
                //
                // Line 2: <i>Text</i>
                else if (line1 == "- <i>" && line2.StartsWith("- ") && line2.EndsWith("</i>"))
                {
                    lines[1] = "<i>" + line2.Substring(2);
                    lines.RemoveAt(0);
                }
                // Line 1: <i>Text 1
                // Line 2: </i>
                //
                // Line 1: <i>Text 1</i>
                else if (line1.StartsWith("<i>") && line1.EndsWith("</i>") == false && line2 == "</i>")
                {
                    lines[0] = lines[0] + "</i>";
                    lines.RemoveAt(1);
                }
                // Line 1: <i>Text 1
                // Line 2: </i>Text 2
                //
                // Line 1: <i>Text 1</i>
                // Line 2: Text 2
                else if (line1.StartsWith("<i>") && line1.EndsWith("</i>") == false && line2.StartsWith("</i>"))
                {
                    lines[0] = lines[0] + "</i>";
                    lines[1] = lines[1].Substring(4);
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
                var results = lines.Select((line, index) => new
                {
                    line,
                    index,
                    isMatchHIPrefix = (cleanHICaseInsensitive ? regexHIPrefixCI : regexHIPrefix).IsMatch(line)
                }).ToArray();

                if (results[0].isMatchHIPrefix && results.Skip(1).All(x => x.isMatchHIPrefix == false))
                {
                    Match match = (cleanHICaseInsensitive ? regexHIPrefixCI : regexHIPrefix).Match(lines[0]);
                    lines[0] = match.Groups["Subtitle"].Value;
                }
                else if (results.Count(x => x.isMatchHIPrefix) > 1)
                {
                    foreach (var item in results)
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
                    isStartsWithNote = line.StartsWith("♪"),
                    isEndsWithDots = line.EndsWith("..."),
                    isEndsWithComma = line.EndsWith(","),
                    isEndsWithPeriod = line.EndsWith("."),
                    isEndsWithQuestionMark = line.EndsWith("?"),
                    isEndsWithExclamationMark = line.EndsWith("!"),
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
                else if (resultsDialog[0].isStartsWithNote && resultsDialog.Skip(1).All(x => x.isMatchDialog))
                {
                    // ♪ Line 1
                    // - Line 2
                    lines[0] = "- " + lines[0];
                    // - ♪ Line 1
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
                else if (resultsDialog[0].isMatchDialog && resultsDialog.Skip(1).All(x => x.isStartsWithNote))
                {
                    // - Line 1
                    // ♪ Line 2
                    for (int i = 1; i < lines.Count; i++)
                        lines[i] = "- " + lines[i];
                    // - Line 1
                    // - ♪ Line 2
                }
                else if (resultsDialog[0].isMatchDialog && resultsDialog.Skip(1).All(x => x.isMatchDialog == false) && resultsDialog.Skip(1).All(x => x.isContainsDialog_CapitalLetter == false))
                {
                    string firstCharSecondLine = (lines[1].Length > 0 ? (lines[1][0]).ToString() : string.Empty);

                    if (regexCapitalLetter.IsMatch(firstCharSecondLine))
                    {
                        // - Line 1 - Dialog
                        // I am line 2
                        if (resultsDialog[0].isMatchDialog &&
                            resultsDialog[0].isContainsDialog_CapitalLetter &&
                            (resultsDialog[1].isStartsWithI || resultsDialog[1].isStartsWithContractionI))
                        {
                            // do nothing
                        }
                        // - Line 1 - Dialog...
                        // Line 2
                        else if (resultsDialog[0].isMatchDialog &&
                            resultsDialog[0].isContainsDialog_CapitalLetter &&
                            resultsDialog[0].isEndsWithDots)
                        {
                            // do nothing
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
                            // do nothing
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
                    string firstCharFirstLine = (lines[0].Length > 0 ? (lines[0][0]).ToString() : string.Empty);
                    string firstCharSecondLine = (lines[1].Length > 0 ? (lines[1][0]).ToString() : string.Empty);

                    if (regexLowerLetter.IsMatch(firstCharFirstLine) && (
                        resultsDialog[0].isEndsWithDots ||
                        resultsDialog[0].isEndsWithPeriod ||
                        resultsDialog[0].isEndsWithQuestionMark ||
                        resultsDialog[0].isEndsWithExclamationMark
                        ))
                    {
                        // line 1.
                        // - Line 2
                        lines[0] = "- " + lines[0];
                        // - line 1.
                        // - Line 2
                    }
                    else if (regexCapitalLetter.IsMatch(firstCharFirstLine))
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

                var resultsNotes = lines.Select((line, index) => new
                {
                    line,
                    index,

                    isStartsWithNote1 = line.StartsWith("♪"),
                    isStartsWithNote2 = line.StartsWith("-♪"),
                    isStartsWithNote3 = line.StartsWith("- ♪"),
                    isStartsWithNote4 = line.StartsWith("<i>♪"),
                    isStartsWithNote5 = line.StartsWith("<i>-♪"),
                    isStartsWithNote6 = line.StartsWith("<i>- ♪"),
                    isStartsWithNote7 = line.StartsWith("-<i>♪"),
                    isStartsWithNote8 = line.StartsWith("- <i>♪"),

                    isEndsWithNote1 = line.EndsWith("♪"),
                    isEndsWithNote2 = line.EndsWith("♪</i>"),
                    isEndsWithNote3 = line.EndsWith("</i>♪"),
                    isEndsWithNote4 = line.EndsWith("</i> ♪"),

                    isStartsWithAmpersand1 = line.StartsWith("&"),
                    isStartsWithAmpersand2 = line.StartsWith("-&"),
                    isStartsWithAmpersand3 = line.StartsWith("- &"),
                    isStartsWithAmpersand4 = line.StartsWith("<i>&"),
                    isStartsWithAmpersand5 = line.StartsWith("<i>-&"),
                    isStartsWithAmpersand6 = line.StartsWith("<i>- &"),
                    isStartsWithAmpersand7 = line.StartsWith("-<i>&"),
                    isStartsWithAmpersand8 = line.StartsWith("- <i>&"),

                    isEndsWithAmpersand1 = line.EndsWith("&"),
                    isEndsWithAmpersand2 = line.EndsWith("&</i>"),
                    isEndsWithAmpersand3 = line.EndsWith("</i>&"),
                    isEndsWithAmpersand4 = line.EndsWith("</i> &"),

                    isStartsWithI1 = line.StartsWith("I "),
                    isStartsWithI2 = line.StartsWith("-I "),
                    isStartsWithI3 = line.StartsWith("- I "),
                    isStartsWithI4 = line.StartsWith("<i>I "),
                    isStartsWithI5 = line.StartsWith("<i>-I "),
                    isStartsWithI6 = line.StartsWith("<i>- I "),
                    isStartsWithI7 = line.StartsWith("-<i>I "),
                    isStartsWithI8 = line.StartsWith("- <i>I "),

                    isEndsWithI1 = line.EndsWith(" I"),
                    isEndsWithI2 = line.EndsWith(" I</i>"),
                    isEndsWithI3 = line.EndsWith(" </i>I"),
                    isEndsWithI4 = line.EndsWith("</i> I")
                }).Select(x => new
                {
                    x.line,
                    x.index,

                    x.isStartsWithNote1,
                    x.isStartsWithNote2,
                    x.isStartsWithNote3,
                    x.isStartsWithNote4,
                    x.isStartsWithNote5,
                    x.isStartsWithNote6,
                    x.isStartsWithNote7,
                    x.isStartsWithNote8,
                    isStartsWithNote =
                        x.isStartsWithNote1 ||
                        x.isStartsWithNote2 ||
                        x.isStartsWithNote3 ||
                        x.isStartsWithNote4 ||
                        x.isStartsWithNote5 ||
                        x.isStartsWithNote6 ||
                        x.isStartsWithNote7 ||
                        x.isStartsWithNote8,

                    x.isEndsWithNote1,
                    x.isEndsWithNote2,
                    x.isEndsWithNote3,
                    x.isEndsWithNote4,
                    isEndsWithNote =
                        x.isEndsWithNote1 ||
                        x.isEndsWithNote2 ||
                        x.isEndsWithNote3 ||
                        x.isEndsWithNote4,

                    x.isStartsWithAmpersand1,
                    x.isStartsWithAmpersand2,
                    x.isStartsWithAmpersand3,
                    x.isStartsWithAmpersand4,
                    x.isStartsWithAmpersand5,
                    x.isStartsWithAmpersand6,
                    x.isStartsWithAmpersand7,
                    x.isStartsWithAmpersand8,
                    isStartsWithAmpersand =
                        x.isStartsWithAmpersand1 ||
                        x.isStartsWithAmpersand2 ||
                        x.isStartsWithAmpersand3 ||
                        x.isStartsWithAmpersand4 ||
                        x.isStartsWithAmpersand5 ||
                        x.isStartsWithAmpersand6 ||
                        x.isStartsWithAmpersand7 ||
                        x.isStartsWithAmpersand8,

                    x.isEndsWithAmpersand1,
                    x.isEndsWithAmpersand2,
                    x.isEndsWithAmpersand3,
                    x.isEndsWithAmpersand4,
                    isEndsWithAmpersand =
                        x.isEndsWithAmpersand1 ||
                        x.isEndsWithAmpersand2 ||
                        x.isEndsWithAmpersand3 ||
                        x.isEndsWithAmpersand4,

                    x.isStartsWithI1,
                    x.isStartsWithI2,
                    x.isStartsWithI3,
                    x.isStartsWithI4,
                    x.isStartsWithI5,
                    x.isStartsWithI6,
                    x.isStartsWithI7,
                    x.isStartsWithI8,
                    isStartsWithI =
                        x.isStartsWithI1 ||
                        x.isStartsWithI2 ||
                        x.isStartsWithI3 ||
                        x.isStartsWithI4 ||
                        x.isStartsWithI5 ||
                        x.isStartsWithI6 ||
                        x.isStartsWithI7 ||
                        x.isStartsWithI8,

                    x.isEndsWithI1,
                    x.isEndsWithI2,
                    x.isEndsWithI3,
                    x.isEndsWithI4,
                    isEndsWithI =
                        x.isEndsWithI1 ||
                        x.isEndsWithI2 ||
                        x.isEndsWithI3 ||
                        x.isEndsWithI4
                }).Select(x => new
                {
                    x.line,
                    x.index,

                    x.isStartsWithNote1,
                    x.isStartsWithNote2,
                    x.isStartsWithNote3,
                    x.isStartsWithNote4,
                    x.isStartsWithNote5,
                    x.isStartsWithNote6,
                    x.isStartsWithNote7,
                    x.isStartsWithNote8,
                    x.isStartsWithNote,

                    x.isEndsWithNote1,
                    x.isEndsWithNote2,
                    x.isEndsWithNote3,
                    x.isEndsWithNote4,
                    x.isEndsWithNote,

                    x.isStartsWithAmpersand1,
                    x.isStartsWithAmpersand2,
                    x.isStartsWithAmpersand3,
                    x.isStartsWithAmpersand4,
                    x.isStartsWithAmpersand5,
                    x.isStartsWithAmpersand6,
                    x.isStartsWithAmpersand7,
                    x.isStartsWithAmpersand8,
                    x.isStartsWithAmpersand,

                    x.isEndsWithAmpersand1,
                    x.isEndsWithAmpersand2,
                    x.isEndsWithAmpersand3,
                    x.isEndsWithAmpersand4,
                    x.isEndsWithAmpersand,

                    x.isStartsWithI1,
                    x.isStartsWithI2,
                    x.isStartsWithI3,
                    x.isStartsWithI4,
                    x.isStartsWithI5,
                    x.isStartsWithI6,
                    x.isStartsWithI7,
                    x.isStartsWithI8,
                    x.isStartsWithI,

                    x.isEndsWithI1,
                    x.isEndsWithI2,
                    x.isEndsWithI3,
                    x.isEndsWithI4,
                    x.isEndsWithI,

                    // ♪ Line
                    isStartsWithAndNotEndWithNote =
                        x.isStartsWithNote &&
                        x.isEndsWithNote == false && x.isEndsWithAmpersand == false && x.isEndsWithI == false,
                    //   Line ♪
                    isNotStartWithAndEndsWithNote =
                        x.isStartsWithNote == false && x.isStartsWithAmpersand == false && x.isStartsWithI == false &&
                        x.isEndsWithNote,

                    // & Line
                    isStartsWithAndNotEndWithAmpersand =
                        x.isStartsWithAmpersand &&
                        x.isEndsWithNote == false && x.isEndsWithAmpersand == false && x.isEndsWithI == false,
                    //   Line &
                    isNotStartWithAndEndsWithAmpersand =
                        x.isStartsWithNote == false && x.isStartsWithAmpersand == false && x.isStartsWithI == false &&
                        x.isEndsWithAmpersand,

                    // I Line
                    isStartsWithAndNotEndWithI =
                        x.isStartsWithI &&
                        x.isEndsWithNote == false && x.isEndsWithAmpersand == false && x.isEndsWithI == false,
                    //   Line I
                    isNotStartWithAndEndsWithI =
                        x.isStartsWithNote == false && x.isStartsWithAmpersand == false && x.isStartsWithI == false &&
                        x.isEndsWithI,

                    isRegularLine =
                        x.isStartsWithNote == false && x.isEndsWithNote == false &&
                        x.isStartsWithAmpersand == false && x.isEndsWithAmpersand == false &&
                        x.isStartsWithI == false && x.isEndsWithI == false
                }).ToArray();

                List<int> startsWithAmpersand = new List<int>();
                List<int> startsWithI = new List<int>();
                List<int> endsWithAmpersand = new List<int>();
                List<int> endsWithI = new List<int>();

                for (int i = 0; i < resultsNotes.Length; i++)
                {
                    // ♪ Line1
                    //   Line2
                    //   Line3 &/I
                    if (resultsNotes[i].isStartsWithAndNotEndWithNote)
                    {
                        for (int j = i + 1; j < resultsNotes.Length; j++)
                        {
                            if (resultsNotes[j].isNotStartWithAndEndsWithAmpersand)
                            {
                                endsWithAmpersand.Add(j);
                                i = j; // j + 1 - 1, +1 is for the next line, -1 is to counter the i++
                                break;
                            }
                            else if (resultsNotes[j].isNotStartWithAndEndsWithI)
                            {
                                endsWithI.Add(j);
                                i = j; // j + 1 - 1, +1 is for the next line, -1 is to counter the i++
                                break;
                            }
                            else if (resultsNotes[j].isRegularLine == false)
                            {
                                i = j - 1; // -1 is to counter the i++
                                break;
                            }
                        }
                    }
                    // & Line1
                    //   Line2
                    //   Line3 ♪
                    else if (resultsNotes[i].isStartsWithAndNotEndWithAmpersand)
                    {
                        for (int j = i + 1; j < resultsNotes.Length; j++)
                        {
                            if (resultsNotes[j].isNotStartWithAndEndsWithNote)
                            {
                                startsWithAmpersand.Add(i);
                                i = j; // j + 1 - 1, +1 is for the next line, -1 is to counter the i++
                                break;
                            }
                            else if (resultsNotes[j].isRegularLine == false)
                            {
                                i = j - 1; // -1 is to counter the i++
                                break;
                            }
                        }
                    }
                    // I Line1
                    //   Line2
                    //   Line3 ♪
                    else if (resultsNotes[i].isStartsWithAndNotEndWithI)
                    {
                        for (int j = i + 1; j < resultsNotes.Length; j++)
                        {
                            if (resultsNotes[j].isNotStartWithAndEndsWithNote)
                            {
                                startsWithI.Add(i);
                                i = j; // j + 1 - 1, +1 is for the next line, -1 is to counter the i++
                                break;
                            }
                            else if (resultsNotes[j].isRegularLine == false)
                            {
                                i = j - 1; // -1 is to counter the i++
                                break;
                            }
                        }
                    }
                }

                foreach (int index in startsWithAmpersand)
                {
                    var item = resultsNotes[index];

                    // "&"
                    if (item.isStartsWithAmpersand1)
                        lines[index] = "♪" + lines[index].Substring(1);
                    // "-&"
                    else if (item.isStartsWithAmpersand2)
                        lines[index] = "-♪" + lines[index].Substring(2);
                    // "- &"
                    else if (item.isStartsWithAmpersand3)
                        lines[index] = "- ♪" + lines[index].Substring(3);
                    // "<i>&"
                    else if (item.isStartsWithAmpersand4)
                        lines[index] = "<i>♪" + lines[index].Substring(4);
                    // "<i>-&"
                    else if (item.isStartsWithAmpersand5)
                        lines[index] = "<i>-♪" + lines[index].Substring(5);
                    // "<i>- &"
                    else if (item.isStartsWithAmpersand6)
                        lines[index] = "<i>- ♪" + lines[index].Substring(6);
                    // "-<i>&"
                    else if (item.isStartsWithAmpersand7)
                        lines[index] = "-<i>♪" + lines[index].Substring(5);
                    // "- <i>&"
                    else if (item.isStartsWithAmpersand8)
                        lines[index] = "- <i>♪" + lines[index].Substring(6);
                }

                foreach (int index in startsWithI)
                {
                    var item = resultsNotes[index];

                    // "I "
                    if (item.isStartsWithI1)
                        lines[index] = "♪ " + lines[index].Substring(2);
                    // "-I "
                    else if (item.isStartsWithI2)
                        lines[index] = "-♪ " + lines[index].Substring(3);
                    // "- I "
                    else if (item.isStartsWithI3)
                        lines[index] = "- ♪ " + lines[index].Substring(4);
                    // "<i>I "
                    else if (item.isStartsWithI4)
                        lines[index] = "<i>♪ " + lines[index].Substring(5);
                    // "<i>-I "
                    else if (item.isStartsWithI5)
                        lines[index] = "<i>-♪ " + lines[index].Substring(6);
                    // "<i>- I "
                    else if (item.isStartsWithI6)
                        lines[index] = "<i>- ♪ " + lines[index].Substring(7);
                    // "-<i>I "
                    else if (item.isStartsWithI7)
                        lines[index] = "-<i>♪ " + lines[index].Substring(6);
                    // "- <i>I "
                    else if (item.isStartsWithI8)
                        lines[index] = "- <i>♪ " + lines[index].Substring(7);
                }

                foreach (int index in endsWithAmpersand)
                {
                    var item = resultsNotes[index];

                    // "&"
                    if (item.isEndsWithAmpersand1)
                        lines[index] = lines[index].Substring(0, lines[index].Length - 1) + "♪";
                    // "&</i>"
                    else if (item.isEndsWithAmpersand2)
                        lines[index] = lines[index].Substring(0, lines[index].Length - 5) + "♪</i>";
                    // "</i>&"
                    else if (item.isEndsWithAmpersand3)
                        lines[index] = lines[index].Substring(0, lines[index].Length - 5) + "</i>♪";
                    // "</i> &"
                    else if (item.isEndsWithAmpersand4)
                        lines[index] = lines[index].Substring(0, lines[index].Length - 6) + "</i> ♪";
                }

                foreach (int index in endsWithI)
                {
                    var item = resultsNotes[index];

                    // " I"
                    if (item.isEndsWithI1)
                        lines[index] = lines[index].Substring(0, lines[index].Length - 2) + " ♪";
                    // " I</i>"
                    else if (item.isEndsWithI2)
                        lines[index] = lines[index].Substring(0, lines[index].Length - 6) + " ♪</i>";
                    // " </i>I"
                    else if (item.isEndsWithI3)
                        lines[index] = lines[index].Substring(0, lines[index].Length - 6) + " </i>♪";
                    // "</i> I"
                    else if (item.isEndsWithI4)
                        lines[index] = lines[index].Substring(0, lines[index].Length - 6) + "</i> ♪";
                }
            }

            if (lines.Count == 1)
            {
                string line = lines[0];
                if (line.StartsWith("- ") == false)
                {
                    int index = -1;
                    if ((index = line.IndexOf(". - ")) != -1 ||
                        (index = line.IndexOf("? - ")) != -1 ||
                        (index = line.IndexOf("! - ")) != -1)
                    {
                        lines.Clear();
                        lines.Add("- " + line.Substring(0, index + 1));
                        lines.Add(line.Substring(index + 2));
                    }
                }
            }

            return lines;
        }

        private static SubtitleError CheckSubtitleMultipleLines(List<string> lines, bool cleanHICaseInsensitive)
        {
            if (lines == null || lines.Count == 0)
                return SubtitleError.Empty_Line;

            SubtitleError subtitleError = SubtitleError.None;

            if (lines.Count == 1)
            {
                if (regexMergedLinesWithHI.IsMatch(lines[0]))
                {
                    subtitleError |= SubtitleError.Hearing_Impaired;
                }
            }

            if (lines.Count > 1)
            {
                string firstLine = lines[0];
                string lastLine = lines[lines.Count - 1];

                if (IsHearingImpairedMultipleLines_RoundBrackets(firstLine, lastLine))
                {
                    if (lines.Skip(1).Take(lines.Count - 2).Any(line => line.Contains("(") || line.Contains(")")) == false)
                    {
                        subtitleError |= SubtitleError.Hearing_Impaired;
                    }
                }

                if (IsHearingImpairedMultipleLines_SquareBrackets(firstLine, lastLine))
                {
                    if (lines.Skip(1).Take(lines.Count - 2).Any(line => line.Contains("[") || line.Contains("]")) == false)
                    {
                        subtitleError |= SubtitleError.Hearing_Impaired;
                    }
                }

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

            if (lines.Count == 3)
            {
                string line1 = lines[0];
                string line2 = lines[1];
                string line3 = lines[2];

                if (line1 == "<i>-" &&
                    line2.StartsWith("- ") == false && line2.StartsWith("<i>") == false && line2.EndsWith("</i>") == false &&
                    line3.StartsWith("- ") && line3.EndsWith("</i>"))
                {
                    subtitleError |= SubtitleError.Redundant_Spaces;
                }
                else if (line1 == "<i>" &&
                    line2.StartsWith("- ") == false && line2.StartsWith("<i>") == false && line2.EndsWith("</i>") == false &&
                    line3.StartsWith("<i>") == false && line3.EndsWith("</i>"))
                {
                    subtitleError |= SubtitleError.Redundant_Spaces;
                }
                else if (line1 == "- <i>" &&
                    line2.StartsWith("- ") && line2.StartsWith("<i>") == false && line2.EndsWith("</i>") == false &&
                    line3.StartsWith("- ") && line3.EndsWith("</i>"))
                {
                    subtitleError |= SubtitleError.Redundant_Spaces;
                }
                else if (line1.StartsWith("<i>") && line1.EndsWith("</i>") == false &&
                    line2.StartsWith("- ") == false && line2.StartsWith("<i>") == false && line2.EndsWith("</i>") == false &&
                    line3.StartsWith("</i>"))
                {
                    subtitleError |= SubtitleError.Redundant_Spaces;
                }
            }

            if (lines.Count == 2)
            {
                string line1 = lines[0];
                string line2 = lines[1];

                if (line1 == "<i>-" && line2.StartsWith("- ") && line2.EndsWith("</i>"))
                {
                    subtitleError |= SubtitleError.Redundant_Spaces;
                }
                else if (line1 == "<i>" && line2.StartsWith("<i>") == false && line2.EndsWith("</i>"))
                {
                    subtitleError |= SubtitleError.Redundant_Spaces;
                }
                else if (line1 == "- <i>" && line2.StartsWith("- ") && line2.EndsWith("</i>"))
                {
                    subtitleError |= SubtitleError.Redundant_Spaces;
                }
                else if (line1.StartsWith("<i>") && line1.EndsWith("</i>") == false && line2 == "</i>")
                {
                    subtitleError |= SubtitleError.Redundant_Spaces;
                }
                else if (line1.StartsWith("<i>") && line1.EndsWith("</i>") == false && line2.StartsWith("</i>"))
                {
                    subtitleError |= SubtitleError.Redundant_Spaces;
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
                var results = lines.Select((line, index) => new
                {
                    line,
                    index,
                    isMatchHIPrefix = (cleanHICaseInsensitive ? regexHIPrefixCI : regexHIPrefix).IsMatch(line)
                }).ToArray();

                if (results[0].isMatchHIPrefix && results.Skip(1).All(x => x.isMatchHIPrefix == false))
                {
                    Match match = (cleanHICaseInsensitive ? regexHIPrefixCI : regexHIPrefix).Match(lines[0]);
                    if (lines[0] != match.Groups["Subtitle"].Value)
                        subtitleError |= SubtitleError.Hearing_Impaired;
                }
                else if (results.Count(x => x.isMatchHIPrefix) > 1)
                {
                    foreach (var item in results)
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
                    isStartsWithNote = line.StartsWith("♪"),
                    isEndsWithDots = line.EndsWith("..."),
                    isEndsWithComma = line.EndsWith(","),
                    isEndsWithPeriod = line.EndsWith("."),
                    isEndsWithQuestionMark = line.EndsWith("?"),
                    isEndsWithExclamationMark = line.EndsWith("!"),
                    isEndsWithLowerCaseLetter = regexEndsWithLowerCaseLetter.IsMatch(line)
                }).ToArray();

                if (resultsDialog[0].isStartsWithDots && resultsDialog.Skip(1).All(x => x.isMatchDialog))
                {
                    subtitleError |= SubtitleError.Dialog_Error;
                }
                else if (resultsDialog[0].isStartsWithNote && resultsDialog.Skip(1).All(x => x.isMatchDialog))
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
                else if (resultsDialog[0].isMatchDialog && resultsDialog.Skip(1).All(x => x.isStartsWithNote))
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
                            // do nothing
                        }
                        else if (resultsDialog[0].isMatchDialog &&
                            resultsDialog[0].isContainsDialog_CapitalLetter &&
                            resultsDialog[0].isEndsWithDots)
                        {
                            // do nothing
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
                            // do nothing
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
                    string firstCharSecondLine = (lines[1][0]).ToString();

                    if (regexLowerLetter.IsMatch(firstCharFirstLine) && (
                        resultsDialog[0].isEndsWithDots ||
                        resultsDialog[0].isEndsWithPeriod ||
                        resultsDialog[0].isEndsWithQuestionMark ||
                        resultsDialog[0].isEndsWithExclamationMark
                        ))
                    {
                        subtitleError |= SubtitleError.Dialog_Error;
                    }
                    else if (regexCapitalLetter.IsMatch(firstCharFirstLine))
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

                var resultsNotes = lines.Select((line, index) => new
                {
                    line,
                    index,

                    isStartsWithNote1 = line.StartsWith("♪"),
                    isStartsWithNote2 = line.StartsWith("-♪"),
                    isStartsWithNote3 = line.StartsWith("- ♪"),
                    isStartsWithNote4 = line.StartsWith("<i>♪"),
                    isStartsWithNote5 = line.StartsWith("<i>-♪"),
                    isStartsWithNote6 = line.StartsWith("<i>- ♪"),
                    isStartsWithNote7 = line.StartsWith("-<i>♪"),
                    isStartsWithNote8 = line.StartsWith("- <i>♪"),

                    isEndsWithNote1 = line.EndsWith("♪"),
                    isEndsWithNote2 = line.EndsWith("♪</i>"),
                    isEndsWithNote3 = line.EndsWith("</i>♪"),
                    isEndsWithNote4 = line.EndsWith("</i> ♪"),

                    isStartsWithAmpersand1 = line.StartsWith("&"),
                    isStartsWithAmpersand2 = line.StartsWith("-&"),
                    isStartsWithAmpersand3 = line.StartsWith("- &"),
                    isStartsWithAmpersand4 = line.StartsWith("<i>&"),
                    isStartsWithAmpersand5 = line.StartsWith("<i>-&"),
                    isStartsWithAmpersand6 = line.StartsWith("<i>- &"),
                    isStartsWithAmpersand7 = line.StartsWith("-<i>&"),
                    isStartsWithAmpersand8 = line.StartsWith("- <i>&"),

                    isEndsWithAmpersand1 = line.EndsWith("&"),
                    isEndsWithAmpersand2 = line.EndsWith("&</i>"),
                    isEndsWithAmpersand3 = line.EndsWith("</i>&"),
                    isEndsWithAmpersand4 = line.EndsWith("</i> &"),

                    isStartsWithI1 = line.StartsWith("I "),
                    isStartsWithI2 = line.StartsWith("-I "),
                    isStartsWithI3 = line.StartsWith("- I "),
                    isStartsWithI4 = line.StartsWith("<i>I "),
                    isStartsWithI5 = line.StartsWith("<i>-I "),
                    isStartsWithI6 = line.StartsWith("<i>- I "),
                    isStartsWithI7 = line.StartsWith("-<i>I "),
                    isStartsWithI8 = line.StartsWith("- <i>I "),

                    isEndsWithI1 = line.EndsWith(" I"),
                    isEndsWithI2 = line.EndsWith(" I</i>"),
                    isEndsWithI3 = line.EndsWith(" </i>I"),
                    isEndsWithI4 = line.EndsWith("</i> I")
                }).Select(x => new
                {
                    x.line,
                    x.index,

                    x.isStartsWithNote1,
                    x.isStartsWithNote2,
                    x.isStartsWithNote3,
                    x.isStartsWithNote4,
                    x.isStartsWithNote5,
                    x.isStartsWithNote6,
                    x.isStartsWithNote7,
                    x.isStartsWithNote8,
                    isStartsWithNote =
                        x.isStartsWithNote1 ||
                        x.isStartsWithNote2 ||
                        x.isStartsWithNote3 ||
                        x.isStartsWithNote4 ||
                        x.isStartsWithNote5 ||
                        x.isStartsWithNote6 ||
                        x.isStartsWithNote7 ||
                        x.isStartsWithNote8,

                    x.isEndsWithNote1,
                    x.isEndsWithNote2,
                    x.isEndsWithNote3,
                    x.isEndsWithNote4,
                    isEndsWithNote =
                        x.isEndsWithNote1 ||
                        x.isEndsWithNote2 ||
                        x.isEndsWithNote3 ||
                        x.isEndsWithNote4,

                    x.isStartsWithAmpersand1,
                    x.isStartsWithAmpersand2,
                    x.isStartsWithAmpersand3,
                    x.isStartsWithAmpersand4,
                    x.isStartsWithAmpersand5,
                    x.isStartsWithAmpersand6,
                    x.isStartsWithAmpersand7,
                    x.isStartsWithAmpersand8,
                    isStartsWithAmpersand =
                        x.isStartsWithAmpersand1 ||
                        x.isStartsWithAmpersand2 ||
                        x.isStartsWithAmpersand3 ||
                        x.isStartsWithAmpersand4 ||
                        x.isStartsWithAmpersand5 ||
                        x.isStartsWithAmpersand6 ||
                        x.isStartsWithAmpersand7 ||
                        x.isStartsWithAmpersand8,

                    x.isEndsWithAmpersand1,
                    x.isEndsWithAmpersand2,
                    x.isEndsWithAmpersand3,
                    x.isEndsWithAmpersand4,
                    isEndsWithAmpersand =
                        x.isEndsWithAmpersand1 ||
                        x.isEndsWithAmpersand2 ||
                        x.isEndsWithAmpersand3 ||
                        x.isEndsWithAmpersand4,

                    x.isStartsWithI1,
                    x.isStartsWithI2,
                    x.isStartsWithI3,
                    x.isStartsWithI4,
                    x.isStartsWithI5,
                    x.isStartsWithI6,
                    x.isStartsWithI7,
                    x.isStartsWithI8,
                    isStartsWithI =
                        x.isStartsWithI1 ||
                        x.isStartsWithI2 ||
                        x.isStartsWithI3 ||
                        x.isStartsWithI4 ||
                        x.isStartsWithI5 ||
                        x.isStartsWithI6 ||
                        x.isStartsWithI7 ||
                        x.isStartsWithI8,

                    x.isEndsWithI1,
                    x.isEndsWithI2,
                    x.isEndsWithI3,
                    x.isEndsWithI4,
                    isEndsWithI =
                        x.isEndsWithI1 ||
                        x.isEndsWithI2 ||
                        x.isEndsWithI3 ||
                        x.isEndsWithI4
                }).Select(x => new
                {
                    x.line,
                    x.index,

                    x.isStartsWithNote1,
                    x.isStartsWithNote2,
                    x.isStartsWithNote3,
                    x.isStartsWithNote4,
                    x.isStartsWithNote5,
                    x.isStartsWithNote6,
                    x.isStartsWithNote7,
                    x.isStartsWithNote8,
                    x.isStartsWithNote,

                    x.isEndsWithNote1,
                    x.isEndsWithNote2,
                    x.isEndsWithNote3,
                    x.isEndsWithNote4,
                    x.isEndsWithNote,

                    x.isStartsWithAmpersand1,
                    x.isStartsWithAmpersand2,
                    x.isStartsWithAmpersand3,
                    x.isStartsWithAmpersand4,
                    x.isStartsWithAmpersand5,
                    x.isStartsWithAmpersand6,
                    x.isStartsWithAmpersand7,
                    x.isStartsWithAmpersand8,
                    x.isStartsWithAmpersand,

                    x.isEndsWithAmpersand1,
                    x.isEndsWithAmpersand2,
                    x.isEndsWithAmpersand3,
                    x.isEndsWithAmpersand4,
                    x.isEndsWithAmpersand,

                    x.isStartsWithI1,
                    x.isStartsWithI2,
                    x.isStartsWithI3,
                    x.isStartsWithI4,
                    x.isStartsWithI5,
                    x.isStartsWithI6,
                    x.isStartsWithI7,
                    x.isStartsWithI8,
                    x.isStartsWithI,

                    x.isEndsWithI1,
                    x.isEndsWithI2,
                    x.isEndsWithI3,
                    x.isEndsWithI4,
                    x.isEndsWithI,

                    // ♪ Line
                    isStartsWithAndNotEndWithNote =
                        x.isStartsWithNote &&
                        x.isEndsWithNote == false && x.isEndsWithAmpersand == false && x.isEndsWithI == false,
                    //   Line ♪
                    isNotStartWithAndEndsWithNote =
                        x.isStartsWithNote == false && x.isStartsWithAmpersand == false && x.isStartsWithI == false &&
                        x.isEndsWithNote,

                    // & Line
                    isStartsWithAndNotEndWithAmpersand =
                        x.isStartsWithAmpersand &&
                        x.isEndsWithNote == false && x.isEndsWithAmpersand == false && x.isEndsWithI == false,
                    //   Line &
                    isNotStartWithAndEndsWithAmpersand =
                        x.isStartsWithNote == false && x.isStartsWithAmpersand == false && x.isStartsWithI == false &&
                        x.isEndsWithAmpersand,

                    // I Line
                    isStartsWithAndNotEndWithI =
                        x.isStartsWithI &&
                        x.isEndsWithNote == false && x.isEndsWithAmpersand == false && x.isEndsWithI == false,
                    //   Line I
                    isNotStartWithAndEndsWithI =
                        x.isStartsWithNote == false && x.isStartsWithAmpersand == false && x.isStartsWithI == false &&
                        x.isEndsWithI,

                    isRegularLine =
                        x.isStartsWithNote == false && x.isEndsWithNote == false &&
                        x.isStartsWithAmpersand == false && x.isEndsWithAmpersand == false &&
                        x.isStartsWithI == false && x.isEndsWithI == false
                }).ToArray();

                List<int> startsWithAmpersand = new List<int>();
                List<int> startsWithI = new List<int>();
                List<int> endsWithAmpersand = new List<int>();
                List<int> endsWithI = new List<int>();

                for (int i = 0; i < resultsNotes.Length; i++)
                {
                    // ♪ Line1
                    //   Line2
                    //   Line3 &/I
                    if (resultsNotes[i].isStartsWithAndNotEndWithNote)
                    {
                        for (int j = i + 1; j < resultsNotes.Length; j++)
                        {
                            if (resultsNotes[j].isNotStartWithAndEndsWithAmpersand)
                            {
                                endsWithAmpersand.Add(j);
                                i = j; // j + 1 - 1, +1 is for the next line, -1 is to counter the i++
                                break;
                            }
                            else if (resultsNotes[j].isNotStartWithAndEndsWithI)
                            {
                                endsWithI.Add(j);
                                i = j; // j + 1 - 1, +1 is for the next line, -1 is to counter the i++
                                break;
                            }
                            else if (resultsNotes[j].isRegularLine == false)
                            {
                                i = j - 1; // -1 is to counter the i++
                                break;
                            }
                        }
                    }
                    // & Line1
                    //   Line2
                    //   Line3 ♪
                    else if (resultsNotes[i].isStartsWithAndNotEndWithAmpersand)
                    {
                        for (int j = i + 1; j < resultsNotes.Length; j++)
                        {
                            if (resultsNotes[j].isNotStartWithAndEndsWithNote)
                            {
                                startsWithAmpersand.Add(i);
                                i = j; // j + 1 - 1, +1 is for the next line, -1 is to counter the i++
                                break;
                            }
                            else if (resultsNotes[j].isRegularLine == false)
                            {
                                i = j - 1; // -1 is to counter the i++
                                break;
                            }
                        }
                    }
                    // I Line1
                    //   Line2
                    //   Line3 ♪
                    else if (resultsNotes[i].isStartsWithAndNotEndWithI)
                    {
                        for (int j = i + 1; j < resultsNotes.Length; j++)
                        {
                            if (resultsNotes[j].isNotStartWithAndEndsWithNote)
                            {
                                startsWithI.Add(i);
                                i = j; // j + 1 - 1, +1 is for the next line, -1 is to counter the i++
                                break;
                            }
                            else if (resultsNotes[j].isRegularLine == false)
                            {
                                i = j - 1; // -1 is to counter the i++
                                break;
                            }
                        }
                    }
                }

                if (startsWithAmpersand.Count > 0 ||
                    startsWithI.Count > 0 ||
                    endsWithAmpersand.Count > 0 ||
                    endsWithI.Count > 0)
                {
                    subtitleError |= SubtitleError.Notes_Error;
                }
            }

            if (lines.Count == 1)
            {
                string line = lines[0];
                if (line.StartsWith("- ") == false)
                {
                    int index = -1;
                    if ((index = line.IndexOf(". - ")) != -1 ||
                        (index = line.IndexOf("? - ")) != -1 ||
                        (index = line.IndexOf("! - ")) != -1)
                    {
                        subtitleError |= SubtitleError.Dialog_Error;
                    }
                }
            }

            return subtitleError;
        }

        #endregion

        #region Clean Single Line Post

        private static string CleanSubtitleLinePost(string line)
        {
            if (IsEmptyLine(line))
                return null;

            return line;
        }

        private static SubtitleError CheckSubtitleLinePost(string line)
        {
            if (IsEmptyLine(line))
                return SubtitleError.Empty_Line;

            SubtitleError subtitleError = SubtitleError.None;

            return subtitleError;
        }

        #endregion

        #region Clean Multiple Lines Post

        public static readonly Regex regexLineWithSingleWord = new Regex(@"^\w+,?$");

        private static List<string> CleanSubtitleMultipleLinesPost(List<string> lines)
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
                        lines[i - 1] = prevLine.Substring(0, prevLine.Length - 4);
                        lines[i] = line.Substring("<i>".Length);
                    }

                    if (IsMergeShortLineWithLongLine(prevLine, line))
                    {
                        lines[i - 1] = lines[i - 1] + " " + line;
                        lines.RemoveAt(i);
                        i--;
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

        private static SubtitleError CheckSubtitleMultipleLinesPost(List<string> lines)
        {
            if (lines == null || lines.Count == 0)
                return SubtitleError.Empty_Line;

            SubtitleError subtitleError = SubtitleError.None;

            // Line 1 - Dialog
            bool isMatchDialog = regexDialog.IsMatch(lines[0]);
            bool isContainsDialog = regexContainsDialog.IsMatch(lines[0]);
            if (isMatchDialog == false && isContainsDialog)
                subtitleError |= SubtitleError.Dialog_Error;

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

                    if (IsMergeShortLineWithLongLine(prevLine, line))
                    {
                        subtitleError |= SubtitleError.Merge_Lines;
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
                            subtitleError |= SubtitleError.Dialog_Error;
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
                        subtitleError |= SubtitleError.OCR_Error;
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
            new FindAndReplace(new Regex(@"^[-!?:_@#.*♪♫¶ ]*$", RegexOptions.Compiled), "", SubtitleError.Empty_Line)
            ,new FindAndReplace(new Regex(@"^<i>[-!?:_@#.*♪♫¶ ]*</i>$", RegexOptions.Compiled), "", SubtitleError.Empty_Line)
        };

        #endregion

        #region Not Subtitle

        public static readonly FindAndReplace[] NotSubtitle = new FindAndReplace[] {
            new FindAndReplace(new Regex(@"(?i)AllSubs\.org", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Best watched using", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Captioned by", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Captioning by", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Captioning made possible by", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Captioning performed by", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Captioning sponsored by", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Captions by", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Captions copyright", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Captions made possible by", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Captions performed by", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Captions sponsored by", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Captions, Inc\.", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Closed Caption", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Closed-Caption", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Contain Strong Language", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Contains Strong Language", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Copyright Australian", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Corrected by", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)DVDRIP by", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)ENGLISH - PSDH", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)ENGLISH - SDH", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)ENGLISH - US", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)ENGLISH PSDH", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)ENGLISH SDH", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Eng subs", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Eng subtitles", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)ExplosiveSkull", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)HighCode", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)MKV Player", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)NETFLIX PRESENTS", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)OCR by", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Open Subtitles", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)OpenSubtitles", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Proofread by", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            // not: gripped by
            ,new FindAndReplace(new Regex(@"(?<!g|G)(?i)Rip by", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?<!g|G)(?i)Ripped by", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)SUBTITLES EDITED BY", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)SharePirate\.com", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Subs by", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Subscene", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Subtitled By", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Subtitles by", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Subtitles:", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Subtitletools\.com", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Subtitling", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Sync by", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Synced & corrected", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Synced and corrected", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Synced by", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Synced:", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Synchronization by", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Synchronized by", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)ThePirateBay", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Translated by", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Translation by", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Translations by", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)

            ,new FindAndReplace(new Regex(@"DIRECTED BY", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"PRODUCED BY", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"WRITTEN BY", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)

            ,new FindAndReplace(new Regex(@"(?i)<font color=", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)^-?\s*<font>.*?</\s*font>$", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)^-?\s*<font\s+.*?</\s*font>$", RegexOptions.Compiled), "", SubtitleError.Not_Subtitle)
        };

        #endregion

        #region Punctuations

        public static readonly FindAndReplace[] Punctuations = new FindAndReplace[] {
            new FindAndReplace(new Regex(@"'’", RegexOptions.Compiled), "'", SubtitleError.Punctuations_Error)
            ,new FindAndReplace(new Regex(@"[´`‘’]", RegexOptions.Compiled), "'", SubtitleError.Punctuations_Error)
            ,new FindAndReplace(new Regex(@"^'{3}", RegexOptions.Compiled), "\"'", SubtitleError.Punctuations_Error)
            ,new FindAndReplace(new Regex(@"'{3}$", RegexOptions.Compiled), "'\"", SubtitleError.Punctuations_Error)
            ,new FindAndReplace(new Regex(@"[“”]", RegexOptions.Compiled), "\"", SubtitleError.Punctuations_Error)
            ,new FindAndReplace(new Regex(@"(\\x22)+", RegexOptions.Compiled), "\"", SubtitleError.Punctuations_Error)
            ,new FindAndReplace(new Regex(@"'{2}", RegexOptions.Compiled), "\"", SubtitleError.Punctuations_Error)
            ,new FindAndReplace(new Regex(@"""{2,}", RegexOptions.Compiled), "\"", SubtitleError.Punctuations_Error)
            ,new FindAndReplace(new Regex(@"[?!:](?<Dot>\.)(?:\s|\b|$)", RegexOptions.Compiled), "Dot", string.Empty, SubtitleError.Punctuations_Error)
            ,new FindAndReplace(new Regex(@"(\w|\s)(?<Dot>\.)[?!:]", RegexOptions.Compiled), "Dot", string.Empty, SubtitleError.Punctuations_Error)
            ,new FindAndReplace(new Regex(@"\(\?\)", RegexOptions.Compiled), "?", SubtitleError.Punctuations_Error)
            ,new FindAndReplace(new Regex(@"\(!\)", RegexOptions.Compiled), "!", SubtitleError.Punctuations_Error)
            ,new FindAndReplace(new Regex(@"\s\?", RegexOptions.Compiled), "?", SubtitleError.Punctuations_Error)
            ,new FindAndReplace(new Regex(@"\s!", RegexOptions.Compiled), "!", SubtitleError.Punctuations_Error)
            ,new FindAndReplace(new Regex(@"\s:", RegexOptions.Compiled), ":", SubtitleError.Punctuations_Error)
            ,new FindAndReplace(new Regex(@"[‐=]", RegexOptions.Compiled), "-", SubtitleError.Punctuations_Error)
            ,new FindAndReplace(new Regex(@"[A-ZÁ-Úa-zá-ú](?<Dash>-)\s-\s[A-ZÁ-Úa-zá-ú]", RegexOptions.Compiled), "Dash", "...", SubtitleError.Punctuations_Error)
            ,new FindAndReplace(new Regex(@"[A-ZÁ-Úa-zá-ú](?<Dash>\s-\s-\s?)[A-ZÁ-Ú]", RegexOptions.Compiled), "Dash", "... - ", SubtitleError.Punctuations_Error)
            ,new FindAndReplace(new Regex(@"-\s-", RegexOptions.Compiled), "-", SubtitleError.Punctuations_Error)
            ,new FindAndReplace(new Regex(@"^[—–―‒]", RegexOptions.Compiled), "-", SubtitleError.Punctuations_Error)
            ,new FindAndReplace(new Regex(@"^<i>(?<Dash>[—–―‒])</i>", RegexOptions.Compiled), "Dash", "-", SubtitleError.Punctuations_Error)
            ,new FindAndReplace("Three Dots", new Regex(@"[…—–―‒]", RegexOptions.Compiled), "...", SubtitleError.Punctuations_Error)
            ,new FindAndReplace("Three Dots", new Regex(@"\.\s\.\.", RegexOptions.Compiled), "...", SubtitleError.Punctuations_Error)
            ,new FindAndReplace("Three Dots", new Regex(@"\.\.\s\.", RegexOptions.Compiled), "...", SubtitleError.Punctuations_Error)
            ,new FindAndReplace("Three Dots", new Regex(@"\.\s\.\s\.", RegexOptions.Compiled), "...", SubtitleError.Punctuations_Error)
            ,new FindAndReplace("Three Dots", new Regex(@"-{2,}", RegexOptions.Compiled), "...", SubtitleError.Punctuations_Error)
            ,new FindAndReplace("Three Dots", new Regex(@",{2,}", RegexOptions.Compiled), "...", SubtitleError.Punctuations_Error)
            ,new FindAndReplace("Three Dots", new Regex(@"\.{4,}", RegexOptions.Compiled), "...", SubtitleError.Punctuations_Error)
            ,new FindAndReplace(new Regex(@"[;，]", RegexOptions.Compiled), ",", SubtitleError.Punctuations_Error)
        };

        #endregion

        #region Notes

        public static readonly FindAndReplace[] Notes = new FindAndReplace[] {
            // - ? Lyrics ?
            new FindAndReplace(new Regex(@"^(?:-\s*)?(?<QM>\?+)\s+.*?(?<QM>\?+)$", RegexOptions.Compiled), "QM", "♪", SubtitleError.Notes_Error)
            // - ♪ Lyrics ?
            ,new FindAndReplace(new Regex(@"^(?:-\s*)?♪+\s+.*?\s(?<QM>\?+)$", RegexOptions.Compiled), "QM", "♪", SubtitleError.Notes_Error)
            // - ? Lyrics ♪
            ,new FindAndReplace(new Regex(@"^(?:-\s*)?(?<QM>\?+)\s+.*?♪+$", RegexOptions.Compiled), "QM", "♪", SubtitleError.Notes_Error)

            ,new FindAndReplace(new Regex(@"[♫¶*]", RegexOptions.Compiled), "♪", SubtitleError.Notes_Error)
            ,new FindAndReplace(new Regex(@"<i>[♪♫¶*#]+</i>", RegexOptions.Compiled), "♪", SubtitleError.Notes_Error)
            ,new FindAndReplace(new Regex(@"\#(?![0-9])", RegexOptions.Compiled), "♪", SubtitleError.Notes_Error)
            ,new FindAndReplace(new Regex(@"♪{2,}", RegexOptions.Compiled), "♪", SubtitleError.Notes_Error)

            // ^j$
            ,new FindAndReplace(new Regex(@"(?i)^(?<OCR>j)$", RegexOptions.Compiled), "OCR", "♪", SubtitleError.Notes_Error)
            // ^j Text
            // ^j"Text
            ,new FindAndReplace(new Regex(@"(?i)^(?<OCR>(j+\s|j+['""&!])+)", RegexOptions.Compiled), "OCR", "♪", SubtitleError.Notes_Error)
            // ^"j Text
            ,new FindAndReplace(new Regex(@"(?i)^(?<OCR>(['""&!]j)+)\s", RegexOptions.Compiled), "OCR", "♪", SubtitleError.Notes_Error)
            // Text j"$
            ,new FindAndReplace(new Regex(@"(?i)\s(?<OCR>(j+['""&!])+)$", RegexOptions.Compiled), "OCR", "♪", SubtitleError.Notes_Error)
            // Text"j$
            ,new FindAndReplace(new Regex(@"(?i)(?<OCR>(['""&!]j+)+)$", RegexOptions.Compiled), "OCR", "♪", SubtitleError.Notes_Error)
            // Text j$
            ,new FindAndReplace(new Regex(@"(?i)\s(?<OCR>j+)$", RegexOptions.Compiled), "OCR", "♪", SubtitleError.Notes_Error)
            // TextJ$
            ,new FindAndReplace(new Regex(@"[a-zá-ú](?<OCR>J+)$", RegexOptions.Compiled), "OCR", "♪", SubtitleError.Notes_Error)

            // ♪ Text j"
            ,new FindAndReplace(new Regex(@"(?i)^♪.*?(?<OCR>j['""&!])$", RegexOptions.Compiled), "OCR", "♪", SubtitleError.Notes_Error)

            // ♪ Text &
            ,new FindAndReplace(new Regex(@"^[♪&].*?(?<OCR>&)$", RegexOptions.Compiled), "OCR", "♪", SubtitleError.Notes_Error)
            // & Text ♪
            ,new FindAndReplace(new Regex(@"^(?<OCR>&).*?[♪&]$", RegexOptions.Compiled), "OCR", "♪", SubtitleError.Notes_Error)

            // ♪ Text I
            ,new FindAndReplace(new Regex(@"^♪.*?\s+(?<OCR>I)$", RegexOptions.Compiled), "OCR", "♪", SubtitleError.Notes_Error)
            // I Text ♪
            ,new FindAndReplace(new Regex(@"^(?<OCR>I)\s+.*?♪$", RegexOptions.Compiled), "OCR", "♪", SubtitleError.Notes_Error)
        };

        #endregion

        #region Hearing Impaired Full Line

        public static readonly FindAndReplace[] HearingImpairedFullLine = new FindAndReplace[] {
            // ^(HI)$
            // ^- (HI)$
            new FindAndReplace("HI Full Line", new Regex(@"^\s*-?\s*\(.*?\)\s*$", RegexOptions.Compiled), "", SubtitleError.Hearing_Impaired)
            ,new FindAndReplace("HI Full Line", new Regex(@"^\s*-?\s*\[.*?\]\s*$", RegexOptions.Compiled), "", SubtitleError.Hearing_Impaired)

            // ^<i>- ♪$
            ,new FindAndReplace(new Regex(@"^\s*<i>\s*-\s*♪+\s*$", RegexOptions.Compiled), "<i>", SubtitleError.Hearing_Impaired)
            // ^- ♪</i>$
            ,new FindAndReplace(new Regex(@"^\s*-\s*♪+\s*</i>\s*$", RegexOptions.Compiled), "</i>", SubtitleError.Hearing_Impaired)

            // ^<i>(HI)</i>$
            // ^- <i>(HI)</i>$
            // ^<i>- (HI)</i>$
            // ^♪ <i>(HI)</i>$
            // ^<i>♪ (HI)</i>$
            ,new FindAndReplace("HI Full Line", new Regex(@"^\s*-?\s*♪?\s*<i>\s*-?\s*♪?\s*\(.*?\)\s*</i>\s*$", RegexOptions.Compiled), "", SubtitleError.Hearing_Impaired)
            ,new FindAndReplace("HI Full Line", new Regex(@"^\s*-?\s*♪?\s*<i>\s*-?\s*♪?\s*\[.*?\]\s*</i>\s*$", RegexOptions.Compiled), "", SubtitleError.Hearing_Impaired)

            ,new FindAndReplace("HI Full Line", new Regex(@"^[" + HI_CHARS + @"\[\]]+:\s*$", RegexOptions.Compiled), "", SubtitleError.Hearing_Impaired)
                    .SetRegexCI(new Regex(@"^[" + HI_CHARS_CI + @"\[\]]+:\s*$", RegexOptions.Compiled))

            ,new FindAndReplace(new Regex(@"^(?<Prefix><i>)?[" + HI_CHARS + @"]+\[.*?\]:\s*$", RegexOptions.Compiled), "${Prefix}", SubtitleError.Hearing_Impaired)
                    .SetRegexCI(new Regex(@"^(?<Prefix><i>)?[" + HI_CHARS_CI + @"]+\[.*?\]:\s*$", RegexOptions.Compiled))
            ,new FindAndReplace(new Regex(@"^(?<Prefix><i>)?[" + HI_CHARS + @"]+\(.*?\):\s*$", RegexOptions.Compiled), "${Prefix}", SubtitleError.Hearing_Impaired)
                    .SetRegexCI(new Regex(@"^(?<Prefix><i>)?[" + HI_CHARS_CI + @"]+\(.*?\):\s*$", RegexOptions.Compiled))

            ,new FindAndReplace("HI Full Line", new Regex(@"^<i>[" + HI_CHARS + @"]+\[.*?\]:\s*</i>$", RegexOptions.Compiled), "", SubtitleError.Hearing_Impaired)
                    .SetRegexCI(new Regex(@"^<i>[" + HI_CHARS_CI + @"]+\[.*?\]:\s*</i>$", RegexOptions.Compiled))
            ,new FindAndReplace("HI Full Line", new Regex(@"^<i>[" + HI_CHARS + @"]+\(.*?\):\s*</i>$", RegexOptions.Compiled), "", SubtitleError.Hearing_Impaired)
                    .SetRegexCI(new Regex(@"^<i>[" + HI_CHARS_CI + @"]+\(.*?\):\s*</i>$", RegexOptions.Compiled))
        };

        #endregion

        #region ASSA Tags

        // https://www.nikse.dk/subtitleedit/formats/assa-override-tags
        public static readonly FindAndReplace[] ASSATags = new FindAndReplace[] {
            // Italic       {\i1}
            // Bold         {\b1}
            // Underline    {\u1}
            // Strikeout    {\s1}
            // Alignment    {\an1}
            // Font spacing {\fsp5}
            new FindAndReplace(new Regex(@"\{\\(?:i|b|u|s|an|fsp)\d+}", RegexOptions.Compiled), "", SubtitleError.ASSA_Tags)
            // Primary color {\c&H0000FF&}
            // Outline color {\3c&H0000FF&}
            // Shadow color  {\4c&H000000&}
            ,new FindAndReplace(new Regex(@"\{\\(?:|3|4)c&H[A-Fa-f0-9]+&}", RegexOptions.Compiled), "", SubtitleError.ASSA_Tags)
            // Outline border {\bord8.5} {\xbord8.5} {\ybord8.5}
            // Shadow {\shad8.5} {\xshad8.5} {\yshad8.5}
            ,new FindAndReplace(new Regex(@"\{\\(?:|x|y)(?:bord|shad)[0-9.]+}", RegexOptions.Compiled), "", SubtitleError.ASSA_Tags)
            // Font size {\fs35.5}
            // Font name {\fnSofia}
            ,new FindAndReplace(new Regex(@"\{\\(?:fs|fn).+?}", RegexOptions.Compiled), "", SubtitleError.ASSA_Tags)
            // Blur edges {\be3.5}
            ,new FindAndReplace(new Regex(@"\{\\be[0-9.]+}", RegexOptions.Compiled), "", SubtitleError.ASSA_Tags)
            // Font scale {\fscx200} {\fscy200} {\fscx200\fscy200}
            ,new FindAndReplace(new Regex(@"\{(?:\\fsc(?:x|y)\d+)+}", RegexOptions.Compiled), "", SubtitleError.ASSA_Tags)
            // Text rotation {\fr45} {\frx45} {\fry45} {\frz-180}
            // Text shearing {\fax1} {\fay-1}
            ,new FindAndReplace(new Regex(@"\{\\(?:fr|fa)(?:|x|y|z)[0-9-]+}", RegexOptions.Compiled), "", SubtitleError.ASSA_Tags)
            // Alpha {\alpha&HCC} {\1a&HCC} {\3a&HCC} {\4a&HCC}
            ,new FindAndReplace(new Regex(@"\{(?:\\(?:alpha|1a|3a|4a)&H[A-Fa-f0-9]+)+}", RegexOptions.Compiled), "", SubtitleError.ASSA_Tags)
            // Reset styles {\r}
            ,new FindAndReplace(new Regex(@"\{\\r}", RegexOptions.Compiled), "", SubtitleError.ASSA_Tags)
            // Position        {\pos(250,270)}
            // Rotation origin {\org(250,270)}
            // Fade            {\fad(250,250)}
            ,new FindAndReplace(new Regex(@"\{\\(?:pos|org|fad)\(\d+,\d+\)}", RegexOptions.Compiled), "", SubtitleError.ASSA_Tags)
            // Fade (Advanced) {\fade(0,255,0,1000,1200,1300,1500)}
            // Move            {\move(350,350,1500,350)} {\move(350,350,1500,350,500,2600)}
            ,new FindAndReplace(new Regex(@"\{\\(?:fade|move)\((?:\d+,)+\d+\)}", RegexOptions.Compiled), "", SubtitleError.ASSA_Tags)
            // Clip {\clip(0,0,320,180)} {\iclip(0,0,320,180)}
            ,new FindAndReplace(new Regex(@"\{\\(?:|i)clip\(\d+,\d+,\d+,\d+\)}", RegexOptions.Compiled), "", SubtitleError.ASSA_Tags)
        };

        #endregion

        #region Redundant Spaces

        public static readonly FindAndReplace[] RedundantSpaces = new FindAndReplace[] {
            new FindAndReplace(new Regex(@"<i/>", RegexOptions.Compiled), "</i>", SubtitleError.Redundant_Spaces)
            ,new FindAndReplace(new Regex(@"</ i>", RegexOptions.Compiled), "</i>", SubtitleError.Redundant_Spaces)

            ,new FindAndReplace(new Regex(@"<u/>", RegexOptions.Compiled), "</u>", SubtitleError.Redundant_Spaces)
            ,new FindAndReplace(new Regex(@"</ u>", RegexOptions.Compiled), "</u>", SubtitleError.Redundant_Spaces)
            ,new FindAndReplace(new Regex(@"<u></u>", RegexOptions.Compiled), "", SubtitleError.Redundant_Spaces)
            ,new FindAndReplace(new Regex(@"<u>\s+</u>", RegexOptions.Compiled), " ", SubtitleError.Redundant_Spaces)

            ,new FindAndReplace(new Regex(@"^(?<Dash>-)(?<Space>\s*)(?<Italic></i>)$", RegexOptions.Compiled), "${Italic}", SubtitleError.Redundant_Spaces)
            ,new FindAndReplace(new Regex(@"^(?<Dash>-)(?<Space>\s*)(?<Italic></i>)", RegexOptions.Compiled), "${Italic}${Space}${Dash}", SubtitleError.Redundant_Spaces)
            ,new FindAndReplace(new Regex(@"<i>-\s+</i>", RegexOptions.Compiled), "- ", SubtitleError.Redundant_Spaces)

            // a<i> b </i>c => a <i>b</i> c
            ,new FindAndReplace(new Regex(@"([^ ])<i>[ ]", RegexOptions.Compiled), "$1 <i>", SubtitleError.Redundant_Spaces)
            ,new FindAndReplace(new Regex(@"[ ]</i>([^ ])", RegexOptions.Compiled), "</i> $1", SubtitleError.Redundant_Spaces)

            ,new FindAndReplace(new Regex(@"\.<i>\s+", RegexOptions.Compiled), ". <i>", SubtitleError.Redundant_Spaces)
            ,new FindAndReplace(new Regex(@",<i>\s+", RegexOptions.Compiled), ", <i>", SubtitleError.Redundant_Spaces)
            ,new FindAndReplace(new Regex(@"<i>\s+", RegexOptions.Compiled), "<i>", SubtitleError.Redundant_Spaces)
            ,new FindAndReplace(new Regex(@"\s+</i>", RegexOptions.Compiled), "</i>", SubtitleError.Redundant_Spaces)

            // " Text => "Text
            ,new FindAndReplace(new Regex(@"^(?<Prefix>[""'])\s+(?<Suffix>[A-ZÁ-Úa-zá-ú0-9])", RegexOptions.Compiled), "${Prefix}${Suffix}", SubtitleError.Redundant_Spaces)
            // Text " => Text"
            ,new FindAndReplace(new Regex(@"(?<Prefix>[A-ZÁ-Úa-zá-ú0-9.?!-])\s+(?<Suffix>[""'])$", RegexOptions.Compiled), "${Prefix}${Suffix}", SubtitleError.Redundant_Spaces)

            // 1 987 => 1987 (Positive lookahead)
            // 1 1 /2 => 1 1/2 (Negative lookahead)
            ,new FindAndReplace("Space After 1", new Regex(@"1\s+(?=[0-9.,/])(?!1/2|1 /2)", RegexOptions.Compiled), "1", SubtitleError.Redundant_Spaces)

            // 9: 55 => 9:55
			,new FindAndReplace(new Regex(@"[0-2]?\d(?<OCR>: )[0-5]\d", RegexOptions.Compiled), "OCR", ":", SubtitleError.Redundant_Spaces)

            // 1 : => 1:
			,new FindAndReplace(new Regex(@"\d(?<OCR> :)", RegexOptions.Compiled), "OCR", ":", SubtitleError.Redundant_Spaces)

            // Spaces after aphostrophes
			,new FindAndReplace(new Regex(@"(?i)[A-ZÁ-Úa-zá-ú](?<OCR>'\s|\s')(?:ll|ve|s|m|d|t|re)\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.Redundant_Spaces)

            // ma' am => ma'am
            ,new FindAndReplace(new Regex(@"(?i)ma(?<OCR>'\s|\s')am\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.Redundant_Spaces)

            // a. m. => a.m.
            ,new FindAndReplace(new Regex(@"(?i)(?:a|p)\.(?<OCR>\s)m\.", RegexOptions.Compiled), "OCR", "", SubtitleError.Redundant_Spaces)

            // Remove space after two or more consecutive dots
            ,new FindAndReplace(new Regex(@"^(?:<i>)?\.{2,}(?<OCR>\s+)", RegexOptions.Compiled), "OCR", "", SubtitleError.Redundant_Spaces)

            // x ...
            ,new FindAndReplace(new Regex(@"(?<OCR>\s+)\.{3}(?:</i>)?$", RegexOptions.Compiled), "OCR", "", SubtitleError.Redundant_Spaces)
            // a ... a
            ,new FindAndReplace(new Regex(@"[A-ZÁ-Úa-zá-ú](?<OCR>\s+)\.{3}\s+[A-ZÁ-Úa-zá-ú]", RegexOptions.Compiled), "OCR", "", SubtitleError.Redundant_Spaces)

            // 1, 000
            ,new FindAndReplace(new Regex(@"\b\d+(?<OCR>, | ,)0{3}\b", RegexOptions.Compiled), "OCR", ",", SubtitleError.Redundant_Spaces)
            
            // Text . Next text
            ,new FindAndReplace(new Regex(@"[A-ZÁ-Úa-zá-ú](?<OCR>\s\.)\s[A-ZÁ-Úa-zá-ú]", RegexOptions.Compiled), "OCR", ". ", SubtitleError.Redundant_Spaces)

            // Ordinal Numbers: 1st, 2nd, 3rd, 4th
            ,new FindAndReplace("Ordinal Numbers", new Regex(@"\b\d*1(?<OCR>\s+)st\b", RegexOptions.Compiled), "OCR", "", SubtitleError.Redundant_Spaces)
            ,new FindAndReplace("Ordinal Numbers", new Regex(@"\b\d*2(?<OCR>\s+)nd\b", RegexOptions.Compiled), "OCR", "", SubtitleError.Redundant_Spaces)
            ,new FindAndReplace("Ordinal Numbers", new Regex(@"\b\d*3(?<OCR>\s+)rd\b", RegexOptions.Compiled), "OCR", "", SubtitleError.Redundant_Spaces)
            ,new FindAndReplace("Ordinal Numbers", new Regex(@"\b\d*[4-9](?<OCR>\s+)th\b", RegexOptions.Compiled), "OCR", "", SubtitleError.Redundant_Spaces)
        };

        #endregion

        #region Hearing Impaired

        public static readonly FindAndReplace[] HearingImpaired = new FindAndReplace[] {
            // <i>- (laughting)</i> =>
            new FindAndReplace(new Regex(@"<i>-\s*\(.*?\)</i>", RegexOptions.Compiled), "", SubtitleError.Hearing_Impaired)
            ,new FindAndReplace(new Regex(@"<i>-\s*\[.*?\]</i>", RegexOptions.Compiled), "", SubtitleError.Hearing_Impaired)

            // <i>- (laughting) => <i>
            ,new FindAndReplace(new Regex(@"^<i>-\s*\(.*?\)$", RegexOptions.Compiled), "<i>", SubtitleError.Hearing_Impaired)
            ,new FindAndReplace(new Regex(@"^<i>-\s*\[.*?\]$", RegexOptions.Compiled), "<i>", SubtitleError.Hearing_Impaired)

            // - (laughting)</i> => </i>
            ,new FindAndReplace(new Regex(@"^-\s*\(.*?\)</i>$", RegexOptions.Compiled), "</i>", SubtitleError.Hearing_Impaired)
            ,new FindAndReplace(new Regex(@"^-\s*\[.*?\]</i>$", RegexOptions.Compiled), "</i>", SubtitleError.Hearing_Impaired)

            // - (MAN): Text => - Text
            ,new FindAndReplace(new Regex(@"^(?<Prefix>- )?\(.*?\)(:\s*)?(?<Subtitle>.+)$", RegexOptions.Compiled), "${Prefix}${Subtitle}", SubtitleError.Hearing_Impaired)
            ,new FindAndReplace(new Regex(@"^(?<Prefix>- )?\[.*?\](:\s*)?(?<Subtitle>.+)$", RegexOptions.Compiled), "${Prefix}${Subtitle}", SubtitleError.Hearing_Impaired)

            // <i>- MAN (laughting): Text</i> => <i>- Text</i>
            ,new FindAndReplace(new Regex(@"^(?<Prefix><i>\s*-?\s*)[A-ZÁ-Ú]*\s*\(.*?\)(:\s*)?(?<Subtitle>.+?)(?<Suffix></i>)$", RegexOptions.Compiled), "${Prefix}${Subtitle}${Suffix}", SubtitleError.Hearing_Impaired)
            ,new FindAndReplace(new Regex(@"^(?<Prefix><i>\s*-?\s*)[A-ZÁ-Ú]*\s*\[.*?\](:\s*)?(?<Subtitle>.+?)(?<Suffix></i>)$", RegexOptions.Compiled), "${Prefix}${Subtitle}${Suffix}", SubtitleError.Hearing_Impaired)

            // <i>MAN (laughting): => <i>
            ,new FindAndReplace(new Regex(@"^(?<Prefix><i>)[A-ZÁ-Ú]*\s*\(.*?\):$", RegexOptions.Compiled), "${Prefix}", SubtitleError.Hearing_Impaired)
            ,new FindAndReplace(new Regex(@"^(?<Prefix><i>)[A-ZÁ-Ú]*\s*\[.*?\]:$", RegexOptions.Compiled), "${Prefix}", SubtitleError.Hearing_Impaired)

            // <i>(laughting) Text => <i>Text
            ,new FindAndReplace(new Regex(@"^(?<Prefix><i>)\s*\(.*?\)\s*(?<Subtitle>.+)$", RegexOptions.Compiled), "${Prefix}${Subtitle}", SubtitleError.Hearing_Impaired)
            ,new FindAndReplace(new Regex(@"^(?<Prefix><i>)\s*\[.*?\]\s*(?<Subtitle>.+)$", RegexOptions.Compiled), "${Prefix}${Subtitle}", SubtitleError.Hearing_Impaired)

            // Text (laughting) => Text
            ,new FindAndReplace(new Regex(@"^(?<Subtitle>.+?)\(.*?\)$", RegexOptions.Compiled), "${Subtitle}", SubtitleError.Hearing_Impaired)
            ,new FindAndReplace(new Regex(@"^(?<Subtitle>.+?)\[.*?\]$", RegexOptions.Compiled), "${Subtitle}", SubtitleError.Hearing_Impaired)

            // MAN #1: Text => Text
            ,new FindAndReplace(new Regex(@"^[A-ZÁ-Ú0-9 #\-'\[\]]*[A-ZÁ-Ú#'\[\]][A-ZÁ-Ú0-9 #\-'\[\]]*:\s*(?<Subtitle>.+?)$", RegexOptions.Compiled), "${Subtitle}", SubtitleError.Hearing_Impaired)
                    .SetRegexCI(new Regex(@"^[A-ZÁ-Úa-zá-ú0-9 #\-'\[\]]*[A-ZÁ-Úa-zá-ú#'\[\]][A-ZÁ-Úa-zá-ú0-9 #\-'\[\]]*:(?!\d\d)\s*(?<Subtitle>.+?)$", RegexOptions.Compiled))

            // Some (laughting) text => Some text
            ,new FindAndReplace(new Regex(@"\s+\(.*?\)\s+", RegexOptions.Compiled), " ", SubtitleError.Hearing_Impaired)
            ,new FindAndReplace(new Regex(@"\s+\[.*?\]\s+", RegexOptions.Compiled), " ", SubtitleError.Hearing_Impaired)

            // Text (laughting)</i> => Text</i>
            ,new FindAndReplace(new Regex(@"\s+\(.*?\)</i>", RegexOptions.Compiled), "</i>", SubtitleError.Hearing_Impaired)
            ,new FindAndReplace(new Regex(@"\s+\[.*?\]</i>", RegexOptions.Compiled), "</i>", SubtitleError.Hearing_Impaired)

            // Text <i>(laughting)</i> => Text
            ,new FindAndReplace(new Regex(@"(?:<i>\s*)\(.*?\)(?:\s*</i>)", RegexOptions.Compiled), "", SubtitleError.Hearing_Impaired)
            ,new FindAndReplace(new Regex(@"(?:<i>\s*)\[.*?\](?:\s*</i>)", RegexOptions.Compiled), "", SubtitleError.Hearing_Impaired)

            // (?!\d\d) prevents cleaning time, like 13:00, in CI mode

            // MAN (laughting): Text => Text
            ,new FindAndReplace(new Regex(@"^[" + HI_CHARS.Replace("-'", "'") + @"]+\(.*?\):\s*", RegexOptions.Compiled), "", SubtitleError.Hearing_Impaired)
                    .SetRegexCI(new Regex(@"^[" + HI_CHARS_CI.Replace("-'", "'") + @"]+\(.*?\):(?!\d\d)\s*", RegexOptions.Compiled))
            ,new FindAndReplace(new Regex(@"^[" + HI_CHARS.Replace("-'", "'") + @"]+\[.*?\]:\s*", RegexOptions.Compiled), "", SubtitleError.Hearing_Impaired)
                    .SetRegexCI(new Regex(@"^[" + HI_CHARS_CI.Replace("-'", "'") + @"]+\[.*?\]:(?!\d\d)\s*", RegexOptions.Compiled))

            // - MAN (laughting): Text => - Text
            ,new FindAndReplace(new Regex(@"^-\s*[" + HI_CHARS + @"]+\(.*?\):\s*", RegexOptions.Compiled), "- ", SubtitleError.Hearing_Impaired)
                    .SetRegexCI(new Regex(@"^-\s*[" + HI_CHARS_CI + @"]+\(.*?\):(?!\d\d)\s*", RegexOptions.Compiled))
            ,new FindAndReplace(new Regex(@"^-\s*[" + HI_CHARS + @"]+\[.*?\]:\s*", RegexOptions.Compiled), "- ", SubtitleError.Hearing_Impaired)
                    .SetRegexCI(new Regex(@"^-\s*[" + HI_CHARS_CI + @"]+\[.*?\]:(?!\d\d)\s*", RegexOptions.Compiled))

            // <i>- MAN LAUGHTING: Text => <i>- Text
            // - <i>MAN LAUGHTING: Text => - <i>Text
            ,new FindAndReplace(new Regex(@"^(?<Prefix>(?:<i>)?-?\s*|-?\s*(?:<i>)?\s*)[" + HI_CHARS + @"]*[A-ZÁ-Ú]+[" + HI_CHARS + @"]*:\s*(?<Subtitle>.*?)$", RegexOptions.Compiled), "${Prefix}${Subtitle}", SubtitleError.Hearing_Impaired)
                    .SetRegexCI(new Regex(@"^(?<Prefix>(?:<i>)?-?\s*|-?\s*(?:<i>)?\s*)[" + HI_CHARS_CI + @"]*[A-ZÁ-Ú]+[" + HI_CHARS_CI + @"]*:(?!\d\d)\s*(?<Subtitle>.*?)$", RegexOptions.Compiled))

            // <i>- : Text => Text
            ,new FindAndReplace(new Regex(@"^(?:\s*<i>)?\s*-\s*:\s*", RegexOptions.Compiled), "", SubtitleError.Hearing_Impaired)

            // <i>: Text => Text
            ,new FindAndReplace(new Regex(@"^(?:\s*<i>)?\s*:\s*", RegexOptions.Compiled), "", SubtitleError.Hearing_Impaired)

            // {MAN} {MAN) {MAN] (MAN} [MAN}
            ,new FindAndReplace(new Regex(@"\{[^\{\[\(\)\]\}]+[\)\]\}]", RegexOptions.Compiled), "", SubtitleError.Hearing_Impaired)
            ,new FindAndReplace(new Regex(@"[\{\[\(][^\{\[\(\)\]\}]+\}", RegexOptions.Compiled), "", SubtitleError.Hearing_Impaired)
        };

        #endregion

        #region Redundant Italics

        public static readonly FindAndReplace[] RedundantItalics = new FindAndReplace[] {
            new FindAndReplace(new Regex(@"<i>(?<Symbol>[-""'.:!?#])</i>", RegexOptions.Compiled), "${Symbol}", SubtitleError.Redundant_Italics)
            ,new FindAndReplace(new Regex(@"<i>\.{2,}</i>", RegexOptions.Compiled), "...", SubtitleError.Redundant_Italics)
            ,new FindAndReplace(new Regex(@"<i></i>", RegexOptions.Compiled), "", SubtitleError.Redundant_Italics)
            ,new FindAndReplace(new Regex(@"<i>\s+</i>", RegexOptions.Compiled), " ", SubtitleError.Redundant_Italics)
            ,new FindAndReplace(new Regex(@"</i>\s+<i>", RegexOptions.Compiled), " ", SubtitleError.Redundant_Italics)
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

			// Gun Calibre
            // Derringer.22 => Derringer .22
            ,new FindAndReplace(new Regex(@"[A-ZÁ-Úa-zá-ú](?<OCR>\.)\d+\b", RegexOptions.Compiled), "OCR", " .", SubtitleError.Missing_Spaces)

            // Add space after a single dot
            ,new FindAndReplace("Space After Dot", new Regex(@"[a-zá-úñä-ü](?<OCR>\.)[^(\s\n\'\.\?\!<"")\,]", RegexOptions.Compiled), "OCR", ". ", SubtitleError.Missing_Spaces,
                new FindAndReplace.IgnoreRule() { ReadPrevCharsFromMatch = 1, IgnoreIfEqualsTo = "Ph.D" }
                , new FindAndReplace.IgnoreRule() { ReadNextCharsFromMatch = 1, IgnoreIfCaseInsensitiveEqualsTo = "a.m." }
                , new FindAndReplace.IgnoreRule() { ReadNextCharsFromMatch = 1, IgnoreIfCaseInsensitiveEqualsTo = "p.m." }
                , new FindAndReplace.IgnoreRule() { ReadNextCharsFromMatch = 1, IgnoreIfCaseInsensitiveEqualsTo = "o.d." }
                , new FindAndReplace.IgnoreRule() { ReadPrevCharsFromMatch = 2, IgnoreIfCaseInsensitiveStartsWith = "www." }
                , new FindAndReplace.IgnoreRule() { ReadNextCharsFromMatch = 2, IgnoreIfCaseInsensitiveEndsWith = ".com" }
                , new FindAndReplace.IgnoreRule() { ReadNextCharsFromMatch = 2, IgnoreIfCaseInsensitiveEndsWith = ".org" }
                , new FindAndReplace.IgnoreRule() { ReadNextCharsFromMatch = 2, IgnoreIfCaseInsensitiveEndsWith = "a.k.a" }
            )

            // Add space after comma
            ,new FindAndReplace("Space After Comma", new Regex(@"(?<OCR>\,)[A-ZÁ-Úa-zá-ú]", RegexOptions.Compiled), "OCR", ", ", SubtitleError.Missing_Spaces)

            // Text...Text => Text... Text
            ,new FindAndReplace("Space After Three Dot", new Regex(@"[A-ZÁ-Úa-zá-ú0-9](?:(?<OCR>\.{2,})[A-ZÁ-Úa-zá-ú0-9])+", RegexOptions.Compiled), "OCR", "... ", SubtitleError.Missing_Spaces)
        };

        #endregion

        #region Trim Spaces

        public static readonly FindAndReplace[] TrimSpaces = new FindAndReplace[] {
            new FindAndReplace(new Regex(@"\s{2,}", RegexOptions.Compiled), " ", SubtitleError.Redundant_Spaces)
            ,new FindAndReplace(new Regex(@"^\s+", RegexOptions.Compiled), "", SubtitleError.Redundant_Spaces)
            ,new FindAndReplace(new Regex(@"\s+$", RegexOptions.Compiled), "", SubtitleError.Redundant_Spaces)
        };

        #endregion

        #region Non-Ansi Chars

        public static readonly FindAndReplace[] NonAnsiChars = new FindAndReplace[] {
            new FindAndReplace(new Regex(@" ", RegexOptions.Compiled), " ", SubtitleError.Non_Ansi_Chars)
            ,new FindAndReplace(new Regex(@"ﬁ", RegexOptions.Compiled), "fi", SubtitleError.Non_Ansi_Chars)
            ,new FindAndReplace(new Regex(@"Α", RegexOptions.Compiled), "A", SubtitleError.Non_Ansi_Chars) //  913 => A
            ,new FindAndReplace(new Regex(@"Β", RegexOptions.Compiled), "B", SubtitleError.Non_Ansi_Chars) //  914 => B
            ,new FindAndReplace(new Regex(@"Ε", RegexOptions.Compiled), "E", SubtitleError.Non_Ansi_Chars) //  917 => E
            ,new FindAndReplace(new Regex(@"Ζ", RegexOptions.Compiled), "Z", SubtitleError.Non_Ansi_Chars) //  918 => Z
            ,new FindAndReplace(new Regex(@"Η", RegexOptions.Compiled), "H", SubtitleError.Non_Ansi_Chars) //  919 => H
            ,new FindAndReplace(new Regex(@"Ι", RegexOptions.Compiled), "I", SubtitleError.Non_Ansi_Chars) //  921 => I
            ,new FindAndReplace(new Regex(@"Κ", RegexOptions.Compiled), "K", SubtitleError.Non_Ansi_Chars) //  922 => K
            ,new FindAndReplace(new Regex(@"Μ", RegexOptions.Compiled), "M", SubtitleError.Non_Ansi_Chars) //  924 => M
            ,new FindAndReplace(new Regex(@"Ν", RegexOptions.Compiled), "N", SubtitleError.Non_Ansi_Chars) //  925 => N
            ,new FindAndReplace(new Regex(@"Ο", RegexOptions.Compiled), "O", SubtitleError.Non_Ansi_Chars) //  927 => O
            ,new FindAndReplace(new Regex(@"Ρ", RegexOptions.Compiled), "P", SubtitleError.Non_Ansi_Chars) //  929 => P
            ,new FindAndReplace(new Regex(@"Τ", RegexOptions.Compiled), "T", SubtitleError.Non_Ansi_Chars) //  932 => T
            ,new FindAndReplace(new Regex(@"Υ", RegexOptions.Compiled), "Y", SubtitleError.Non_Ansi_Chars) //  933 => Y
            ,new FindAndReplace(new Regex(@"Χ", RegexOptions.Compiled), "X", SubtitleError.Non_Ansi_Chars) //  935 => X
            ,new FindAndReplace(new Regex(@"ϲ", RegexOptions.Compiled), "c", SubtitleError.Non_Ansi_Chars) // 1010 => c
            ,new FindAndReplace(new Regex(@"ϳ", RegexOptions.Compiled), "j", SubtitleError.Non_Ansi_Chars) // 1011 => j
            ,new FindAndReplace(new Regex(@"Ϲ", RegexOptions.Compiled), "C", SubtitleError.Non_Ansi_Chars) // 1017 => C
            ,new FindAndReplace(new Regex(@"Ϻ", RegexOptions.Compiled), "M", SubtitleError.Non_Ansi_Chars) // 1018 => M
            ,new FindAndReplace(new Regex(@"Ѕ", RegexOptions.Compiled), "S", SubtitleError.Non_Ansi_Chars) // 1029 => S
            ,new FindAndReplace(new Regex(@"І", RegexOptions.Compiled), "I", SubtitleError.Non_Ansi_Chars) // 1030 => I
            ,new FindAndReplace(new Regex(@"Ј", RegexOptions.Compiled), "J", SubtitleError.Non_Ansi_Chars) // 1032 => J
            ,new FindAndReplace(new Regex(@"А", RegexOptions.Compiled), "A", SubtitleError.Non_Ansi_Chars) // 1040 => A
            ,new FindAndReplace(new Regex(@"В", RegexOptions.Compiled), "B", SubtitleError.Non_Ansi_Chars) // 1042 => B
            ,new FindAndReplace(new Regex(@"Е", RegexOptions.Compiled), "E", SubtitleError.Non_Ansi_Chars) // 1045 => E
            ,new FindAndReplace(new Regex(@"К", RegexOptions.Compiled), "K", SubtitleError.Non_Ansi_Chars) // 1050 => K
            ,new FindAndReplace(new Regex(@"М", RegexOptions.Compiled), "M", SubtitleError.Non_Ansi_Chars) // 1052 => M
            ,new FindAndReplace(new Regex(@"Н", RegexOptions.Compiled), "H", SubtitleError.Non_Ansi_Chars) // 1053 => H
            ,new FindAndReplace(new Regex(@"О", RegexOptions.Compiled), "O", SubtitleError.Non_Ansi_Chars) // 1054 => O
            ,new FindAndReplace(new Regex(@"Р", RegexOptions.Compiled), "P", SubtitleError.Non_Ansi_Chars) // 1056 => P
            ,new FindAndReplace(new Regex(@"С", RegexOptions.Compiled), "C", SubtitleError.Non_Ansi_Chars) // 1057 => C
            ,new FindAndReplace(new Regex(@"Т", RegexOptions.Compiled), "T", SubtitleError.Non_Ansi_Chars) // 1058 => T
            ,new FindAndReplace(new Regex(@"У", RegexOptions.Compiled), "y", SubtitleError.Non_Ansi_Chars) // 1059 => y
            ,new FindAndReplace(new Regex(@"Х", RegexOptions.Compiled), "X", SubtitleError.Non_Ansi_Chars) // 1061 => X
            ,new FindAndReplace(new Regex(@"а", RegexOptions.Compiled), "a", SubtitleError.Non_Ansi_Chars) // 1072 => a
            ,new FindAndReplace(new Regex(@"е", RegexOptions.Compiled), "e", SubtitleError.Non_Ansi_Chars) // 1077 => e
            ,new FindAndReplace(new Regex(@"о", RegexOptions.Compiled), "o", SubtitleError.Non_Ansi_Chars) // 1086 => o
            ,new FindAndReplace(new Regex(@"р", RegexOptions.Compiled), "p", SubtitleError.Non_Ansi_Chars) // 1088 => p
            ,new FindAndReplace(new Regex(@"с", RegexOptions.Compiled), "c", SubtitleError.Non_Ansi_Chars) // 1089 => c
            ,new FindAndReplace(new Regex(@"у", RegexOptions.Compiled), "y", SubtitleError.Non_Ansi_Chars) // 1091 => y
            ,new FindAndReplace(new Regex(@"х", RegexOptions.Compiled), "x", SubtitleError.Non_Ansi_Chars) // 1093 => x
        };

        #endregion

        #region Encoded HTML

        public static readonly FindAndReplace[] EncodedHTML = new FindAndReplace[] {
            new FindAndReplace(new Regex(@"&amp;", RegexOptions.Compiled), "&", SubtitleError.Encoded_HTML)
            ,new FindAndReplace(new Regex(@"&lt;", RegexOptions.Compiled), "<", SubtitleError.Encoded_HTML)
            ,new FindAndReplace(new Regex(@"&gt;", RegexOptions.Compiled), ">", SubtitleError.Encoded_HTML)
            ,new FindAndReplace(new Regex(@"&quot;", RegexOptions.Compiled), "\"", SubtitleError.Encoded_HTML)
        };

        #endregion

        #region Malformed Letters

        public static readonly FindAndReplace[] MalformedLetters = new FindAndReplace[] {
            new FindAndReplace(new Regex(@"\b(?<OCR>I-l)", RegexOptions.Compiled), "OCR", "H", SubtitleError.Malformed_Letters)
            ,new FindAndReplace(new Regex(@"\b(?<OCR>I- l)", RegexOptions.Compiled), "OCR", "H", SubtitleError.Malformed_Letters)
            ,new FindAndReplace(new Regex(@"\b(?<OCR>L\\/l)", RegexOptions.Compiled), "OCR", "M", SubtitleError.Malformed_Letters)
            ,new FindAndReplace(new Regex(@"\b(?<OCR>I\\/l)", RegexOptions.Compiled), "OCR", "M", SubtitleError.Malformed_Letters)
            ,new FindAndReplace(new Regex(@"\b(?<OCR>L/V)", RegexOptions.Compiled), "OCR", "W", SubtitleError.Malformed_Letters)
            ,new FindAndReplace(new Regex(@"(?<OCR>\(\))(kay|K)", RegexOptions.Compiled), "OCR", "O", SubtitleError.Malformed_Letters)
            ,new FindAndReplace(new Regex(@"(?<OCR>n/v)", RegexOptions.Compiled), "OCR", "rw", SubtitleError.Malformed_Letters)
            // /t => It
            ,new FindAndReplace(new Regex(@"(?:^|\s)(?<OCR>/t)", RegexOptions.Compiled), "OCR", "It", SubtitleError.Malformed_Letters)
            // morn => mom
            ,new FindAndReplace(new Regex(@"\b(?i:m)o(?<OCR>rn)\b", RegexOptions.Compiled), "OCR", "m", SubtitleError.Malformed_Letters)
        };

        #endregion

        #region Contractions

        public static readonly FindAndReplace[] Contractions = new FindAndReplace[] {
            new FindAndReplace(new Regex(@"\b(?i:I)(?<OCR>""|''|'’| ""|"" )d\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.Contractions_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:I)(?<OCR>""|''|'’| ""|"" )ll\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.Contractions_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:I)(?<OCR>""|''|'’| ""|"" )m\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.Contractions_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:I)(?<OCR>""|''|'’| ""|"" )ve\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.Contractions_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:a)ren(?<OCR>""|''|'’| ""|"" )t\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.Contractions_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:c)ouldn(?<OCR>""|''|'’| ""|"" )t\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.Contractions_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:d)on(?<OCR>""|''|'’| ""|"" )t\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.Contractions_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:h)e(?<OCR>""|''|'’| ""|"" )ll\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.Contractions_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:h)e(?<OCR>""|''|'’| ""|"" )s\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.Contractions_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:i)sn(?<OCR>""|''|'’| ""|"" )t\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.Contractions_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:i)t(?<OCR>""|''|'’| ""|"" )s\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.Contractions_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:l)et(?<OCR>""|''|'’| ""|"" )s\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.Contractions_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:s)he(?<OCR>""|''|'’| ""|"" )ll\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.Contractions_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:s)he(?<OCR>""|''|'’| ""|"" )s\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.Contractions_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:t)hat(?<OCR>""|''|'’| ""|"" )s\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.Contractions_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:t)here(?<OCR>""|''|'’| ""|"" )ll\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.Contractions_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:t)here(?<OCR>""|''|'’| ""|"" )s\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.Contractions_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:t)hey(?<OCR>""|''|'’| ""|"" )re\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.Contractions_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:w)e(?<OCR>""|''|'’| ""|"" )re\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.Contractions_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:w)eren(?<OCR>""|''|'’| ""|"" )t\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.Contractions_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:w)hat(?<OCR>""|''|'’| ""|"" )s\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.Contractions_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:w)ould(?<OCR>""|''|'’| ""|"" )ve\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.Contractions_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:w)ouldn(?<OCR>""|''|'’| ""|"" )t\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.Contractions_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:y)ou(?<OCR>""|''|'’| ""|"" )ll\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.Contractions_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:y)ou(?<OCR>""|''|'’| ""|"" )re\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.Contractions_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:y)ou(?<OCR>""|''|'’| ""|"" )ve\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.Contractions_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:o)(?<OCR>""|''|'’| ""|"" )er\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.Contractions_Error)
            // 'tweren't
            ,new FindAndReplace(new Regex(@"(?i:t)weren(?<OCR>""|''|'’| ""|"" )t\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.Contractions_Error)
            ,new FindAndReplace(new Regex(@"\s?(?<OCR>""|''|'’| ""|"" )(?i:t)weren't\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.Contractions_Error)
            // 'em
            ,new FindAndReplace(new Regex(@"\b\s(?<OCR>""|''|'’| ""|"" )em\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.Contractions_Error)
            // 's
            ,new FindAndReplace(new Regex(@"[a-zá-ú](?<OCR>""|''|'’| ""|"" )s\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.Contractions_Error)
            // in ' => in' (sayin')
            ,new FindAndReplace(new Regex(@"\win(?<OCR>\s)'", RegexOptions.Compiled), "OCR", "", SubtitleError.Contractions_Error)
        };

        #endregion

        #region Accent Letters

        public static readonly FindAndReplace[] AccentLetters = new FindAndReplace[] {
            new FindAndReplace(new Regex(@"\b(?i:F)ianc(?<OCR>'e)\b", RegexOptions.Compiled), "OCR", "é", SubtitleError.Accent_Letters)
            ,new FindAndReplace(new Regex(@"\b(?i:C)af(?<OCR>'e)\b", RegexOptions.Compiled), "OCR", "é", SubtitleError.Accent_Letters)
            ,new FindAndReplace(new Regex(@"\b(?i:C)ach(?<OCR>'e)\b", RegexOptions.Compiled), "OCR", "é", SubtitleError.Accent_Letters)
        };

        #endregion

        #region I and L

        public static readonly FindAndReplace[] I_And_L = new FindAndReplace[] {
            // Roman numerals
            new FindAndReplace(new Regex(@"\b[VXLCDM]*(?<OCR>lll)\b", RegexOptions.Compiled), "OCR", "III", SubtitleError.I_And_L_Error)
            ,new FindAndReplace(new Regex(@"[^.?!—–―‒-][^']\b[IVXLCDM]*(?<OCR>ll)I{0,1}\b", RegexOptions.Compiled), "OCR", "II", SubtitleError.I_And_L_Error)
            ,new FindAndReplace(new Regex(@"^(?<OCR>ll)\b", RegexOptions.Compiled), "OCR", "II", SubtitleError.I_And_L_Error)

            ,new FindAndReplace(new Regex(@"\b[IVXLCDM]*(?<OCR>l)[IVX]*\b", RegexOptions.Compiled), "OCR", "I", SubtitleError.I_And_L_Error,
                new FindAndReplace.IgnoreRule() { ReadNextCharsFromMatch = 8, IgnoreIfEqualsTo = "Il y avait" }
                , new FindAndReplace.IgnoreRule() { ReadNextCharsFromMatch = 4, IgnoreIfEqualsTo = "Il est" }
                , new FindAndReplace.IgnoreRule() { ReadNextCharsFromMatch = 5, IgnoreIfEqualsTo = "Il faut" }
                , new FindAndReplace.IgnoreRule() { ReadNextCharsFromMatch = 5, IgnoreIfEqualsTo = "Il y a " }
            )

			// Replace "II" with "ll" at the end of a lowercase word
			,new FindAndReplace(new Regex(@"[a-zá-ú](?<OCR>II)", RegexOptions.Compiled), "OCR", "ll", SubtitleError.I_And_L_Error)
			// Replace "II" with "ll" at the beginning of a lowercase word
			,new FindAndReplace(new Regex(@"(?<OCR>II)[a-zá-ú]", RegexOptions.Compiled), "OCR", "ll", SubtitleError.I_And_L_Error)
			// Replace "I" with "l" in the middle of a lowercase word
			,new FindAndReplace(new Regex(@"[a-zá-ú](?<OCR>I)[a-zá-ú]", RegexOptions.Compiled), "OCR", "l", SubtitleError.I_And_L_Error,
                new FindAndReplace.IgnoreRule() { ReadPrevCharsFromMatch = 1, IgnoreIfStartsWith = "McI" } // ignore names McIntyre
            )
			// Replace "I" with "l" at the end of a lowercase word
			,new FindAndReplace(new Regex(@"[a-zá-ú](?<OCR>I)\b", RegexOptions.Compiled), "OCR", "l", SubtitleError.I_And_L_Error)
			// Replace "l" with "I" in the middle of an uppercase word
			,new FindAndReplace(new Regex(@"[A-ZÁ-Ú](?<OCR>l)[A-ZÁ-Ú]", RegexOptions.Compiled), "OCR", "I", SubtitleError.I_And_L_Error)
			// Replace "l" with "I" at the end of an uppercase word
			,new FindAndReplace(new Regex(@"[A-ZÁ-Ú]{2,}(?<OCR>l)", RegexOptions.Compiled), "OCR", "I", SubtitleError.I_And_L_Error)

			// Replace a single "l" with "I"
			,new FindAndReplace(new Regex(@"\b(?<OCR>l)\b", RegexOptions.Compiled), "OCR", "I", SubtitleError.I_And_L_Error)

			// Replace a single "i" with "I", but not <i> or </i>
            ,new FindAndReplace(new Regex(@"\b(?<OCR>(?<!<|/)i(?!>))\b", RegexOptions.Compiled), "OCR", "I", SubtitleError.I_And_L_Error)

			// Replace "I'II"/"you'II" etc. with "I'll"/"you'll" etc.
			,new FindAndReplace(new Regex(@"[A-ZÁ-Úa-zá-ú]'(?<OCR>II)\b", RegexOptions.Compiled), "OCR", "ll", SubtitleError.I_And_L_Error)
			// Replace "I 'II" with "I'll" or "I' II" with "I'll" - rare cases with a space before or after the apostrophe
			,new FindAndReplace(new Regex(@"[A-ZÁ-Úa-zá-ú](?<OCR>'\sII|\s'II)\b", RegexOptions.Compiled), "OCR", "'ll", SubtitleError.I_And_L_Error)

            // All
            ,new FindAndReplace(new Regex(@"\bA(?<OCR>II)\b", RegexOptions.Compiled), "OCR", "ll", SubtitleError.I_And_L_Error)

            // iing => ing
            ,new FindAndReplace(new Regex(@"(?<OCR>ii)(?:ng|n')", RegexOptions.Compiled), "OCR", "i", SubtitleError.I_And_L_Error,
                new FindAndReplace.IgnoreRule() { ReadPrevCharsFromMatch = 3, IgnoreIfCaseInsensitiveStartsWith = "Taxiin" } // Taxiing
                , new FindAndReplace.IgnoreRule() { ReadPrevCharsFromMatch = 2, IgnoreIfCaseInsensitiveStartsWith = "Skiin" } // Skiing
            )

            // Live, Living
            ,new FindAndReplace(new Regex(@"^(?<OCR>I)(?:ive|iving)\b", RegexOptions.Compiled), "OCR", "L", SubtitleError.I_And_L_Error)
            // live, living
            ,new FindAndReplace(new Regex(@"\s+(?<OCR>I)(?:ive|iving)\b", RegexOptions.Compiled), "OCR", "l", SubtitleError.I_And_L_Error)

            // "I" at the beginning of a word before lowercase vowels is most likely an "l"
            ,new FindAndReplace(new Regex(@"\b(?<OCR>I)[oaeiuyá-ú]", RegexOptions.Compiled), "OCR", "l", SubtitleError.I_And_L_Error,
                new FindAndReplace.IgnoreRule() { ReadNextCharsFromMatch = 2, IgnoreIfEqualsTo = "Iago" }
                , new FindAndReplace.IgnoreRule() { ReadNextCharsFromMatch = 4, IgnoreIfEqualsTo = "Ionian" }
            )

            // "I" after an uppercase letter at the beginning and before a lowercase letter is most likely an "l"
            ,new FindAndReplace(new Regex(@"\b[A-ZÁ-Ú](?<OCR>I)[a-zá-ú]", RegexOptions.Compiled), "OCR", "l", SubtitleError.I_And_L_Error)

            // "l" at the beginning before a consonant different from "l" is most likely an "I"
            ,new FindAndReplace(new Regex(@"\b(?<OCR>l)[^aeiouyàá-úl]", RegexOptions.Compiled), "OCR", "I", SubtitleError.I_And_L_Error,
                new FindAndReplace.IgnoreRule() { ReadNextCharsFromMatch = 1, IgnoreIfEqualsTo = "lbs" }
            )

			// Fixes for "I" at the beginning of the word before lowercase vowels
			// The name "Ian"
			,new FindAndReplace(new Regex(@"\b(?<OCR>lan)\b", RegexOptions.Compiled), "OCR", "Ian", SubtitleError.I_And_L_Error)
			// The name "Iowa"
			,new FindAndReplace(new Regex(@"\b(?<OCR>lowa)\b", RegexOptions.Compiled), "OCR", "Iowa", SubtitleError.I_And_L_Error)
			// The word "Ill"
			,new FindAndReplace(new Regex(@"[.?!-]\s?(?<OCR>III)\b", RegexOptions.Compiled), "OCR", "Ill", SubtitleError.I_And_L_Error)
            ,new FindAndReplace(new Regex(@"^(?<OCR>III)\b.", RegexOptions.Compiled), "OCR", "Ill", SubtitleError.I_And_L_Error)
			// The word "Ion" and its derivatives
			,new FindAndReplace(new Regex(@"\b(?<OCR>l)on\b.", RegexOptions.Compiled), "OCR", "I", SubtitleError.I_And_L_Error)
            ,new FindAndReplace(new Regex(@"\b(?<OCR>l)oni", RegexOptions.Compiled), "OCR", "I", SubtitleError.I_And_L_Error)
			// The word "Iodine" and its derivatives
			,new FindAndReplace(new Regex(@"\b(?<OCR>l)odi", RegexOptions.Compiled), "OCR", "I", SubtitleError.I_And_L_Error)
            ,new FindAndReplace(new Regex(@"\b(?<OCR>l)odo", RegexOptions.Compiled), "OCR", "I", SubtitleError.I_And_L_Error)

            ,new FindAndReplace(new Regex(@"\b(?<OCR>L)\b", RegexOptions.Compiled), "OCR", "I", SubtitleError.I_And_L_Error,
                new FindAndReplace.IgnoreRule() { ReadPrevCharsFromMatch = 1, ReadNextCharsFromMatch = 1, IgnoreIfEqualsTo = "-L-" }
                , new FindAndReplace.IgnoreRule() { ReadPrevCharsFromMatch = 1, ReadNextCharsFromMatch = 1, IgnoreIfEqualsTo = "-L." }
                , new FindAndReplace.IgnoreRule() { ReadPrevCharsFromMatch = 1, ReadNextCharsFromMatch = 1, IgnoreIfEqualsTo = "-L." }
                , new FindAndReplace.IgnoreRule() { ReadPrevCharsFromMatch = 1, ReadNextCharsFromMatch = 1, IgnoreIfEqualsTo = ".L." }
                , new FindAndReplace.IgnoreRule() { ReadNextCharsFromMatch = 3, IgnoreIfEqualsTo = "L.A." }
                , new FindAndReplace.IgnoreRule() { ReadNextCharsFromMatch = 1, IgnoreIfEqualsTo = "L'" }
            )

            ,new FindAndReplace(new Regex(@"\b(?<OCR>L)'m\b", RegexOptions.Compiled), "OCR", "I", SubtitleError.I_And_L_Error)
            ,new FindAndReplace(new Regex(@"\b(?<OCR>L)'d\b", RegexOptions.Compiled), "OCR", "I", SubtitleError.I_And_L_Error)
            ,new FindAndReplace(new Regex(@"\b(?<OCR>L)t's\b", RegexOptions.Compiled), "OCR", "I", SubtitleError.I_And_L_Error)
            ,new FindAndReplace(new Regex(@"\b(?<OCR>L)ts\b", RegexOptions.Compiled), "OCR", "I", SubtitleError.I_And_L_Error)
            ,new FindAndReplace(new Regex(@"\b(?<OCR>L)n\b", RegexOptions.Compiled), "OCR", "I", SubtitleError.I_And_L_Error)
            ,new FindAndReplace(new Regex(@"\b(?<OCR>L)s\b", RegexOptions.Compiled), "OCR", "I", SubtitleError.I_And_L_Error)
            ,new FindAndReplace(new Regex(@"\b(?<OCR>L)f\b", RegexOptions.Compiled), "OCR", "I", SubtitleError.I_And_L_Error)
        };

        #endregion

        #region Merged Words

        public static readonly FindAndReplace[] MergedWords = new FindAndReplace[] {
            new FindAndReplace(new Regex(@"\b(?<OCR>ofthe)\b", RegexOptions.Compiled), "OCR", "of the", SubtitleError.Merged_Words_Error)
            ,new FindAndReplace(new Regex(@"\b(?<OCR>onthe)\b", RegexOptions.Compiled), "OCR", "on the", SubtitleError.Merged_Words_Error)
            ,new FindAndReplace(new Regex(@"\b(?<OCR>fora)\b", RegexOptions.Compiled), "OCR", "for a", SubtitleError.Merged_Words_Error)
            ,new FindAndReplace(new Regex(@"\b(?<OCR>numberi)\b", RegexOptions.Compiled), "OCR", "number one", SubtitleError.Merged_Words_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:time)(?<OCR>to)\b", RegexOptions.Compiled), "OCR", " to", SubtitleError.Merged_Words_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:don't)(?<OCR>do)\b", RegexOptions.Compiled), "OCR", " do", SubtitleError.Merged_Words_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:don't)(?<OCR>just)\b", RegexOptions.Compiled), "OCR", " just", SubtitleError.Merged_Words_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:for)(?<OCR>just)\b", RegexOptions.Compiled), "OCR", " just", SubtitleError.Merged_Words_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:after)(?<OCR>just)\b", RegexOptions.Compiled), "OCR", " just", SubtitleError.Merged_Words_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:off)(?<OCR>too)\b", RegexOptions.Compiled), "OCR", " too", SubtitleError.Merged_Words_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:off)(?<OCR>first)\b", RegexOptions.Compiled), "OCR", " first", SubtitleError.Merged_Words_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:if)(?<OCR>this)\b", RegexOptions.Compiled), "OCR", " this", SubtitleError.Merged_Words_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:if)(?<OCR>they)\b", RegexOptions.Compiled), "OCR", " they", SubtitleError.Merged_Words_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:of)(?<OCR>this)\b", RegexOptions.Compiled), "OCR", " this", SubtitleError.Merged_Words_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:of)(?<OCR>them)\b", RegexOptions.Compiled), "OCR", " them", SubtitleError.Merged_Words_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:of)(?<OCR>thing)\b", RegexOptions.Compiled), "OCR", " thing", SubtitleError.Merged_Words_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:of)(?<OCR>things)\b", RegexOptions.Compiled), "OCR", " things", SubtitleError.Merged_Words_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:of)(?<OCR>too)\b", RegexOptions.Compiled), "OCR", " too", SubtitleError.Merged_Words_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:if)(?<OCR>we)\b", RegexOptions.Compiled), "OCR", " we", SubtitleError.Merged_Words_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:if)(?<OCR>the)\b", RegexOptions.Compiled), "OCR", " the", SubtitleError.Merged_Words_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:if)(?<OCR>those)\b", RegexOptions.Compiled), "OCR", " those", SubtitleError.Merged_Words_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:you)(?<OCR>have)\b", RegexOptions.Compiled), "OCR", " have", SubtitleError.Merged_Words_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:that)(?<OCR>j)", RegexOptions.Compiled), "OCR", " j", SubtitleError.Merged_Words_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:this)(?<OCR>j)", RegexOptions.Compiled), "OCR", " j", SubtitleError.Merged_Words_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:of)(?<OCR>j)", RegexOptions.Compiled), "OCR", " j", SubtitleError.Merged_Words_Error)
        };

        #endregion

        #region O and 0

        public static readonly FindAndReplace[] O_And_0 = new FindAndReplace[] {
            new FindAndReplace(new Regex(@"[0-9](?<OCR>O)", RegexOptions.Compiled), "OCR", "0", SubtitleError.O_And_0_Error)
            ,new FindAndReplace(new Regex(@"[0-9](?<OCR>\.O)", RegexOptions.Compiled), "OCR", ".0", SubtitleError.O_And_0_Error)
            ,new FindAndReplace(new Regex(@"[0-9](?<OCR>,O)", RegexOptions.Compiled), "OCR", ",0", SubtitleError.O_And_0_Error)
            // S0ME
            ,new FindAndReplace(new Regex(@"[A-ZÁ-Ú](?<OCR>0)", RegexOptions.Compiled), "OCR", "O", SubtitleError.O_And_0_Error)
            // 0ver
            ,new FindAndReplace(new Regex(@"\b(?<OCR>0)[A-ZÁ-Úa-zá-ú]", RegexOptions.Compiled), "OCR", "O", SubtitleError.O_And_0_Error)
            // Someb0dy
            ,new FindAndReplace(new Regex(@"[a-zá-ú](?<OCR>0)[a-zá-ú]", RegexOptions.Compiled), "OCR", "o", SubtitleError.O_And_0_Error)
        };

        #endregion

        #region OCR Errors

        public static readonly FindAndReplace[] OCRErrors = new FindAndReplace[] {
            // Mr. Mrs. Dr. St.
            new FindAndReplace("Dot After Abbreviation", new Regex(@"\b(?:Mr|Mrs|Dr|St)(?<OCR>\s+)\b", RegexOptions.Compiled), "OCR", ". ", SubtitleError.OCR_Error)

            // a.m. p.m.
            ,new FindAndReplace(new Regex(@"(?<OCR>a|p)\.M\.", RegexOptions.Compiled), "${OCR}.m.", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"(?<OCR>A|P)\.m\.", RegexOptions.Compiled), "${OCR}.M.", SubtitleError.OCR_Error)

            // l.U. => L.U. (replace match to upper case)
            ,new FindAndReplace(new Regex(@"[a-zá-úñä-ü]\.[A-ZÁ-Ú]\.", RegexOptions.Compiled), m => m.ToString().ToUpper(), SubtitleError.OCR_Error)

            // I-I-I, I-I
            //,new FindAndReplace(new Regex(@"(?<OCR>I- I- I)", RegexOptions.Compiled), "OCR", "I... I... I", SubtitleError.OCR_Error)
            //,new FindAndReplace(new Regex(@"(?<OCR>I-I-I)", RegexOptions.Compiled), "OCR", "I... I... I", SubtitleError.OCR_Error)
            //,new FindAndReplace(new Regex(@"(?<OCR>I- I)", RegexOptions.Compiled), "OCR", "I... I", SubtitleError.OCR_Error)
            //,new FindAndReplace(new Regex(@"(?<OCR>I-I)", RegexOptions.Compiled), "OCR", "I... I", SubtitleError.OCR_Error)

            // -</i> => ...</i>
            ,new FindAndReplace("Three Dots", new Regex(@"(?<OCR>\s*-\s*)</i>$", RegexOptions.Compiled), "OCR", "...", SubtitleError.OCR_Error)
            // Text - $ => Text...$
            // doesn't match un-fuckin-$reasonable
            ,new FindAndReplace("Three Dots", new Regex(@"(?<![A-ZÁ-Úa-zá-ú]+-[A-ZÁ-Úa-zá-ú]+)(?<OCR>\s*-\s*)$", RegexOptions.Compiled), "OCR", "...", SubtitleError.OCR_Error)
            // text - text => text... text
            ,new FindAndReplace("Three Dots", new Regex(@"[a-zá-ú](?<OCR> - )[a-zá-ú]", RegexOptions.Compiled), "OCR", "... ", SubtitleError.OCR_Error)
            // text - text => text... text
            ,new FindAndReplace("Three Dots", new Regex(@"[Ia-zá-ú](?<OCR>-)\s[A-ZÁ-Ú]", RegexOptions.Compiled), "OCR", "...", SubtitleError.OCR_Error)

            // Text..
            ,new FindAndReplace("Three Dots", new Regex(@"[A-ZÁ-Úa-zá-ú](?<OCR>\.{2})(?:\s|♪|</i>|$)", RegexOptions.Compiled), "OCR", "...", SubtitleError.OCR_Error)
            // ..Text
            ,new FindAndReplace("Three Dots", new Regex(@"(?:\s|♪|<i>|^)(?<OCR>\.{2})[A-ZÁ-Úa-zá-ú]", RegexOptions.Compiled), "OCR", "...", SubtitleError.OCR_Error)

            // I6 => 16
            ,new FindAndReplace("I And 1", new Regex(@"\b(?<OCR>I)\d+\b", RegexOptions.Compiled), "OCR", "1", SubtitleError.OCR_Error)

            // 1 => I
            // 1 can
            ,new FindAndReplace("I And 1", new Regex(@"(?i)\b(?<OCR>1)\s+(?:can|can't|did|didn't|do|don't|had|hadn't|am|ain't|will|won't|would|wouldn't)\b", RegexOptions.Compiled), "OCR", "I", SubtitleError.OCR_Error)
            // can 1
            ,new FindAndReplace("I And 1", new Regex(@"(?i)\b(?:can|can't|did|didn't|do|don't|had|hadn't|am|ain't|will|won't|would|wouldn't)\s+(?<OCR>1)\b", RegexOptions.Compiled), "OCR", "I", SubtitleError.OCR_Error)
        };

        #endregion

        #region Find And Replace Rules Pre

        public static readonly FindAndReplace[] FindAndReplaceRulesPre =
            NonAnsiChars
            .Concat(EncodedHTML)
            .ToArray();

        #endregion

        #region Find And Replace Rules

        public static readonly FindAndReplace[] FindAndReplaceRules =
            Punctuations
            .Concat(Notes)
            .Concat(MalformedLetters)
            .Concat(HearingImpairedFullLine)
            .Concat(ASSATags)
            .Concat(RedundantSpaces)
            .Concat(HearingImpaired)
            .Concat(RedundantItalics)
            .Concat(MissingSpaces)
            .Concat(TrimSpaces)
            .Concat(Contractions)
            .Concat(AccentLetters)
            .Concat(I_And_L)
            .Concat(MergedWords)
            .Concat(O_And_0)
            .Concat(OCRErrors)
            .ToArray();

        #endregion

        private static string CleanLine(string line, FindAndReplace[] rules, bool cleanHICaseInsensitive, bool isPrintCleaning)
        {
            SubtitleError subtitleError = SubtitleError.None;
            return CleanLine(line, rules, cleanHICaseInsensitive, isPrintCleaning, ref subtitleError);
        }

        private static string CleanLine(string line, FindAndReplace[] rules, bool cleanHICaseInsensitive, bool isPrintCleaning, ref SubtitleError subtitleError)
        {
            if (string.IsNullOrEmpty(line))
            {
                subtitleError = SubtitleError.Empty_Line;
                return null;
            }

            int ruleCounter = 0;

            foreach (var rule in rules)
            {
                string cleanLine = rule.CleanLine(line, cleanHICaseInsensitive);

                if (line != cleanLine)
                {
                    ruleCounter++;

                    if (isPrintCleaning)
                    {
                        Console.WriteLine(ruleCounter);
                        Console.WriteLine("Regex:  " + (cleanHICaseInsensitive ? rule.ToStringCI() : rule.ToString()));
                        PrintColorfulLines(line, cleanLine, rule, cleanHICaseInsensitive);
                    }

                    line = cleanLine;
                    subtitleError |= rule.SubtitleError;

                    if (subtitleError.IsSet(SubtitleError.Empty_Line))
                    {
                        subtitleError = SubtitleError.Empty_Line;
                        return null;
                    }
                    else if (subtitleError.IsSet(SubtitleError.Not_Subtitle))
                    {
                        subtitleError = SubtitleError.Not_Subtitle;
                        return null;
                    }
                }
            }

            if (isPrintCleaning && ruleCounter > 0)
            {
                Console.WriteLine("******************************************");
                Console.WriteLine();
            }

            return line;
        }

        private class LineSegment
        {
            public int Index { get; set; }
            public int Length { get; set; }
            public bool IsCapture { get; set; }
        }

        private static void PrintColorfulLines(string line, string cleanLine, FindAndReplace rule, bool cleanHICaseInsensitive)
        {
            if (rule.Replacement.Contains("${"))
            {
                Console.WriteLine("Before: " + line);
                Console.WriteLine("After:  " + cleanLine);
                return;
            }

            List<LineSegment> segments = new List<LineSegment>();

            Regex regex = (cleanHICaseInsensitive ? rule.RegexCI : rule.Regex);
            var matches = regex.Matches(line);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    var groups = match.Groups.Cast<Group>();
                    if (match.Groups.Count > 1)
                        groups = groups.Skip(1);

                    foreach (Group group in groups)
                    {
                        if (group.Success)
                        {
                            foreach (Capture capture in group.Captures)
                            {
                                bool isDuplicate = false;
                                foreach (LineSegment segment in segments)
                                {
                                    if (segment.Index == capture.Index && segment.Length == capture.Length)
                                    {
                                        isDuplicate = true;
                                        break;
                                    }
                                }

                                if (isDuplicate == false)
                                {
                                    segments.Add(new LineSegment()
                                    {
                                        Index = capture.Index,
                                        Length = capture.Length,
                                        IsCapture = true
                                    });
                                }
                            }
                        }
                    }
                }
            }

            segments.Sort((x, y) => x.Index.CompareTo(y.Index));

            int index = 0;
            int count = segments.Count;
            for (int i = 0; i < count; i++)
            {
                LineSegment segment = segments[i];

                if (index < segment.Index)
                {
                    segments.Add(new LineSegment()
                    {
                        Index = index,
                        Length = segment.Index - index,
                        IsCapture = false
                    });
                }

                index = segment.Index + segment.Length;
            }

            if (index < line.Length)
            {
                segments.Add(new LineSegment()
                {
                    Index = index,
                    Length = line.Length - index,
                    IsCapture = false
                });
            }

            segments.Sort((x, y) => x.Index.CompareTo(y.Index));

            Console.Write("Before: ");
            PrintColorfulLines(line, segments, false);
            Console.Write("After:  ");
            PrintColorfulLines(line, segments, true, rule.Replacement);
        }

        private static void PrintColorfulLines(string line, List<LineSegment> segments, bool withReplacement, string replacement = null)
        {
            foreach (LineSegment segment in segments)
            {
                if (segment.IsCapture)
                {
                    if (withReplacement)
                        Colorful.Console.Write(replacement, Color.Red);
                    else
                        Colorful.Console.Write(line.Substring(segment.Index, segment.Length), Color.Red);
                }
                else
                {
                    Console.Write(line.Substring(segment.Index, segment.Length));
                }
            }

            Console.WriteLine();
        }

        /************************************************************************/

        private static bool IsEmptyLine(string line)
        {
            return string.IsNullOrEmpty(CleanLine(line, EmptyLine, false, false));
        }

        private static bool IsNotSubtitle(string line)
        {
            return string.IsNullOrEmpty(CleanLine(line, NotSubtitle, false, false));
        }

        public static readonly Regex regexHIPrefix = new Regex(@"^(?<Prefix>(?:<i>)?-?\s*|-?\s*(?:<i>)?\s*)[" + HI_CHARS + @"]*[A-ZÁ-Ú]+[" + HI_CHARS + @"]*:\s*(?<Subtitle>.*?)$", RegexOptions.Compiled);
        public static readonly Regex regexHIPrefixCI = new Regex(@"^(?<Prefix>(?:<i>)?-?\s*|-?\s*(?:<i>)?\s*)[" + HI_CHARS_CI + @"]*[A-ZÁ-Ú]+[" + HI_CHARS_CI + @"]*:(?!\d\d)\s*(?<Subtitle>.*?)$", RegexOptions.Compiled);
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
            return line1.EndsWith("</i>") && line2.StartsWith("<i>");
        }

        private static bool IsMergeShortLineWithLongLine(string line1, string line2)
        {
            return
                regexLineWithSingleWord.IsMatch(line1) &&
                line2.StartsWith("-") == false &&
                line1.Length + line2.Length + 1 <= SINGLE_LINE_MAX_LENGTH;
        }

        #endregion

        #region Add Time

        public static readonly DateTime DateTimeZero = new DateTime(1900, 1, 1);
        private static readonly Exception addTimeError = new Exception("Add time failed.\nResult less than 00:00:00,000.");

        public static void AddTime(this List<Subtitle> subtitles, string diffTime, int? subtitleNumber = null)
        {
            subtitles.AddTime(ParseDiffTime(diffTime), subtitleNumber);
        }

        public static void AddTime(this List<Subtitle> subtitles, TimeSpan span, int? subtitleNumber = null)
        {
            if (subtitles == null || subtitles.Count == 0)
                return;

            if (span == TimeSpan.Zero)
                return;

            if (subtitleNumber != null && subtitleNumber.Value >= 1)
            {
                Subtitle firstSubtitle = subtitles.Skip(subtitleNumber.Value - 1).FirstOrDefault();
                if (VerifyAddTime(firstSubtitle, span))
                {
                    foreach (var subtitle in subtitles.Skip(subtitleNumber.Value - 1))
                    {
                        subtitle.Show += span;
                        subtitle.Hide += span;
                    }
                }
                else
                {
                    throw addTimeError;
                }
            }
            else
            {
                Subtitle firstSubtitle = subtitles.FirstOrDefault();
                if (VerifyAddTime(firstSubtitle, span))
                {
                    foreach (var subtitle in subtitles)
                    {
                        subtitle.Show += span;
                        subtitle.Hide += span;
                    }
                }
                else
                {
                    throw addTimeError;
                }
            }
        }

        private static bool VerifyAddTime(Subtitle subtitle, TimeSpan span)
        {
            return (
                subtitle != null &&
                subtitle.Show + span < DateTimeZero
            ) == false;
        }

        #endregion

        #region Set Show Time

        public static void SetShowTime(this List<Subtitle> subtitles, string showTime, int? subtitleNumber = 1)
        {
            subtitles.SetShowTime(ParseShowTime(showTime), subtitleNumber);
        }

        public static void SetShowTime(this List<Subtitle> subtitles, DateTime show, int? subtitleNumber = 1)
        {
            if (subtitles == null || subtitles.Count == 0)
                return;

            if (show == DateTime.MinValue)
                return;

            int index = (subtitleNumber ?? 1) - 1;
            if (0 <= index && index <= subtitles.Count - 1)
            {
                TimeSpan span = show - subtitles[index].Show;
                Subtitle firstSubtitle = subtitles.Skip(index).FirstOrDefault();
                if (VerifyAddTime(firstSubtitle, span))
                {
                    foreach (var subtitle in subtitles.Skip(index))
                    {
                        subtitle.Show += span;
                        subtitle.Hide += span;
                    }
                }
                else
                {
                    throw addTimeError;
                }
            }
        }

        #endregion

        #region Adjust Timing

        public static void AdjustTiming(this List<Subtitle> subtitles, string firstShowTime, string lastShowTime)
        {
            subtitles.AdjustTiming(ParseShowTime(firstShowTime), ParseShowTime(lastShowTime));
        }

        public static void AdjustTiming(this List<Subtitle> subtitles, DateTime firstShowTime, DateTime lastShowTime)
        {
            subtitles.AdjustTiming(
                subtitles[0].Show, // first subtitle's show time
                firstShowTime, // new first subtitle's show time
                subtitles[subtitles.Count - 1].Show, // last subtitle's show time
                lastShowTime // new last subtitle's show time
            );
        }

        public static void AdjustTiming(this List<Subtitle> subtitles, DateTime x1Show, DateTime x2Show, DateTime y1Show, DateTime y2Show)
        {
            if (subtitles == null || subtitles.Count == 0)
                return;

            if (x1Show == DateTime.MinValue || x2Show == DateTime.MinValue || y1Show == DateTime.MinValue || y2Show == DateTime.MinValue)
                return;

            if (x1Show == x2Show && y1Show == y2Show)
                return;

            // x1 -> x2
            int x1 = x1Show.ToMilliseconds();
            int x2 = x2Show.ToMilliseconds();

            // y1 -> y2
            int y1 = y1Show.ToMilliseconds();
            int y2 = y2Show.ToMilliseconds();

            // y = v1 * x + v2
            // (x2,y2) = v1 * (x1,y1) + v2
            // x2 = v1*x1 + v2
            // y2 = v1*y1 + v2
            double v1 = 1.0 * (x2 - y2) / (x1 - y1);
            double v2 = x2 - (v1 * x1); // = y2 - (v1 * y1)

            foreach (Subtitle subtitle in subtitles)
            {
                subtitle.Show = DateTimeZero.AddMilliseconds((v1 * subtitle.Show.ToMilliseconds()) + v2);
                subtitle.Hide = DateTimeZero.AddMilliseconds((v1 * subtitle.Hide.ToMilliseconds()) + v2);
            }
        }

        #endregion

        #region Reorder

        public static List<Subtitle> Reorder(this List<Subtitle> subtitles)
        {
            if (subtitles == null)
                return new List<Subtitle>();

            if (subtitles.Count == 0)
                return subtitles;

            subtitles.Sort();

            return subtitles;
        }

        #endregion

        #region Balance Lines

        private const int SINGLE_LINE_MAX_LENGTH = 43;

        public static readonly FindAndReplace[] FindAndReplaceRulesDisplayCharCount =
            new FindAndReplace[] {
                new FindAndReplace(new Regex(@"</?\s*[iub]\s*>", RegexOptions.Compiled), "", SubtitleError.None)
            }
            .Concat(ASSATags)
            .ToArray();

        public static int GetDisplayCharCount(string line)
        {
            foreach (var rule in FindAndReplaceRulesDisplayCharCount)
                line = rule.CleanLine(line);
            return line.Length;
        }

        private class LineBalance
        {
            public int DisplayCharCount;
            public bool IsMatchDialog;
            public bool ContainsItalicsStart;
            public bool ContainsItalicsEnd;
            public bool EndsWithPunctuation;
        }

        public static List<Subtitle> BalanceLines(this List<Subtitle> subtitles)
        {
            if (subtitles == null)
                return new List<Subtitle>();

            if (subtitles.Count == 0)
                return subtitles;

            for (int k = 0; k < subtitles.Count; k++)
            {
                Subtitle subtitle = subtitles[k];

                if (subtitle.Lines.Count > 1)
                {
                    var results = subtitle.Lines.Select(line => new LineBalance()
                    {
                        DisplayCharCount = GetDisplayCharCount(line),
                        IsMatchDialog = regexDialog.IsMatch(line),
                        ContainsItalicsStart = line.Contains("<i>"),
                        ContainsItalicsEnd = line.Contains("</i>") || line.Contains("</ i>"),
                        EndsWithPunctuation = line.EndsWith(".") || line.EndsWith("?") || line.EndsWith("!") || line.EndsWith("-")
                    }).ToList();

                    for (int i = 1; i < subtitle.Lines.Count; i++)
                    {
                        LineBalance results1 = results[i - 1];
                        LineBalance results2 = results[i];

                        if (
                            results2.IsMatchDialog == false &&
                            results1.ContainsItalicsStart == false && results1.ContainsItalicsEnd == false &&
                            results2.ContainsItalicsStart == false && results2.ContainsItalicsEnd == false &&
                            results1.EndsWithPunctuation == false &&
                            results1.DisplayCharCount + results2.DisplayCharCount + 1 <= SINGLE_LINE_MAX_LENGTH)
                        {
                            subtitle.Lines[i - 1] = subtitle.Lines[i - 1] + " " + subtitle.Lines[i];
                            results[i - 1].DisplayCharCount = results[i - 1].DisplayCharCount + results[i].DisplayCharCount + 1;
                            results[i - 1].EndsWithPunctuation = results[i].EndsWithPunctuation;
                            subtitle.Lines.RemoveAt(i);
                            results.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }

            return subtitles;
        }

        #endregion

        #region Errors

        public interface IError
        {
            string Description { get; }
            bool HasError(string line);
            string GetErrors(string line);
        }

        private class Error : IError
        {
            public virtual Regex Regex { get; set; }
            public virtual string Description { get; set; }

            public virtual bool HasError(string line)
            {
                return Regex.IsMatch(line);
            }

            public virtual string GetErrors(string line)
            {
                return string.Join(" | ", Regex.Matches(line).Cast<Match>().Select(m => m.Value));
            }
        }

        private class ComplexError : Error
        {
            public Regex ExcludeRegex { get; set; }

            public override bool HasError(string line)
            {
                return base.HasError(line) && ExcludeRegex.IsMatch(line) == false;
            }
        }

        public static readonly IError[] ErrorList = new IError[]
        {
            new Error() {
                Regex = new Regex(@"[\({\[\]}\)]", RegexOptions.Compiled),
                Description = "Brackets"
            }
            , new Error() {
                Regex = new Regex(@"[~_#]", RegexOptions.Compiled),
                Description = "Special characters"
            }
            , new Error() {
                Regex = new Regex(@"[A-ZÁ-Úa-zá-ú0-9#\-'.]+:\s", RegexOptions.Compiled),
                Description = "Colon"
            }
            , new Error() {
                Regex = new Regex(@"<(?!/?i>)|(?<!</?i)>", RegexOptions.Compiled),
                Description = "Italic with space"
            }
            , new Error() {
                // Of course 1 can
                Regex = new Regex(@"[A-ZÁ-Úa-zá-ú]\s+1\s+[A-ZÁ-Úa-zá-ú]", RegexOptions.Compiled),
                Description = "1 instead of I"
            }
            , new Error() {
                // a/b
                Regex = new Regex(@"[A-ZÁ-Úa-zá-ú]/[A-ZÁ-Úa-zá-ú]", RegexOptions.Compiled),
                Description = "Slash"
            }
            , new Error() {
                // " / " -> " I "
                Regex = new Regex(@"\s+/\s+", RegexOptions.Compiled),
                Description = "Slash instead of I"
            }
            , new Error() {
                // replace with new line
                Regex = new Regex(@"[!?][A-ZÁ-Úa-zá-ú]", RegexOptions.Compiled),
                Description = "Possible missing new line after punctuation"
            }
            , new ComplexError() {
                Regex = new Regex(@"^[A-ZÁ-Úa-zá-ú0-9#\-'.]+:", RegexOptions.Compiled),
                // not time
                ExcludeRegex = new Regex(@"^\d{1,2}:\d{2}", RegexOptions.Compiled),
                Description = "Possible hearing-impaired"
            }
            , new Error() {
                Regex = new Regex(@"^[A-ZÁ-Ú]+$", RegexOptions.Compiled),
                Description = "Possible hearing-impaired (All caps)"
            }
            , new ComplexError() {
                Regex = new Regex(@"^[" + HI_CHARS + @"]+$", RegexOptions.Compiled),
                // A... I... OK. 100. 123.45.
                ExcludeRegex = new Regex(@"^(-\s)?(A[A. ]*|I[I. ]*|OK|O\.K\.|L\.A\.|F\.B\.I\.|\d+(\.\d+)+|\d+(-\d+)+|\d+)\.*$", RegexOptions.Compiled),
                Description = "Possible miswritten line"
            }
            , new Error() {
                Regex = new Regex(@"(?<!""[A-ZÁ-Úa-zá-ú0-9 #\-'.]+)(""[!?])(\s|$)", RegexOptions.Compiled),
                Description = "Punctuation outside of quotation marks"
            }
            , new Error() {
                Regex = new Regex(@"(?<!in)'\?$", RegexOptions.Compiled),
                Description = "Ending with comma and question mark"
            }
            , new Error() {
                // ignore Mc, PhD
                Regex = new Regex(@"((?<!M)c|(?<!P)h|[a-bd-gi-zá-ú])[A-ZÁ-Ú]", RegexOptions.Compiled),
                Description = "Lower letter before capital letter"
            }
            , new Error() {
                Regex = new Regex(@"<i><i>", RegexOptions.Compiled),
                Description = "Consecutive opening italic"
            }
            , new Error() {
                Regex = new Regex(@"</i></i>", RegexOptions.Compiled),
                Description = "Consecutive closing italic"
            }
            /*, new Error() {
                Regex = new Regex(@"^""[^""]*$", RegexOptions.Compiled),
                Description = "Opening quotation marks without closing"
            }
            , new Error() {
                Regex = new Regex(@"^'[^']*$", RegexOptions.Compiled),
                Description = "Opening quotation marks without closing"
            }
            , new Error() {
                Regex = new Regex(@"^[^""]*""$", RegexOptions.Compiled),
                Description = "Closing quotation marks without opening"
            }
            , new Error() {
                Regex = new Regex(@"^[^']*'$", RegexOptions.Compiled),
                Description = "Closing quotation marks without opening"
            }*/
        };

        public static string[] GetSubtitlesErrors(List<Subtitle> subtitles)
        {
            List<string> errorLines = new List<string>();

            foreach (var subtitle in subtitles)
            {
                bool hasError = false;
                List<string> errorDescriptions = new List<string>();

                foreach (var error in ErrorList)
                {
                    foreach (string line in subtitle.Lines)
                    {
                        if (error.HasError(line))
                        {
                            if (errorDescriptions.Contains(error.Description) == false)
                            {
                                errorLines.Add("- " + error.Description);
                                //errorLines.Add(error.GetErrors(line));
                                errorDescriptions.Add(error.Description);
                            }

                            hasError = true;
                        }
                    }
                }

                if (hasError)
                {
                    int index = subtitles.IndexOf(subtitle);
                    errorLines.AddRange(subtitle.ToLines(index));
                }
            }

            return errorLines.ToArray();
        }

        public static bool HasErrors(this Subtitle subtitle)
        {
            return subtitle.Lines.Any(HasErrors);
        }

        public static bool HasErrors(string line)
        {
            return ErrorList.Any(err => err.HasError(line));
        }

        #endregion
    }
}
