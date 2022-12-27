using System;
using SubtitlesCleanerLibrary;

namespace SubtitlesCleanerEditor
{
    public class ErrorRow
    {
        public int Num { get; set; }
        public string Error { get; set; }
        public SubtitleError SubtitleError { get; set; }
    }
}
