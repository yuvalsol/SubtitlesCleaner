using System;
using System.Windows.Forms;

namespace SubtitlesCleanerEditor
{
    public partial class SearchAndReplaceForm : Form
    {
        public SearchAndReplaceForm()
        {
            InitializeComponent();
        }

        private void SearchAndReplaceForm_Load(object sender, EventArgs e)
        {
            txtSearch.Focus();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public Find GetFind()
        {
            return new Find()
            {
                Search = txtSearch.Text,
                MatchCase = chkMatchCase.Checked,
                MatchWholeWord = chkMatchWholeWord.Checked
            };
        }

        private FindAndReplace GetFindAndReplace()
        {
            return new FindAndReplace()
            {
                Search = txtSearch.Text,
                Replace = txtReplace.Text,
                MatchCase = chkMatchCase.Checked,
                MatchWholeWord = chkMatchWholeWord.Checked
            };
        }

        public event EventHandler<Find> FindNext;

        private void btnFindNext_Click(object sender, EventArgs e)
        {
            DoFindNext();
        }

        private void DoFindNext()
        {
            if (FindNext != null)
                FindNext(this, GetFind());
        }

        public event EventHandler<Find> FindPrevious;

        private void btnFindPrevious_Click(object sender, EventArgs e)
        {
            DoFindPrevious();
        }

        private void DoFindPrevious()
        {
            if (FindPrevious != null)
                FindPrevious(this, GetFind());
        }

        public event EventHandler<FindAndReplace> ReplaceNext;

        private void btnReplaceNext_Click(object sender, EventArgs e)
        {
            DoReplaceNext();
        }

        private void DoReplaceNext()
        {
            if (ReplaceNext != null)
                ReplaceNext(this, GetFindAndReplace());
        }

        public event EventHandler<FindAndReplace> ReplaceAll;

        private void btnReplaceAll_Click(object sender, EventArgs e)
        {
            DoReplaceAll();
        }

        private void DoReplaceAll()
        {
            if (ReplaceAll != null)
                ReplaceAll(this, GetFindAndReplace());
        }

        private void SearchAndReplaceForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Shift && e.KeyCode == Keys.F3)
                DoFindPrevious();
            else if (e.KeyCode == Keys.F3)
                DoFindNext();
            else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.R)
                DoReplaceNext();
            else if (e.Modifiers == Keys.Alt && e.KeyCode == Keys.A)
                DoReplaceAll();
        }

        private void txt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                DoFindNext();
                this.Close();
            }
        }
    }
}
