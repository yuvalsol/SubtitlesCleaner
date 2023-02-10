using System;
using SubtitlesCleaner.Library;

namespace SubtitlesCleaner.Editor
{
    public class ErrorRow
    {
        public int Num { get; set; }
        public string Error { get; set; }
        public SubtitleError SubtitleError { get; set; }
    }
}
