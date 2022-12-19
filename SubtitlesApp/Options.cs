using System;
using CommandLine;

namespace SubtitlesApp
{
    class Options
    {
        [Option("print", Required = false, HelpText = "Print to console")]
        public bool print { get; set; }

        [Option("save", Required = false, HelpText = "Save to file")]
        public bool save { get; set; }

        [Option("outFile", Required = false, HelpText = "Output file name when saving to file")]
        public string outFile { get; set; }

        [Option("outPath", Required = false, HelpText = "Output path when saving to file")]
        public string outPath { get; set; }

        [Option("clean", Required = false, HelpText = "Clean the subtitles")]
        public bool clean { get; set; }

        [Option("firstSubtitlesCount", Required = false, HelpText = "Get the first number of subtitles")]
        public int? firstSubtitlesCount { get; set; }

        [Option("cleanHICaseInsensitive", Required = false, HelpText = "Clean HI case-insensitive")]
        public bool cleanHICaseInsensitive { get; set; }

        [Option("subtitlesOrder", Required = false, HelpText = "Set subtitles order")]
        public bool subtitlesOrder { get; set; }

        [Option("linesBalance", Required = false, HelpText = "Balance lines in subtitles")]
        public bool linesBalance { get; set; }

        [Option("addTime", Required = false, HelpText = "Add time to subtitles")]
        public bool addTime { get; set; }

        [Option("timeAdded", Required = false, HelpText = "Added time to subtitles")]
        public string timeAdded { get; set; }

        [Option("setShowTime", Required = false, HelpText = "Move subtitles to show time")]
        public bool setShowTime { get; set; }

        [Option("subtitleNumber", Required = false, HelpText = "Add time or set show time starting from subtitle number")]
        public int? subtitleNumber { get; set; }

        [Option("adjustTiming", Required = false, HelpText = "Adjust subtitles timing by 2 sync points")]
        public bool adjustTiming { get; set; }

        [Option("showTime", Required = false, HelpText = "Show time")]
        public string showTime { get; set; }

        [Option("hideTime", Required = false, HelpText = "Hide time")]
        public string hideTime { get; set; }

        [Option("path", Required = false, HelpText = "Path to file or folder")]
        public string path { get; set; }

        public override string ToString()
        {
            string cmd = "SubtitlesApp.exe";

            if (print)
                cmd += " --print";

            if (save)
                cmd += " --save";

            if (string.IsNullOrEmpty(outFile) == false)
                cmd += " --outFile \"" + outFile + "\"";

            if (string.IsNullOrEmpty(outPath) == false)
                cmd += " --outPath \"" + outPath + "\"";

            if (clean)
                cmd += " --clean";

            if (firstSubtitlesCount != null)
                cmd += " --firstSubtitlesCount " + firstSubtitlesCount.Value;

            if (cleanHICaseInsensitive)
                cmd += " --cleanHICaseInsensitive";

            if (subtitlesOrder)
                cmd += " --subtitlesOrder";

            if (linesBalance)
                cmd += " --linesBalance";

            if (addTime)
                cmd += " --addTime";

            if (string.IsNullOrEmpty(timeAdded) == false)
                cmd += " --timeAdded \"" + timeAdded + "\"";

            if (setShowTime)
                cmd += " --setShowTime";

            if (subtitleNumber != null)
                cmd += " --subtitleNumber " + subtitleNumber.Value;

            if (adjustTiming)
                cmd += " --adjustTiming";

            if (string.IsNullOrEmpty(showTime) == false)
                cmd += " --showTime \"" + showTime + "\"";

            if (string.IsNullOrEmpty(hideTime) == false)
                cmd += " --hideTime \"" + hideTime + "\"";

            if (string.IsNullOrEmpty(path) == false)
                cmd += " --path \"" + path + "\"";

            return cmd.Trim();
        }
    }
}
