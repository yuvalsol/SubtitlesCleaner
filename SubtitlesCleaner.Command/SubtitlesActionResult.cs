using System;
using System.Text;

namespace SubtitlesCleaner.Command
{
    internal class SubtitlesActionResult
    {
        public string FilePath { get; internal set; }
        public SharedOptions SharedOptions { get; internal set; }
        public bool Succeeded { get; internal set; }
        public StringBuilder Log { get; internal set; }
        public Exception Error { get; internal set; }
        public int FileIndex { get; internal set; }
    }
}
