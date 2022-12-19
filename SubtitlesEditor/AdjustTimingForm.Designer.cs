namespace SubtitlesEditor
{
    partial class AdjustTimingForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnClose = new System.Windows.Forms.Button();
            this.lblArrow1 = new System.Windows.Forms.Label();
            this.lblArrow2 = new System.Windows.Forms.Label();
            this.btnAdjust = new System.Windows.Forms.Button();
            this.timePickerY2 = new SubtitlesEditor.TimePicker();
            this.timePickerY1 = new SubtitlesEditor.TimePicker();
            this.timePickerX2 = new SubtitlesEditor.TimePicker();
            this.timePickerX1 = new SubtitlesEditor.TimePicker();
            this.btnLoadFromFile = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.AutoSize = true;
            this.btnClose.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(676, 107);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(45, 24);
            this.btnClose.TabIndex = 7;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblArrow1
            // 
            this.lblArrow1.AutoSize = true;
            this.lblArrow1.Location = new System.Drawing.Point(352, 18);
            this.lblArrow1.Name = "lblArrow1";
            this.lblArrow1.Size = new System.Drawing.Size(27, 14);
            this.lblArrow1.TabIndex = 5;
            this.lblArrow1.Text = "—>";
            // 
            // lblArrow2
            // 
            this.lblArrow2.AutoSize = true;
            this.lblArrow2.Location = new System.Drawing.Point(352, 65);
            this.lblArrow2.Name = "lblArrow2";
            this.lblArrow2.Size = new System.Drawing.Size(27, 14);
            this.lblArrow2.TabIndex = 6;
            this.lblArrow2.Text = "—>";
            // 
            // btnAdjust
            // 
            this.btnAdjust.AutoSize = true;
            this.btnAdjust.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnAdjust.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnAdjust.Location = new System.Drawing.Point(10, 107);
            this.btnAdjust.Name = "btnAdjust";
            this.btnAdjust.Size = new System.Drawing.Size(52, 24);
            this.btnAdjust.TabIndex = 5;
            this.btnAdjust.Text = "Adjust";
            this.btnAdjust.UseVisualStyleBackColor = true;
            // 
            // timePickerY2
            // 
            this.timePickerY2.AutoSize = true;
            this.timePickerY2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.timePickerY2.DiffValue = System.TimeSpan.Parse("00:00:00");
            this.timePickerY2.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.timePickerY2.HH = 0;
            this.timePickerY2.Location = new System.Drawing.Point(379, 60);
            this.timePickerY2.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.timePickerY2.MM = 0;
            this.timePickerY2.MS = 0;
            this.timePickerY2.Name = "timePickerY2";
            this.timePickerY2.Size = new System.Drawing.Size(342, 31);
            this.timePickerY2.SS = 0;
            this.timePickerY2.TabIndex = 4;
            this.timePickerY2.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            // 
            // timePickerY1
            // 
            this.timePickerY1.AutoSize = true;
            this.timePickerY1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.timePickerY1.DiffValue = System.TimeSpan.Parse("00:00:00");
            this.timePickerY1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.timePickerY1.HH = 0;
            this.timePickerY1.Location = new System.Drawing.Point(10, 60);
            this.timePickerY1.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.timePickerY1.MM = 0;
            this.timePickerY1.MS = 0;
            this.timePickerY1.Name = "timePickerY1";
            this.timePickerY1.Size = new System.Drawing.Size(342, 31);
            this.timePickerY1.SS = 0;
            this.timePickerY1.TabIndex = 3;
            this.timePickerY1.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            // 
            // timePickerX2
            // 
            this.timePickerX2.AutoSize = true;
            this.timePickerX2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.timePickerX2.DiffValue = System.TimeSpan.Parse("00:00:00");
            this.timePickerX2.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.timePickerX2.HH = 0;
            this.timePickerX2.Location = new System.Drawing.Point(379, 13);
            this.timePickerX2.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.timePickerX2.MM = 0;
            this.timePickerX2.MS = 0;
            this.timePickerX2.Name = "timePickerX2";
            this.timePickerX2.Size = new System.Drawing.Size(342, 31);
            this.timePickerX2.SS = 0;
            this.timePickerX2.TabIndex = 2;
            this.timePickerX2.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            // 
            // timePickerX1
            // 
            this.timePickerX1.AutoSize = true;
            this.timePickerX1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.timePickerX1.DiffValue = System.TimeSpan.Parse("00:00:00");
            this.timePickerX1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.timePickerX1.HH = 0;
            this.timePickerX1.Location = new System.Drawing.Point(10, 13);
            this.timePickerX1.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.timePickerX1.MM = 0;
            this.timePickerX1.MS = 0;
            this.timePickerX1.Name = "timePickerX1";
            this.timePickerX1.Size = new System.Drawing.Size(342, 31);
            this.timePickerX1.SS = 0;
            this.timePickerX1.TabIndex = 1;
            this.timePickerX1.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            // 
            // btnLoadFromFile
            // 
            this.btnLoadFromFile.AutoSize = true;
            this.btnLoadFromFile.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnLoadFromFile.Location = new System.Drawing.Point(78, 107);
            this.btnLoadFromFile.Name = "btnLoadFromFile";
            this.btnLoadFromFile.Size = new System.Drawing.Size(95, 24);
            this.btnLoadFromFile.TabIndex = 6;
            this.btnLoadFromFile.Text = "Load From File";
            this.btnLoadFromFile.UseVisualStyleBackColor = true;
            this.btnLoadFromFile.Click += new System.EventHandler(this.btnLoadFromFile_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "SubRip files (*.srt)|*.srt";
            // 
            // AdjustTimingForm
            // 
            this.AcceptButton = this.btnAdjust;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(729, 142);
            this.Controls.Add(this.btnLoadFromFile);
            this.Controls.Add(this.btnAdjust);
            this.Controls.Add(this.lblArrow2);
            this.Controls.Add(this.lblArrow1);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.timePickerY2);
            this.Controls.Add(this.timePickerY1);
            this.Controls.Add(this.timePickerX2);
            this.Controls.Add(this.timePickerX1);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AdjustTimingForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Adjust Timing";
            this.Load += new System.EventHandler(this.AdjustForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TimePicker timePickerX1;
        private TimePicker timePickerX2;
        private TimePicker timePickerY1;
        private TimePicker timePickerY2;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblArrow1;
        private System.Windows.Forms.Label lblArrow2;
        private System.Windows.Forms.Button btnAdjust;
        private System.Windows.Forms.Button btnLoadFromFile;
        private System.Windows.Forms.OpenFileDialog openFileDialog;

    }
}