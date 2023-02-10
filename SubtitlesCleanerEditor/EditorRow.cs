using System;
using SubtitlesCleanerLibrary;

namespace SubtitlesCleanerEditor
{
    public class EditorRow
    {
        public int Num { get; set; }
        public DateTime ShowValue { get; set; }
        public string Show { get; set; }
        public string Hide { get; set; }
        public string Duration { get; set; }
        public string Text { get; set; }
        public string Lines { get; set; }
        public string CleanText { get; set; }
        public string CleanLines { get; set; }
        public SubtitleError SubtitleError { get; set; }
    }
}
