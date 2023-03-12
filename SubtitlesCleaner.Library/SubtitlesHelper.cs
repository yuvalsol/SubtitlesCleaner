using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace SubtitlesCleaner.Library
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
        //public static readonly Regex regexAccentedCharacters = new Regex(@"[À-Ýà-ÿ]", RegexOptions.Compiled);

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
                    if (subtitle.Lines == null)
                        subtitle.Lines = new List<string>();
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
            if (subtitles.IsNullOrEmpty())
                return new string[0];

            return subtitles.SelectMany((subtitle, index) => subtitle.ToLines(index)).ToArray();
        }

        public static string[] ToLines(this Subtitle subtitle, int index)
        {
            string[] lines = new string[(subtitle.Lines.HasAny() ? subtitle.Lines.Count : 0) + 3];
            lines[0] = (index + 1).ToString();
            lines[1] = subtitle.TimeToString();
            if (subtitle.Lines.HasAny())
                subtitle.Lines.CopyTo(lines, 2);
            lines[lines.Length - 1] = string.Empty;
            return lines;
        }

        public static List<Subtitle> Clone(this List<Subtitle> subtitles)
        {
            return subtitles.Select(subtitle => subtitle.Clone()).Cast<Subtitle>().ToList();
        }

        #endregion

        #region Clean Subtitles

        public static Subtitle CleanSubtitle(this Subtitle subtitle, bool cleanHICaseInsensitive, bool isPrintCleaning)
        {
            return CleanSubtitle(subtitle, cleanHICaseInsensitive, false, isPrintCleaning);
        }

        public static List<Subtitle> CleanSubtitles(this List<Subtitle> subtitles, bool cleanHICaseInsensitive, bool isPrintCleaning)
        {
            return CleanSubtitles(subtitles, cleanHICaseInsensitive, false, isPrintCleaning);
        }

        private static Subtitle CleanSubtitle(this Subtitle subtitle, bool cleanHICaseInsensitive, bool isCheckMode, bool isPrintCleaning)
        {
            return new List<Subtitle>() { subtitle }.CleanSubtitles(cleanHICaseInsensitive, isCheckMode, isPrintCleaning)?.FirstOrDefault();
        }

        private static List<Subtitle> CleanSubtitles(this List<Subtitle> subtitles, bool cleanHICaseInsensitive, bool isCheckMode, bool isPrintCleaning)
        {
            if (subtitles.IsNullOrEmpty())
                return null;

            subtitles = IterateSubtitlesPre(subtitles, cleanHICaseInsensitive, isCheckMode, isPrintCleaning);
            if (subtitles.IsNullOrEmpty())
                return null;

            bool subtitlesChanged = false;
            List<int> infiniteLoopProbableCulprits = new List<int>();
            int loopCount = 0;
            int loopThresh = 6;
            do
            {
                subtitlesChanged = false;
                int lastSubtitleToChange = -1;
                subtitles = IterateSubtitles(subtitles, cleanHICaseInsensitive, isCheckMode, ref subtitlesChanged, ref lastSubtitleToChange, isPrintCleaning);
                if (subtitles.IsNullOrEmpty())
                    return null;

                loopCount++;

                if (subtitlesChanged && loopCount > 1) // most subtitles that need to be deleted, will do so on loop 1
                {
                    if (lastSubtitleToChange != -1)
                        infiniteLoopProbableCulprits.Add(lastSubtitleToChange);
                }

                if (subtitlesChanged && loopCount == loopThresh)
                {
                    Subtitle probableCulprit = null;

                    var culprits = infiniteLoopProbableCulprits
                        .GroupBy(n => n)
                        .Select(g => new { Index = g.Key, Count = g.Count() })
                        .OrderByDescending(x => x.Count)
                        .ThenByDescending(x => x.Index);

                    var probableCulpritItem = culprits.FirstOrDefault();
                    if (probableCulpritItem != null)
                    {
                        if (0 <= probableCulpritItem.Index && probableCulpritItem.Index <= subtitles.Count - 1)
                            probableCulprit = subtitles[probableCulpritItem.Index];
                    }

                    string errorMessage = "Subtitles Cleaning Infinite Loop";
                    if (probableCulprit != null)
                    {
                        errorMessage +=
                            Environment.NewLine + "Subtitle probable cause for infinite loop" +
                            Environment.NewLine + probableCulprit.TimeToString() +
                            Environment.NewLine + probableCulprit.ToString();
                    }

                    throw new Exception(errorMessage);
                }
            } while (subtitlesChanged);

            subtitles = IterateSubtitlesPost(subtitles, cleanHICaseInsensitive, isCheckMode, isPrintCleaning);
            if (subtitles.IsNullOrEmpty())
                return null;

            foreach (var subtitle in subtitles)
            {
                if (subtitle.SubtitleError.IsSet(SubtitleError.Non_Subtitle))
                    subtitle.SubtitleError = SubtitleError.Non_Subtitle;
            }

            return subtitles;
        }

        private static List<Subtitle> IterateSubtitlesPre(List<Subtitle> subtitles, bool cleanHICaseInsensitive, bool isCheckMode, bool isPrintCleaning)
        {
            if (subtitles.IsNullOrEmpty())
                return null;

            for (int k = subtitles.Count - 1; k >= 0; k--)
            {
                Subtitle subtitle = subtitles[k];
                SubtitleError subtitleError = SubtitleError.None;

                if (subtitle.Lines.HasAny())
                {
                    for (int i = subtitle.Lines.Count - 1; i >= 0; i--)
                    {
                        string line = subtitle.Lines[i];

                        subtitleError = SubtitleError.None;
                        bool isEmpty = string.IsNullOrEmpty(CleanLine(line, EmptyLine, false, isCheckMode, ref subtitleError, isPrintCleaning));
                        if (isEmpty)
                        {
                            subtitle.Lines.RemoveAt(i);
                            if (isCheckMode)
                                subtitle.SubtitleError |= SubtitleError.Empty_Line;
                            if (subtitle.Lines.Count == 0)
                            {
                                subtitle.Lines = null;
                                break;
                            }
                        }
                        else
                        {
                            subtitleError = SubtitleError.None;
                            string cleanLine = (CleanLine(line, NonSubtitle, false, isCheckMode, ref subtitleError, isPrintCleaning) ?? string.Empty).Trim();
                            bool isChanged = (line != cleanLine);
                            if (isChanged)
                            {
                                subtitle.Lines = null;
                                if (isCheckMode)
                                    subtitle.SubtitleError |= SubtitleError.Non_Subtitle;
                                break;
                            }
                            else
                            {
                                subtitleError = SubtitleError.None;
                                cleanLine = (CleanLine(line, FindAndReplaceRulesPre, cleanHICaseInsensitive, isCheckMode, ref subtitleError, isPrintCleaning) ?? string.Empty).Trim();
                                isChanged = (line != cleanLine);
                                if (isChanged)
                                {
                                    if (isCheckMode)
                                        subtitle.SubtitleError |= subtitleError;

                                    if (string.IsNullOrEmpty(cleanLine))
                                    {
                                        subtitle.Lines.RemoveAt(i);
                                        if (subtitle.Lines.Count == 0)
                                        {
                                            subtitle.Lines = null;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        subtitle.Lines[i] = cleanLine;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    // if the subtitle is complete without lines
                    // this is the one and only block it will enter
                    if (isCheckMode)
                        subtitle.SubtitleError |= SubtitleError.Empty_Line;
                }

                if (subtitle.Lines.HasAny())
                {
                    subtitleError = SubtitleError.None;
                    List<string> cleanLines = subtitle.Lines.GetRange(0, subtitle.Lines.Count);
                    cleanLines = CleanSubtitleMultipleLinesPre(cleanLines, cleanHICaseInsensitive, isCheckMode, ref subtitleError, isPrintCleaning);
                    bool isChanged =
                        cleanLines == null ||
                        subtitle.Lines.Count != cleanLines.Count ||
                        subtitle.Lines.Zip(cleanLines, (l1, l2) => l1 != l2).Any(isLineChanged => isLineChanged);
                    if (isChanged)
                    {
                        if (isCheckMode)
                            subtitle.SubtitleError |= subtitleError;

                        if (cleanLines.IsNullOrEmpty())
                        {
                            if (isCheckMode)
                                subtitle.Lines = null;
                            else
                                subtitles.RemoveAt(k);
                        }
                        else
                        {
                            subtitle.Lines = cleanLines;
                        }
                    }
                }
                else
                {
                    if (isCheckMode)
                        subtitle.Lines = null;
                    else
                        subtitles.RemoveAt(k);
                }

                if (subtitles.Count == 0)
                    return null;
            }

            return subtitles;
        }

        private static List<Subtitle> IterateSubtitles(List<Subtitle> subtitles, bool cleanHICaseInsensitive, bool isCheckMode, ref bool subtitlesChanged, ref int lastSubtitleToChange, bool isPrintCleaning)
        {
            if (subtitles.IsNullOrEmpty())
                return null;

            for (int k = subtitles.Count - 1; k >= 0; k--)
            {
                Subtitle subtitle = subtitles[k];
                SubtitleError subtitleError = SubtitleError.None;

                if (subtitle.Lines.HasAny())
                {
                    for (int i = subtitle.Lines.Count - 1; i >= 0; i--)
                    {
                        string line = subtitle.Lines[i];

                        subtitleError = SubtitleError.None;
                        bool isEmpty = string.IsNullOrEmpty(CleanLine(line, EmptyLine, false, isCheckMode, ref subtitleError, isPrintCleaning));
                        if (isEmpty)
                        {
                            subtitle.Lines.RemoveAt(i);
                            subtitlesChanged = true;
                            lastSubtitleToChange = k;
                            if (isCheckMode)
                                subtitle.SubtitleError |= SubtitleError.Empty_Line;
                            if (subtitle.Lines.Count == 0)
                            {
                                subtitle.Lines = null;
                                break;
                            }
                        }
                        else
                        {
                            subtitleError = SubtitleError.None;
                            string cleanLine = (CleanLine(line, FindAndReplaceRules, cleanHICaseInsensitive, isCheckMode, ref subtitleError, isPrintCleaning) ?? string.Empty).Trim();
                            bool isChanged = (line != cleanLine);
                            if (isChanged)
                            {
                                subtitlesChanged = true;
                                lastSubtitleToChange = k;
                                if (isCheckMode)
                                    subtitle.SubtitleError |= subtitleError;

                                if (string.IsNullOrEmpty(cleanLine))
                                {
                                    subtitle.Lines.RemoveAt(i);
                                    if (subtitle.Lines.Count == 0)
                                    {
                                        subtitle.Lines = null;
                                        break;
                                    }
                                }
                                else
                                {
                                    subtitle.Lines[i] = cleanLine;
                                }
                            }
                        }
                    }
                }

                if (subtitle.Lines.HasAny())
                {
                    subtitleError = SubtitleError.None;
                    List<string> cleanLines = subtitle.Lines.GetRange(0, subtitle.Lines.Count);
                    cleanLines = CleanSubtitleMultipleLines(cleanLines, cleanHICaseInsensitive, isCheckMode, ref subtitleError, isPrintCleaning);
                    bool isChanged =
                        cleanLines == null ||
                        subtitle.Lines.Count != cleanLines.Count ||
                        subtitle.Lines.Zip(cleanLines, (l1, l2) => l1 != l2).Any(isLineChanged => isLineChanged);
                    if (isChanged)
                    {
                        subtitlesChanged = true;
                        lastSubtitleToChange = k;
                        if (isCheckMode)
                            subtitle.SubtitleError |= subtitleError;

                        if (cleanLines.IsNullOrEmpty())
                        {
                            if (isCheckMode)
                                subtitle.Lines = null;
                            else
                                subtitles.RemoveAt(k);
                        }
                        else
                        {
                            subtitle.Lines = cleanLines;
                        }
                    }
                }
                else
                {
                    if (isCheckMode)
                        subtitle.Lines = null;
                    else
                        subtitles.RemoveAt(k);
                }

                if (subtitles.Count == 0)
                    return null;
            }

            return subtitles;
        }

        private static List<Subtitle> IterateSubtitlesPost(List<Subtitle> subtitles, bool cleanHICaseInsensitive, bool isCheckMode, bool isPrintCleaning)
        {
            if (subtitles.IsNullOrEmpty())
                return null;

            for (int k = subtitles.Count - 1; k >= 0; k--)
            {
                Subtitle subtitle = subtitles[k];
                SubtitleError subtitleError = SubtitleError.None;

                if (subtitle.Lines.HasAny())
                {
                    for (int i = subtitle.Lines.Count - 1; i >= 0; i--)
                    {
                        string line = subtitle.Lines[i];

                        subtitleError = SubtitleError.None;
                        bool isEmpty = string.IsNullOrEmpty(CleanLine(line, EmptyLine, false, isCheckMode, ref subtitleError, isPrintCleaning));
                        if (isEmpty)
                        {
                            subtitle.Lines.RemoveAt(i);
                            if (isCheckMode)
                                subtitle.SubtitleError |= SubtitleError.Empty_Line;
                            if (subtitle.Lines.Count == 0)
                            {
                                subtitle.Lines = null;
                                break;
                            }
                        }
                    }
                }

                if (subtitle.Lines.HasAny())
                {
                    subtitleError = SubtitleError.None;
                    List<string> cleanLines = subtitle.Lines.GetRange(0, subtitle.Lines.Count);
                    cleanLines = CleanSubtitleMultipleLinesPost(cleanLines, cleanHICaseInsensitive, isCheckMode, ref subtitleError, isPrintCleaning);
                    bool isChanged =
                        cleanLines == null ||
                        subtitle.Lines.Count != cleanLines.Count ||
                        subtitle.Lines.Zip(cleanLines, (l1, l2) => l1 != l2).Any(isLineChanged => isLineChanged);
                    if (isChanged)
                    {
                        if (isCheckMode)
                            subtitle.SubtitleError |= subtitleError;

                        if (cleanLines.IsNullOrEmpty())
                        {
                            if (isCheckMode)
                                subtitle.Lines = null;
                            else
                                subtitles.RemoveAt(k);
                        }
                        else
                        {
                            subtitle.Lines = cleanLines;
                        }
                    }
                }
                else
                {
                    if (isCheckMode)
                        subtitle.Lines = null;
                    else
                        subtitles.RemoveAt(k);
                }

                if (subtitles.Count == 0)
                    return null;
            }

            return subtitles;
        }

        private static string CleanLine(string line, FindAndReplace[] rules, bool cleanHICaseInsensitive, bool isCheckMode, ref SubtitleError subtitleError, bool isPrintCleaning)
        {
            if (string.IsNullOrEmpty(line))
            {
                if (isCheckMode)
                    subtitleError |= SubtitleError.Empty_Line;
                return null;
            }

            foreach (var rule in rules)
            {
                string cleanLine = rule.CleanLine(line, cleanHICaseInsensitive);

                if (line != cleanLine)
                {
                    if (isPrintCleaning)
                        PrintCleaning(line, cleanLine, rule, cleanHICaseInsensitive);

                    line = cleanLine;

                    if (isCheckMode)
                        subtitleError |= rule.SubtitleError;
                }
            }

            return line;
        }

        #region Multiple Lines Pre

        private static List<string> CleanSubtitleMultipleLinesPre(List<string> lines, bool cleanHICaseInsensitive, bool isCheckMode, ref SubtitleError subtitleError, bool isPrintCleaning)
        {
            if (lines.IsNullOrEmpty())
            {
                if (isCheckMode)
                    subtitleError |= SubtitleError.Empty_Line;
                return null;
            }

            /*lines = CleanMissingNewLineMultipleLines(lines, cleanHICaseInsensitive, isCheckMode, ref subtitleError, isPrintCleaning);*/
            lines = CleanLyricsMultipleLines(lines, cleanHICaseInsensitive, isCheckMode, ref subtitleError, isPrintCleaning);
            lines = CleanHIPrefixWithoutDialogDash(lines, cleanHICaseInsensitive, isCheckMode, ref subtitleError, isPrintCleaning);

            return lines;
        }

        /*public static readonly Regex regexMissingNewLine = new Regex(@"[!?][A-ZÀ-Ýa-zà-ÿ]", RegexOptions.Compiled);

        public static List<string> CleanMissingNewLineMultipleLines(List<string> lines, bool cleanHICaseInsensitive, bool isCheckMode, ref SubtitleError subtitleError, bool isPrintCleaning)
        {
            if (lines.IsNullOrEmpty())
            {
                if (isCheckMode)
                    subtitleError |= SubtitleError.Empty_Line;
                return null;
            }

            for (int i = lines.Count - 1; i >= 0; i--)
            {
                string line = lines[i];
                if (regexMissingNewLine.IsMatch(line))
                {
                    string lineBefore = (isPrintCleaning ? line : null);

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

                    if (isPrintCleaning)
                        PrintCleaning(lineBefore, splitLines, regexMissingNewLine, null);

                    if (isCheckMode)
                        subtitleError |= SubtitleError.Missing_New_Line;
                }
            }

            return lines;
        }*/

        public static readonly Regex regexNoteStart = new Regex(@"^(?:-\s*)?(?<Note>♪+)", RegexOptions.Compiled);
        public static readonly Regex regexNoteEnd = new Regex(@"\s+(?<Note>♪+)$", RegexOptions.Compiled);
        public static readonly Regex regexQMStart = new Regex(@"^(?:-\s*)?(?<QM>\?+)", RegexOptions.Compiled);
        public static readonly Regex regexQMEnd = new Regex(@"\s+(?<QM>\?+)$", RegexOptions.Compiled);

        public static List<string> CleanLyricsMultipleLines(List<string> lines, bool cleanHICaseInsensitive, bool isCheckMode, ref SubtitleError subtitleError, bool isPrintCleaning)
        {
            if (lines.IsNullOrEmpty())
            {
                if (isCheckMode)
                    subtitleError |= SubtitleError.Empty_Line;
                return null;
            }

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
                    isStartsWithDash = line.StartsWith("-")
                }).ToArray();

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
                            {
                                lines[startItem.index] = regexQMStart.ReplaceGroup(startItem.line, "QM", "♪");

                                if (isPrintCleaning)
                                    PrintCleaning(startItem.line, lines[startItem.index], regexQMStart, groupName: "QM", replacement: "♪");

                                if (isCheckMode)
                                    subtitleError |= SubtitleError.Notes_Error;
                            }

                            if (endItem.isEndsWithQM)
                            {
                                lines[endItem.index] = regexQMEnd.ReplaceGroup(endItem.line, "QM", "♪");

                                if (isPrintCleaning)
                                    PrintCleaning(endItem.line, lines[endItem.index], regexQMEnd, groupName: "QM", replacement: "♪");

                                if (isCheckMode)
                                    subtitleError |= SubtitleError.Notes_Error;
                            }

                            if (startItem.isStartsWithDash &&
                                itemsBetween.All(item => item.isStartsWithDash == false) &&
                                endItem.isStartsWithDash == false)
                            {
                                string lineBefore = (isPrintCleaning ? lines[startItem.index] : null);

                                lines[startItem.index] = lines[startItem.index].TrimStart('-');

                                if (isPrintCleaning)
                                    PrintCleaning(lineBefore, lines[startItem.index]);

                                if (isCheckMode)
                                    subtitleError |= SubtitleError.Notes_Error;
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
            }

            return lines;
        }

        public static readonly Regex regexHIPrefix = new Regex(@"^(?<Prefix>(?:<i>)?-?\s*|-?\s*(?:<i>)?\s*)[" + HI_CHARS + @"]*[A-ZÀ-Ý]+[" + HI_CHARS + @"]*:\s*(?<Subtitle>.*?)$", RegexOptions.Compiled);
        public static readonly Regex regexHIPrefixCI = new Regex(@"^(?<Prefix>(?:<i>)?-?\s*|-?\s*(?:<i>)?\s*)[" + HI_CHARS_CI + @"]*[A-ZÀ-Ý]+[" + HI_CHARS_CI + @"]*:(?!\d\d)\s*(?<Subtitle>.*?)$", RegexOptions.Compiled);
        public static readonly Regex regexHIPrefixWithoutDialogDash = new Regex(regexHIPrefix.ToString().Replace("-?", string.Empty), RegexOptions.Compiled);
        public static readonly Regex regexHIPrefixWithoutDialogDashCI = new Regex(regexHIPrefixCI.ToString().Replace("-?", string.Empty), RegexOptions.Compiled);

        public static List<string> CleanHIPrefixWithoutDialogDash(List<string> lines, bool cleanHICaseInsensitive, bool isCheckMode, ref SubtitleError subtitleError, bool isPrintCleaning)
        {
            if (lines.IsNullOrEmpty())
            {
                if (isCheckMode)
                    subtitleError |= SubtitleError.Empty_Line;
                return null;
            }

            if (lines.Count > 1)
            {
                FindAndReplace[] regexInlineHIWithoutDialog = HearingImpaired.ByGroup("Inline HI Without Dialog");
                var results1 = lines.Select((line, index) => new
                {
                    line,
                    index,
                    isMatchDialog = regexDialog.IsMatch(line),
                    isInlineHIWithoutDialog = regexInlineHIWithoutDialog.Any(far => (cleanHICaseInsensitive && far.HasRegexCI ? far.RegexCI : far.Regex).IsMatch(line))
                }).ToArray();

                for (int i = 1; i < results1.Length; i++)
                {
                    var prevItem = results1[i - 1];
                    var item = results1[i];

                    // Line 1. (or - Line 1.)
                    // MAN: Line 2.
                    //
                    // - Line 1.
                    // - MAN: Line 2.
                    if (item.isMatchDialog == false &&
                        prevItem.isInlineHIWithoutDialog == false &&
                        item.isInlineHIWithoutDialog)
                    {
                        if (prevItem.isMatchDialog == false)
                            lines[i - 1] = "- " + prevItem.line;
                        lines[i] = "- " + item.line;

                        if (isPrintCleaning)
                            PrintCleaning(new string[] { prevItem.line, item.line }, new string[] { lines[i - 1], lines[i] });

                        if (isCheckMode)
                            subtitleError |= SubtitleError.Dialog_Error;
                    }
                }

                FindAndReplace[] regexHIFullLine = HearingImpairedFullLine.ByGroup("HI Full Line");

                var results2 = lines.Select((line, index) => new
                {
                    line,
                    index,
                    isMatchDialog = regexDialog.IsMatch(line),
                    isHIFullLine = regexHIFullLine.Any(far => (cleanHICaseInsensitive && far.HasRegexCI ? far.RegexCI : far.Regex).IsMatch(line))
                }).ToArray();

                List<int> toDeleeIndexes = new List<int>();

                for (int i = 2; i < results2.Length; i++)
                {
                    var prevItem2 = results2[i - 2];
                    var prevItem1 = results2[i - 1];
                    var item = results2[i];

                    // Line 1. (or - Line 1.)
                    // MAN:
                    // Line 3.
                    //
                    // - Line 1.
                    // - MAN: Line 3.
                    if (prevItem1.isMatchDialog == false &&
                        item.isMatchDialog == false &&
                        prevItem2.isHIFullLine == false &&
                        prevItem1.isHIFullLine &&
                        item.isHIFullLine == false)
                    {
                        if (prevItem2.isMatchDialog == false)
                            lines[i - 2] = "- " + prevItem2.line;
                        lines[i - 1] = "- " + prevItem1.line + " " + item.line;
                        toDeleeIndexes.Add(i);

                        if (isPrintCleaning)
                            PrintCleaning(new string[] { prevItem2.line, prevItem1.line, item.line }, new string[] { lines[i - 2], lines[i - 1] });

                        i += 2; // 3 (the next 3) - 1 (counter the i++)

                        if (isCheckMode)
                            subtitleError |= SubtitleError.Dialog_Error;
                    }
                }

                for (int i = toDeleeIndexes.Count - 1; i >= 0; i--)
                {
                    lines.RemoveAt(toDeleeIndexes[i]);
                }

                var results3 = lines.Select((line, index) => new
                {
                    line,
                    index,
                    isMatchHIPrefix = (cleanHICaseInsensitive ? regexHIPrefixWithoutDialogDashCI : regexHIPrefixWithoutDialogDash).IsMatch(line)
                }).ToArray();

                if (results3.Count(x => x.isMatchHIPrefix) > 1)
                {
                    foreach (var item in results3)
                    {
                        if (item.isMatchHIPrefix)
                        {
                            string lineBefore = (isPrintCleaning ? lines[item.index] : null);

                            Match match = (cleanHICaseInsensitive ? regexHIPrefixWithoutDialogDashCI : regexHIPrefixWithoutDialogDash).Match(lines[item.index]);
                            lines[item.index] = (match.Groups["Prefix"].Value + " - " + match.Groups["Subtitle"].Value).Trim();

                            if (isPrintCleaning)
                                PrintCleaning(lineBefore, lines[item.index], (cleanHICaseInsensitive ? regexHIPrefixWithoutDialogDashCI : regexHIPrefixWithoutDialogDash), "${Prefix} - ${Subtitle}");
                        }
                    }

                    if (isCheckMode)
                        subtitleError |= SubtitleError.Hearing_Impaired;
                }
            }

            return lines;
        }

        #endregion

        #region Multiple Lines

        private static List<string> CleanSubtitleMultipleLines(List<string> lines, bool cleanHICaseInsensitive, bool isCheckMode, ref SubtitleError subtitleError, bool isPrintCleaning)
        {
            if (lines.IsNullOrEmpty())
            {
                if (isCheckMode)
                    subtitleError |= SubtitleError.Empty_Line;
                return null;
            }

            lines = CleanMergedLinesWithHIToSingleLine(lines, cleanHICaseInsensitive, isCheckMode, ref subtitleError, isPrintCleaning);
            lines = CleanHearingImpairedMultipleLines(lines, cleanHICaseInsensitive, isCheckMode, ref subtitleError, isPrintCleaning);
            lines = CleanItalicsThreeLines(lines, cleanHICaseInsensitive, isCheckMode, ref subtitleError, isPrintCleaning);
            lines = CleanItalicsTwoLines(lines, cleanHICaseInsensitive, isCheckMode, ref subtitleError, isPrintCleaning);
            lines = CleanDialogSingleLine(lines, cleanHICaseInsensitive, isCheckMode, ref subtitleError, isPrintCleaning);
            lines = CleanHIPrefixSingleLine(lines, cleanHICaseInsensitive, isCheckMode, ref subtitleError, isPrintCleaning);
            lines = CleanHIPrefixMultipleLines(lines, cleanHICaseInsensitive, isCheckMode, ref subtitleError, isPrintCleaning);
            lines = CleanDialogMultipleLines(lines, cleanHICaseInsensitive, isCheckMode, ref subtitleError, isPrintCleaning);
            lines = CleanNotesMultipleLines(lines, cleanHICaseInsensitive, isCheckMode, ref subtitleError, isPrintCleaning);
            lines = CleanStartWithPunctuationSingleLine(lines, cleanHICaseInsensitive, isCheckMode, ref subtitleError, isPrintCleaning);

            return lines;
        }

        public static readonly Regex regexMergedLinesWithHI = new Regex(@"(?<Line1>^-[^-]+)(?<Line2><i>-.*?</i>$)", RegexOptions.Compiled);

        public static List<string> CleanMergedLinesWithHIToSingleLine(List<string> lines, bool cleanHICaseInsensitive, bool isCheckMode, ref SubtitleError subtitleError, bool isPrintCleaning)
        {
            if (lines.IsNullOrEmpty())
            {
                if (isCheckMode)
                    subtitleError |= SubtitleError.Empty_Line;
                return null;
            }

            if (lines.Count == 1)
            {
                // - Line 1 <i>- Line 2</i>
                //
                // - Line 1
                // <i>- Line 2</i>
                if (regexMergedLinesWithHI.IsMatch(lines[0]))
                {
                    string lineBefore = (isPrintCleaning ? lines[0] : null);

                    Match match = regexMergedLinesWithHI.Match(lines[0]);
                    string line1 = match.Groups["Line1"].Value;
                    string line2 = match.Groups["Line2"].Value;
                    lines[0] = line1;
                    lines.Add(line2);

                    if (isPrintCleaning)
                        PrintCleaning(lineBefore, new string[] { line1, line2 }, regexMergedLinesWithHI, "${Line1}|${Line2}");

                    if (isCheckMode)
                        subtitleError |= SubtitleError.Hearing_Impaired;
                }
            }

            return lines;
        }

        // start (: ^ <i>? ♪? ( anything except () $
        //   end ): ^ anything except () ) ♪? <i>? $
        public static readonly Regex regexHI1Start = new Regex(@"^(?:-|<i>|♪|\s)*\([^\(\)]*?$", RegexOptions.Compiled);
        public static readonly Regex regexHI1End = new Regex(@"^[^\(\)]*?\)\s*(?:\s*♪)?(?:\s*</i>)?$", RegexOptions.Compiled);
        public static readonly Regex regexHI2Start = new Regex(@"^(?:-|<i>|♪|\s)*\[[^\[\]]*?$", RegexOptions.Compiled);
        public static readonly Regex regexHI2End = new Regex(@"^[^\[\]]*?\]\s*(?:\s*♪)?(?:\s*</i>)?$", RegexOptions.Compiled);

        public static List<string> CleanHearingImpairedMultipleLines(List<string> lines, bool cleanHICaseInsensitive, bool isCheckMode, ref SubtitleError subtitleError, bool isPrintCleaning)
        {
            if (lines.IsNullOrEmpty())
            {
                if (isCheckMode)
                    subtitleError |= SubtitleError.Empty_Line;
                return null;
            }

            if (lines.Count > 1)
            {
                string firstLine = lines[0];
                string lastLine = lines[lines.Count - 1];

                // first line starts with (
                // last line ends with )
                // lines between don't have ()
                if (regexHI1Start.IsMatch(firstLine) && regexHI1End.IsMatch(lastLine))
                {
                    if (lines.Skip(1).Take(lines.Count - 2).Any(line => line.Contains("(") || line.Contains(")")) == false)
                    {
                        if (isPrintCleaning)
                        {
                            PrintCleaningMethodName();
                            PrintCleaningRegex(regexHI1Start, regexHI1End);
                            PrintCleaning(lines, string.Empty, name: null);
                        }

                        if (isCheckMode)
                            subtitleError |= SubtitleError.Hearing_Impaired;
                        return null;
                    }
                }

                // first line starts with [
                // last line ends with ]
                // lines between don't have []
                if (regexHI2Start.IsMatch(firstLine) && regexHI2End.IsMatch(lastLine))
                {
                    if (lines.Skip(1).Take(lines.Count - 2).Any(line => line.Contains("[") || line.Contains("]")) == false)
                    {
                        if (isPrintCleaning)
                        {
                            PrintCleaningMethodName();
                            PrintCleaningRegex(regexHI2Start, regexHI2End);
                            PrintCleaning(lines, string.Empty, name: null);
                        }

                        if (isCheckMode)
                            subtitleError |= SubtitleError.Hearing_Impaired;
                        return null;
                    }
                }

                // consecutive lines have parentheses or brackets
                for (int i = 1; i < lines.Count; i++)
                {
                    string line1 = lines[i - 1];
                    string line2 = lines[i];

                    if (regexHI1Start.IsMatch(line1) && regexHI1End.IsMatch(line2))
                    {
                        lines.RemoveAt(i);
                        lines.RemoveAt(i - 1);
                        i--;

                        if (isPrintCleaning)
                        {
                            PrintCleaningMethodName();
                            PrintCleaningRegex(regexHI1Start, regexHI1End);
                            PrintCleaning(new string[] { line1, line2 }, string.Empty, name: null);
                        }

                        if (isCheckMode)
                            subtitleError |= SubtitleError.Hearing_Impaired;
                    }
                    else if (regexHI2Start.IsMatch(line1) && regexHI2End.IsMatch(line2))
                    {
                        lines.RemoveAt(i);
                        lines.RemoveAt(i - 1);
                        i--;

                        if (isPrintCleaning)
                        {
                            PrintCleaningMethodName();
                            PrintCleaningRegex(regexHI2Start, regexHI2End);
                            PrintCleaning(new string[] { line1, line2 }, string.Empty, name: null);
                        }

                        if (isCheckMode)
                            subtitleError |= SubtitleError.Hearing_Impaired;
                    }
                }
            }

            return lines;
        }

        public static List<string> CleanItalicsThreeLines(List<string> lines, bool cleanHICaseInsensitive, bool isCheckMode, ref SubtitleError subtitleError, bool isPrintCleaning)
        {
            if (lines.IsNullOrEmpty())
            {
                if (isCheckMode)
                    subtitleError |= SubtitleError.Empty_Line;
                return null;
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

                    if (isPrintCleaning)
                        PrintCleaning(new string[] { line1, line2, line3 }, new string[] { lines[1], line3 });

                    if (isCheckMode)
                        subtitleError |= SubtitleError.Redundant_Spaces;
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

                    if (isPrintCleaning)
                        PrintCleaning(new string[] { line1, line2, line3 }, new string[] { lines[1], line3 });

                    if (isCheckMode)
                        subtitleError |= SubtitleError.Redundant_Spaces;
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

                    if (isPrintCleaning)
                        PrintCleaning(new string[] { line1, line2, line3 }, new string[] { lines[1], line3 });

                    if (isCheckMode)
                        subtitleError |= SubtitleError.Redundant_Spaces;
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

                    if (isPrintCleaning)
                        PrintCleaning(new string[] { line1, line2, line3 }, new string[] { line1, lines[1], lines[2] });

                    if (isCheckMode)
                        subtitleError |= SubtitleError.Redundant_Spaces;
                }
            }

            return lines;
        }

        public static List<string> CleanItalicsTwoLines(List<string> lines, bool cleanHICaseInsensitive, bool isCheckMode, ref SubtitleError subtitleError, bool isPrintCleaning)
        {
            if (lines.IsNullOrEmpty())
            {
                if (isCheckMode)
                    subtitleError |= SubtitleError.Empty_Line;
                return null;
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

                    if (isPrintCleaning)
                        PrintCleaning(new string[] { line1, line2 }, lines[1]);

                    if (isCheckMode)
                        subtitleError |= SubtitleError.Redundant_Spaces;
                }
                // Line 1: <i>
                // Line 2: Text</i>
                //
                // Line 2: <i>Text</i>
                else if (line1 == "<i>" && line2.StartsWith("<i>") == false && line2.EndsWith("</i>"))
                {
                    lines[1] = "<i>" + line2;
                    lines.RemoveAt(0);

                    if (isPrintCleaning)
                        PrintCleaning(new string[] { line1, line2 }, lines[1]);

                    if (isCheckMode)
                        subtitleError |= SubtitleError.Redundant_Spaces;
                }
                // Line 1: - <i>
                // Line 2: - Text</i>
                //
                // Line 2: <i>Text</i>
                else if (line1 == "- <i>" && line2.StartsWith("- ") && line2.EndsWith("</i>"))
                {
                    lines[1] = "<i>" + line2.Substring(2);
                    lines.RemoveAt(0);

                    if (isPrintCleaning)
                        PrintCleaning(new string[] { line1, line2 }, lines[1]);

                    if (isCheckMode)
                        subtitleError |= SubtitleError.Redundant_Spaces;
                }
                // Line 1: <i>Text 1
                // Line 2: </i>
                //
                // Line 1: <i>Text 1</i>
                else if (line1.StartsWith("<i>") && line1.EndsWith("</i>") == false && line2 == "</i>")
                {
                    lines[0] = lines[0] + "</i>";
                    lines.RemoveAt(1);

                    if (isPrintCleaning)
                        PrintCleaning(new string[] { line1, line2 }, lines[0]);

                    if (isCheckMode)
                        subtitleError |= SubtitleError.Redundant_Spaces;
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

                    if (isPrintCleaning)
                        PrintCleaning(new string[] { line1, line2 }, new string[] { lines[0], lines[1] });

                    if (isCheckMode)
                        subtitleError |= SubtitleError.Redundant_Spaces;
                }
            }

            return lines;
        }

        public static List<string> CleanDialogSingleLine(List<string> lines, bool cleanHICaseInsensitive, bool isCheckMode, ref SubtitleError subtitleError, bool isPrintCleaning)
        {
            if (lines.IsNullOrEmpty())
            {
                if (isCheckMode)
                    subtitleError |= SubtitleError.Empty_Line;
                return null;
            }

            if (lines.Count == 1)
            {
                if (regexDialog.IsMatch(lines[0]))
                {
                    string lineBefore = (isPrintCleaning ? lines[0] : null);

                    Match match = regexDialog.Match(lines[0]);
                    lines[0] = match.Groups["Italic"].Value + match.Groups["Subtitle"].Value;

                    if (isPrintCleaning)
                        PrintCleaning(lineBefore, lines[0], regexDialog, "${Italic}${Subtitle}");

                    if (isCheckMode)
                        subtitleError |= SubtitleError.Dialog_Error;
                }

                if (lines[0] == "-" || lines[0] == "<i>" || lines[0] == "</i>")
                {
                    if (isPrintCleaning)
                        PrintCleaning(lines[0], string.Empty);

                    if (isCheckMode)
                        subtitleError |= SubtitleError.Empty_Line;
                    return null;
                }
            }

            return lines;
        }

        public static List<string> CleanHIPrefixSingleLine(List<string> lines, bool cleanHICaseInsensitive, bool isCheckMode, ref SubtitleError subtitleError, bool isPrintCleaning)
        {
            if (lines.IsNullOrEmpty())
            {
                if (isCheckMode)
                    subtitleError |= SubtitleError.Empty_Line;
                return null;
            }

            if (lines.Count == 1)
            {
                if ((cleanHICaseInsensitive ? regexHIPrefixCI : regexHIPrefix).IsMatch(lines[0]))
                {
                    string lineBefore = (isPrintCleaning ? lines[0] : null);

                    Match match = (cleanHICaseInsensitive ? regexHIPrefixCI : regexHIPrefix).Match(lines[0]);
                    lines[0] = match.Groups["Prefix"].Value + match.Groups["Subtitle"].Value;

                    if (isPrintCleaning)
                        PrintCleaning(lineBefore, lines[0], (cleanHICaseInsensitive ? regexHIPrefixCI : regexHIPrefix), "${Prefix}${Subtitle}");

                    if (isCheckMode)
                        subtitleError |= SubtitleError.Hearing_Impaired;
                }

                if (lines[0] == "-" || lines[0] == "<i>" || lines[0] == "</i>")
                {
                    if (isPrintCleaning)
                        PrintCleaning(lines[0], string.Empty);

                    if (isCheckMode)
                        subtitleError |= SubtitleError.Empty_Line;
                    return null;
                }
            }

            return lines;
        }

        public static List<string> CleanHIPrefixMultipleLines(List<string> lines, bool cleanHICaseInsensitive, bool isCheckMode, ref SubtitleError subtitleError, bool isPrintCleaning)
        {
            if (lines.IsNullOrEmpty())
            {
                if (isCheckMode)
                    subtitleError |= SubtitleError.Empty_Line;
                return null;
            }

            if (lines.Count > 1)
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
                    string newLine = match.Groups["Subtitle"].Value;
                    if (lines[0] != newLine)
                    {
                        string lineBefore = (isPrintCleaning ? lines[0] : null);

                        lines[0] = newLine;

                        if (isPrintCleaning)
                            PrintCleaning(lineBefore, lines[0], (cleanHICaseInsensitive ? regexHIPrefixCI : regexHIPrefix), "${Subtitle}");

                        if (isCheckMode)
                            subtitleError |= SubtitleError.Hearing_Impaired;
                    }
                }
                else if (resultsHIPrefix.Count(x => x.isMatchHIPrefix) > 1)
                {
                    foreach (var item in resultsHIPrefix)
                    {
                        if (item.isMatchHIPrefix)
                        {
                            Match match = (cleanHICaseInsensitive ? regexHIPrefixCI : regexHIPrefix).Match(lines[item.index]);
                            string newLine = match.Groups["Prefix"].Value + match.Groups["Subtitle"].Value;
                            if (lines[item.index] != newLine)
                            {
                                string lineBefore = (isPrintCleaning ? lines[item.index] : null);

                                lines[item.index] = newLine;

                                if (isPrintCleaning)
                                    PrintCleaning(lineBefore, lines[item.index], (cleanHICaseInsensitive ? regexHIPrefixCI : regexHIPrefix), "${Prefix}${Subtitle}");

                                if (isCheckMode)
                                    subtitleError |= SubtitleError.Hearing_Impaired;
                            }
                        }
                    }
                }
            }

            return lines;
        }

        public static readonly Regex regexDialog = new Regex(@"^(?<Italic><i>)?-\s*(?<Subtitle>.*?)$", RegexOptions.Compiled);
        public static readonly Regex regexContainsDialog = new Regex(@" - [A-ZÀ-Ý]", RegexOptions.Compiled);
        public static readonly Regex regexEndsWithLowerCaseLetter = new Regex(@"[a-zà-ÿ]$", RegexOptions.Compiled);
        public static readonly Regex regexCapitalLetter = new Regex(@"[A-ZÀ-Ý]", RegexOptions.Compiled);
        public static readonly Regex regexLowerLetter = new Regex(@"[a-zà-ÿ]", RegexOptions.Compiled);

        public static List<string> CleanDialogMultipleLines(List<string> lines, bool cleanHICaseInsensitive, bool isCheckMode, ref SubtitleError subtitleError, bool isPrintCleaning)
        {
            if (lines.IsNullOrEmpty())
            {
                if (isCheckMode)
                    subtitleError |= SubtitleError.Empty_Line;
                return null;
            }

            if (lines.Count > 1)
            {
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
                    List<string> linesBefore = (isPrintCleaning ? new List<string>(lines) : null);

                    // ...line 1
                    // - Line 2
                    //
                    // - ...line 1
                    // - Line 2
                    lines[0] = "- " + lines[0];

                    if (isPrintCleaning)
                        PrintCleaning(linesBefore, lines);

                    if (isCheckMode)
                        subtitleError |= SubtitleError.Dialog_Error;
                }
                else if (resultsDialog[0].isStartsWithNote && resultsDialog.Skip(1).All(x => x.isMatchDialog))
                {
                    List<string> linesBefore = (isPrintCleaning ? new List<string>(lines) : null);

                    // ♪ Line 1
                    // - Line 2
                    //
                    // - ♪ Line 1
                    // - Line 2
                    lines[0] = "- " + lines[0];

                    if (isPrintCleaning)
                        PrintCleaning(linesBefore, lines);

                    if (isCheckMode)
                        subtitleError |= SubtitleError.Dialog_Error;
                }
                else if (resultsDialog[0].isStartsWithDotsAndItalics && resultsDialog.Skip(1).All(x => x.isMatchDialog))
                {
                    List<string> linesBefore = (isPrintCleaning ? new List<string>(lines) : null);

                    // <i>...line 1
                    // - Line 2
                    //
                    // <i>- ...line 1
                    // - Line 2
                    lines[0] = "<i>- " + lines[0].Substring(3);

                    if (isPrintCleaning)
                        PrintCleaning(linesBefore, lines);

                    if (isCheckMode)
                        subtitleError |= SubtitleError.Dialog_Error;
                }
                else if (resultsDialog[0].isMatchDialog && resultsDialog.Skip(1).All(x => x.isStartsWithDots || x.isStartsWithDotsAndItalics))
                {
                    List<string> linesBefore = (isPrintCleaning ? new List<string>(lines) : null);

                    // - Line 1
                    // ...line 2
                    //
                    // - Line 1
                    // - ...line 2
                    for (int i = 1; i < lines.Count; i++)
                        lines[i] = "- " + lines[i];

                    if (isPrintCleaning)
                        PrintCleaning(linesBefore, lines);

                    if (isCheckMode)
                        subtitleError |= SubtitleError.Dialog_Error;
                }
                else if (resultsDialog[0].isMatchDialog && resultsDialog.Skip(1).All(x => x.isStartsWithNote))
                {
                    List<string> linesBefore = (isPrintCleaning ? new List<string>(lines) : null);

                    // - Line 1
                    // ♪ Line 2
                    //
                    // - Line 1
                    // - ♪ Line 2
                    for (int i = 1; i < lines.Count; i++)
                        lines[i] = "- " + lines[i];

                    if (isPrintCleaning)
                        PrintCleaning(linesBefore, lines);

                    if (isCheckMode)
                        subtitleError |= SubtitleError.Dialog_Error;
                }
                else if (resultsDialog[0].isMatchDialog && resultsDialog.Skip(1).All(x => x.isMatchDialog == false) && resultsDialog.Skip(1).All(x => x.isContainsDialog_CapitalLetter == false))
                {
                    string firstCharSecondLine = (lines[1].Length > 0 ? lines[1][0].ToString() : string.Empty);

                    if (regexCapitalLetter.IsMatch(firstCharSecondLine))
                    {
                        if (resultsDialog[0].isMatchDialog &&
                            resultsDialog[0].isContainsDialog_CapitalLetter &&
                            (resultsDialog[1].isStartsWithI || resultsDialog[1].isStartsWithContractionI))
                        {
                            // - Line 1 - Dialog
                            // I am line 2
                            //
                            // do nothing
                        }
                        else if (resultsDialog[0].isMatchDialog &&
                            resultsDialog[0].isContainsDialog_CapitalLetter &&
                            resultsDialog[0].isEndsWithDots)
                        {
                            // - Line 1 - Dialog...
                            // Line 2
                            //
                            // do nothing
                        }
                        else if (resultsDialog[0].isMatchDialog &&
                            resultsDialog[0].isEndsWithComma &&
                            (resultsDialog[1].isStartsWithI || resultsDialog[1].isStartsWithContractionI))
                        {
                            // - Line 1,
                            // I'll Line 2
                            //
                            // Line 1,
                            // I'll Line 2
                            Match match = regexDialog.Match(lines[0]);
                            string newLine = match.Groups["Italic"].Value + match.Groups["Subtitle"].Value;
                            if (lines[0] != newLine)
                            {
                                List<string> linesBefore = (isPrintCleaning ? new List<string>(lines) : null);

                                lines[0] = newLine;

                                if (isPrintCleaning)
                                    PrintCleaning(linesBefore, lines, regexDialog, "${Italic}${Subtitle}");

                                if (isCheckMode)
                                    subtitleError |= SubtitleError.Dialog_Error;
                            }
                        }
                        else if (resultsDialog[0].isMatchDialog &&
                            resultsDialog[0].isEndsWithLowerCaseLetter &&
                            (resultsDialog[1].isStartsWithI || resultsDialog[1].isStartsWithContractionI))
                        {
                            // - Line 1 end with lower case letter
                            // I'll Line 2
                            //
                            // Line 1 end with lower case letter
                            // I'll Line 2
                            Match match = regexDialog.Match(lines[0]);
                            string newLine = match.Groups["Italic"].Value + match.Groups["Subtitle"].Value;
                            if (lines[0] != newLine)
                            {
                                List<string> linesBefore = (isPrintCleaning ? new List<string>(lines) : null);

                                lines[0] = newLine;

                                if (isPrintCleaning)
                                    PrintCleaning(linesBefore, lines, regexDialog, "${Italic}${Subtitle}");

                                if (isCheckMode)
                                    subtitleError |= SubtitleError.Dialog_Error;
                            }
                        }
                        else
                        {
                            List<string> linesBefore = (isPrintCleaning ? new List<string>(lines) : null);

                            // - Line 1
                            // Line 2
                            //
                            // - Line 1
                            // - Line 2
                            lines[1] = "- " + lines[1];

                            if (isPrintCleaning)
                                PrintCleaning(linesBefore, lines);

                            if (isCheckMode)
                                subtitleError |= SubtitleError.Dialog_Error;
                        }
                    }
                    else
                    {
                        if (resultsDialog[0].isMatchDialog &&
                            resultsDialog[0].isContainsDialog_CapitalLetter &&
                            resultsDialog[1].isMatchDialog == false)
                        {
                            // - Line 1 - Dialog
                            // line 2
                            //
                            // do nothing
                        }
                        else
                        {
                            // - Line 1
                            // line 2
                            //
                            // Line 1
                            // line 2
                            Match match = regexDialog.Match(lines[0]);
                            string newLine = match.Groups["Italic"].Value + match.Groups["Subtitle"].Value;
                            if (lines[0] != newLine)
                            {
                                List<string> linesBefore = (isPrintCleaning ? new List<string>(lines) : null);

                                lines[0] = newLine;

                                if (isPrintCleaning)
                                    PrintCleaning(linesBefore, lines, regexDialog, "${Italic}${Subtitle}");

                                if (isCheckMode)
                                    subtitleError |= SubtitleError.Dialog_Error;
                            }
                        }
                    }
                }
                else if (resultsDialog[0].isMatchDialog == false && resultsDialog.Skip(1).All(x => x.isMatchDialog))
                {
                    bool isStartsWithItalics = false;
                    string line0 = lines[0];
                    if (isStartsWithItalics = line0.StartsWith("<i>"))
                        line0 = line0.Substring(3);

                    string firstCharFirstLine = (lines[0].Length > 0 ? lines[0][0].ToString() : string.Empty);
                    if (isStartsWithItalics)
                        firstCharFirstLine = (lines[0].Length > 3 ? lines[0][3].ToString() : string.Empty);

                    if (regexLowerLetter.IsMatch(firstCharFirstLine) && (
                        resultsDialog[0].isEndsWithDots ||
                        resultsDialog[0].isEndsWithPeriod ||
                        resultsDialog[0].isEndsWithQuestionMark ||
                        resultsDialog[0].isEndsWithExclamationMark))
                    {
                        List<string> linesBefore = (isPrintCleaning ? new List<string>(lines) : null);

                        // line 1.
                        // - Line 2
                        //
                        // - line 1.
                        // - Line 2
                        lines[0] = (isStartsWithItalics ? "<i>" : string.Empty) + "- " + line0;

                        if (isPrintCleaning)
                            PrintCleaning(linesBefore, lines);

                        if (isCheckMode)
                            subtitleError |= SubtitleError.Dialog_Error;
                    }
                    else if (regexCapitalLetter.IsMatch(firstCharFirstLine))
                    {
                        List<string> linesBefore = (isPrintCleaning ? new List<string>(lines) : null);

                        // Line 1
                        // - Line 2
                        //
                        // - Line 1
                        // - Line 2
                        lines[0] = (isStartsWithItalics ? "<i>" : string.Empty) + "- " + line0;

                        if (isPrintCleaning)
                            PrintCleaning(linesBefore, lines);

                        if (isCheckMode)
                            subtitleError |= SubtitleError.Dialog_Error;
                    }
                    else
                    {
                        // Line 1
                        // - line 2
                        //
                        // Line 1
                        // line 2
                        Match match = regexDialog.Match(lines[1]);
                        string newLine = match.Groups["Italic"].Value + match.Groups["Subtitle"].Value;
                        if (lines[1] != newLine)
                        {
                            List<string> linesBefore = (isPrintCleaning ? new List<string>(lines) : null);

                            lines[1] = newLine;

                            if (isPrintCleaning)
                                PrintCleaning(linesBefore, lines, regexDialog, "${Italic}${Subtitle}");

                            if (isCheckMode)
                                subtitleError |= SubtitleError.Dialog_Error;
                        }
                    }
                }
                else if (resultsDialog.Count(x => x.isMatchDialog) > 1)
                {
                    foreach (var item in resultsDialog)
                    {
                        if (item.isMatchDialog)
                        {
                            Match match = regexDialog.Match(lines[item.index]);
                            string newLine = match.Groups["Italic"].Value + "- " + match.Groups["Subtitle"].Value;
                            if (lines[item.index] != newLine)
                            {
                                string lineBefore = (isPrintCleaning ? lines[item.index] : null);

                                lines[item.index] = newLine;

                                if (isPrintCleaning)
                                    PrintCleaning(lineBefore, lines[item.index], regexDialog, "${Italic} - ${Subtitle}");

                                if (isCheckMode)
                                    subtitleError |= SubtitleError.Dialog_Error;
                            }
                        }
                    }
                }
            }

            return lines;
        }

        public static List<string> CleanNotesMultipleLines(List<string> lines, bool cleanHICaseInsensitive, bool isCheckMode, ref SubtitleError subtitleError, bool isPrintCleaning)
        {
            if (lines.IsNullOrEmpty())
            {
                if (isCheckMode)
                    subtitleError |= SubtitleError.Empty_Line;
                return null;
            }

            if (lines.Count > 1)
            {
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

                List<string> linesBefore = (startsWithAmpersand.HasAny() && startsWithI.HasAny() && endsWithAmpersand.HasAny() && endsWithI.HasAny() ? new List<string>(lines) : null);

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

                if (startsWithAmpersand.HasAny() && startsWithI.HasAny() && endsWithAmpersand.HasAny() && endsWithI.HasAny())
                {
                    if (isPrintCleaning)
                        PrintCleaning(linesBefore, lines);

                    if (isCheckMode)
                        subtitleError |= SubtitleError.Notes_Error;
                }
            }

            return lines;
        }

        public static List<string> CleanStartWithPunctuationSingleLine(List<string> lines, bool cleanHICaseInsensitive, bool isCheckMode, ref SubtitleError subtitleError, bool isPrintCleaning)
        {
            if (lines.IsNullOrEmpty())
            {
                if (isCheckMode)
                    subtitleError |= SubtitleError.Empty_Line;
                return null;
            }

            if (lines.Count == 1)
            {
                string line = lines[0];
                if (line.StartsWith("- ") == false)
                {
                    int index;
                    if ((index = line.IndexOf(". - ")) != -1 ||
                        (index = line.IndexOf("? - ")) != -1 ||
                        (index = line.IndexOf("! - ")) != -1)
                    {
                        lines.Clear();
                        lines.Add("- " + line.Substring(0, index + 1));
                        lines.Add(line.Substring(index + 2));

                        if (isPrintCleaning)
                            PrintCleaning(line, lines);

                        if (isCheckMode)
                            subtitleError |= SubtitleError.Dialog_Error;
                    }
                }
            }

            return lines;
        }

        #endregion

        #region Multiple Lines Post

        private static List<string> CleanSubtitleMultipleLinesPost(List<string> lines, bool cleanHICaseInsensitive, bool isCheckMode, ref SubtitleError subtitleError, bool isPrintCleaning)
        {
            if (lines.IsNullOrEmpty())
            {
                if (isCheckMode)
                    subtitleError |= SubtitleError.Empty_Line;
                return null;
            }

            lines = CleanOpenBracket(lines, cleanHICaseInsensitive, isCheckMode, ref subtitleError, isPrintCleaning);
            lines = CleanMissingDialogDashSingleLine(lines, cleanHICaseInsensitive, isCheckMode, ref subtitleError, isPrintCleaning);
            lines = CleanRedundantItalicsMultipleLines(lines, cleanHICaseInsensitive, isCheckMode, ref subtitleError, isPrintCleaning);
            lines = CleanMergeLines(lines, cleanHICaseInsensitive, isCheckMode, ref subtitleError, isPrintCleaning);
            lines = CleanMissingDialogDashMultipleLines(lines, cleanHICaseInsensitive, isCheckMode, ref subtitleError, isPrintCleaning);
            lines = CleanLineEndWithApostropheAndQuestionMark(lines, cleanHICaseInsensitive, isCheckMode, ref subtitleError, isPrintCleaning);

            return lines;
        }

        public static List<string> CleanOpenBracket(List<string> lines, bool cleanHICaseInsensitive, bool isCheckMode, ref SubtitleError subtitleError, bool isPrintCleaning)
        {
            if (lines.IsNullOrEmpty())
            {
                if (isCheckMode)
                    subtitleError |= SubtitleError.Empty_Line;
                return null;
            }

            // [need
            // 
            // I need
            if (lines.Count > 0)
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    string line = lines[i];
                    string nextLine = (i + 1 < lines.Count ? lines[i + 1] : null);

                    int index = line.IndexOf("[");
                    if (index != -1 &&
                        line.IndexOf("]", index) == -1 &&
                        (string.IsNullOrEmpty(nextLine) || nextLine.IndexOf("]") == -1) &&
                        index + 1 < line.Length)
                    {
                        string nextChar = line[index + 1].ToString();
                        if (regexCapitalLetter.IsMatch(nextChar))
                        {
                            lines[i] = line.Remove(index, 2).Insert(index, "I " + nextChar.ToLower());

                            if (isPrintCleaning)
                                PrintCleaning(line, lines[i]);

                            if (isCheckMode)
                                subtitleError |= SubtitleError.Malformed_Letters;
                        }
                        else if (regexLowerLetter.IsMatch(nextChar))
                        {
                            lines[i] = line.Remove(index, 1).Insert(index, "I ");

                            if (isPrintCleaning)
                                PrintCleaning(line, lines[i]);

                            if (isCheckMode)
                                subtitleError |= SubtitleError.Malformed_Letters;
                        }
                    }
                }
            }

            return lines;
        }

        public static List<string> CleanMissingDialogDashSingleLine(List<string> lines, bool cleanHICaseInsensitive, bool isCheckMode, ref SubtitleError subtitleError, bool isPrintCleaning)
        {
            if (lines.IsNullOrEmpty())
            {
                if (isCheckMode)
                    subtitleError |= SubtitleError.Empty_Line;
                return null;
            }

            // Line 1 - Dialog
            // 
            // - Line 1 - Dialog
            if (lines.Count > 0)
            {
                bool isMatchDialog = regexDialog.IsMatch(lines[0]);
                bool isContainsDialog = regexContainsDialog.IsMatch(lines[0]);
                if (isMatchDialog == false && isContainsDialog)
                {
                    string lineBefore = (isPrintCleaning ? lines[0] : null);

                    if (lines[0].StartsWith("<i>"))
                        lines[0] = "<i>- " + lines[0].Substring(3);
                    else
                        lines[0] = "- " + lines[0];

                    if (isPrintCleaning)
                        PrintCleaning(lineBefore, lines[0], regexContainsDialog, null);

                    if (isCheckMode)
                        subtitleError |= SubtitleError.Dialog_Error;
                }
            }

            return lines;
        }

        public static List<string> CleanRedundantItalicsMultipleLines(List<string> lines, bool cleanHICaseInsensitive, bool isCheckMode, ref SubtitleError subtitleError, bool isPrintCleaning)
        {
            if (lines.IsNullOrEmpty())
            {
                if (isCheckMode)
                    subtitleError |= SubtitleError.Empty_Line;
                return null;
            }

            // <i>Line 1</i>
            // <i>Line 2</i>
            //
            // <i>Line 1
            // Line 2</i>
            if (lines.Count > 1)
            {
                for (int i = 1; i < lines.Count; i++)
                {
                    string prevLine = lines[i - 1];
                    string line = lines[i];

                    if (prevLine.EndsWith("</i>") && line.StartsWith("<i>"))
                    {
                        lines[i - 1] = prevLine.Substring(0, prevLine.Length - 4);
                        lines[i] = line.Substring("<i>".Length);

                        if (isPrintCleaning)
                            PrintCleaning(new string[] { prevLine, line }, new string[] { lines[i - 1], lines[i] });

                        if (isCheckMode)
                            subtitleError |= SubtitleError.Redundant_Italics;
                    }
                }
            }

            return lines;
        }

        public static readonly Regex regexLine1WithSingleWord = new Regex(@"^\w+,?$", RegexOptions.Compiled);
        public static readonly Regex regexLine2WithSingleWord = new Regex(@"^\w+\.?$", RegexOptions.Compiled);

        public static List<string> CleanMergeLines(List<string> lines, bool cleanHICaseInsensitive, bool isCheckMode, ref SubtitleError subtitleError, bool isPrintCleaning)
        {
            if (lines.IsNullOrEmpty())
            {
                if (isCheckMode)
                    subtitleError |= SubtitleError.Empty_Line;
                return null;
            }

            if (lines.Count > 1)
            {
                bool isDialogFound = false;

                for (int i = 0; i < lines.Count - 1; i++)
                {
                    string line1 = lines[i];
                    string line2 = lines[i + 1];

                    if (line1.Length + line2.Length + 1 <= SINGLE_LINE_MAX_LENGTH)
                    {
                        bool isMergeLines = false;

                        if (regexDialog.IsMatch(line2) == false)
                        {
                            isDialogFound = isDialogFound || regexDialog.IsMatch(line1);

                            // Word,
                            // line
                            //
                            // Word, line
                            isMergeLines = regexLine1WithSingleWord.IsMatch(line1);

                            // Line,
                            // Word
                            //
                            // Line, Word
                            if (isMergeLines == false)
                                isMergeLines = line1.EndsWith(",") && regexLine2WithSingleWord.IsMatch(line2);

                            if (isMergeLines == false)
                            {
                                // - Line 1
                                // line 2
                                // - Line 3
                                // 
                                // - Line 1 line 2
                                // - Line 3
                                // or
                                // - Line
                                // - Line 1
                                // line 2
                                // 
                                // - Line
                                // - Line 1 line 2
                                string line3 = (i + 2 < lines.Count ? lines[i + 2] : null);
                                isMergeLines =
                                    regexDialog.IsMatch(line1) &&
                                    regexLowerLetter.IsMatch(line2.Length > 0 ? line2[0].ToString() : string.Empty) &&
                                    (isDialogFound || (string.IsNullOrEmpty(line3) == false && regexDialog.IsMatch(line3)));
                            }
                        }

                        if (isMergeLines)
                        {
                            lines[i] = line1 + " " + line2;
                            lines.RemoveAt(i + 1);
                            i--;

                            if (isPrintCleaning)
                                PrintCleaning(new string[] { line1, line2 }, lines[i + 1]); // i-- before

                            if (isCheckMode)
                                subtitleError |= SubtitleError.Merge_Lines;
                        }
                    }
                }
            }

            return lines;
        }

        public static List<string> CleanMissingDialogDashMultipleLines(List<string> lines, bool cleanHICaseInsensitive, bool isCheckMode, ref SubtitleError subtitleError, bool isPrintCleaning)
        {
            if (lines.IsNullOrEmpty())
            {
                if (isCheckMode)
                    subtitleError |= SubtitleError.Empty_Line;
                return null;
            }

            // Line 1
            // Line 2 - Text
            //
            // - Line 1
            // Line 2 - Text
            if (lines.Count > 1)
            {
                string line0 = lines[0];
                if ((line0.StartsWith("- ") || line0.StartsWith("<i>- ") || line0.StartsWith("- <i>")) == false)
                {
                    for (int i = 1; i < lines.Count; i++)
                    {
                        string line = lines[i];
                        if (line.Contains(" - "))
                        {
                            List<string> linesBefore = (isPrintCleaning ? new List<string>(lines) : null);

                            if (line0.StartsWith("<i>"))
                                lines[0] = "<i>- " + line0.Substring(3);
                            else
                                lines[0] = "- " + line0;

                            if (isPrintCleaning)
                                PrintCleaning(linesBefore, lines);

                            if (isCheckMode)
                                subtitleError |= SubtitleError.Dialog_Error;

                            break;
                        }
                    }
                }
            }

            return lines;
        }

        public static List<string> CleanLineEndWithApostropheAndQuestionMark(List<string> lines, bool cleanHICaseInsensitive, bool isCheckMode, ref SubtitleError subtitleError, bool isPrintCleaning)
        {
            if (lines.IsNullOrEmpty())
            {
                if (isCheckMode)
                    subtitleError |= SubtitleError.Empty_Line;
                return null;
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
                        lines[i] = line.Replace("'?", "?");

                        if (isPrintCleaning)
                            PrintCleaning(line, lines[i]);

                        if (isCheckMode)
                            subtitleError |= SubtitleError.OCR_Error;
                    }
                }
            }

            return lines;
        }

        #endregion

        #endregion

        #region Clean Empty And Non-Subtitles

        public static List<Subtitle> CleanEmptyAndNonSubtitles(this List<Subtitle> subtitles, bool isPrintCleaning)
        {
            return CleanEmptyAndNonSubtitles(subtitles, false, isPrintCleaning);
        }

        private static List<Subtitle> CleanEmptyAndNonSubtitles(this List<Subtitle> subtitles, bool isCheckMode, bool isPrintCleaning)
        {
            if (subtitles.IsNullOrEmpty())
                return null;

            for (int k = subtitles.Count - 1; k >= 0; k--)
            {
                Subtitle subtitle = subtitles[k];
                SubtitleError subtitleError = SubtitleError.None;

                if (subtitle.Lines.HasAny())
                {
                    for (int i = subtitle.Lines.Count - 1; i >= 0; i--)
                    {
                        string line = subtitle.Lines[i];

                        subtitleError = SubtitleError.None;
                        bool isEmpty = string.IsNullOrEmpty(CleanLine(line, EmptyLine, false, isCheckMode, ref subtitleError, isPrintCleaning));
                        if (isEmpty)
                        {
                            subtitle.Lines.RemoveAt(i);
                            if (isCheckMode)
                                subtitle.SubtitleError |= SubtitleError.Empty_Line;
                            if (subtitle.Lines.Count == 0)
                            {
                                subtitle.Lines = null;
                                break;
                            }
                        }
                        else
                        {
                            subtitleError = SubtitleError.None;
                            string cleanLine = (CleanLine(line, NonSubtitle, false, isCheckMode, ref subtitleError, isPrintCleaning) ?? string.Empty).Trim();
                            bool isChanged = (line != cleanLine);
                            if (isChanged)
                            {
                                subtitle.Lines = null;
                                if (isCheckMode)
                                    subtitle.SubtitleError |= SubtitleError.Non_Subtitle;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if (isCheckMode)
                        subtitle.SubtitleError |= SubtitleError.Empty_Line;
                }

                if (subtitle.Lines.IsNullOrEmpty())
                {
                    if (isCheckMode)
                        subtitle.Lines = null;
                    else
                        subtitles.RemoveAt(k);
                }

                if (subtitles.Count == 0)
                    return null;
            }

            return subtitles;
        }

        #endregion

        #region Check Subtitles

        public static void CheckSubtitle(this Subtitle subtitle, bool cleanHICaseInsensitive, bool isPrintCleaning)
        {
            new List<Subtitle>() { subtitle }.CheckSubtitles(cleanHICaseInsensitive, isPrintCleaning);
        }

        public static void CheckSubtitles(this List<Subtitle> subtitles, bool cleanHICaseInsensitive, bool isPrintCleaning)
        {
            if (subtitles.IsNullOrEmpty())
                return;

            var tempSubtitles = subtitles.Clone();
            foreach (var subtitle in tempSubtitles)
                subtitle.SubtitleError = SubtitleError.None;
            tempSubtitles = tempSubtitles.CleanSubtitles(cleanHICaseInsensitive, true, isPrintCleaning);

            if (subtitles.Count != tempSubtitles.Count)
                throw new Exception("Checking subtitles failed. Number of subtitles is different before and after the cleaning");

            foreach (var item in subtitles.Zip(tempSubtitles, (s, t) => new { Subtitle = s, t.SubtitleError }))
                item.Subtitle.SubtitleError = item.SubtitleError;
        }

        #endregion

        #region Cleaning Regex Rules

        private const string HI_CHARS = @"A-ZÀ-Ý0-9 #\-'.&";
        private const string HI_CHARS_CI = @"A-ZÀ-Ýa-zà-ÿ0-9 #\-'.&";

        #region Empty Line

        public static readonly FindAndReplace[] EmptyLine = new FindAndReplace[] {
            new FindAndReplace(new Regex(@"^[-!?:_@#.*♪♫¶ ]*$", RegexOptions.Compiled), "", SubtitleError.Empty_Line)
            ,new FindAndReplace(new Regex(@"^<i>[-!?:_@#.*♪♫¶ ]*</i>$", RegexOptions.Compiled), "", SubtitleError.Empty_Line)
        };

        #endregion

        #region Non Subtitle

        public static readonly FindAndReplace[] NonSubtitle = new FindAndReplace[] {
            new FindAndReplace(new Regex(@"(?i)AllSubs\.org", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Best watched using", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Captioned by", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Captioning by", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Captioning made possible by", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Captioning performed by", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Captioning sponsored by", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Captions by", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Captions copyright", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Captions made possible by", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Captions performed by", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Captions sponsored by", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Captions, Inc\.", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Closed Caption", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Closed-Caption", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Contain Strong Language", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Contains Strong Language", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Copyright Australian", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Corrected by", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)DVDRIP by", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)ENGLISH - PSDH", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)ENGLISH - SDH", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)ENGLISH - US", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)ENGLISH PSDH", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)ENGLISH SDH", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Eng subs", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Eng subtitles", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)ExplosiveSkull", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)HighCode", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)MKV Player", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)NETFLIX PRESENTS", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)OCR by", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Open Subtitles", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)OpenSubtitles", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Proofread by", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            // not: gripped by
            ,new FindAndReplace(new Regex(@"(?<!g|G)(?i)Rip by", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?<!g|G)(?i)Ripped by", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)SUBTITLES EDITED BY", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)SharePirate\.com", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Subs by", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Subscene", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Subtitled By", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Subtitles by", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Subtitles:", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Subtitletools\.com", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Subtitling", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Sync by", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Synced & corrected", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Synced and corrected", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Synced by", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Synced:", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Synchronization by", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Synchronized by", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)ThePirateBay", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Translated by", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Translation by", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)Translations by", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)

            ,new FindAndReplace(new Regex(@"DIRECTED BY", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"PRODUCED BY", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"WRITTEN BY", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)

            ,new FindAndReplace(new Regex(@"(?i)<font color=", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)^-?\s*<font>.*?</\s*font>$", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
            ,new FindAndReplace(new Regex(@"(?i)^-?\s*<font\s+.*?</\s*font>$", RegexOptions.Compiled), "", SubtitleError.Non_Subtitle)
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
            ,new FindAndReplace(new Regex(@"((?<!\w\.)\w|\s)(?<Dot>\.)[?!:]", RegexOptions.Compiled), "Dot", string.Empty, SubtitleError.Punctuations_Error) // don't clean acronym with periods
            ,new FindAndReplace(new Regex(@"\(\?\)", RegexOptions.Compiled), "?", SubtitleError.Punctuations_Error)
            ,new FindAndReplace(new Regex(@"\(!\)", RegexOptions.Compiled), "!", SubtitleError.Punctuations_Error)
            ,new FindAndReplace(new Regex(@"\s\?", RegexOptions.Compiled), "?", SubtitleError.Punctuations_Error)
            ,new FindAndReplace(new Regex(@"\s!", RegexOptions.Compiled), "!", SubtitleError.Punctuations_Error)
            ,new FindAndReplace(new Regex(@"\s:", RegexOptions.Compiled), ":", SubtitleError.Punctuations_Error)
            ,new FindAndReplace(new Regex(@"[‐=]", RegexOptions.Compiled), "-", SubtitleError.Punctuations_Error)
            ,new FindAndReplace(new Regex(@"[A-ZÀ-Ýa-zà-ÿ](?<Dash>-)\s-\s[A-ZÀ-Ýa-zà-ÿ]", RegexOptions.Compiled), "Dash", "...", SubtitleError.Punctuations_Error)
            ,new FindAndReplace(new Regex(@"[A-ZÀ-Ýa-zà-ÿ](?<Dash>\s-\s-\s?)[A-ZÀ-Ý]", RegexOptions.Compiled), "Dash", "... - ", SubtitleError.Punctuations_Error)
            ,new FindAndReplace(new Regex(@"-\s-", RegexOptions.Compiled), "-", SubtitleError.Punctuations_Error)
            ,new FindAndReplace(new Regex(@"^[—–―‒]", RegexOptions.Compiled), "-", SubtitleError.Punctuations_Error)
            ,new FindAndReplace(new Regex(@"^<i>(?<Dash>[—–―‒])</i>", RegexOptions.Compiled), "Dash", "-", SubtitleError.Punctuations_Error)
            ,new FindAndReplace(new Regex(@"Uh(?<Dash>[—–―‒])huh", RegexOptions.Compiled), "Dash", "-", SubtitleError.Punctuations_Error)
            ,new FindAndReplace(new Regex(@"\w+(?<Dash>[—–―‒])\w+(?:(?<Dash>[-—–―‒])\w+)+", RegexOptions.Compiled), "Dash", "-", SubtitleError.Punctuations_Error)
            ,new FindAndReplace(new Regex(@"(?:(?<Dash>[-—–―‒])\w+)+\w+(?<Dash>[—–―‒])\w+", RegexOptions.Compiled), "Dash", "-", SubtitleError.Punctuations_Error)
            ,new FindAndReplace("Three Dots", new Regex(@"[…—–―‒]", RegexOptions.Compiled), "...", SubtitleError.Punctuations_Error)
            ,new FindAndReplace("Three Dots", new Regex(@"\.\s\.\.", RegexOptions.Compiled), "...", SubtitleError.Punctuations_Error)
            ,new FindAndReplace("Three Dots", new Regex(@"\.\.\s\.", RegexOptions.Compiled), "...", SubtitleError.Punctuations_Error)
            ,new FindAndReplace("Three Dots", new Regex(@"\.\s\.\s\.", RegexOptions.Compiled), "...", SubtitleError.Punctuations_Error)
            ,new FindAndReplace("Three Dots", new Regex(@"\.[-_]\.", RegexOptions.Compiled), "...", SubtitleError.Punctuations_Error)
            ,new FindAndReplace("Three Dots", new Regex(@"_\.", RegexOptions.Compiled), "...", SubtitleError.Punctuations_Error)
            ,new FindAndReplace("Three Dots", new Regex(@"-{2,}", RegexOptions.Compiled), "...", SubtitleError.Punctuations_Error)
            ,new FindAndReplace("Three Dots", new Regex(@",{2,}", RegexOptions.Compiled), "...", SubtitleError.Punctuations_Error)
            ,new FindAndReplace("Three Dots", new Regex(@"\.{4,}", RegexOptions.Compiled), "...", SubtitleError.Punctuations_Error)
            // - _oh__ => - Oh...
            ,new FindAndReplace("Three Dots", new Regex(@"^(?<Dash>-)?\s*_(?<Subtitle1>[A-ZÀ-Ýa-zà-ÿ])(?<Subtitle2>[A-ZÀ-Ýa-zà-ÿ]*)_{1,}""?", RegexOptions.Compiled), m => {
                Group Dash = m.Groups["Dash"];
                if (Dash.Success)
                    return string.Format("{0} {1}{2}...", Dash.Value, m.Groups["Subtitle1"].Value.ToUpper(), m.Groups["Subtitle2"].Value);
                else
                    return string.Format("{0}{1}...", m.Groups["Subtitle1"].Value.ToUpper(), m.Groups["Subtitle2"].Value);
            }, SubtitleError.Punctuations_Error)
            ,new FindAndReplace("Three Dots", new Regex(@"(_{1,}""|""?_{1,})$", RegexOptions.Compiled), "...", SubtitleError.Punctuations_Error)
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
            ,new FindAndReplace(new Regex(@"[a-zà-ÿ](?<OCR>J+)$", RegexOptions.Compiled), "OCR", "♪", SubtitleError.Notes_Error)

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
            ,new FindAndReplace(new Regex(@"^(?<Prefix>[""'])\s+(?<Suffix>[A-ZÀ-Ýa-zà-ÿ0-9])", RegexOptions.Compiled), "${Prefix}${Suffix}", SubtitleError.Redundant_Spaces)
            // Text " => Text"
            ,new FindAndReplace(new Regex(@"(?<Prefix>[A-ZÀ-Ýa-zà-ÿ0-9.?!-])\s+(?<Suffix>[""'])$", RegexOptions.Compiled), "${Prefix}${Suffix}", SubtitleError.Redundant_Spaces)

            // 1 987 => 1987 (Positive lookahead)
            // 1 1 /2 => 1 1/2 (Negative lookahead)
            ,new FindAndReplace("Space After 1", new Regex(@"1\s+(?=[0-9.,/])(?!1/2|1 /2)", RegexOptions.Compiled), "1", SubtitleError.Redundant_Spaces)

            // 9: 55 => 9:55
			,new FindAndReplace(new Regex(@"[0-2]?\d(?<OCR>: )[0-5]\d", RegexOptions.Compiled), "OCR", ":", SubtitleError.Redundant_Spaces)

            // 1 : => 1:
			,new FindAndReplace(new Regex(@"\d(?<OCR> :)", RegexOptions.Compiled), "OCR", ":", SubtitleError.Redundant_Spaces)

            // Spaces after aphostrophes
			,new FindAndReplace(new Regex(@"(?i)[A-ZÀ-Ýa-zà-ÿ](?<OCR>'\s|\s')(?:ll|ve|s|m|d|t|re)\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.Redundant_Spaces)

            // ma' am => ma'am
            ,new FindAndReplace(new Regex(@"(?i)ma(?<OCR>'\s|\s')am\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.Redundant_Spaces)

            // a. m. => a.m.
            ,new FindAndReplace(new Regex(@"(?i)(?:a|p)\.(?<OCR>\s)m\.", RegexOptions.Compiled), "OCR", "", SubtitleError.Redundant_Spaces)

            // Remove space after two or more consecutive dots
            ,new FindAndReplace(new Regex(@"^(?:<i>)?\.{2,}(?<OCR>\s+)", RegexOptions.Compiled), "OCR", "", SubtitleError.Redundant_Spaces)

            // x ...
            ,new FindAndReplace(new Regex(@"(?<OCR>\s+)\.{3}(?:</i>)?$", RegexOptions.Compiled), "OCR", "", SubtitleError.Redundant_Spaces)
            // a ... a
            ,new FindAndReplace(new Regex(@"[A-ZÀ-Ýa-zà-ÿ](?<OCR>\s+)\.{3}\s+[A-ZÀ-Ýa-zà-ÿ]", RegexOptions.Compiled), "OCR", "", SubtitleError.Redundant_Spaces)

            // 1, 000
            ,new FindAndReplace(new Regex(@"\b\d+(?<OCR>, | ,)0{3}\b", RegexOptions.Compiled), "OCR", ",", SubtitleError.Redundant_Spaces)
            
            // Text . Next text => Text. Next text
            ,new FindAndReplace(new Regex(@"[A-ZÀ-Ýa-zà-ÿ](?<OCR>\s)[.,!?](?:\s[A-ZÀ-Ýa-zà-ÿ]|$)", RegexOptions.Compiled), "OCR", "", SubtitleError.Redundant_Spaces)

            // Ordinal Numbers: 1st, 2nd, 3rd, 4th
            ,new FindAndReplace("Ordinal Numbers", new Regex(@"\b\d*1(?<OCR>\s+)st\b", RegexOptions.Compiled), "OCR", "", SubtitleError.Redundant_Spaces)
            ,new FindAndReplace("Ordinal Numbers", new Regex(@"\b\d*2(?<OCR>\s+)nd\b", RegexOptions.Compiled), "OCR", "", SubtitleError.Redundant_Spaces)
            ,new FindAndReplace("Ordinal Numbers", new Regex(@"\b\d*3(?<OCR>\s+)rd\b", RegexOptions.Compiled), "OCR", "", SubtitleError.Redundant_Spaces)
            ,new FindAndReplace("Ordinal Numbers", new Regex(@"\b\d*[4-9](?<OCR>\s+)th\b", RegexOptions.Compiled), "OCR", "", SubtitleError.Redundant_Spaces)

            // $ 1 => $1
			,new FindAndReplace(new Regex(@"(?<OCR>\$\s)\d", RegexOptions.Compiled), "OCR", "$", SubtitleError.Redundant_Spaces)

            // 1 % => 1%
			,new FindAndReplace(new Regex(@"\d(?<OCR>\s%)", RegexOptions.Compiled), "OCR", "%", SubtitleError.Redundant_Spaces)

            // H i => Hi
			,new FindAndReplace(new Regex(@"\b(?i:H)(?<OCR>\s)i\b", RegexOptions.Compiled), "OCR", "", SubtitleError.Redundant_Spaces)
        };

        #endregion

        #region Hearing Impaired

        public static readonly FindAndReplace[] HearingImpaired = new FindAndReplace[] {
            // ^<i>(laughting)</i> Text => Text
            new FindAndReplace(new Regex(@"^<i>\s*\(.*?\)\s*</i>\s*", RegexOptions.Compiled), "", SubtitleError.Hearing_Impaired)
            ,new FindAndReplace(new Regex(@"^<i>\s*\[.*?\]\s*</i>\s*", RegexOptions.Compiled), "", SubtitleError.Hearing_Impaired)

            // Text <i>(laughting)</i>$ => Text
            ,new FindAndReplace(new Regex(@"\s*<i>\s*\(.*?\)\s*</i>$", RegexOptions.Compiled), "", SubtitleError.Hearing_Impaired)
            ,new FindAndReplace(new Regex(@"\s*<i>\s*\[.*?\]\s*</i>$", RegexOptions.Compiled), "", SubtitleError.Hearing_Impaired)

            // <i>- (laughting)</i> =>
            ,new FindAndReplace(new Regex(@"<i>-\s*\(.*?\)</i>", RegexOptions.Compiled), "", SubtitleError.Hearing_Impaired)
            ,new FindAndReplace(new Regex(@"<i>-\s*\[.*?\]</i>", RegexOptions.Compiled), "", SubtitleError.Hearing_Impaired)

            // <i>- (laughting) => <i>
            ,new FindAndReplace(new Regex(@"^<i>-\s*\(.*?\)$", RegexOptions.Compiled), "<i>", SubtitleError.Hearing_Impaired)
            ,new FindAndReplace(new Regex(@"^<i>-\s*\[.*?\]$", RegexOptions.Compiled), "<i>", SubtitleError.Hearing_Impaired)

            // - (laughting)</i> => </i>
            ,new FindAndReplace(new Regex(@"^-\s*\(.*?\)</i>$", RegexOptions.Compiled), "</i>", SubtitleError.Hearing_Impaired)
            ,new FindAndReplace(new Regex(@"^-\s*\[.*?\]</i>$", RegexOptions.Compiled), "</i>", SubtitleError.Hearing_Impaired)

            // - (MAN): Text => - Text
            ,new FindAndReplace(new Regex(@"^(?<Prefix>- )?\(.*?\):?\s*(?<Subtitle>.+)$", RegexOptions.Compiled), "${Prefix}${Subtitle}", SubtitleError.Hearing_Impaired)
            ,new FindAndReplace(new Regex(@"^(?<Prefix>- )?\[.*?\]:?\s*(?<Subtitle>.+)$", RegexOptions.Compiled), "${Prefix}${Subtitle}", SubtitleError.Hearing_Impaired)

            // <i>- MAN (laughting): Text</i> => <i>- Text</i>
            ,new FindAndReplace(new Regex(@"^(?<Prefix><i>\s*-?\s*)[A-ZÀ-Ý]*\s*\(.*?\):?\s*(?<Subtitle>.+?)(?<Suffix></i>)$", RegexOptions.Compiled), "${Prefix}${Subtitle}${Suffix}", SubtitleError.Hearing_Impaired)
            ,new FindAndReplace(new Regex(@"^(?<Prefix><i>\s*-?\s*)[A-ZÀ-Ý]*\s*\[.*?\]:?\s*(?<Subtitle>.+?)(?<Suffix></i>)$", RegexOptions.Compiled), "${Prefix}${Subtitle}${Suffix}", SubtitleError.Hearing_Impaired)

            // <i>MAN (laughting): => <i>
            ,new FindAndReplace(new Regex(@"^(?<Prefix><i>)[A-ZÀ-Ý]*\s*\(.*?\):$", RegexOptions.Compiled), "${Prefix}", SubtitleError.Hearing_Impaired)
            ,new FindAndReplace(new Regex(@"^(?<Prefix><i>)[A-ZÀ-Ý]*\s*\[.*?\]:$", RegexOptions.Compiled), "${Prefix}", SubtitleError.Hearing_Impaired)

            // <i>(laughting) Text => <i>Text
            ,new FindAndReplace(new Regex(@"^(?<Prefix><i>)\s*\(.*?\)\s*(?<Subtitle>.+)$", RegexOptions.Compiled), "${Prefix}${Subtitle}", SubtitleError.Hearing_Impaired)
            ,new FindAndReplace(new Regex(@"^(?<Prefix><i>)\s*\[.*?\]\s*(?<Subtitle>.+)$", RegexOptions.Compiled), "${Prefix}${Subtitle}", SubtitleError.Hearing_Impaired)

            // Text (laughting) => Text
            ,new FindAndReplace(new Regex(@"^(?<Subtitle>.+?)\s*\(.*?\)$", RegexOptions.Compiled), "${Subtitle}", SubtitleError.Hearing_Impaired)
            ,new FindAndReplace(new Regex(@"^(?<Subtitle>.+?)\s*\[.*?\]$", RegexOptions.Compiled), "${Subtitle}", SubtitleError.Hearing_Impaired)

            // MAN #1: Text => Text
            ,new FindAndReplace("Inline HI Without Dialog",
                                new Regex(@"^[A-ZÀ-Ý0-9 #\'\[\]][A-ZÀ-Ý0-9 #\-'\[\]]*[A-ZÀ-Ý#'\[\]][A-ZÀ-Ý0-9 #\-'\[\]]*:\s*(?<Subtitle>.+?)$", RegexOptions.Compiled), "${Subtitle}", SubtitleError.Hearing_Impaired)
                    .SetRegexCI(new Regex(@"^[A-ZÀ-Ýa-zà-ÿ0-9 #\'\[\]][A-ZÀ-Ýa-zà-ÿ0-9 #\-'\[\]]*[A-ZÀ-Ýa-zà-ÿ#'\[\]][A-ZÀ-Ýa-zà-ÿ0-9 #\-'\[\]]*:(?!\d\d)\s*(?<Subtitle>.+?)$", RegexOptions.Compiled))

            // Some (laughting) text => Some text
            ,new FindAndReplace(new Regex(@"\s+\(.*?\)\s+", RegexOptions.Compiled), " ", SubtitleError.Hearing_Impaired)
            ,new FindAndReplace(new Regex(@"\s+\[.*?\]\s+", RegexOptions.Compiled), " ", SubtitleError.Hearing_Impaired)

            // Text (laughting)</i> => Text</i>
            ,new FindAndReplace(new Regex(@"\s+\(.*?\)</i>", RegexOptions.Compiled), "</i>", SubtitleError.Hearing_Impaired)
            ,new FindAndReplace(new Regex(@"\s+\[.*?\]</i>", RegexOptions.Compiled), "</i>", SubtitleError.Hearing_Impaired)

            // Some <i>(laughting)</i> Text => Some Text
            ,new FindAndReplace(new Regex(@"\s*<i>\s*\(.*?\)\s*</i>\s*", RegexOptions.Compiled), " ", SubtitleError.Hearing_Impaired)
            ,new FindAndReplace(new Regex(@"\s*<i>\s*\[.*?\]\s*</i>\s*", RegexOptions.Compiled), " ", SubtitleError.Hearing_Impaired)

            // (?!\d\d) prevents cleaning time, like 13:00, in CI mode

            // MAN (laughting): Text => Text
            ,new FindAndReplace("Inline HI Without Dialog",
                                new Regex(@"^[" + HI_CHARS.Replace("-'", "'") + @"]+\(.*?\):\s*", RegexOptions.Compiled), "", SubtitleError.Hearing_Impaired)
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
            ,new FindAndReplace(new Regex(@"^(?<Prefix>(?:<i>)?-?\s*|-?\s*(?:<i>)?\s*)[" + HI_CHARS + @"]*[A-ZÀ-Ý]+[" + HI_CHARS + @"]*:\s*(?<Subtitle>.*?)$", RegexOptions.Compiled), "${Prefix}${Subtitle}", SubtitleError.Hearing_Impaired)
                    .SetRegexCI(new Regex(@"^(?<Prefix>(?:<i>)?-?\s*|-?\s*(?:<i>)?\s*)[" + HI_CHARS_CI + @"]*[A-ZÀ-Ý]+[" + HI_CHARS_CI + @"]*:(?!\d\d)\s*(?<Subtitle>.*?)$", RegexOptions.Compiled))

            // <i>- : Text => Text
            ,new FindAndReplace(new Regex(@"^(?:\s*<i>)?\s*-\s*:\s*", RegexOptions.Compiled), "", SubtitleError.Hearing_Impaired)

            // <i>: Text => Text
            ,new FindAndReplace(new Regex(@"^(?:\s*<i>)?\s*:\s*", RegexOptions.Compiled), "", SubtitleError.Hearing_Impaired)

            // {Text) {Text] {Text}
            ,new FindAndReplace(new Regex(@"\{[^\{\[\(\)\]\}]+[\)\]\}]", RegexOptions.Compiled), "", SubtitleError.Hearing_Impaired)
            // {Text} [Text} (Text}
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

            // \s-Text => \s- Text
            ,new FindAndReplace("Space After Dialog Dash", new Regex(@"(?<Prefix>\s+)(?<Dash>-)(?<Suffix>[A-ZÀ-Ýa-zà-ÿ])", RegexOptions.Compiled), "${Prefix}${Dash} ${Suffix}", SubtitleError.Missing_Spaces)

			// Gun Calibre
            // Derringer.22 => Derringer .22
            ,new FindAndReplace(new Regex(@"[A-ZÀ-Ýa-zà-ÿ](?<OCR>\.)\d+\b", RegexOptions.Compiled), "OCR", " .", SubtitleError.Missing_Spaces)

            // fix acronym with periods (replace match to upper case)
            // this is an OCR_Error but it needs to be executed before the next one - Add space after a single dot
            // c.E.O. => C.E.O.
            ,new FindAndReplace("Space After Dot", new Regex(@"(\b[a-zà-ÿ])\.([A-ZÀ-Ý]\.)+", RegexOptions.Compiled), m => m.ToString().ToUpper(), SubtitleError.OCR_Error)
            // A.k.a. => A.K.A.
            ,new FindAndReplace("Space After Dot", new Regex(@"(\b[A-ZÀ-Ý])\.([a-zà-ÿ]\.)+", RegexOptions.Compiled), m => m.ToString().ToUpper(), SubtitleError.OCR_Error)

            // Add space after a single dot
            ,new FindAndReplace("Space After Dot", new Regex(@"[a-zà-ÿ](?<OCR>\.)[^(\s\n\'\.\?\!<"")\,]", RegexOptions.Compiled), "OCR", ". ", SubtitleError.Missing_Spaces,
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
            ,new FindAndReplace("Space After Comma", new Regex(@"(?<OCR>\,)[A-ZÀ-Ýa-zà-ÿ]", RegexOptions.Compiled), "OCR", ", ", SubtitleError.Missing_Spaces)

            // Add space after exclamation mark, question mark
            ,new FindAndReplace(new Regex(@"(?<Prefix>[!?])(?<Suffix>[A-ZÀ-Ýa-zà-ÿ])", RegexOptions.Compiled), "${Prefix} ${Suffix}", SubtitleError.Missing_Spaces)

            // Text...Text => Text... Text
            ,new FindAndReplace("Space After Three Dot", new Regex(@"[A-ZÀ-Ýa-zà-ÿ0-9](?:(?<OCR>\.{2,})[A-ZÀ-Ýa-zà-ÿ0-9'])+", RegexOptions.Compiled), "OCR", "... ", SubtitleError.Missing_Spaces)

            // "Quotation"Text => "Quotation" Text
            ,new FindAndReplace(new Regex(@"^.*?"".*?(?<OCR>"")[A-ZÀ-Ýa-zà-ÿ0-9][^""]*$", RegexOptions.Compiled), "OCR", @""" ", SubtitleError.Missing_Spaces)
            // Text"Quotation" => Text "Quotation"
            ,new FindAndReplace(new Regex(@"^[^""]*[A-ZÀ-Ýa-zà-ÿ0-9](?<OCR>"").*?"".*?$", RegexOptions.Compiled), "OCR", @" """, SubtitleError.Missing_Spaces)
        };

        #endregion

        #region Trim Spaces

        public static readonly FindAndReplace[] TrimSpaces = new FindAndReplace[] {
            new FindAndReplace(new Regex(@"^\s+$", RegexOptions.Compiled), "", SubtitleError.Redundant_Spaces)
            ,new FindAndReplace(new Regex(@"^\s+", RegexOptions.Compiled), "", SubtitleError.Redundant_Spaces)
            ,new FindAndReplace(new Regex(@"\s+$", RegexOptions.Compiled), "", SubtitleError.Redundant_Spaces)
            ,new FindAndReplace(new Regex(@"\s{2,}", RegexOptions.Compiled), " ", SubtitleError.Redundant_Spaces)
        };

        #endregion

        #region Non-Ansi Chars

        public static readonly FindAndReplace[] NonAnsiChars = new FindAndReplace[] {
            new FindAndReplace(new Regex(@" ", RegexOptions.Compiled), " ", SubtitleError.Non_Ansi_Chars)
            ,new FindAndReplace(new Regex(@"ﬁ", RegexOptions.Compiled), "fi", SubtitleError.Non_Ansi_Chars)
            ,new FindAndReplace(new Regex(@"§", RegexOptions.Compiled), "sl", SubtitleError.Non_Ansi_Chars)
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
            ,new FindAndReplace(new Regex(@"κ", RegexOptions.Compiled), "k", SubtitleError.Non_Ansi_Chars) //  954 => k
            ,new FindAndReplace(new Regex(@"ν", RegexOptions.Compiled), "v", SubtitleError.Non_Ansi_Chars) //  957 => v
            ,new FindAndReplace(new Regex(@"ο", RegexOptions.Compiled), "o", SubtitleError.Non_Ansi_Chars) //  959 => o
            ,new FindAndReplace(new Regex(@"ρ", RegexOptions.Compiled), "p", SubtitleError.Non_Ansi_Chars) //  961 => p
            ,new FindAndReplace(new Regex(@"ω", RegexOptions.Compiled), "w", SubtitleError.Non_Ansi_Chars) //  969 => w
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
            ,new FindAndReplace(new Regex(@"Ь", RegexOptions.Compiled), "b", SubtitleError.Non_Ansi_Chars) // 1068 => b
            ,new FindAndReplace(new Regex(@"а", RegexOptions.Compiled), "a", SubtitleError.Non_Ansi_Chars) // 1072 => a
            ,new FindAndReplace(new Regex(@"в", RegexOptions.Compiled), "B", SubtitleError.Non_Ansi_Chars) // 1074 => B
            ,new FindAndReplace(new Regex(@"е", RegexOptions.Compiled), "e", SubtitleError.Non_Ansi_Chars) // 1077 => e
            ,new FindAndReplace(new Regex(@"к", RegexOptions.Compiled), "k", SubtitleError.Non_Ansi_Chars) // 1082 => k
            ,new FindAndReplace(new Regex(@"м", RegexOptions.Compiled), "M", SubtitleError.Non_Ansi_Chars) // 1084 => M
            ,new FindAndReplace(new Regex(@"н", RegexOptions.Compiled), "H", SubtitleError.Non_Ansi_Chars) // 1085 => H
            ,new FindAndReplace(new Regex(@"о", RegexOptions.Compiled), "o", SubtitleError.Non_Ansi_Chars) // 1086 => o
            ,new FindAndReplace(new Regex(@"р", RegexOptions.Compiled), "p", SubtitleError.Non_Ansi_Chars) // 1088 => p
            ,new FindAndReplace(new Regex(@"с", RegexOptions.Compiled), "c", SubtitleError.Non_Ansi_Chars) // 1089 => c
            ,new FindAndReplace(new Regex(@"т", RegexOptions.Compiled), "T", SubtitleError.Non_Ansi_Chars) // 1090 => T
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
            ,new FindAndReplace(new Regex(@"\b(?<OCR>ll/l)", RegexOptions.Compiled), "OCR", "M", SubtitleError.Malformed_Letters)
            ,new FindAndReplace(new Regex(@"\b(?<OCR>L/V)", RegexOptions.Compiled), "OCR", "W", SubtitleError.Malformed_Letters)
            ,new FindAndReplace(new Regex(@"(?<OCR>\(\))(kay|K)", RegexOptions.Compiled), "OCR", "O", SubtitleError.Malformed_Letters)
            // aften/vards, othen/vise, papen/vork => afterwards, otherwise, paperwork
            ,new FindAndReplace(new Regex(@"[A-ZÀ-Ýa-zà-ÿ](?<OCR>n/v)", RegexOptions.Compiled), "OCR", "rw", SubtitleError.Malformed_Letters)
            // fonnard => forward
            ,new FindAndReplace(new Regex(@"(?i:f)o(?<OCR>nn)ard", RegexOptions.Compiled), "OCR", "rw", SubtitleError.Malformed_Letters)
            // I'/I, I '/I
            ,new FindAndReplace(new Regex(@"[A-ZÀ-Ýa-zà-ÿ](?<OCR>\s?'/(i|I))", RegexOptions.Compiled), "OCR", "'ll", SubtitleError.Malformed_Letters)
            // He/io, Mil/ion, Rea/iy, Wi/I, Sha/I
            ,new FindAndReplace(new Regex(@"[A-ZÀ-Ýa-zà-ÿ](?<OCR>/(i|I))", RegexOptions.Compiled), "OCR", "ll", SubtitleError.Malformed_Letters)
            // I'i/, We 'i/, They'i|
            ,new FindAndReplace(new Regex(@"[A-ZÀ-Ýa-zà-ÿ](?<OCR>\s?'(i|I)(/|\|))", RegexOptions.Compiled), "OCR", "'ll", SubtitleError.Malformed_Letters)
            // prob/em => problem (lem)
            // coke/y => cokely (ly)
            // gal/o => gallo (lo/llo)
            // hi/dy =< hildy (ldy)
            ,new FindAndReplace(new Regex(@"[A-ZÀ-Ýa-zà-ÿ](?<OCR>/)(em|y|o|lo|dy)", RegexOptions.Compiled), "OCR", "l", SubtitleError.Malformed_Letters)
            // /t => It
            ,new FindAndReplace(new Regex(@"(?:^|\s|\b)(?<OCR>/t)", RegexOptions.Compiled), "OCR", "It", SubtitleError.Malformed_Letters)
            // |'m => I'm
            ,new FindAndReplace(new Regex(@"(?:^|\s|\b)(?<OCR>\|)'[A-ZÀ-Ýa-zà-ÿ]", RegexOptions.Compiled), "OCR", "I", SubtitleError.Malformed_Letters)
            // morn => mom
            ,new FindAndReplace(new Regex(@"\b(?i:m)o(?<OCR>rn)\b", RegexOptions.Compiled), "OCR", "m", SubtitleError.Malformed_Letters)
            // Theyte => They're
            ,new FindAndReplace(new Regex(@"\b(?i:t)hey(?<OCR>t)e\b", RegexOptions.Compiled), "OCR", "'r", SubtitleError.Malformed_Letters)
            // Wete => We're
            ,new FindAndReplace(new Regex(@"\b(?i:w)e(?<OCR>t)e\b", RegexOptions.Compiled), "OCR", "'r", SubtitleError.Malformed_Letters)
            // Youte => You're
            ,new FindAndReplace(new Regex(@"\b(?i:y)ou(?<OCR>t)e\b", RegexOptions.Compiled), "OCR", "'r", SubtitleError.Malformed_Letters)

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
            ,new FindAndReplace(new Regex(@"[a-zà-ÿ](?<OCR>""|''|'’| ""|"" )s\b", RegexOptions.Compiled), "OCR", "'", SubtitleError.Contractions_Error)
            // in ' => in' (sayin')
            ,new FindAndReplace(new Regex(@"\win(?<OCR>\s)'", RegexOptions.Compiled), "OCR", "", SubtitleError.Contractions_Error)
        };

        #endregion

        #region Accent Letters

        public static readonly FindAndReplace[] AccentLetters = new FindAndReplace[] {
            new FindAndReplace(new Regex(@"\b(?i:F)ianc(?<OCR>'e|Ã©)e?", RegexOptions.Compiled), "OCR", "é", SubtitleError.Accent_Letters)
            ,new FindAndReplace(new Regex(@"\b(?i:C)af(?<OCR>'e|Ã©)", RegexOptions.Compiled), "OCR", "é", SubtitleError.Accent_Letters)
            ,new FindAndReplace(new Regex(@"\b(?i:C)ach(?<OCR>'e|Ã©)", RegexOptions.Compiled), "OCR", "é", SubtitleError.Accent_Letters)
            ,new FindAndReplace(new Regex(@"\b(?i:M)(?<OCR>'e|Ã©)xico", RegexOptions.Compiled), "OCR", "é", SubtitleError.Accent_Letters)
            ,new FindAndReplace(new Regex(@"\b(?i:R)(?<OCR>'e|Ã©)sum(?<OCR>'e|Ã©)", RegexOptions.Compiled), "OCR", "é", SubtitleError.Accent_Letters)
        };

        #endregion

        #region I and L

        public static readonly FindAndReplace[] I_And_L = new FindAndReplace[] {
            // Roman numerals
            new FindAndReplace(new Regex(@"\b[VXLCDM]*(?<OCR>lll)\b", RegexOptions.Compiled), "OCR", "III", SubtitleError.I_And_L_Error)
            ,new FindAndReplace(new Regex(@"[^.?!—–―‒-][^']\b[IVXLCDM]*(?<OCR>ll)I{0,1}\b", RegexOptions.Compiled), "OCR", "II", SubtitleError.I_And_L_Error)
            ,new FindAndReplace(new Regex(@"^(?<OCR>ll)\b", RegexOptions.Compiled), "OCR", "II", SubtitleError.I_And_L_Error)

            ,new FindAndReplace(new Regex(@"\b[IVXLCDM]*(?<OCR>l)[IVX]*\b", RegexOptions.Compiled), "OCR", "I", SubtitleError.I_And_L_Error,
                new FindAndReplace.IgnoreRule() { ReadPrevCharsFromMatch = 1, ReadNextCharsFromMatch = 1, IgnoreIfStartsWith = @"-l", IgnoreIfEndsWith = @"l-" }
                , new FindAndReplace.IgnoreRule() { ReadNextCharsFromMatch = 8, IgnoreIfEqualsTo = "Il y avait" }
                , new FindAndReplace.IgnoreRule() { ReadNextCharsFromMatch = 4, IgnoreIfEqualsTo = "Il est" }
                , new FindAndReplace.IgnoreRule() { ReadNextCharsFromMatch = 5, IgnoreIfEqualsTo = "Il faut" }
                , new FindAndReplace.IgnoreRule() { ReadNextCharsFromMatch = 5, IgnoreIfEqualsTo = "Il y a " }
                , new FindAndReplace.IgnoreRule() { ReadNextCharsFromMatch = 6, IgnoreIfEqualsTo = "l'chaim" }
            )

            // Lowercase word at the end: II => ll
			,new FindAndReplace(new Regex(@"[a-zà-ÿ](?<OCR>II)", RegexOptions.Compiled), "OCR", "ll", SubtitleError.I_And_L_Error)
            // Lowercase word at the beginning: II => ll
			,new FindAndReplace(new Regex(@"(?<OCR>II)[a-zà-ÿ]", RegexOptions.Compiled), "OCR", "ll", SubtitleError.I_And_L_Error)
            // Lowercase word at the middle: I => l
			,new FindAndReplace(new Regex(@"[a-zà-ÿ](?<OCR>I)[a-zà-ÿ]", RegexOptions.Compiled), "OCR", "l", SubtitleError.I_And_L_Error,
                new FindAndReplace.IgnoreRule() { ReadPrevCharsFromMatch = 1, IgnoreIfStartsWith = "McI" } // ignore names McIntyre
            )
            // Lowercase word at the end: I => l
			,new FindAndReplace(new Regex(@"[a-zà-ÿ](?<OCR>I)\b", RegexOptions.Compiled), "OCR", "l", SubtitleError.I_And_L_Error)
            // Uppercase word at the middle: l => I
			,new FindAndReplace(new Regex(@"[A-ZÀ-Ý](?<OCR>l)[A-ZÀ-Ý]", RegexOptions.Compiled), "OCR", "I", SubtitleError.I_And_L_Error)
            // Uppercase word at the end: l => I
			,new FindAndReplace(new Regex(@"[A-ZÀ-Ý]{2,}(?<OCR>l)", RegexOptions.Compiled), "OCR", "I", SubtitleError.I_And_L_Error)

			// Replace single l with I. not preceding l- and following -l
            ,new FindAndReplace(new Regex(@"\b(?<OCR>l)\b", RegexOptions.Compiled), "OCR", "I", SubtitleError.I_And_L_Error,
                new FindAndReplace.IgnoreRule() { ReadPrevCharsFromMatch = 1, ReadNextCharsFromMatch = 1, IgnoreIfStartsWith = @"-l", IgnoreIfEndsWith = @"l-" }
                , new FindAndReplace.IgnoreRule() { ReadNextCharsFromMatch = 6, IgnoreIfEqualsTo = "l'chaim" }
            )
            
            // i-i-i => I-I-I (but not i-i-it or i-i-is)
            ,new FindAndReplace(new Regex(@"\b(?<OCR>i)(?:-(?<OCR>i))+\b", RegexOptions.Compiled), "OCR", "I", SubtitleError.I_And_L_Error,
                new FindAndReplace.IgnoreRule() { ReadNextCharsFromMatch = 3, IgnoreIfMatchsWithRegex = @"-i\B" }
            )
            
			// Replace single i with I, but not <i> or </i> and not -i or i-
            ,new FindAndReplace(new Regex(@"\b(?<OCR>i)\b", RegexOptions.Compiled), "OCR", "I", SubtitleError.I_And_L_Error,
                new FindAndReplace.IgnoreRule() { ReadPrevCharsFromMatch = 1, ReadNextCharsFromMatch = 1, IgnoreIfStartsWith = @"<i", IgnoreIfEndsWith = @"i>" }
                , new FindAndReplace.IgnoreRule() { ReadPrevCharsFromMatch = 2, ReadNextCharsFromMatch = 1, IgnoreIfStartsWith = @"</i", IgnoreIfEndsWith = @"i>" }
                , new FindAndReplace.IgnoreRule() { ReadPrevCharsFromMatch = 1, ReadNextCharsFromMatch = 1, IgnoreIfStartsWith = @"-i", IgnoreIfEndsWith = @"i-" }
            )

            // I'II => I'll
			,new FindAndReplace(new Regex(@"[A-ZÀ-Ýa-zà-ÿ]'(?<OCR>II)\b", RegexOptions.Compiled), "OCR", "ll", SubtitleError.I_And_L_Error)

            // I 'II/I' II => I'll
			,new FindAndReplace(new Regex(@"[A-ZÀ-Ýa-zà-ÿ](?<OCR>'\sII|\s'II)\b", RegexOptions.Compiled), "OCR", "'ll", SubtitleError.I_And_L_Error)

            // AII => All
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

            // I at the beginning of a word before lowercase vowels is most likely an l
            ,new FindAndReplace(new Regex(@"\b(?<OCR>I)[aeiouyà-ÿ]", RegexOptions.Compiled), "OCR", "l", SubtitleError.I_And_L_Error,
                new FindAndReplace.IgnoreRule() { ReadNextCharsFromMatch = 1, IgnoreIfEqualsTo = "Ian" }
                , new FindAndReplace.IgnoreRule() { ReadNextCharsFromMatch = 1, IgnoreIfEqualsTo = "Ion" }
                , new FindAndReplace.IgnoreRule() { ReadNextCharsFromMatch = 2, IgnoreIfEqualsTo = "Iago" }
                , new FindAndReplace.IgnoreRule() { ReadNextCharsFromMatch = 4, IgnoreIfEqualsTo = "Ionian" }
            )

            // I after an uppercase letter at the beginning and before a lowercase letter is most likely an l
            ,new FindAndReplace(new Regex(@"\b[A-ZÀ-Ý](?<OCR>I)[a-zà-ÿ]", RegexOptions.Compiled), "OCR", "l", SubtitleError.I_And_L_Error,
                new FindAndReplace.IgnoreRule() { IgnoreIfCaseInsensitiveEqualsTo = "GIs" }
            )

            // l at the beginning before a consonant different from l is most likely an I
            ,new FindAndReplace(new Regex(@"\b(?<OCR>l)[^aeiouyà-ÿl]", RegexOptions.Compiled), "OCR", "I", SubtitleError.I_And_L_Error,
                new FindAndReplace.IgnoreRule() { ReadPrevCharsFromMatch = 1, ReadNextCharsFromMatch = 1, IgnoreIfStartsWith = @"-l", IgnoreIfEndsWith = @"l-" }
                , new FindAndReplace.IgnoreRule() { ReadNextCharsFromMatch = 1, IgnoreIfEqualsTo = "lbs" }
                , new FindAndReplace.IgnoreRule() { ReadNextCharsFromMatch = 5, IgnoreIfEqualsTo = "l'chaim" }
            )

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

            // Single L. ignore L- -L- -L L. .L. .L L'
            // ignore if preceding by Mr. or Mrs.
            ,new FindAndReplace(new Regex(@"\b(?<OCR>L)\b", RegexOptions.Compiled), "OCR", "I", SubtitleError.I_And_L_Error,
                new FindAndReplace.IgnoreRule() { ReadPrevCharsFromMatch = 1, ReadNextCharsFromMatch = 1, IgnoreIfEqualsTo = "-L-" }
                , new FindAndReplace.IgnoreRule() { ReadNextCharsFromMatch = 1, IgnoreIfEqualsTo = "L-" }
                , new FindAndReplace.IgnoreRule() { ReadPrevCharsFromMatch = 1, IgnoreIfEqualsTo = "-L" }
                , new FindAndReplace.IgnoreRule() { ReadPrevCharsFromMatch = 1, ReadNextCharsFromMatch = 1, IgnoreIfEqualsTo = ".L." }
                , new FindAndReplace.IgnoreRule() { ReadNextCharsFromMatch = 1, IgnoreIfEqualsTo = "L." }
                , new FindAndReplace.IgnoreRule() { ReadPrevCharsFromMatch = 1, IgnoreIfEqualsTo = ".L" }
                , new FindAndReplace.IgnoreRule() { ReadNextCharsFromMatch = 1, IgnoreIfEqualsTo = "L'" }
                , new FindAndReplace.IgnoreRule() { ReadPrevCharsFromMatch = 5, IgnoreIfEqualsTo = "Mrs. L" }
                , new FindAndReplace.IgnoreRule() { ReadPrevCharsFromMatch = 4, IgnoreIfEqualsTo = "Mrs L" }
                , new FindAndReplace.IgnoreRule() { ReadPrevCharsFromMatch = 4, IgnoreIfEqualsTo = "Mr. L" }
                , new FindAndReplace.IgnoreRule() { ReadPrevCharsFromMatch = 3, IgnoreIfEqualsTo = "Mr L" }
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
            new FindAndReplace(new Regex(@"\b(?<Prefix>of|on|if|in)(?i)(?<Suffix>the|you|we|him|her|it|this|they|them|those|thing|things|too)\b", RegexOptions.Compiled), "${Prefix} ${Suffix}", SubtitleError.Merged_Words_Error)
            ,new FindAndReplace(new Regex(@"\b(?<Prefix>of|on|if|in)(?<Suffix>[A-ZÀ-Ý][a-zà-ÿ])", RegexOptions.Compiled), "${Prefix} ${Suffix}", SubtitleError.Merged_Words_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:after|don't|for|of|our|that|this)(?<OCR>j)", RegexOptions.Compiled), "OCR", " j", SubtitleError.Merged_Words_Error)
            ,new FindAndReplace(new Regex(@"(?i:y)(?<OCR>j)", RegexOptions.Compiled), "OCR", " j", SubtitleError.Merged_Words_Error)
            ,new FindAndReplace(new Regex(@"\b(?<OCR>off)(?i:first|too)\b", RegexOptions.Compiled), "OCR", "off ", SubtitleError.Merged_Words_Error)
            ,new FindAndReplace(new Regex(@"\b(?<OCR>fora)\b", RegexOptions.Compiled), "OCR", "for a", SubtitleError.Merged_Words_Error)
            ,new FindAndReplace(new Regex(@"\b(?<OCR>numberi)\b", RegexOptions.Compiled), "OCR", "number one", SubtitleError.Merged_Words_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:time)(?<OCR>to)\b", RegexOptions.Compiled), "OCR", " to", SubtitleError.Merged_Words_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:don't)(?<OCR>do)\b", RegexOptions.Compiled), "OCR", " do", SubtitleError.Merged_Words_Error)
            ,new FindAndReplace(new Regex(@"\b(?i:you)(?<OCR>have)\b", RegexOptions.Compiled), "OCR", " have", SubtitleError.Merged_Words_Error)
        };

        #endregion

        #region O and 0

        public static readonly FindAndReplace[] O_And_0 = new FindAndReplace[] {
            new FindAndReplace(new Regex(@"[0-9](?<OCR>O)", RegexOptions.Compiled), "OCR", "0", SubtitleError.O_And_0_Error)
            ,new FindAndReplace(new Regex(@"[0-9](?<OCR>\.O)", RegexOptions.Compiled), "OCR", ".0", SubtitleError.O_And_0_Error)
            ,new FindAndReplace(new Regex(@"[0-9](?<OCR>,O)", RegexOptions.Compiled), "OCR", ",0", SubtitleError.O_And_0_Error)
            // S0ME
            ,new FindAndReplace(new Regex(@"[A-ZÀ-Ý](?<OCR>0)", RegexOptions.Compiled), "OCR", "O", SubtitleError.O_And_0_Error)
            // 0ver
            ,new FindAndReplace(new Regex(@"\b(?<OCR>0)[A-ZÀ-Ýa-zà-ÿ]", RegexOptions.Compiled), "OCR", "O", SubtitleError.O_And_0_Error)
            // Someb0dy
            ,new FindAndReplace(new Regex(@"[a-zà-ÿ](?<OCR>0)[a-zà-ÿ]", RegexOptions.Compiled), "OCR", "o", SubtitleError.O_And_0_Error)
        };

        #endregion

        #region OCR Errors

        public static readonly FindAndReplace[] OCRErrors = new FindAndReplace[] {
            // Mr. Mrs. Dr. St. Sr. Jr.
            new FindAndReplace("Dot After Abbreviation", new Regex(@"\b(?:Mr|Mrs|Dr|St|Sr|Jr)(?<OCR>\s+)\b", RegexOptions.Compiled), "OCR", ". ", SubtitleError.OCR_Error)

            // O'Sullivan, O'Connor, O'Brien, O'Leary
            ,new FindAndReplace(new Regex(@"\b(?<OCR>o)'[A-ZÀ-Ý][a-zà-ÿ]", RegexOptions.Compiled), "OCR", "O", SubtitleError.OCR_Error)

            // a.m. p.m.
            ,new FindAndReplace(new Regex(@"(?<OCR>a|p)\.M\.", RegexOptions.Compiled), "${OCR}.m.", SubtitleError.OCR_Error)
            ,new FindAndReplace(new Regex(@"(?<OCR>A|P)\.m\.", RegexOptions.Compiled), "${OCR}.M.", SubtitleError.OCR_Error)

            // I-I-I, I-I
            //,new FindAndReplace(new Regex(@"(?<OCR>I- I- I)", RegexOptions.Compiled), "OCR", "I... I... I", SubtitleError.OCR_Error)
            //,new FindAndReplace(new Regex(@"(?<OCR>I-I-I)", RegexOptions.Compiled), "OCR", "I... I... I", SubtitleError.OCR_Error)
            //,new FindAndReplace(new Regex(@"(?<OCR>I- I)", RegexOptions.Compiled), "OCR", "I... I", SubtitleError.OCR_Error)
            //,new FindAndReplace(new Regex(@"(?<OCR>I-I)", RegexOptions.Compiled), "OCR", "I... I", SubtitleError.OCR_Error)

            // -</i> => ...</i>  -" => ..."
            ,new FindAndReplace("Three Dots", new Regex(@"(?<OCR>\s*-\s*)(?:</i>|"")$", RegexOptions.Compiled), "OCR", "...", SubtitleError.OCR_Error)

            // T- Text => T... Text (the capital letter is the same)
            ,new FindAndReplace("Three Dots", new Regex(@"\b(?<C>[A-ZÀ-Ý])(?:(?<OCR>-)\s\k<C>)+", RegexOptions.Compiled), "OCR", "...", SubtitleError.OCR_Error)

            // ... V- T => ... V... T
            ,new FindAndReplace("Three Dots", new Regex(@"\.{3}\s[A-ZÀ-Ý](?<OCR>-)\s[A-ZÀ-Ý]", RegexOptions.Compiled), "OCR", "...", SubtitleError.OCR_Error)

            // Text - $ => Text...$
            // doesn't match un-fuckin-$reasonable
            ,new FindAndReplace("Three Dots", new Regex(@"(?<![A-ZÀ-Ýa-zà-ÿ]+-[A-ZÀ-Ýa-zà-ÿ]+)(?<OCR>\s*-\s*)$", RegexOptions.Compiled), "OCR", "...", SubtitleError.OCR_Error)
            // text - text => text... text
            ,new FindAndReplace("Three Dots", new Regex(@"[a-zà-ÿ](?<OCR> - )[a-zà-ÿ]", RegexOptions.Compiled), "OCR", "... ", SubtitleError.OCR_Error)
            // text- Text => text... Text
            ,new FindAndReplace("Three Dots", new Regex(@"[Ia-zà-ÿ](?<OCR>-)\s[A-ZÀ-Ý]", RegexOptions.Compiled), "OCR", "...", SubtitleError.OCR_Error)
            // text- text => text... text
            //,new FindAndReplace("Three Dots", new Regex(@"[a-zà-ÿ](?<OCR>-)\s[a-zà-ÿ]", RegexOptions.Compiled), "OCR", "...", SubtitleError.OCR_Error)

            // Text..
            ,new FindAndReplace("Three Dots", new Regex(@"[A-ZÀ-Ýa-zà-ÿ](?<OCR>\.{2})(?:\s|♪|</i>|$)", RegexOptions.Compiled), "OCR", "...", SubtitleError.OCR_Error)
            // ..Text
            ,new FindAndReplace("Three Dots", new Regex(@"(?:\s|♪|<i>|^)(?<OCR>\.{2})[A-ZÀ-Ýa-zà-ÿ]", RegexOptions.Compiled), "OCR", "...", SubtitleError.OCR_Error)

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

        #endregion

        #region Print Cleaning

        private static void PrintCleaningMethodName([CallerMemberName] string name = null)
        {
            if (string.IsNullOrEmpty(name) == false)
                Console.WriteLine("Method: " + name);
        }

        private static void PrintCleaningRegex(Regex regexCleaning1, Regex regexCleaning2)
        {
            PrintCleaningRegex(regexCleaning1, null, null);
            PrintCleaningRegex(regexCleaning2, null, null);
        }

        private static void PrintCleaningRegex(Regex regexCleaning, string groupName, string replacement)
        {
            if (regexCleaning != null)
            {
                Console.WriteLine(
                    "Regex:  " +
                    regexCleaning.ToString() +
                    (string.IsNullOrEmpty(groupName) ? string.Empty : " -> " + groupName) +
                    (replacement == null ? string.Empty : " -> " + (replacement == string.Empty || replacement.StartsWith(" ") || replacement.EndsWith(" ") ? "\"" + replacement + "\"" : replacement))
                );
            }
        }

        private static void PrintCleaning(string text, string cleanText, FindAndReplace rule, bool cleanHICaseInsensitive)
        {
            Console.WriteLine("Error:  " + rule.SubtitleError);
            PrintCleaning(text, cleanText, cleanHICaseInsensitive ? rule.RegexCI : rule.Regex, rule.GroupName, rule.Evaluator != null && string.IsNullOrEmpty(rule.Replacement) ? null : rule.Replacement, null);
        }

        private static void PrintCleaning(IEnumerable<string> text, string cleanText, [CallerMemberName] string name = null)
        {
            PrintCleaning(string.Join(Environment.NewLine, text), cleanText, null, null, null, name);
        }

        private static void PrintCleaning(string text, IEnumerable<string> cleanText, [CallerMemberName] string name = null)
        {
            PrintCleaning(text, string.Join(Environment.NewLine, cleanText), null, null, null, name);
        }

        private static void PrintCleaning(string text, IEnumerable<string> cleanText, Regex regexCleaning, string replacement, [CallerMemberName] string name = null)
        {
            PrintCleaning(text, string.Join(Environment.NewLine, cleanText), regexCleaning, null, replacement, name);
        }

        private static void PrintCleaning(IEnumerable<string> text, IEnumerable<string> cleanText, [CallerMemberName] string name = null)
        {
            PrintCleaning(string.Join(Environment.NewLine, text), string.Join(Environment.NewLine, cleanText), null, null, null, name);
        }

        private static void PrintCleaning(IEnumerable<string> text, IEnumerable<string> cleanText, Regex regexCleaning, string replacement, [CallerMemberName] string name = null)
        {
            PrintCleaning(string.Join(Environment.NewLine, text), string.Join(Environment.NewLine, cleanText), regexCleaning, null, replacement, name);
        }

        private static void PrintCleaning(string text, string cleanText, [CallerMemberName] string name = null)
        {
            PrintCleaning(text, cleanText, null, null, null, name);
        }

        private static void PrintCleaning(string text, string cleanText, Regex regexCleaning, string replacement, [CallerMemberName] string name = null)
        {
            PrintCleaning(text, cleanText, regexCleaning, null, replacement, name);
        }

        private static void PrintCleaning(string text, string cleanText, Regex regexCleaning, string groupName, string replacement, [CallerMemberName] string name = null)
        {
            PrintCleaningMethodName(name);
            PrintCleaningRegex(regexCleaning, groupName, replacement);
            PrintCleaningBeforeAndAfter(text, cleanText);
            Console.WriteLine();
        }

        private static readonly Color textDeletedColor = Color.Red;
        private static readonly Color textInsertedColor = Color.FromArgb(171, 242, 188);

        private static void PrintCleaningBeforeAndAfter(string text, string cleanText)
        {
            var results = NetDiff.DiffUtil.Diff(text ?? string.Empty, cleanText ?? string.Empty);

            Console.Write("Before: ");
            foreach (var item in results)
            {
                if (item.Status == NetDiff.DiffStatus.Equal)
                {
                    if (item.Obj1 != '\r')
                        Console.Write(item.Obj1);
                    if (item.Obj1 == '\n')
                        Console.Write("        ");
                }
                else if (item.Status == NetDiff.DiffStatus.Deleted)
                {
                    if (item.Obj1 != default(char) && item.Obj1 != '\r')
                    {
                        if (item.Obj1 != '\n')
                        {
                            ColorChar(item.Obj1, textDeletedColor);
                        }
                        else
                        {
                            Console.WriteLine();
                            Console.Write("        ");
                        }
                    }
                }
                else if (item.Status == NetDiff.DiffStatus.Inserted)
                {
                    if (item.Obj1 != default(char) && item.Obj1 != '\r')
                    {
                        if (item.Obj1 != '\n')
                        {
                            ColorChar(item.Obj1, textInsertedColor);
                        }
                        else
                        {
                            Console.WriteLine();
                            Console.Write("        ");
                        }
                    }
                }
                else if (item.Status == NetDiff.DiffStatus.Modified)
                {
                    if (item.Obj1 != item.Obj2)
                    {
                        if (item.Obj1 != default(char) && item.Obj1 != '\r')
                        {
                            if (item.Obj1 != '\n')
                            {
                                ColorChar(item.Obj1, textDeletedColor);
                            }
                            else
                            {
                                Console.WriteLine();
                                Console.Write("        ");
                            }
                        }
                    }
                }
            }
            Console.WriteLine();

            Console.Write("After:  ");
            foreach (var item in results)
            {
                if (item.Status == NetDiff.DiffStatus.Equal)
                {
                    if (item.Obj2 != '\r')
                        Console.Write(item.Obj2);
                    if (item.Obj2 == '\n')
                        Console.Write("        ");
                }
                else if (item.Status == NetDiff.DiffStatus.Deleted)
                {
                    if (item.Obj2 != default(char) && item.Obj2 != '\r')
                    {
                        if (item.Obj2 != '\n')
                        {
                            ColorChar(item.Obj2, textDeletedColor);
                        }
                        else
                        {
                            Console.WriteLine();
                            Console.Write("        ");
                        }
                    }
                }
                else if (item.Status == NetDiff.DiffStatus.Inserted)
                {
                    if (item.Obj2 != default(char) && item.Obj2 != '\r')
                    {
                        if (item.Obj2 != '\n')
                        {
                            ColorChar(item.Obj2, textInsertedColor);
                        }
                        else
                        {
                            Console.WriteLine();
                            Console.Write("        ");
                        }
                    }
                }
                else if (item.Status == NetDiff.DiffStatus.Modified)
                {
                    if (item.Obj1 != item.Obj2)
                    {
                        if (item.Obj2 != default(char) && item.Obj2 != '\r')
                        {
                            if (item.Obj2 != '\n')
                            {
                                ColorChar(item.Obj2, textInsertedColor);
                            }
                            else
                            {
                                Console.WriteLine();
                                Console.Write("        ");
                            }
                        }
                    }
                }
            }
            Console.WriteLine();
        }

        private static void ColorChar(char chr, Color backColor)
        {
            Console.ForegroundColor = ConsoleColor.Black;
            Colorful.Console.BackgroundColor = backColor;
            Console.Write(chr);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Black;
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
            if (subtitles.IsNullOrEmpty())
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
            if (subtitles.IsNullOrEmpty())
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
            if (subtitles.IsNullOrEmpty())
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
            internal int DisplayCharCount;
            internal bool IsMatchDialog;
            internal bool ContainsItalicsStart;
            internal bool ContainsItalicsEnd;
            internal bool EndsWithPunctuation;
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

                if (subtitle.Lines != null && subtitle.Lines.Count > 1)
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

                        if (results2.IsMatchDialog == false &&
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

        private interface IError
        {
            string Description { get; }
            bool HasError(string line);
            string GetErrors(string line);
        }

        private class Error : IError
        {
            internal Regex Regex;

            public virtual string Description { get; internal set; }

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
            internal Regex ExcludeRegex;

            public override bool HasError(string line)
            {
                return base.HasError(line) && ExcludeRegex.IsMatch(line) == false;
            }
        }

        private static readonly IError[] ErrorList = new IError[]
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
                Regex = new Regex(@"[A-ZÀ-Ýa-zà-ÿ0-9#\-'.]+:\s", RegexOptions.Compiled),
                Description = "Colon"
            }
            , new Error() {
                Regex = new Regex(@"<(?!/?i>)|(?<!</?i)>", RegexOptions.Compiled),
                Description = "Italic with space"
            }
            , new Error() {
                // Of course 1 can
                Regex = new Regex(@"[A-ZÀ-Ýa-zà-ÿ]\s+1\s+[A-ZÀ-Ýa-zà-ÿ]", RegexOptions.Compiled),
                Description = "1 instead of I"
            }
            , new Error() {
                // a/b
                Regex = new Regex(@"[A-ZÀ-Ýa-zà-ÿ]/[A-ZÀ-Ýa-zà-ÿ]", RegexOptions.Compiled),
                Description = "Slash"
            }
            , new Error() {
                // " / " -> " I "
                Regex = new Regex(@"\s+/\s+", RegexOptions.Compiled),
                Description = "Slash instead of I"
            }
            , new Error() {
                // replace with new line
                Regex = new Regex(@"[!?][A-ZÀ-Ýa-zà-ÿ]", RegexOptions.Compiled),
                Description = "Possible missing new line after punctuation"
            }
            , new ComplexError() {
                Regex = new Regex(@"^[A-ZÀ-Ýa-zà-ÿ0-9#\-'.]+:", RegexOptions.Compiled),
                // exclude time
                ExcludeRegex = new Regex(@"^\d{1,2}:\d{2}", RegexOptions.Compiled),
                Description = "Possible hearing-impaired"
            }
            , new Error() {
                Regex = new Regex(@"^[A-ZÀ-Ý]+$", RegexOptions.Compiled),
                Description = "Possible hearing-impaired (All caps)"
            }
            , new ComplexError() {
                Regex = new Regex(@"^[" + HI_CHARS + @"]+$", RegexOptions.Compiled),
                // exclude:                        A...    I...    OK                         123.45      555-12345  12  A-B-C-D
                ExcludeRegex = new Regex(@"^(-\s)?(A[A. ]*|I[I. ]*|OK|O\.K\.|L\.A\.|F\.B\.I\.|\d+(\.\d+)+|\d+(-\d+)+|\d+|[A-Z](-[A-Z]){2,})[!?.]*$", RegexOptions.Compiled),
                Description = "Possible miswritten line"
            }
            /*, new Error() {
                Regex = new Regex(@"(?<!""[A-ZÀ-Ýa-zà-ÿ0-9 #\-'.]+)(""[!?])(\s|$)", RegexOptions.Compiled),
                Description = "Punctuation outside of quotation marks"
            }*/
            , new Error() {
                Regex = new Regex(@"(?<!in)'\?$", RegexOptions.Compiled),
                Description = "Ending with comma and question mark"
            }
            , new Error() {
                // ignore Mc, PhD
                Regex = new Regex(@"((?<!M)c|(?<!P)h|[a-bd-gi-zà-ÿ])[A-ZÀ-Ý]", RegexOptions.Compiled),
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

                if (subtitle.Lines.HasAny())
                {
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
                }

                if (hasError)
                {
                    int index = subtitles.IndexOf(subtitle);
                    errorLines.AddRange(subtitle.ToLines(index));
                }
            }

            return errorLines.ToArray();
        }

        public static bool HasErrors(string line)
        {
            return ErrorList.Any(err => err.HasError(line));
        }

        #endregion
    }
}
