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
    public partial class RepeatButton : Button
    {
        public RepeatButton()
        {
            InitializeComponent();

            timer.Interval = 80;
            timer.Tick += delegate { OnDepressed(); };
        }

        private readonly Timer timer = new Timer();

        public event EventHandler Depressed;

        public virtual int Interval
        {
            get { return timer.Interval; }
            set { timer.Interval = value; }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            timer.Stop();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            timer.Start();
        }

        protected virtual void OnDepressed()
        {
            var handler = this.Depressed;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}
