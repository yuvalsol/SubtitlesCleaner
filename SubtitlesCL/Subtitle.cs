using System;
using System.Collections.Generic;

namespace SubtitlesCL
{
    public class Subtitle : ICloneable
    {
        public DateTime Show { get; set; }
        public DateTime Hide { get; set; }
        public List<string> Lines { get; set; }
        public SubtitleError SubtitleError { get; set; }

        public Subtitle()
        {
            Show = DateTime.MinValue;
            Hide = DateTime.MinValue;
            Lines = new List<string>();
            SubtitleError = SubtitleError.None;
        }

        public object Clone()
        {
            return new Subtitle()
            {
                Show = Show,
                Hide = Hide,
                Lines = new List<string>(Lines),
                SubtitleError = SubtitleError
            };
        }

        public TimeSpan Duration { get { return Hide - Show; } }

        private const string timeFormat = "{0:D2}:{1:D2}:{2:D2},{3:D3}";

        public string ShowToString()
        {
            return string.Format(timeFormat, Show.Hour, Show.Minute, Show.Second, Show.Millisecond);
        }

        public string HideToString()
        {
            return string.Format(timeFormat, Hide.Hour, Hide.Minute, Hide.Second, Hide.Millisecond);
        }

        public string DurationToString()
        {
            TimeSpan duration = Duration;
            return string.Format(timeFormat, duration.Hours, duration.Minutes, duration.Seconds, duration.Milliseconds);
        }

        public string TimeToString()
        {
            return string.Format("{0} --> {1}", ShowToString(), HideToString());
        }

        public string ToStringWithPipe()
        {
            return string.Join("|", Lines);
        }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, Lines);
        }
    }
}
