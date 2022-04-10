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

        [Option("shift", Required = false, HelpText = "Shift subtitles by shift time")]
        public bool shift { get; set; }

        [Option("shiftTime", Required = false, HelpText = "Shift time")]
        public string shiftTime { get; set; }

        [Option("move", Required = false, HelpText = "Move subtitles to show time")]
        public bool move { get; set; }

        [Option("showTime", Required = false, HelpText = "Show time")]
        public string showTime { get; set; }

        [Option("subtitleNumber", Required = false, HelpText = "Shift/Move from subtitle number")]
        public int? subtitleNumber { get; set; }

        [Option("adjust", Required = false, HelpText = "Adjust subtitles timings")]
        public bool adjust { get; set; }

        [Option("showStart", Required = false, HelpText = "Show Start")]
        public string showStart { get; set; }

        [Option("showEnd", Required = false, HelpText = "Show end")]
        public string showEnd { get; set; }

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

            if (shift)
                cmd += " --shift";

            if (string.IsNullOrEmpty(shiftTime) == false)
                cmd += " --shiftTime \"" + shiftTime + "\"";

            if (move)
                cmd += " --move";

            if (string.IsNullOrEmpty(showTime) == false)
                cmd += " --showTime \"" + showTime + "\"";

            if (subtitleNumber != null)
                cmd += " --subtitleNumber " + subtitleNumber.Value;

            if (adjust)
                cmd += " --adjust";

            if (string.IsNullOrEmpty(showStart) == false)
                cmd += " --showStart \"" + showStart + "\"";

            if (string.IsNullOrEmpty(showEnd) == false)
                cmd += " --showEnd \"" + showEnd + "\"";

            if (string.IsNullOrEmpty(path) == false)
                cmd += " --path \"" + path + "\"";

            return cmd.Trim();
        }
    }
}
