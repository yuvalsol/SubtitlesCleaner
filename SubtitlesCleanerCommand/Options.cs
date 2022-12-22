using CommandLine;

namespace SubtitlesCleanerCommand
{
    class ReadAndWriteOptions
    {
        [Option("path", Required = true, HelpText = "Path to subtitle file or folder")]
        public string path { get; set; }

        [Option("print", Required = false, HelpText = "Print to console", Group = "Output")]
        public bool print { get; set; }

        [Option("save", Required = false, HelpText = "Save to file", Group = "Output")]
        public bool save { get; set; }

        [Option("outFile", Required = false, HelpText = "Output file name when saving to file")]
        public string outFile { get; set; }

        [Option("outPath", Required = false, HelpText = "Output path when saving to file")]
        public string outPath { get; set; }
    }

    [Verb("clean", HelpText = "Clean subtitles")]
    class CleanOptions : ReadAndWriteOptions
    {
        [Option("cleanHICaseInsensitive", Required = false, HelpText = "Clean HI case-insensitive")]
        public bool cleanHICaseInsensitive { get; set; }

        [Option("firstSubtitlesCount", Required = false, HelpText = "Read only the specified first number of subtitles")]
        public int? firstSubtitlesCount { get; set; }
    }

    [Verb("addTime", HelpText = "Add time to subtitles")]
    class AddTimeOptions : ReadAndWriteOptions
    {
        [Option("timeAdded", Required = true, HelpText = "Added time to subtitles")]
        public string timeAdded { get; set; }

        [Option("subtitleNumber", Required = false, HelpText = "Start operation from specified subtitle. If omitted, starts with first subtitle")]
        public int? subtitleNumber { get; set; }

        [Option("firstSubtitlesCount", Required = false, HelpText = "Read only the specified first number of subtitles")]
        public int? firstSubtitlesCount { get; set; }
    }

    [Verb("setShowTime", HelpText = "Move subtitles to show time")]
    class SetShowTimeOptions : ReadAndWriteOptions
    {
        [Option("showTime", Required = false, HelpText = "Show time")]
        public string showTime { get; set; }

        [Option("subtitleNumber", Required = false, HelpText = "Start operation from specified subtitle. If omitted, starts with first subtitle")]
        public int? subtitleNumber { get; set; }

        [Option("firstSubtitlesCount", Required = false, HelpText = "Read only the specified first number of subtitles")]
        public int? firstSubtitlesCount { get; set; }
    }

    [Verb("adjustTiming", HelpText = "Adjust subtitles timing by 2 sync points")]
    class AdjustTimingOptions : ReadAndWriteOptions
    {
        [Option("firstShowTime", Required = true, HelpText = "First subtitle's show time")]
        public string firstShowTime { get; set; }

        [Option("lastShowTime", Required = true, HelpText = "Last subtitle's show time")]
        public string lastShowTime { get; set; }

        [Option("firstSubtitlesCount", Required = false, HelpText = "Read only the specified first number of subtitles")]
        public int? firstSubtitlesCount { get; set; }
    }

    [Verb("reorder", HelpText = "Reorder subtitles based on their show time")]
    class ReorderOptions : ReadAndWriteOptions
    {
    }

    [Verb("balanceLines", HelpText = "Merge short line with preceding long line")]
    class BalanceLinesOptions : ReadAndWriteOptions
    {
    }
}
