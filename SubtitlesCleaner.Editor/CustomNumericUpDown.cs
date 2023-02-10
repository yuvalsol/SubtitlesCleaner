using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SubtitlesCleaner.Editor
{
    public partial class CustomNumericUpDown : NumericUpDown
    {
        public CustomNumericUpDown()
        {
            InitializeComponent();
        }

        protected string format;

        [Category("Data"), Description("The string representation of the numeric value.")]
        public string Format
        {
            get
            {
                return this.format;
            }
            set
            {
                if (this.format != value)
                {
                    this.format = value;
                    this.Invalidate();
                }
            }
        }

        protected override void UpdateEditText()
        {
            if (string.IsNullOrEmpty(this.Format) == false)
                this.Text = this.Value.ToString(this.Format);
        }
    }
}
