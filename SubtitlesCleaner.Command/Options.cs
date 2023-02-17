using System.Text;
using CommandLine;

namespace SubtitlesCleaner.Command
{
    internal abstract class SharedOptions
    {
        [Option("path", Required = true, HelpText = "Path to subtitle file or folder")]
        public string path { get; set; }

        [Option("print", HelpText = "Print to console")]
        public bool print { get; set; }

        [Option("save", HelpText = "Save to file")]
        public bool save { get; set; }

        [Option("outputFile", HelpText = "Output file. If omitted, the program outputs on the original file")]
        public string outputFile { get; set; }

        [Option("outputFolder", HelpText = "Output folder. If omitted, the program outputs in the original folder")]
        public string outputFolder { get; set; }

        [Option("suppressBackupFile", HelpText = "Do not create backup file of the original subtitles")]
        public bool suppressBackupFile { get; set; }

        [Option("log", HelpText = "Write informative messages to log file. Overwrites existing log file")]
        public string log { get; set; }

        [Option("log+", HelpText = "Write informative messages to log file. Appends to existing log file")]
        public string logAppend { get; set; }

        [Option("csv", HelpText = "Write informative messages in a comma-separated values")]
        public bool csv { get; set; }

        [Option("quiet", HelpText = "Do not write informative messages")]
        public bool quiet { get; set; }

        [Option("sequential", HelpText = "Process subtitle files in sequential order, one after another, instead of concurrently")]
        public bool sequential { get; set; }

        public abstract string Verb { get; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (string.IsNullOrEmpty(path) == false)
            {
                sb.Append(" --path \"");
                sb.Append(path);
                sb.Append("\"");
            }

            if (print)
                sb.Append(" --print");

            if (save)
                sb.Append(" --save");

            if (string.IsNullOrEmpty(outputFile) == false)
            {
                sb.Append(" --outputFile \"");
                sb.Append(outputFile);
                sb.Append("\"");
            }

            if (string.IsNullOrEmpty(outputFolder) == false)
            {
                sb.Append(" --outputFolder \"");
                sb.Append(outputFolder);
                sb.Append("\"");
            }

            if (suppressBackupFile)
                sb.Append(" --suppressBackupFile");

            if (string.IsNullOrEmpty(log) == false)
            {
                sb.Append(" --log \"");
                sb.Append(log);
                sb.Append("\"");
            }

            if (string.IsNullOrEmpty(logAppend) == false)
            {
                sb.Append(" --log+ \"");
                sb.Append(logAppend);
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

    [Verb("clean", HelpText = "Clean subtitles")]
    internal class CleanSubtitlesOptions : SharedOptions
    {
        [Option("cleanHICaseInsensitive", HelpText = "Clean HI case-insensitive")]
        public bool cleanHICaseInsensitive { get; set; }

        [Option("firstSubtitlesCount", HelpText = "Read only the specified first number of subtitles")]
        public int? firstSubtitlesCount { get; set; }

        [Option("suppressErrorFile", HelpText = "Do not create error file with possible errors")]
        public bool suppressErrorFile { get; set; }

        [Option("printCleaning", HelpText = "Print to console what the cleaning process does")]
        public bool printCleaning { get; set; }

        public override string Verb { get { return "clean"; } }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Verb);

            if (cleanHICaseInsensitive)
                sb.Append(" --cleanHICaseInsensitive");

            if (firstSubtitlesCount != null)
            {
                sb.Append(" --firstSubtitlesCount ");
                sb.Append(firstSubtitlesCount.Value);
            }

            if (suppressErrorFile)
                sb.Append(" --suppressErrorFile");

            if (printCleaning)
                sb.Append(" --printCleaning");

            sb.Append(base.ToString());

            return sb.ToString();
        }
    }

    [Verb("cleanEmptyAndNonSubtitles", HelpText = "Clean empty and non-subtitles")]
    internal class CleanEmptyAndNonSubtitlesOptions : SharedOptions
    {
        [Option("firstSubtitlesCount", HelpText = "Read only the specified first number of subtitles")]
        public int? firstSubtitlesCount { get; set; }

        [Option("printCleaning", HelpText = "Print to console what the cleaning process does")]
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

    [Verb("addTime", HelpText = "Add time to subtitles")]
    internal class AddTimeOptions : SharedOptions
    {
        [Option("timeAdded", Required = true, HelpText = "Added time to subtitles")]
        public string timeAdded { get; set; }

        [Option("subtitleNumber", HelpText = "Start operation from specified subtitle. If omitted, starts with first subtitle")]
        public int? subtitleNumber { get; set; }

        [Option("firstSubtitlesCount", HelpText = "Read only the specified first number of subtitles")]
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

    [Verb("setShowTime", HelpText = "Move subtitles to show time")]
    internal class SetShowTimeOptions : SharedOptions
    {
        [Option("showTime", Required = true, HelpText = "Show time")]
        public string showTime { get; set; }

        [Option("subtitleNumber", HelpText = "Start operation from specified subtitle. If omitted, starts with first subtitle")]
        public int? subtitleNumber { get; set; }

        [Option("firstSubtitlesCount", HelpText = "Read only the specified first number of subtitles")]
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

    [Verb("adjustTiming", HelpText = "Adjust subtitles timing by 2 sync points")]
    internal class AdjustTimingOptions : SharedOptions
    {
        [Option("firstShowTime", Required = true, HelpText = "First subtitle's show time")]
        public string firstShowTime { get; set; }

        [Option("lastShowTime", Required = true, HelpText = "Last subtitle's show time")]
        public string lastShowTime { get; set; }

        [Option("firstSubtitlesCount", HelpText = "Read only the specified first number of subtitles")]
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

    [Verb("reorder", HelpText = "Reorder subtitles based on their show time")]
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

    [Verb("balanceLines", HelpText = "Merge short line with long line")]
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

    [Verb("usage", HelpText = "Command usage")]
    internal class UsageOptions
    {
        [Option("clean", HelpText = "Print usage for clean", Group = "verb")]
        public bool clean { get; set; }

        [Option("cleanEmptyAndNonSubtitles", HelpText = "Print usage for cleanEmptyAndNonSubtitles", Group = "verb")]
        public bool cleanEmptyAndNonSubtitles { get; set; }

        [Option("addTime", HelpText = "Print usage for addTime", Group = "verb")]
        public bool addTime { get; set; }

        [Option("setShowTime", HelpText = "Print usage for setShowTime", Group = "verb")]
        public bool setShowTime { get; set; }

        [Option("adjustTiming", HelpText = "Print usage for adjustTiming", Group = "verb")]
        public bool adjustTiming { get; set; }

        [Option("reorder", HelpText = "Print usage for reorder", Group = "verb")]
        public bool reorder { get; set; }

        [Option("balanceLines", HelpText = "Print usage for balanceLines", Group = "verb")]
        public bool balanceLines { get; set; }
    }
}
