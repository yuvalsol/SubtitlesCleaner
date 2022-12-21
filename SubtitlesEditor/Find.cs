using System;

namespace SubtitlesCleanerEditor
{
    public class Find
    {
        public string Search { get; set; }
        public bool MatchCase { get; set; }
        public bool MatchWholeWord { get; set; }
    }

    public class FindAndReplace : Find
    {
        public string Replace { get; set; }
    }
}
