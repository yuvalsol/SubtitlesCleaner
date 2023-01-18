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

namespace SubtitlesCleanerEditor
{
    public partial class QuickActionPreviewForm : Form
    {
        private string filePath;
        private string action;
        private string preview;

        public QuickActionPreviewForm(string filePath, string action, string preview)
        {
            InitializeComponent();

            this.filePath = filePath;
            this.action = action;
            this.preview = preview;

            this.Text = action + " - " + this.Text;

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
                string actionCleanName = action.Replace("♪", string.Empty).Replace(".", string.Empty);
                actionCleanName = string.Concat(actionCleanName.Split(Path.GetInvalidFileNameChars()));
                actionCleanName = actionCleanName.Trim();

                fileName = Path.GetFileNameWithoutExtension(filePath) + " - " + actionCleanName + ".txt";

                string path = Path.GetFullPath(Path.Combine(
                    Path.GetDirectoryName(filePath),
                    fileName
                ));

                File.WriteAllText(path, preview, Encoding.UTF8);

                MessageBox.Show(this, "File saved to" + Environment.NewLine + fileName, "File Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Failed to save file." + Environment.NewLine + fileName + Environment.NewLine + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
