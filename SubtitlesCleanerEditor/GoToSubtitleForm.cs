using System;
using System.Windows.Forms;

namespace SubtitlesCleanerEditor
{
    public partial class GoToSubtitleForm : Form
    {
        public GoToSubtitleForm()
        {
            InitializeComponent();
        }

        private void GoToSubtitleForm_Load(object sender, EventArgs e)
        {
            txtOnLoad = txtSubtitleNumber.Text;
            txtSubtitleNumber.SelectAll();
            txtSubtitleNumber.Select();
        }

        private string txtOnLoad;

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
            txtSubtitleNumber.Text = txtOnLoad;
        }

        public int GetSubtitleNumber()
        {
            int subtitleNumber;
            if (int.TryParse(txtSubtitleNumber.Text, out subtitleNumber))
                return subtitleNumber;
            return -1;
        }

        private void txtSubtitleNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar)) == false;
        }
    }
}
