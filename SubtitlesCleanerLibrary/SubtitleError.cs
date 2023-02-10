using System;
using System.Drawing;

namespace SubtitlesCleaner.Library
{
    [Flags]
    public enum SubtitleError
    {
        None = 0,

        [ErrorColor(181, 181, 181)]
        Empty_Line = (1 << 0),

        [ErrorColor(207, 207, 207)]
        Non_Subtitle = (1 << 1),

        [ErrorColor(220, 250, 174)]
        Missing_New_Line = (1 << 2),

        [ErrorColor(220, 232, 174)]
        Merge_Lines = (1 << 3),

        [ErrorColor(198, 230, 209)]
        Redundant_Italics = (1 << 4),

        [ErrorColor(240, 255, 225)]
        Redundant_Spaces = (1 << 5),

        [ErrorColor(198, 255, 209)]
        Missing_Spaces = (1 << 6),

        [ErrorColor(232, 221, 248)]
        ASSA_Tags = (1 << 7),

        [ErrorColor(196, 198, 239)]
        Notes_Error = (1 << 8),

        [ErrorColor(189, 219, 250)]
        Punctuations_Error = (1 << 9),

        [ErrorColor(172, 201, 230)]
        Non_Ansi_Chars = (1 << 10),

        [ErrorColor(154, 179, 204)]
        Encoded_HTML = (1 << 11),

        [ErrorColor(206, 206, 113)]
        Malformed_Letters = (1 << 12),

        [ErrorColor(192, 192, 160)]
        Accent_Letters = (1 << 13),

        [ErrorColor(206, 206, 157)]
        Merged_Words_Error = (1 << 14),

        [ErrorColor(209, 181, 155)]
        Contractions_Error = (1 << 15),

        [ErrorColor(219, 204, 190)]
        O_And_0_Error = (1 << 16),

        [ErrorColor(237, 214, 193)]
        I_And_L_Error = (1 << 17),

        [ErrorColor(255, 229, 204)]
        OCR_Error = (1 << 18),

        [ErrorColor(253, 222, 128)]
        Dialog_Error = (1 << 19),

        [ErrorColor(255, 255, 153)]
        Hearing_Impaired = (1 << 20)
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class ErrorColorAttribute : Attribute
    {
        public ErrorColorAttribute(
            int backRed, int backGreen, int backBlue,
            int foreRed = 0, int foreGreen = 0, int foreBlue = 0)
        {
            BackErrorColor = Color.FromArgb(backRed, backGreen, backBlue);
            ForeErrorColor = Color.FromArgb(foreRed, foreGreen, foreBlue);
        }

        public Color BackErrorColor { get; private set; }
        public Color ForeErrorColor { get; private set; }
    }

    public static partial class SubtitleErrorExtensions
    {
        public static bool IsSet(this SubtitleError enumValue, SubtitleError flag)
        {
            return (enumValue & flag) == flag;
        }

        public static bool IsOnlySet(this SubtitleError enumValue, SubtitleError flag)
        {
            return (enumValue & ~flag) == SubtitleError.None;
        }

        public static SubtitleError RemoveFlag(this SubtitleError enumValue, SubtitleError flag)
        {
            return enumValue & ~flag;
        }

        public static Color BackErrorColor(this SubtitleError enumValue)
        {
            var field = typeof(SubtitleError).GetField(enumValue.ToString());
            var attributes = field.GetCustomAttributes(typeof(ErrorColorAttribute), false);
            return (attributes.Length > 0 ? ((ErrorColorAttribute)attributes[0]).BackErrorColor : Color.White);
        }

        public static Color ForeErrorColor(this SubtitleError enumValue)
        {
            var field = typeof(SubtitleError).GetField(enumValue.ToString());
            var attributes = field.GetCustomAttributes(typeof(ErrorColorAttribute), false);
            return (attributes.Length > 0 ? ((ErrorColorAttribute)attributes[0]).ForeErrorColor : Color.Black);
        }
    }
}
