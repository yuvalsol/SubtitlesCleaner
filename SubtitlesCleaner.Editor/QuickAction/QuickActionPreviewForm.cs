using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SubtitlesCleaner.Editor
{
    public partial class QuickActionPreviewForm : Form
    {
        private string filePath;
        private string preview;
        private string action;
        private bool isPreviewChanges;

        public QuickActionPreviewForm(string filePath, string preview, string action) : this(filePath, preview, action, false) { }

        public QuickActionPreviewForm(string filePath, string preview) : this(filePath, preview, null, true) { }

        private QuickActionPreviewForm(string filePath, string preview, string action, bool isPreviewChanges)
        {
            InitializeComponent();

            this.filePath = filePath;
            this.preview = preview;
            this.action = action;
            this.isPreviewChanges = isPreviewChanges;

            this.Text = (isPreviewChanges ? "Preview Changes" : action) + " - " + this.Text;

            txtPreview.Text = preview;
            txtPreview.SelectionStart = 0;
            txtPreview.SelectionLength = 0;

            btnOK.Select();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(preview))
                    Clipboard.Clear();
                else
                    Clipboard.SetText(preview);
            }
            catch
            {
            }
        }

        private void btnSaveToFile_Click(object sender, EventArgs e)
        {
            string fileName = null;

            try
            {
                string actionCleanName = (isPreviewChanges ? "Preview Changes" : action.Replace("♪", string.Empty).Replace(".", string.Empty));
                actionCleanName = string.Concat(actionCleanName.Split(Path.GetInvalidFileNameChars()));
                actionCleanName = actionCleanName.Trim();

                fileName = Path.GetFileNameWithoutExtension(filePath) + " - " + actionCleanName + (isPreviewChanges ? ".srt" : ".txt");

                string path = Path.GetFullPath(Path.Combine(
                    Path.GetDirectoryName(filePath),
                    fileName
                ));

                File.WriteAllText(path, preview, Encoding.UTF8);

                MessageBoxHelper.ShowWithOKBtn(this, "File saved to" + Environment.NewLine + fileName, "File Saved", MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowWithOKBtn(this, "Failed to save file." + Environment.NewLine + fileName + Environment.NewLine + ex.Message, "Error", MessageBoxIcon.Error);
            }
        }
    }
}
