using CommandLine;

namespace SubtitlesCleaner.Command
{
    class SharedOptions
    {
        [Option("path", Required = true, HelpText = "Path to subtitle file or folder")]
        public string path { get; set; }

        [Option("print", Required = false, HelpText = "Print to console", Group = "Output")]
        public bool print { get; set; }

        [Option("save", Required = false, HelpText = "Save to file", Group = "Output")]
        public bool save { get; set; }

        [Option("outputFile", Required = false, HelpText = "Output file. If omitted, the program outputs on the original file")]
        public string outputFile { get; set; }

        [Option("outputFolder", Required = false, HelpText = "Output folder. If omitted, the program outputs in the original folder")]
        public string outputFolder { get; set; }

        [Option("suppressBackupFile", Required = false, HelpText = "Do not create backup file of the original subtitles")]
        public bool suppressBackupFile { get; set; }

        [Option("log", Required = false, HelpText = "Write informative messages to log file. Overwrites existing log file")]
        public string log { get; set; }

        [Option("log+", Required = false, HelpText = "Write informative messages to log file. Appends to existing log file")]
        public string logAppend { get; set; }

        [Option("csv", Required = false, HelpText = "Write informative messages in a comma-separated values")]
        public bool csv { get; set; }

        [Option("quiet", Required = false, HelpText = "Do not write informative messages")]
        public bool quiet { get; set; }
    }

    [Verb("clean", HelpText = "Clean subtitles")]
    class CleanSubtitlesOptions : SharedOptions
    {
        [Option("cleanHICaseInsensitive", Required = false, HelpText = "Clean HI case-insensitive")]
        public bool cleanHICaseInsensitive { get; set; }

        [Option("firstSubtitlesCount", Required = false, HelpText = "Read only the specified first number of subtitles")]
        public int? firstSubtitlesCount { get; set; }

        [Option("suppressErrorFile", Required = false, HelpText = "Do not create error file with possible errors")]
        public bool suppressErrorFile { get; set; }

        [Option("printCleaning", Required = false, HelpText = "Print to console what the cleaning process does")]
        public bool printCleaning { get; set; }
    }

    [Verb("cleanEmptyAndNonSubtitles", HelpText = "Clean empty and non-subtitles")]
    class CleanEmptyAndNonSubtitlesOptions : SharedOptions
    {
        [Option("firstSubtitlesCount", Required = false, HelpText = "Read only the specified first number of subtitles")]
        public int? firstSubtitlesCount { get; set; }

        [Option("printCleaning", Required = false, HelpText = "Print to console what the cleaning process does")]
        public bool printCleaning { get; set; }
    }

    [Verb("addTime", HelpText = "Add time to subtitles")]
    class AddTimeOptions : SharedOptions
    {
        [Option("timeAdded", Required = true, HelpText = "Added time to subtitles")]
        public string timeAdded { get; set; }

        [Option("subtitleNumber", Required = false, HelpText = "Start operation from specified subtitle. If omitted, starts with first subtitle")]
        public int? subtitleNumber { get; set; }

        [Option("firstSubtitlesCount", Required = false, HelpText = "Read only the specified first number of subtitles")]
        public int? firstSubtitlesCount { get; set; }
    }

    [Verb("setShowTime", HelpText = "Move subtitles to show time")]
    class SetShowTimeOptions : SharedOptions
    {
        [Option("showTime", Required = false, HelpText = "Show time")]
        public string showTime { get; set; }

        [Option("subtitleNumber", Required = false, HelpText = "Start operation from specified subtitle. If omitted, starts with first subtitle")]
        public int? subtitleNumber { get; set; }

        [Option("firstSubtitlesCount", Required = false, HelpText = "Read only the specified first number of subtitles")]
        public int? firstSubtitlesCount { get; set; }
    }

    [Verb("adjustTiming", HelpText = "Adjust subtitles timing by 2 sync points")]
    class AdjustTimingOptions : SharedOptions
    {
        [Option("firstShowTime", Required = true, HelpText = "First subtitle's show time")]
        public string firstShowTime { get; set; }

        [Option("lastShowTime", Required = true, HelpText = "Last subtitle's show time")]
        public string lastShowTime { get; set; }

        [Option("firstSubtitlesCount", Required = false, HelpText = "Read only the specified first number of subtitles")]
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
        [Option("clean", Required = false, HelpText = "Print usage for clean")]
        public bool clean { get; set; }

        [Option("cleanEmptyAndNonSubtitles", Required = false, HelpText = "Print usage for cleanEmptyAndNonSubtitles")]
        public bool cleanEmptyAndNonSubtitles { get; set; }

        [Option("addTime", Required = false, HelpText = "Print usage for addTime")]
        public bool addTime { get; set; }

        [Option("setShowTime", Required = false, HelpText = "Print usage for setShowTime")]
        public bool setShowTime { get; set; }

        [Option("adjustTiming", Required = false, HelpText = "Print usage for adjustTiming")]
        public bool adjustTiming { get; set; }

        [Option("reorder", Required = false, HelpText = "Print usage for reorder")]
        public bool reorder { get; set; }

        [Option("balanceLines", Required = false, HelpText = "Print usage for balanceLines")]
        public bool balanceLines { get; set; }
    }
}
