using System;
using System.Collections.Generic;
using SubtitlesCL;

namespace SubtitlesEditor
{
    public class EditorRow
    {
        public int Num { get; set; }
        public DateTime ShowValue { get; set; }
        public string Show { get; set; }
        public string Text { get; set; }
        public string Lines { get; set; }
        public SubtitleError SubtitleError { get; set; }
    }
}
