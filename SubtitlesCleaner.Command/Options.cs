using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;
using CommandLine.Text;

namespace SubtitlesCleaner.Command
{
    internal abstract class SharedOptions
    {
        [Option("path", Required = true, HelpText = "Path to subtitle file or folder.")]
        public string path { get; set; }

        [Option("subfolders", Required = false, HelpText = "Include all subfolders under path.")]
        public bool subfolders { get; set; }

        [Option("print", HelpText = "Print to console.")]
        public bool print { get; set; }

        [Option("save", HelpText = "Save to file.")]
        public bool save { get; set; }

        [Option("outputFile", HelpText = "Output file. If omitted, the program outputs on the original file.")]
        public string outputFile { get; set; }

        [Option("outputFolder", HelpText = "Output folder. If omitted, the program outputs in the original folder.")]
        public string outputFolder { get; set; }

        [Option("suppressBackupFile", HelpText = "Do not create backup file of the original subtitles file.")]
        public bool suppressBackupFile { get; set; }

        [Option("suppressBackupFileOnSame", HelpText = "Do not create backup file if processing results the same file.")]
        public bool suppressBackupFileOnSame { get; set; }

        [Option("log", HelpText = "Write informative messages to log file. Overwrites existing log file.")]
        public string log { get; set; }

        [Option("log+", HelpText = "Write informative messages to log file. Appends to existing log file.")]
        public string logAppend { get; set; }

        [Option("csv", HelpText = "Write informative messages in a comma-separated values.")]
        public bool csv { get; set; }

        [Option("quiet", HelpText = "Do not write informative messages.")]
        public bool quiet { get; set; }

        [Option("sequential", HelpText = "Process subtitle files in sequential order, one after another, instead of concurrently.")]
        public bool sequential { get; set; }

        public abstract string Verb { get; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (string.IsNullOrEmpty(path) == false)
            {
                sb.Append(" --path @\"");
                sb.Append(path.EscapeVerbatim());
                sb.Append("\"");
            }

            if (subfolders)
                sb.Append(" --subfolders");

            if (print)
                sb.Append(" --print");

            if (save)
                sb.Append(" --save");

            if (string.IsNullOrEmpty(outputFile) == false)
            {
                sb.Append(" --outputFile @\"");
                sb.Append(outputFile.EscapeVerbatim());
                sb.Append("\"");
            }

            if (string.IsNullOrEmpty(outputFolder) == false)
            {
                sb.Append(" --outputFolder @\"");
                sb.Append(outputFolder.EscapeVerbatim());
                sb.Append("\"");
            }

            if (suppressBackupFile)
                sb.Append(" --suppressBackupFile");

            if (suppressBackupFileOnSame)
                sb.Append(" --suppressBackupFileOnSame");

            if (string.IsNullOrEmpty(log) == false)
            {
                sb.Append(" --log @\"");
                sb.Append(log.EscapeVerbatim());
                sb.Append("\"");
            }

            if (string.IsNullOrEmpty(logAppend) == false)
            {
                sb.Append(" --log+ @\"");
                sb.Append(logAppend.EscapeVerbatim());
                sb.Append("\"");
            }

            if (csv)
                sb.Append(" --csv");

            if (quiet)
                sb.Append(" --quiet");

            if (sequential)
                sb.Append(" --sequential");

            return sb.ToString();
        }
    }

    [Verb("clean", HelpText = "Clean subtitles.")]
    internal class CleanSubtitlesOptions : SharedOptions
    {
        [Option("cleanHICaseInsensitive", HelpText = "Clean HI case-insensitive.")]
        public bool cleanHICaseInsensitive { get; set; }

        [Option("dictionaryCleaning", HelpText = "Clean misspelled words with English dictionary.")]
        public bool dictionaryCleaning { get; set; }

        [Option("firstSubtitlesCount", HelpText = "Read only the specified first number of subtitles.")]
        public int? firstSubtitlesCount { get; set; }

        [Option("suppressWarningsFile", HelpText = "Do not create warnings file.")]
        public bool suppressWarningsFile { get; set; }

        [Option("printCleaning", HelpText = "Print to console what the cleaning process does.")]
        public bool printCleaning { get; set; }

        public override string Verb { get { return "clean"; } }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Verb);

            if (cleanHICaseInsensitive)
                sb.Append(" --cleanHICaseInsensitive");

            if (dictionaryCleaning)
                sb.Append(" --dictionaryCleaning");

            if (firstSubtitlesCount != null)
            {
                sb.Append(" --firstSubtitlesCount ");
                sb.Append(firstSubtitlesCount.Value);
            }

            if (suppressWarningsFile)
                sb.Append(" --suppressWarningsFile");

            if (printCleaning)
                sb.Append(" --printCleaning");

            sb.Append(base.ToString());

            return sb.ToString();
        }

        [Usage(ApplicationAlias = "SubtitlesCleanerCommand.exe")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example(
                    "Clean subtitle file",
                    new CleanSubtitlesOptions
                    {
                        path = @"C:\My Documents\Subtitle.srt",
                        save = true
                    }
                );

                yield return new Example(
                    "Clean subtitle file and save results in another folder",
                    new CleanSubtitlesOptions
                    {
                        path = @"C:\My Documents\Subtitle.srt",
                        save = true,
                        outputFile = @"C:\My Documents\Subtitles"
                    }
                );

                yield return new Example(
                    "Clean hearing-impaired case-insensitive",
                    new CleanSubtitlesOptions
                    {
                        cleanHICaseInsensitive = true,
                        path = @"C:\My Documents\Subtitle.srt",
                        save = true
                    }
                );

                yield return new Example(
                    "Clean subtitle file and suppress backup & warnings files",
                    new CleanSubtitlesOptions
                    {
                        path = @"C:\My Documents\Subtitle.srt",
                        save = true,
                        suppressBackupFile = true,
                        suppressWarningsFile = true
                    }
                );

                yield return new Example(
                    "Clean subtitle file and suppress warnings file. Create backup file if the cleaned subtitles are not the same as the original subtitles",
                    new CleanSubtitlesOptions
                    {
                        path = @"C:\My Documents\Subtitle.srt",
                        save = true,
                        suppressBackupFileOnSame = true,
                        suppressWarningsFile = true
                    }
                );

                yield return new Example(
                    "Clean all subtitle files in the folder",
                    new CleanSubtitlesOptions
                    {
                        path = @"C:\My Documents\Subtitles",
                        save = true
                    }
                );

                yield return new Example(
                    "Print to console",
                    new CleanSubtitlesOptions
                    {
                        path = @"C:\My Documents\Subtitle.srt",
                        print = true
                    }
                );

                yield return new Example(
                    "Print to console the cleaned subtitles and the cleaning process. Very useful when tracking cleaning errors",
                    new CleanSubtitlesOptions
                    {
                        path = @"C:\My Documents\Subtitle.srt",
                        print = true,
                        printCleaning = true
                    }
                );

                yield return new Example(
                    "Clean all subtitle files in the folder. Write informative messages to text log file log_clean.txt",
                    new CleanSubtitlesOptions
                    {
                        path = @"C:\My Documents\Subtitles",
                        save = true,
                        log = @"C:\My Documents\Subtitles\log_clean.txt"
                    }
                );

                yield return new Example(
                    "Clean all subtitle files in the folder. Write informative messages to csv log file log_clean.csv. If the file already exists, append to it",
                    new CleanSubtitlesOptions
                    {
                        path = @"C:\My Documents\Subtitles",
                        save = true,
                        logAppend = @"C:\My Documents\Subtitles\log_clean.csv",
                        csv = true
                    }
                );

                yield return new Example(
                    "Clean all subtitle files in the folder. Don't write informative messages",
                    new CleanSubtitlesOptions
                    {
                        path = @"C:\My Documents\Subtitles",
                        save = true,
                        quiet = true
                    }
                );

                yield return new Example(
                    "Clean all subtitle files in the folder, one after the other, in sequential order. Don't write informative messages",
                    new CleanSubtitlesOptions
                    {
                        path = @"C:\My Documents\Subtitles",
                        save = true,
                        quiet = true,
                        sequential = true
                    }
                );
            }
        }
    }

    [Verb("cleanEmptyAndNonSubtitles", HelpText = "Clean empty and non-subtitles.")]
    internal class CleanEmptyAndNonSubtitlesOptions : SharedOptions
    {
        [Option("firstSubtitlesCount", HelpText = "Read only the specified first number of subtitles.")]
        public int? firstSubtitlesCount { get; set; }

        [Option("printCleaning", HelpText = "Print to console what the cleaning process does.")]
        public bool printCleaning { get; set; }

        public override string Verb { get { return "cleanEmptyAndNonSubtitles"; } }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Verb);

            if (firstSubtitlesCount != null)
            {
                sb.Append(" --firstSubtitlesCount ");
                sb.Append(firstSubtitlesCount.Value);
            }

            if (printCleaning)
                sb.Append(" --printCleaning");

            sb.Append(base.ToString());

            return sb.ToString();
        }
    }

    [Verb("addTime", HelpText = "Add time to subtitles.")]
    internal class AddTimeOptions : SharedOptions
    {
        [Option("timeAdded", Required = true, HelpText = "Added time to subtitles.")]
        public string timeAdded { get; set; }

        [Option("subtitleNumber", HelpText = "Start operation from specified subtitle. If omitted, starts with first subtitle.")]
        public int? subtitleNumber { get; set; }

        [Option("firstSubtitlesCount", HelpText = "Read only the specified first number of subtitles.")]
        public int? firstSubtitlesCount { get; set; }

        public override string Verb { get { return "addTime"; } }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Verb);

            if (string.IsNullOrEmpty(timeAdded) == false)
            {
                sb.Append(" --timeAdded ");
                sb.Append(timeAdded);
            }

            if (subtitleNumber != null)
            {
                sb.Append(" --subtitleNumber ");
                sb.Append(subtitleNumber.Value);
            }

            if (firstSubtitlesCount != null)
            {
                sb.Append(" --firstSubtitlesCount ");
                sb.Append(firstSubtitlesCount.Value);
            }

            sb.Append(base.ToString());

            return sb.ToString();
        }
    }

    [Verb("setShowTime", HelpText = "Move subtitles to show time.")]
    internal class SetShowTimeOptions : SharedOptions
    {
        [Option("showTime", Required = true, HelpText = "Show time.")]
        public string showTime { get; set; }

        [Option("subtitleNumber", HelpText = "Start operation from specified subtitle. If omitted, starts with first subtitle.")]
        public int? subtitleNumber { get; set; }

        [Option("firstSubtitlesCount", HelpText = "Read only the specified first number of subtitles.")]
        public int? firstSubtitlesCount { get; set; }

        public override string Verb { get { return "setShowTime"; } }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Verb);

            if (string.IsNullOrEmpty(showTime) == false)
            {
                sb.Append(" --showTime ");
                sb.Append(showTime);
            }

            if (subtitleNumber != null)
            {
                sb.Append(" --subtitleNumber ");
                sb.Append(subtitleNumber.Value);
            }

            if (firstSubtitlesCount != null)
            {
                sb.Append(" --firstSubtitlesCount ");
                sb.Append(firstSubtitlesCount.Value);
            }

            sb.Append(base.ToString());

            return sb.ToString();
        }
    }

    [Verb("adjustTiming", HelpText = "Adjust subtitles timing by 2 sync points.")]
    internal class AdjustTimingOptions : SharedOptions
    {
        [Option("firstShowTime", Required = true, HelpText = "First subtitle's show time.")]
        public string firstShowTime { get; set; }

        [Option("lastShowTime", Required = true, HelpText = "Last subtitle's show time.")]
        public string lastShowTime { get; set; }

        [Option("firstSubtitlesCount", HelpText = "Read only the specified first number of subtitles.")]
        public int? firstSubtitlesCount { get; set; }

        public override string Verb { get { return "adjustTiming"; } }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Verb);

            if (string.IsNullOrEmpty(firstShowTime) == false)
            {
                sb.Append(" --firstShowTime ");
                sb.Append(firstShowTime);
            }

            if (string.IsNullOrEmpty(lastShowTime) == false)
            {
                sb.Append(" --lastShowTime ");
                sb.Append(lastShowTime);
            }

            if (firstSubtitlesCount != null)
            {
                sb.Append(" --firstSubtitlesCount ");
                sb.Append(firstSubtitlesCount.Value);
            }

            sb.Append(base.ToString());

            return sb.ToString();
        }
    }

    [Verb("reorder", HelpText = "Reorder subtitles based on their show time.")]
    internal class ReorderOptions : SharedOptions
    {
        public override string Verb { get { return "reorder"; } }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Verb);

            sb.Append(base.ToString());

            return sb.ToString();
        }
    }

    [Verb("balanceLines", HelpText = "Merge short line with long line.")]
    internal class BalanceLinesOptions : SharedOptions
    {
        public override string Verb { get { return "balanceLines"; } }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Verb);

            sb.Append(base.ToString());

            return sb.ToString();
        }
    }
}
