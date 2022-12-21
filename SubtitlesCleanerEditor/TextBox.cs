using System;
using System.Windows.Forms;

namespace SubtitlesCleanerEditor
{
    public class TextBox : System.Windows.Forms.TextBox
    {
        public event EventHandler<Tuple<EditorRow, string>> LeaveWithChangedText;

        private DataGridView lstEditor;

        public void Init(DataGridView lstEditor)
        {
            this.lstEditor = lstEditor;
        }

        private EditorRow editorRow;
        private string textOnEnter;
        private bool textChanged;

        protected override void OnEnter(EventArgs e)
        {
            editorRow = lstEditor.SelectedRows[0].DataBoundItem as EditorRow;
            textOnEnter = this.Text;
            textChanged = false;
            base.OnEnter(e);
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
            if (textChanged)
                OnLeaveWithChangedText(e);
        }

        protected virtual void OnLeaveWithChangedText(EventArgs e)
        {
            if (LeaveWithChangedText != null)
                LeaveWithChangedText(this, new Tuple<EditorRow, string>(editorRow, this.Text));
        }

        protected override void OnTextChanged(EventArgs e)
        {
            textChanged = (textOnEnter != this.Text);
            base.OnTextChanged(e);
        }
    }
}
