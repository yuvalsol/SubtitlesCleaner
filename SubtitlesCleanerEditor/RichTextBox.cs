using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace SubtitlesCleanerEditor
{
    public class RichTextBox : System.Windows.Forms.RichTextBox
    {
        [Browsable(true)]
        [Category("Focus")]
        [Description("Event raised when the control is no longer the active control of the form and the value of the Text property is changed on Control.")]
        public event EventHandler<LeaveWithChangedTextEventArgs> LeaveWithChangedText;

        private DataGridView lstEditor;

        public void Init(DataGridView lstEditor)
        {
            this.lstEditor = lstEditor;
        }

        private EditorRow editorRow;
        private string textOnEnter;

        protected override void OnEnter(EventArgs e)
        {
            editorRow = (lstEditor.SelectedRows != null && lstEditor.SelectedRows.Count > 0 ? lstEditor.SelectedRows[0] : null)?.DataBoundItem as EditorRow;
            textOnEnter = this.Text;
            base.OnEnter(e);
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
            OnLeaveWithChangedText(e);
        }

        protected virtual void OnLeaveWithChangedText(EventArgs e)
        {
            if (textOnEnter != this.Text && editorRow != null)
                LeaveWithChangedText?.Invoke(this, new LeaveWithChangedTextEventArgs(editorRow, this.Text));
        }
    }

    public class LeaveWithChangedTextEventArgs : EventArgs
    {
        public LeaveWithChangedTextEventArgs(EditorRow editorRow, string text)
        {
            this.EditorRow = editorRow;
            this.Text = text;
        }

        public EditorRow EditorRow { get; private set; }
        public string Text { get; private set; }
    }
}
