using System;
using System.Drawing;

namespace SubtitlesCleanerLibrary
{
    [Flags]
    public enum SubtitleError
    {
        None = 0,

        [ErrorColor(128, 128, 128, 255, 255, 255)]
        Empty_Line = (1 << 0),

        [ErrorColor(207, 207, 207)]
        Not_Subtitle = (1 << 1),

        [ErrorColor(108, 170, 35, 255, 255, 255)]
        Merge_Lines = (1 << 2),

        [ErrorColor(240, 255, 225)]
        Redundant_Italics = (1 << 3),

        [ErrorColor(198, 255, 209)]
        Missing_Spaces = (1 << 4),

        [ErrorColor(198, 230, 209)]
        Redundant_Spaces = (1 << 5),

        [ErrorColor(232, 221, 248)]
        ASSA_Tags = (1 << 6),

        [ErrorColor(172, 201, 230)]
        Punctuations_Error = (1 << 7),

        [ErrorColor(196, 198, 239)]
        Notes_Error = (1 << 8),

        [ErrorColor(255, 229, 204)]
        Dialog_Error = (1 << 9),

        [ErrorColor(255, 146, 146)]
        Non_Ansi_Chars = (1 << 10),

        [ErrorColor(255, 134, 134)]
        Encoded_HTML = (1 << 11),

        [ErrorColor(255, 105, 105, 255, 255, 255)]
        Malformed_Letters = (1 << 12),

        [ErrorColor(255, 105, 105, 255, 255, 255)]
        Contractions_Error = (1 << 13),

        [ErrorColor(255, 88, 88, 255, 255, 255)]
        Accent_Letters = (1 << 14),

        [ErrorColor(255, 70, 70, 255, 255, 255)]
        I_And_L_Error = (1 << 15),

        [ErrorColor(255, 49, 49, 255, 255, 255)]
        Merged_Words_Error = (1 << 16),

        [ErrorColor(255, 26, 26, 255, 255, 255)]
        O_And_0_Error = (1 << 17),

        [ErrorColor(255, 0, 0, 255, 255, 255)]
        OCR_Error = (1 << 18),

        [ErrorColor(255, 255, 153)]
        Hearing_Impaired = (1 << 19)
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
