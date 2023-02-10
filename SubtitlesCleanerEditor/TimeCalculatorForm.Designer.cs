
namespace SubtitlesCleaner.Editor
{
    partial class TimeCalculatorForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblSwitch1And2 = new System.Windows.Forms.Label();
            this.lblSwitch2And3 = new System.Windows.Forms.Label();
            this.timePicker4 = new SubtitlesCleaner.Editor.TimePicker();
            this.timePicker3 = new SubtitlesCleaner.Editor.TimePicker();
            this.timePicker2 = new SubtitlesCleaner.Editor.TimePicker();
            this.timePicker1 = new SubtitlesCleaner.Editor.TimePicker();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 13F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(356, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(18, 22);
            this.label1.TabIndex = 0;
            this.label1.Text = "-";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 13F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(352, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(25, 22);
            this.label2.TabIndex = 0;
            this.label2.Text = "+";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 13F, System.Drawing.FontStyle.Bold);
            this.label3.Location = new System.Drawing.Point(352, 115);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(25, 22);
            this.label3.TabIndex = 0;
            this.label3.Text = "=";
            // 
            // btnClose
            // 
            this.btnClose.AutoSize = true;
            this.btnClose.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(10, 212);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(45, 24);
            this.btnClose.TabIndex = 7;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblSwitch1And2
            // 
            this.lblSwitch1And2.AutoSize = true;
            this.lblSwitch1And2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblSwitch1And2.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.lblSwitch1And2.Location = new System.Drawing.Point(149, 47);
            this.lblSwitch1And2.Name = "lblSwitch1And2";
            this.lblSwitch1And2.Size = new System.Drawing.Size(86, 13);
            this.lblSwitch1And2.TabIndex = 2;
            this.lblSwitch1And2.Text = "<= Switch =>";
            this.lblSwitch1And2.Click += new System.EventHandler(this.lblSwitch1And2_Click);
            // 
            // lblSwitch2And3
            // 
            this.lblSwitch2And3.AutoSize = true;
            this.lblSwitch2And3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblSwitch2And3.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.lblSwitch2And3.Location = new System.Drawing.Point(149, 98);
            this.lblSwitch2And3.Name = "lblSwitch2And3";
            this.lblSwitch2And3.Size = new System.Drawing.Size(86, 13);
            this.lblSwitch2And3.TabIndex = 4;
            this.lblSwitch2And3.Text = "<= Switch =>";
            this.lblSwitch2And3.Click += new System.EventHandler(this.lblSwitch2And3_Click);
            // 
            // timePicker4
            // 
            this.timePicker4.AutoSize = true;
            this.timePicker4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.timePicker4.DiffValue = System.TimeSpan.Parse("00:00:00");
            this.timePicker4.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.timePicker4.HH = 0;
            this.timePicker4.Location = new System.Drawing.Point(10, 165);
            this.timePicker4.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.timePicker4.MM = 0;
            this.timePicker4.MS = 0;
            this.timePicker4.Name = "timePicker4";
            this.timePicker4.ReadOnly = true;
            this.timePicker4.ShowSign = true;
            this.timePicker4.Size = new System.Drawing.Size(249, 31);
            this.timePicker4.SS = 0;
            this.timePicker4.TabIndex = 6;
            this.timePicker4.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            // 
            // timePicker3
            // 
            this.timePicker3.AutoSize = true;
            this.timePicker3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.timePicker3.DiffValue = System.TimeSpan.Parse("00:00:00");
            this.timePicker3.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.timePicker3.HH = 0;
            this.timePicker3.Location = new System.Drawing.Point(10, 114);
            this.timePicker3.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.timePicker3.MM = 0;
            this.timePicker3.MS = 0;
            this.timePicker3.Name = "timePicker3";
            this.timePicker3.Size = new System.Drawing.Size(342, 31);
            this.timePicker3.SS = 0;
            this.timePicker3.TabIndex = 5;
            this.timePicker3.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.timePicker3.ValueChanged += new System.EventHandler(this.timePicker_ValueChanged);
            // 
            // timePicker2
            // 
            this.timePicker2.AutoSize = true;
            this.timePicker2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.timePicker2.DiffValue = System.TimeSpan.Parse("00:00:00");
            this.timePicker2.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.timePicker2.HH = 0;
            this.timePicker2.Location = new System.Drawing.Point(10, 64);
            this.timePicker2.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.timePicker2.MM = 0;
            this.timePicker2.MS = 0;
            this.timePicker2.Name = "timePicker2";
            this.timePicker2.Size = new System.Drawing.Size(342, 31);
            this.timePicker2.SS = 0;
            this.timePicker2.TabIndex = 3;
            this.timePicker2.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.timePicker2.ValueChanged += new System.EventHandler(this.timePicker_ValueChanged);
            // 
            // timePicker1
            // 
            this.timePicker1.AutoSize = true;
            this.timePicker1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.timePicker1.DiffValue = System.TimeSpan.Parse("00:00:00");
            this.timePicker1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.timePicker1.HH = 0;
            this.timePicker1.Location = new System.Drawing.Point(10, 13);
            this.timePicker1.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.timePicker1.MM = 0;
            this.timePicker1.MS = 0;
            this.timePicker1.Name = "timePicker1";
            this.timePicker1.Size = new System.Drawing.Size(342, 31);
            this.timePicker1.SS = 0;
            this.timePicker1.TabIndex = 1;
            this.timePicker1.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.timePicker1.ValueChanged += new System.EventHandler(this.timePicker_ValueChanged);
            // 
            // TimeCalculatorForm
            // 
            this.AcceptButton = this.btnClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(384, 247);
            this.Controls.Add(this.lblSwitch2And3);
            this.Controls.Add(this.lblSwitch1And2);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.timePicker4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.timePicker3);
            this.Controls.Add(this.timePicker2);
            this.Controls.Add(this.timePicker1);
            this.Font = new System.Drawing.Font("Tahoma", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TimeCalculatorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Time Calculator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TimePicker timePicker1;
        private TimePicker timePicker2;
        private TimePicker timePicker3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private TimePicker timePicker4;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblSwitch1And2;
        private System.Windows.Forms.Label lblSwitch2And3;
    }
}