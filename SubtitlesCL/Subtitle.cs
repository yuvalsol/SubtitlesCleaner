using System;
using System.Collections;
using System.Collections.Generic;

namespace SubtitlesCL
{
    public class Subtitle : ICloneable, IComparable, IComparable<Subtitle>, IComparer, IComparer<Subtitle>, IEquatable<Subtitle>, IEqualityComparer<Subtitle>
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


        #region IComparable

        public virtual int CompareTo(object obj)
        {
            return CompareSubtitles(this, obj as Subtitle);
        }

        #endregion

        #region IComparable<Subtitle>

        public int CompareTo(Subtitle other)
        {
            return CompareSubtitles(this, other);
        }

        #endregion

        #region IComparer

        public virtual int Compare(object x, object y)
        {
            return CompareSubtitles(x as Subtitle, y as Subtitle);
        }

        #endregion

        #region IComparer<Subtitle>

        public int Compare(Subtitle x, Subtitle y)
        {
            return CompareSubtitles(x, y);
        }

        #endregion

        #region IEquatable<Subtitle>

        public bool Equals(Subtitle other)
        {
            return CompareSubtitles(this, other) == 0;
        }

        #endregion

        #region IEqualityComparer<Subtitle>

        public bool Equals(Subtitle x, Subtitle y)
        {
            return CompareSubtitles(x, y) == 0;
        }

        public int GetHashCode(Subtitle obj)
        {
            if (obj == null)
                return 0;
            return obj.GetHashCode();
        }

        #endregion

        #region Compare Subtitles

        protected static int CompareSubtitles(Subtitle x, Subtitle y)
        {
            if (ReferenceEquals(x, y))
                return 0;

            if (ReferenceEquals(y, null))
                return 1;

            if (ReferenceEquals(x, null))
                return -1;

            return x.Show.CompareTo(y.Show);
        }

        #endregion
    }
}
