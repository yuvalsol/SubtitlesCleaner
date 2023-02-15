using CommandLine;

namespace SubtitlesCleaner.Command
{
    class SharedOptions
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
    }

    [Verb("clean", HelpText = "Clean subtitles")]
    class CleanSubtitlesOptions : SharedOptions
    {
        [Option("cleanHICaseInsensitive", HelpText = "Clean HI case-insensitive")]
        public bool cleanHICaseInsensitive { get; set; }

        [Option("firstSubtitlesCount", HelpText = "Read only the specified first number of subtitles")]
        public int? firstSubtitlesCount { get; set; }

        [Option("suppressErrorFile", HelpText = "Do not create error file with possible errors")]
        public bool suppressErrorFile { get; set; }

        [Option("printCleaning", HelpText = "Print to console what the cleaning process does")]
        public bool printCleaning { get; set; }
    }

    [Verb("cleanEmptyAndNonSubtitles", HelpText = "Clean empty and non-subtitles")]
    class CleanEmptyAndNonSubtitlesOptions : SharedOptions
    {
        [Option("firstSubtitlesCount", HelpText = "Read only the specified first number of subtitles")]
        public int? firstSubtitlesCount { get; set; }

        [Option("printCleaning", HelpText = "Print to console what the cleaning process does")]
        public bool printCleaning { get; set; }
    }

    [Verb("addTime", HelpText = "Add time to subtitles")]
    class AddTimeOptions : SharedOptions
    {
        [Option("timeAdded", Required = true, HelpText = "Added time to subtitles")]
        public string timeAdded { get; set; }

        [Option("subtitleNumber", HelpText = "Start operation from specified subtitle. If omitted, starts with first subtitle")]
        public int? subtitleNumber { get; set; }

        [Option("firstSubtitlesCount", HelpText = "Read only the specified first number of subtitles")]
        public int? firstSubtitlesCount { get; set; }
    }

    [Verb("setShowTime", HelpText = "Move subtitles to show time")]
    class SetShowTimeOptions : SharedOptions
    {
        [Option("showTime", Required = true, HelpText = "Show time")]
        public string showTime { get; set; }

        [Option("subtitleNumber", HelpText = "Start operation from specified subtitle. If omitted, starts with first subtitle")]
        public int? subtitleNumber { get; set; }

        [Option("firstSubtitlesCount", HelpText = "Read only the specified first number of subtitles")]
        public int? firstSubtitlesCount { get; set; }
    }

    [Verb("adjustTiming", HelpText = "Adjust subtitles timing by 2 sync points")]
    class AdjustTimingOptions : SharedOptions
    {
        [Option("firstShowTime", Required = true, HelpText = "First subtitle's show time")]
        public string firstShowTime { get; set; }

        [Option("lastShowTime", Required = true, HelpText = "Last subtitle's show time")]
        public string lastShowTime { get; set; }

        [Option("firstSubtitlesCount", HelpText = "Read only the specified first number of subtitles")]
        public int? firstSubtitlesCount { get; set; }
    }

    [Verb("reorder", HelpText = "Reorder subtitles based on their show time")]
    class ReorderOptions : SharedOptions
    {
    }

    [Verb("balanceLines", HelpText = "Merge short line with long line")]
    class BalanceLinesOptions : SharedOptions
    {
    }

    [Verb("usage", HelpText = "Command usage")]
    class UsageOptions
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
