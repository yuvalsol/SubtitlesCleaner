﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using SubtitlesCL;

namespace SubtitlesEditor
{
    public partial class AdjustForm : Form
    {
        public AdjustForm(string initialDirectory, DateTime firstShow, DateTime lastShow)
        {
            InitializeComponent();

            timePickerX1.Value = timePickerX2.Value = firstShow;
            timePickerY1.Value = timePickerY2.Value = lastShow;

            openFileDialog.InitialDirectory = initialDirectory;
        }

        public DateTime X1 { get { return timePickerX1.Value; } }
        public DateTime X2 { get { return timePickerX2.Value; } }
        public DateTime Y1 { get { return timePickerY1.Value; } }
        public DateTime Y2 { get { return timePickerY2.Value; } }

        private void AdjustForm_Load(object sender, EventArgs e)
        {
            timePickerX2.Select();
        }

        private void btnLoadFromFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                if (string.IsNullOrEmpty(filePath) == false)
                {
                    string extension = Path.GetExtension(filePath);
                    bool isSRT = string.Compare(extension, ".srt", true) == 0;
                    if (isSRT)
                    {
                        List<Subtitle> subtitles = SubtitlesHelper.GetSubtitles(filePath);

                        if (subtitles != null && subtitles.Count > 0)
                        {
                            timePickerX2.Value = subtitles[0].Show;

                            if (subtitles.Count > 1)
                                timePickerY2.Value = subtitles[subtitles.Count - 1].Show;
                        }
                    }
                }
            }
        }
    }
}
