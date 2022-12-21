using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SubtitlesCleanerEditor
{
    public partial class TimeCalculatorForm : Form
    {
        public TimeCalculatorForm()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public DateTime DiffValue1
        {
            get { return timePicker1.Value; }
            set { timePicker1.Value = value; }
        }

        public DateTime DiffValue2
        {
            get { return timePicker2.Value; }
            set { timePicker2.Value = value; }
        }

        public DateTime DiffValue3
        {
            get { return timePicker3.Value; }
            set { timePicker3.Value = value; }
        }

        public TimeSpan DiffValue4
        {
            get { return timePicker4.DiffValue; }
        }

        private void timePicker_ValueChanged(object sender, EventArgs e)
        {
            OnValueChanged();
        }

        private void OnValueChanged()
        {
            timePicker4.DiffValue = timePicker1.DiffValue - timePicker2.DiffValue + timePicker3.DiffValue;
        }

        private void lblSwitch1And2_Click(object sender, EventArgs e)
        {
            var diffValue1 = timePicker1.DiffValue;
            timePicker1.DiffValue = timePicker2.DiffValue;
            timePicker2.DiffValue = diffValue1;
        }

        private void lblSwitch2And3_Click(object sender, EventArgs e)
        {
            var diffValue2 = timePicker2.DiffValue;
            timePicker2.DiffValue = timePicker3.DiffValue;
            timePicker3.DiffValue = diffValue2;
        }
    }
}
