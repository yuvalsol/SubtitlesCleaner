using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SubtitlesCL;

namespace SubtitlesEditor
{
    public partial class TimePicker : UserControl
    {
        public TimePicker()
        {
            InitializeComponent();
        }

        public int HH { get { return decimal.ToInt32(numericUpDownHH.Value); } set { numericUpDownHH.Value = value; } }
        public int MM { get { return decimal.ToInt32(numericUpDownMM.Value); } set { numericUpDownMM.Value = value; } }
        public int SS { get { return decimal.ToInt32(numericUpDownSS.Value); } set { numericUpDownSS.Value = value; } }
        public int MS { get { return decimal.ToInt32(numericUpDownMS.Value); } set { numericUpDownMS.Value = value; } }

        public override string ToString()
        {
            return string.Format("{0:D2}:{1:D2}:{2:D2},{3:D3}", HH, MM, SS, MS);
        }

        public void Reset()
        {
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

        public event EventHandler ValueChanged;

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
                DateTime time = SubtitlesHelper.ParseShowTime(Clipboard.GetText());
                if (time != DateTime.MinValue)
                    this.Value = time;
            }
            catch
            {
            }
        }
    }
}
