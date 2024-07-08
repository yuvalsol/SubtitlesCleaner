using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Media;

namespace System.Windows.Forms
{
    public partial class CustomMessageBox : Form
    {
        public bool Quiet { get; set; }

        private CustomMessageBox() : base()
        {
            InitializeComponent();
            this.BackColor = lblMessage.BackColor;
        }

        private CustomMessageBox(bool quiet) : this()
        {
            this.Quiet = quiet;
        }

        private void CustomMessageBox_Shown(object sender, EventArgs e)
        {
            if (this.Quiet == false)
                SystemSounds.Beep.Play();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            else if ((keyData & ~(Keys.ControlKey | Keys.C)) == Keys.None)
            {
                CopyToClipboard();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void CopyToClipboard()
        {
            try
            {
                Clipboard.SetText((this.Text + Environment.NewLine + Environment.NewLine + lblMessage.Text).Trim());
                if (this.Quiet == false)
                    SystemSounds.Hand.Play();
            }
            catch { }
        }

        public static DialogResult Show(
            string text,
            string caption = null,
            CustomMessageBoxButtons buttons = CustomMessageBoxButtons.None,
            MessageBoxIcon icon = MessageBoxIcon.None,
            CustomAppearance appearance = null,
            CustomButton[] customButtons = null,
            CustomButtonText[] buttonTexts = null,
            bool quiet = false)
        {
            return Show(null, text, caption, buttons, icon, appearance, customButtons, buttonTexts, quiet);
        }

        public static DialogResult Show(
            IWin32Window owner,
            string text,
            string caption = null,
            CustomMessageBoxButtons buttons = CustomMessageBoxButtons.None,
            MessageBoxIcon icon = MessageBoxIcon.None,
            CustomAppearance appearance = null,
            CustomButton[] customButtons = null,
            CustomButtonText[] buttonTexts = null,
            bool quiet = false)
        {
            var messageBox = new CustomMessageBox(quiet);
            messageBox.Init(text, caption, buttons, icon, appearance, customButtons, buttonTexts);

            if (owner != null)
                return messageBox.ShowDialog(owner);
            else
                return messageBox.ShowDialog();
        }

        private void Init(
            string text,
            string caption = null,
            CustomMessageBoxButtons buttons = CustomMessageBoxButtons.None,
            MessageBoxIcon icon = MessageBoxIcon.None,
            CustomAppearance appearance = null,
            CustomButton[] customButtons = null,
            CustomButtonText[] buttonTexts = null)
        {
            int lblW = lblMessage.Size.Width;
            int lblH = lblMessage.Size.Height;

            lblMessage.Text = text;

            int lblNewW = lblMessage.Size.Width;
            int lblNewH = lblMessage.Size.Height;

            if (lblNewW > lblW)
            {
                this.Width += lblNewW - lblW;
            }

            if (lblNewH > lblH)
            {
                using (var g = this.CreateGraphics())
                {
                    int lineSpacing = lblMessage.Font.FontFamily.GetLineSpacing(lblMessage.Font.Style);
                    float lineSpacingPixel = lblMessage.Font.Size * lineSpacing / lblMessage.Font.FontFamily.GetEmHeight(lblMessage.Font.Style);
                    this.Height += (int)(lblNewH - lblH + lineSpacingPixel);
                }
            }

            this.Text = caption;

            if (icon == MessageBoxIcon.None)
            {
                int iconX = pbxIcon.Location.X;
                int diffX = lblMessage.Location.X - iconX;
                pbxIcon.Hide();
                lblMessage.Location = new Point(iconX, lblMessage.Location.Y);
                lblMessage.Size = new Size(lblMessage.Size.Width + diffX, lblMessage.Size.Height);
            }
            else if (icon == MessageBoxIcon.Error || icon == MessageBoxIcon.Stop || icon == MessageBoxIcon.Hand)
            {
                pbxIcon.Image = SystemIcons.Error.ToBitmap();
            }
            else if (icon == MessageBoxIcon.Question)
            {
                pbxIcon.Image = SystemIcons.Question.ToBitmap();
            }
            else if (icon == MessageBoxIcon.Exclamation || icon == MessageBoxIcon.Warning)
            {
                pbxIcon.Image = SystemIcons.Exclamation.ToBitmap();
            }
            else if (icon == MessageBoxIcon.Information || icon == MessageBoxIcon.Asterisk)
            {
                pbxIcon.Image = SystemIcons.Information.ToBitmap();
            }

            List<Button> btns = null;

            bool hasCustomButtons = customButtons != null && customButtons.Length > 0;

            if (buttons == CustomMessageBoxButtons.None && hasCustomButtons == false)
            {
                int pnlH = pnlButtons.Height;
                lblMessage.Size = new Size(lblMessage.Size.Width, lblMessage.Size.Height + pnlH);
                pnlButtons.Hide();
                this.Height -= pnlH;
            }
            else
            {
                btns = new List<Button>();

                AddButton(btns, buttons, CustomMessageBoxButtons.OK, DialogResult.OK, "OK", buttonTexts);
                AddButton(btns, buttons, CustomMessageBoxButtons.Abort, DialogResult.Abort, "Abort", buttonTexts);
                AddButton(btns, buttons, CustomMessageBoxButtons.Retry, DialogResult.Retry, "Retry", buttonTexts);
                AddButton(btns, buttons, CustomMessageBoxButtons.Ignore, DialogResult.Ignore, "Ignore", buttonTexts);
                AddButton(btns, buttons, CustomMessageBoxButtons.Yes, DialogResult.Yes, "Yes", buttonTexts);
                AddButton(btns, buttons, CustomMessageBoxButtons.No, DialogResult.No, "No", buttonTexts);
                AddButton(btns, buttons, CustomMessageBoxButtons.Cancel, DialogResult.Cancel, "Cancel", buttonTexts);

                AddButton(btns, buttons, CustomMessageBoxButtons.Copy, DialogResult.None, "Copy", buttonTexts, (object sender, CustomMessageBoxEventArgs e) =>
                {
                    CopyToClipboard();
                });

                if (hasCustomButtons)
                {
                    foreach (var customButton in customButtons)
                    {
                        if (string.IsNullOrEmpty(customButton.Text) == false)
                            btns.Add(GetButton(customButton.Text, customButton.DialogResult ?? DialogResult.None, customButton.Click));
                    }
                }

                int tabIndex = 1;
                foreach (var btn in btns)
                    btn.TabIndex = tabIndex++;

                btns.Reverse();
                pnlButtons.Controls.AddRange(btns.ToArray());
            }

            if (appearance == null)
                return;

            if (appearance.ForeColor != null)
                this.ForeColor = lblMessage.ForeColor = appearance.ForeColor.Value;

            if (appearance.BackColor != null)
                this.BackColor = lblMessage.BackColor = appearance.BackColor.Value;

            if (appearance.Font != null)
                this.Font = lblMessage.Font = new Font(appearance.Font, FontStyle.Regular);

            if ((string.IsNullOrEmpty(appearance.FontFamilyName) &&
                appearance.FontSize == null &&
                appearance.FontStyle == null) == false)
            {
                this.Font = lblMessage.Font = new Font
                (
                    string.IsNullOrEmpty(appearance.FontFamilyName) ? this.Font.FontFamily.Name : appearance.FontFamilyName,
                    appearance.FontSize == null ? this.Font.Size : appearance.FontSize.Value,
                    appearance.FontStyle == null ? this.Font.Style : appearance.FontStyle.Value
                );
            }

            if (appearance.TextAlign != null)
            {
                lblMessage.TextAlign = appearance.TextAlign.Value;

                if (lblMessage.TextAlign == ContentAlignment.MiddleLeft ||
                    lblMessage.TextAlign == ContentAlignment.MiddleCenter ||
                    lblMessage.TextAlign == ContentAlignment.MiddleRight)
                {
                    pbxIcon.Location = new Point(
                        pbxIcon.Location.X,
                        ((2 * lblMessage.Location.Y) + lblMessage.Height - pbxIcon.Height) / 2
                    );
                }
                else if (lblMessage.TextAlign == ContentAlignment.BottomLeft ||
                    lblMessage.TextAlign == ContentAlignment.BottomCenter ||
                    lblMessage.TextAlign == ContentAlignment.BottomRight)
                {
                    pbxIcon.Location = new Point(
                        pbxIcon.Location.X,
                        lblMessage.Location.Y + lblMessage.Height - pbxIcon.Height
                    );
                }
            }

            if (appearance.RightToLeft == true)
            {
                lblMessage.RightToLeft = RightToLeft.Yes;

                if (icon != MessageBoxIcon.None)
                {
                    int iconX = pbxIcon.Location.X;
                    int lblX1 = lblMessage.Location.X;
                    int lblX2 = lblX1 + lblMessage.Width;
                    int iconNewX = lblX2 - pbxIcon.Width;
                    int lblNewX = iconX;

                    pbxIcon.Location = new Point(iconNewX, pbxIcon.Location.Y);
                    lblMessage.Location = new Point(lblNewX, lblMessage.Location.Y);
                }

                pnlButtons.RightToLeft = RightToLeft.No;

                if (btns != null && btns.Count > 0)
                {
                    foreach (var btn in btns)
                        btn.RightToLeft = RightToLeft.Yes;
                }
            }

            if (appearance.ButtonsAppearance == null)
                return;

            if (appearance.ButtonsAppearance.ButtonsPanelBackColor != null)
                pnlButtons.BackColor = appearance.ButtonsAppearance.ButtonsPanelBackColor.Value;

            if (btns == null || btns.Count == 0)
                return;

            foreach (var btn in btns)
            {
                if (appearance.ButtonsAppearance.ForeColor != null)
                    btn.ForeColor = appearance.ButtonsAppearance.ForeColor.Value;

                if (appearance.ButtonsAppearance.BackColor != null)
                    btn.BackColor = appearance.ButtonsAppearance.BackColor.Value;

                if (appearance.ButtonsAppearance.Font != null)
                    btn.Font = new Font(appearance.ButtonsAppearance.Font, FontStyle.Regular);

                if ((string.IsNullOrEmpty(appearance.ButtonsAppearance.FontFamilyName) &&
                    appearance.ButtonsAppearance.FontSize == null &&
                    appearance.ButtonsAppearance.FontStyle == null) == false)
                {
                    btn.Font = new Font
                    (
                        string.IsNullOrEmpty(appearance.ButtonsAppearance.FontFamilyName) ? btn.Font.FontFamily.Name : appearance.ButtonsAppearance.FontFamilyName,
                        appearance.ButtonsAppearance.FontSize == null ? btn.Font.Size : appearance.ButtonsAppearance.FontSize.Value,
                        appearance.ButtonsAppearance.FontStyle == null ? btn.Font.Style : appearance.ButtonsAppearance.FontStyle.Value
                    );
                }
            }
        }

        private void AddButton(
            List<Button> btns,
            CustomMessageBoxButtons buttons,
            CustomMessageBoxButtons button,
            DialogResult dialogResult,
            string text,
            CustomButtonText[] buttonTexts,
            CustomMessageBoxEventHandler click = null)
        {
            if ((buttons & button) == button)
            {
                var newText = buttonTexts?.FirstOrDefault(x => x.Button == button)?.Text;
                if (string.IsNullOrEmpty(newText) == false)
                    text = newText;

                btns.Add(GetButton(text, dialogResult, click));
            }
        }

        private Button GetButton(
            string text,
            DialogResult dialogResult = DialogResult.None,
            CustomMessageBoxEventHandler click = null)
        {
            Button btn = new Button()
            {
                Text = text,
                Size = new Size(90, 40),
                DialogResult = dialogResult
            };

            if (click != null)
            {
                btn.Click += (object sender, EventArgs e) =>
                {
                    foreach (CustomMessageBoxEventHandler listener in click.GetInvocationList())
                        listener.Invoke(sender, new CustomMessageBoxEventArgs(lblMessage.Text, this.Text));
                };
            }

            return btn;
        }
    }

    #region CustomMessageBoxButtons

    [Flags]
    public enum CustomMessageBoxButtons
    {
        None = 0,
        OK = 1,
        Abort = 1 << 1,
        Retry = 1 << 2,
        Ignore = 1 << 3,
        Yes = 1 << 4,
        No = 1 << 5,
        Cancel = 1 << 6,
        Copy = 1 << 7,
        OKCancel = OK | Cancel,
        AbortRetryIgnore = Abort | Retry | Ignore,
        YesNoCancel = Yes | No | Cancel,
        YesNo = Yes | No,
        RetryCancel = Retry | Cancel,
    }

    #endregion

    #region CustomMessageBoxEventHandler

    public delegate void CustomMessageBoxEventHandler(object sender, CustomMessageBoxEventArgs e);

    public class CustomMessageBoxEventArgs : EventArgs
    {
        public string Text { get; private set; }
        public string Caption { get; private set; }

        public CustomMessageBoxEventArgs(string text, string caption) : base()
        {
            Text = text;
            Caption = caption;
        }

        public new static readonly CustomMessageBoxEventArgs Empty = new CustomMessageBoxEventArgs(null, null);
    }

    #endregion

    #region CustomAppearance

    public class CustomAppearance : ICloneable
    {
        public Color? ForeColor { get; set; }
        public Color? BackColor { get; set; }
        public Font Font { get; set; }
        public string FontFamilyName { get; set; }
        public float? FontSize { get; set; }
        public FontStyle? FontStyle { get; set; }
        public ContentAlignment? TextAlign { get; set; }
        public bool? RightToLeft { get; set; }
        public CustomButtonAppearance ButtonsAppearance { get; set; }

        public CustomAppearance(
            Color? foreColor = null,
            Color? backColor = null,
            Font font = null,
            string fontFamilyName = null,
            float? fontSize = null,
            FontStyle? fontStyle = null,
            ContentAlignment? textAlign = null,
            bool? rightToLeft = null,
            CustomButtonAppearance buttonsAppearance = null)
        {
            ForeColor = foreColor;
            BackColor = backColor;
            Font = font;
            FontFamilyName = fontFamilyName;
            FontSize = fontSize;
            FontStyle = fontStyle;
            TextAlign = textAlign;
            RightToLeft = rightToLeft;
            ButtonsAppearance = buttonsAppearance;
        }

        public CustomAppearance(CustomAppearance prototype) : this(
            prototype.ForeColor,
            prototype.BackColor,
            (prototype.Font != null ? (Font)prototype.Font.Clone() : null),
            prototype.FontFamilyName,
            prototype.FontSize,
            prototype.FontStyle,
            prototype.TextAlign,
            prototype.RightToLeft,
            (prototype.ButtonsAppearance != null ? (CustomButtonAppearance)prototype.ButtonsAppearance.Clone() : null))
        { }

        public object Clone()
        {
            return new CustomAppearance(this);
        }
    }

    #endregion

    #region CustomButtonAppearance

    public class CustomButtonAppearance : ICloneable
    {
        public Color? ButtonsPanelBackColor { get; set; }
        public Color? ForeColor { get; set; }
        public Color? BackColor { get; set; }
        public Font Font { get; set; }
        public string FontFamilyName { get; set; }
        public float? FontSize { get; set; }
        public FontStyle? FontStyle { get; set; }

        public CustomButtonAppearance(
            Color? buttonsPanelBackColor = null,
            Color? foreColor = null,
            Color? backColor = null,
            Font font = null,
            string fontFamilyName = null,
            float? fontSize = null,
            FontStyle? fontStyle = null)
        {
            ButtonsPanelBackColor = buttonsPanelBackColor;
            ForeColor = foreColor;
            BackColor = backColor;
            Font = font;
            FontFamilyName = fontFamilyName;
            FontSize = fontSize;
            FontStyle = fontStyle;
        }

        public CustomButtonAppearance(CustomButtonAppearance prototype) : this(
            prototype.ButtonsPanelBackColor,
            prototype.ForeColor,
            prototype.BackColor,
            (prototype.Font != null ? (Font)prototype.Font.Clone() : null),
            prototype.FontFamilyName,
            prototype.FontSize,
            prototype.FontStyle)
        { }

        public object Clone()
        {
            return new CustomButtonAppearance(this);
        }
    }

    #endregion

    #region CustomButton

    public class CustomButton : ICloneable
    {
        public string Text { get; set; }
        public DialogResult? DialogResult { get; set; }
        public CustomMessageBoxEventHandler Click { get; set; }

        public CustomButton(
            string text,
            DialogResult? dialogResult = null,
            CustomMessageBoxEventHandler click = null)
        {
            Text = text;
            DialogResult = dialogResult;
            Click = click;
        }

        public CustomButton(CustomButton prototype) : this(
            prototype.Text,
            prototype.DialogResult,
            (prototype.Click != null ? (CustomMessageBoxEventHandler)prototype.Click.Clone() : null))
        { }

        public object Clone()
        {
            return new CustomButton(this);
        }
    }

    #endregion

    #region CustomButtonText

    public class CustomButtonText
    {
        public CustomMessageBoxButtons Button { get; private set; }
        public string Text { get; private set; }

        public CustomButtonText(CustomMessageBoxButtons button, string text)
        {
            if ((button == CustomMessageBoxButtons.OK ||
                button == CustomMessageBoxButtons.Abort ||
                button == CustomMessageBoxButtons.Retry ||
                button == CustomMessageBoxButtons.Ignore ||
                button == CustomMessageBoxButtons.Yes ||
                button == CustomMessageBoxButtons.No ||
                button == CustomMessageBoxButtons.Cancel ||
                button == CustomMessageBoxButtons.Copy) == false)
            {
                throw new ArgumentOutOfRangeException(nameof(button), "Valid values are OK, Abort, Retry, Ignore, Yes, No, Cancel, Copy");
            }

            Button = button;
            Text = text;
        }
    }

    #endregion
}