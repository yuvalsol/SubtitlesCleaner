using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SubtitlesCleanerLibrary;

namespace SubtitlesCleanerEditor
{
    public partial class TimePicker : UserControl
    {
        public TimePicker()
        {
            InitializeComponent();
        }

        private bool readOnly;
        [Description("Read Only"), Category("Behavoir"), DefaultValue(false)]
        public bool ReadOnly
        {
            get
            {
                return readOnly;
            }

            set
            {
                readOnly = value;

                if (readOnly)
                {
                    lblSign.Click -= lblSign_Click;
                    lblSign.Cursor = Cursors.Default;
                    toolTip.SetToolTip(lblSign, null);
                }
                else
                {
                    lblSign.Click -= lblSign_Click;
                    lblSign.Click += lblSign_Click;
                    lblSign.Cursor = Cursors.Hand;
                    toolTip.SetToolTip(lblSign, "Change Sign");
                }

                numericUpDownHH.ReadOnly = readOnly;
                numericUpDownMM.ReadOnly = readOnly;
                numericUpDownSS.ReadOnly = readOnly;
                numericUpDownMS.ReadOnly = readOnly;

                btnPlus.Visible = !readOnly;
                btnMinus.Visible = !readOnly;
                btnReset.Visible = !readOnly;
                btnPaste.Visible = !readOnly;
            }
        }

        [Description("Show Sign"), Category("Data"), DefaultValue(false)]
        public bool ShowSign
        {
            get { return lblSign.Visible; }
            set { lblSign.Visible = value; }
        }

        public string Sign
        {
            get { return ShowSign ? lblSign.Text : string.Empty; }
            private set { if (ShowSign) lblSign.Text = value; }
        }

        public bool IsNegativeTime { get { return ShowSign && Sign == "-"; } }

        public void SwitchSign()
        {
            if (ShowSign)
                Sign = (IsNegativeTime ? "+" : "-");
        }

        public int HH { get { return decimal.ToInt32(numericUpDownHH.Value); } set { numericUpDownHH.Value = value; } }
        public int MM { get { return decimal.ToInt32(numericUpDownMM.Value); } set { numericUpDownMM.Value = value; } }
        public int SS { get { return decimal.ToInt32(numericUpDownSS.Value); } set { numericUpDownSS.Value = value; } }
        public int MS { get { return decimal.ToInt32(numericUpDownMS.Value); } set { numericUpDownMS.Value = value; } }

        public override string ToString()
        {
            return string.Format("{0}{1:D2}:{2:D2}:{3:D2},{4:D3}", (IsNegativeTime ? Sign : string.Empty), HH, MM, SS, MS);
        }

        public void Reset()
        {
            Sign = "+";
            HH = 0;
            MM = 0;
            SS = 0;
            MS = 0;
        }

        public DateTime Value
        {
            get
            {
                return new DateTime(1900, 1, 1, HH, MM, SS, MS);
            }

            set
            {
                HH = value.Hour;
                MM = value.Minute;
                SS = value.Second;
                MS = value.Millisecond;
            }
        }

        public TimeSpan DiffValue
        {
            get
            {
                var span = new TimeSpan(0, HH, MM, SS, MS);
                if (IsNegativeTime)
                    return span.Negate();
                else
                    return span;
            }

            set
            {
                bool isNegative = value < TimeSpan.Zero;
                Sign = (isNegative ? "-" : "+");
                HH = (isNegative ? -value.Hours : value.Hours);
                MM = (isNegative ? -value.Minutes : value.Minutes);
                SS = (isNegative ? -value.Seconds : value.Seconds);
                MS = (isNegative ? -value.Milliseconds : value.Milliseconds);
            }
        }

        public event EventHandler ValueChanged;

        private void lblSign_Click(object sender, EventArgs e)
        {
            SwitchSign();

            var handler = this.ValueChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        public static TimePicker operator +(TimePicker timePicker, int milliseconds)
        {
            timePicker.Value = timePicker.Value.AddMilliseconds(milliseconds);
            return timePicker;
        }

        public static TimePicker operator -(TimePicker timePicker, int milliseconds)
        {
            timePicker.Value = timePicker.Value.AddMilliseconds(-milliseconds);
            return timePicker;
        }

        private void numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (MS == -1)
            {
                MS = 999;
                SS -= 1;
            }
            else if (MS == 1000)
            {
                MS = 0;
                SS += 1;
            }

            if (SS == -1)
            {
                SS = 59;
                MM -= 1;
            }
            else if (SS == 60)
            {
                SS = 0;
                MM += 1;
            }

            if (MM == -1)
            {
                MM = 59;
                HH -= 1;
            }
            else if (MM == 60)
            {
                MM = 0;
                HH += 1;
            }

            if (HH == -1)
            {
                HH = 23;
            }
            else if (HH == 24)
            {
                HH = 0;
            }

            var handler = this.ValueChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        bool selectByMouse = false;

        private void numericUpDown_Enter(object sender, EventArgs e)
        {
            NumericUpDown curBox = sender as NumericUpDown;
            curBox.Select();
            curBox.Select(0, curBox.Text.Length);
            if (MouseButtons == MouseButtons.Left)
                selectByMouse = true;
        }

        private void numericUpDown_MouseDown(object sender, MouseEventArgs e)
        {
            NumericUpDown curBox = sender as NumericUpDown;
            if (selectByMouse)
            {
                curBox.Select(0, curBox.Text.Length);
                selectByMouse = false;
            }
        }

        private const int MillisecondsInterval = 50;

        public event EventHandler<int> MillisecondsAdded;

        private void btnPlus_Depressed(object sender, EventArgs e)
        {
            this.Value = this.Value.AddMilliseconds(MillisecondsInterval);

            var handler = this.MillisecondsAdded;
            if (handler != null)
                handler(this, MillisecondsInterval);
        }

        private void btnMinus_Depressed(object sender, EventArgs e)
        {
            this.Value = this.Value.AddMilliseconds(-MillisecondsInterval);

            var handler = this.MillisecondsAdded;
            if (handler != null)
                handler(this, -MillisecondsInterval);
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(this.ToString());
            }
            catch
            {
            }
        }

        private void btnPaste_Click(object sender, EventArgs e)
        {
            try
            {
                if (ShowSign)
                {
                    TimeSpan span = SubtitlesHelper.ParseDiffTime(Clipboard.GetText());
                    if (span != TimeSpan.Zero)
                    {
                        this.DiffValue = span;
                    }
                    else
                    {
                        DateTime time = SubtitlesHelper.ParseShowTime(Clipboard.GetText());
                        if (time != DateTime.MinValue)
                            this.Value = time;
                    }
                }
                else
                {
                    DateTime time = SubtitlesHelper.ParseShowTime(Clipboard.GetText());
                    if (time != DateTime.MinValue)
                    {
                        this.Value = time;
                    }
                    else
                    {
                        TimeSpan span = SubtitlesHelper.ParseDiffTime(Clipboard.GetText());
                        if (span != TimeSpan.Zero)
                        {
                            if (span >= TimeSpan.Zero)
                                this.DiffValue = span;
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Reset();
        }
    }
}
