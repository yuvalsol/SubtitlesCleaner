using System;
using System.Drawing;
using System.Windows.Forms;

namespace SubtitlesCleaner.Editor
{
    internal static class MessageBoxHelper
    {
        public static DialogResult Show(string text, string caption, MessageBoxIcon icon, Color? foreColor = null)
        {
            return Show(null, text, caption, icon, foreColor);
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxIcon icon, Color? foreColor = null)
        {
            return CustomMessageBox.Show(
                owner, text, caption, CustomMessageBoxButtons.OK | CustomMessageBoxButtons.Copy, icon,
                GetAppearance(foreColor: foreColor)
            );
        }

        public static DialogResult ShowWithOKBtn(IWin32Window owner, string text, string caption, MessageBoxIcon icon, Color? foreColor = null)
        {
            return CustomMessageBox.Show(
                owner, text, caption, CustomMessageBoxButtons.OK, icon,
                GetAppearance(foreColor: foreColor, textAlign: ContentAlignment.MiddleLeft)
            );
        }

        private static CustomAppearance GetAppearance(Color? foreColor = null, ContentAlignment? textAlign = null)
        {
            return new CustomAppearance(
                foreColor: foreColor,
                textAlign: textAlign,
                buttonsAppearance: new CustomButtonAppearance(
                     buttonsPanelBackColor: Color.FromArgb(238, 244, 249),
                     backColor: Color.White,
                     foreColor: Color.Black
                )
            );
        }
    }
}
