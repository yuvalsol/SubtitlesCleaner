namespace SubtitlesCleaner.Editor
{
    partial class TimePicker
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.lblSign = new System.Windows.Forms.Label();
            this.numericUpDownHH = new SubtitlesCleaner.Editor.CustomNumericUpDown();
            this.lblSeperator1 = new System.Windows.Forms.Label();
            this.numericUpDownMM = new SubtitlesCleaner.Editor.CustomNumericUpDown();
            this.lblSeperator2 = new System.Windows.Forms.Label();
            this.numericUpDownSS = new SubtitlesCleaner.Editor.CustomNumericUpDown();
            this.lblSeperator3 = new System.Windows.Forms.Label();
            this.numericUpDownMS = new SubtitlesCleaner.Editor.CustomNumericUpDown();
            this.btnPlus = new SubtitlesCleaner.Editor.RepeatButton();
            this.btnMinus = new SubtitlesCleaner.Editor.RepeatButton();
            this.btnReset = new SubtitlesCleaner.Editor.RepeatButton();
            this.btnCopy = new SubtitlesCleaner.Editor.RepeatButton();
            this.btnPaste = new SubtitlesCleaner.Editor.RepeatButton();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHH)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMM)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSS)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMS)).BeginInit();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.lblSign);
            this.flowLayoutPanel1.Controls.Add(this.numericUpDownHH);
            this.flowLayoutPanel1.Controls.Add(this.lblSeperator1);
            this.flowLayoutPanel1.Controls.Add(this.numericUpDownMM);
            this.flowLayoutPanel1.Controls.Add(this.lblSeperator2);
            this.flowLayoutPanel1.Controls.Add(this.numericUpDownSS);
            this.flowLayoutPanel1.Controls.Add(this.lblSeperator3);
            this.flowLayoutPanel1.Controls.Add(this.numericUpDownMS);
            this.flowLayoutPanel1.Controls.Add(this.btnPlus);
            this.flowLayoutPanel1.Controls.Add(this.btnMinus);
            this.flowLayoutPanel1.Controls.Add(this.btnReset);
            this.flowLayoutPanel1.Controls.Add(this.btnCopy);
            this.flowLayoutPanel1.Controls.Add(this.btnPaste);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(350, 28);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // lblSign
            // 
            this.lblSign.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblSign.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.lblSign.Location = new System.Drawing.Point(0, 3);
            this.lblSign.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.lblSign.Name = "lblSign";
            this.lblSign.Size = new System.Drawing.Size(11, 22);
            this.lblSign.TabIndex = 1;
            this.lblSign.Text = "+";
            this.lblSign.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip.SetToolTip(this.lblSign, "Change Sign");
            this.lblSign.Visible = false;
            this.lblSign.Click += new System.EventHandler(this.lblSign_Click);
            // 
            // numericUpDownHH
            // 
            this.numericUpDownHH.Format = "00";
            this.numericUpDownHH.Location = new System.Drawing.Point(14, 3);
            this.numericUpDownHH.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.numericUpDownHH.Maximum = new decimal(new int[] {
            24,
            0,
            0,
            0});
            this.numericUpDownHH.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.numericUpDownHH.Name = "numericUpDownHH";
            this.numericUpDownHH.Size = new System.Drawing.Size(40, 22);
            this.numericUpDownHH.TabIndex = 2;
            this.numericUpDownHH.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip.SetToolTip(this.numericUpDownHH, "Hours");
            this.numericUpDownHH.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
            this.numericUpDownHH.Enter += new System.EventHandler(this.numericUpDown_Enter);
            this.numericUpDownHH.MouseDown += new System.Windows.Forms.MouseEventHandler(this.numericUpDown_MouseDown);
            // 
            // lblSeperator1
            // 
            this.lblSeperator1.Location = new System.Drawing.Point(54, 3);
            this.lblSeperator1.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.lblSeperator1.Name = "lblSeperator1";
            this.lblSeperator1.Size = new System.Drawing.Size(11, 22);
            this.lblSeperator1.TabIndex = 0;
            this.lblSeperator1.Text = ":";
            this.lblSeperator1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // numericUpDownMM
            // 
            this.numericUpDownMM.Format = "00";
            this.numericUpDownMM.Location = new System.Drawing.Point(65, 3);
            this.numericUpDownMM.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.numericUpDownMM.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.numericUpDownMM.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.numericUpDownMM.Name = "numericUpDownMM";
            this.numericUpDownMM.Size = new System.Drawing.Size(40, 22);
            this.numericUpDownMM.TabIndex = 3;
            this.numericUpDownMM.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip.SetToolTip(this.numericUpDownMM, "Minutes");
            this.numericUpDownMM.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
            this.numericUpDownMM.Enter += new System.EventHandler(this.numericUpDown_Enter);
            this.numericUpDownMM.MouseDown += new System.Windows.Forms.MouseEventHandler(this.numericUpDown_MouseDown);
            // 
            // lblSeperator2
            // 
            this.lblSeperator2.Location = new System.Drawing.Point(105, 3);
            this.lblSeperator2.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.lblSeperator2.Name = "lblSeperator2";
            this.lblSeperator2.Size = new System.Drawing.Size(11, 22);
            this.lblSeperator2.TabIndex = 0;
            this.lblSeperator2.Text = ":";
            this.lblSeperator2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // numericUpDownSS
            // 
            this.numericUpDownSS.Format = "00";
            this.numericUpDownSS.Location = new System.Drawing.Point(116, 3);
            this.numericUpDownSS.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.numericUpDownSS.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.numericUpDownSS.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.numericUpDownSS.Name = "numericUpDownSS";
            this.numericUpDownSS.Size = new System.Drawing.Size(40, 22);
            this.numericUpDownSS.TabIndex = 4;
            this.numericUpDownSS.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip.SetToolTip(this.numericUpDownSS, "Seconds");
            this.numericUpDownSS.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
            this.numericUpDownSS.Enter += new System.EventHandler(this.numericUpDown_Enter);
            this.numericUpDownSS.MouseDown += new System.Windows.Forms.MouseEventHandler(this.numericUpDown_MouseDown);
            // 
            // lblSeperator3
            // 
            this.lblSeperator3.Location = new System.Drawing.Point(156, 3);
            this.lblSeperator3.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.lblSeperator3.Name = "lblSeperator3";
            this.lblSeperator3.Size = new System.Drawing.Size(11, 22);
            this.lblSeperator3.TabIndex = 0;
            this.lblSeperator3.Text = ",";
            this.lblSeperator3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // numericUpDownMS
            // 
            this.numericUpDownMS.Format = "000";
            this.numericUpDownMS.Location = new System.Drawing.Point(167, 3);
            this.numericUpDownMS.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.numericUpDownMS.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownMS.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.numericUpDownMS.Name = "numericUpDownMS";
            this.numericUpDownMS.Size = new System.Drawing.Size(50, 22);
            this.numericUpDownMS.TabIndex = 5;
            this.numericUpDownMS.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip.SetToolTip(this.numericUpDownMS, "Milliseconds");
            this.numericUpDownMS.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
            this.numericUpDownMS.Enter += new System.EventHandler(this.numericUpDown_Enter);
            this.numericUpDownMS.MouseDown += new System.Windows.Forms.MouseEventHandler(this.numericUpDown_MouseDown);
            // 
            // btnPlus
            // 
            this.btnPlus.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnPlus.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.btnPlus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(110)))), ((int)(((byte)(165)))));
            this.btnPlus.Interval = 80;
            this.btnPlus.Location = new System.Drawing.Point(223, 3);
            this.btnPlus.Name = "btnPlus";
            this.btnPlus.Size = new System.Drawing.Size(20, 20);
            this.btnPlus.TabIndex = 6;
            this.btnPlus.Text = "+";
            this.toolTip.SetToolTip(this.btnPlus, "+50 ms");
            this.btnPlus.UseVisualStyleBackColor = true;
            this.btnPlus.Depressed += new System.EventHandler(this.btnPlus_Depressed);
            // 
            // btnMinus
            // 
            this.btnMinus.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnMinus.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.btnMinus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(110)))), ((int)(((byte)(165)))));
            this.btnMinus.Interval = 80;
            this.btnMinus.Location = new System.Drawing.Point(249, 3);
            this.btnMinus.Name = "btnMinus";
            this.btnMinus.Size = new System.Drawing.Size(20, 20);
            this.btnMinus.TabIndex = 7;
            this.btnMinus.Text = "-";
            this.toolTip.SetToolTip(this.btnMinus, "-50 ms");
            this.btnMinus.UseVisualStyleBackColor = true;
            this.btnMinus.Depressed += new System.EventHandler(this.btnMinus_Depressed);
            // 
            // btnReset
            // 
            this.btnReset.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnReset.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.btnReset.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(110)))), ((int)(((byte)(165)))));
            this.btnReset.Interval = 80;
            this.btnReset.Location = new System.Drawing.Point(275, 3);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(20, 20);
            this.btnReset.TabIndex = 8;
            this.btnReset.Text = "R";
            this.toolTip.SetToolTip(this.btnReset, "Reset Time");
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnCopy.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.btnCopy.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(110)))), ((int)(((byte)(165)))));
            this.btnCopy.Interval = 80;
            this.btnCopy.Location = new System.Drawing.Point(301, 3);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(20, 20);
            this.btnCopy.TabIndex = 9;
            this.btnCopy.Text = "C";
            this.toolTip.SetToolTip(this.btnCopy, "Copy Time");
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnPaste
            // 
            this.btnPaste.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnPaste.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.btnPaste.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(110)))), ((int)(((byte)(165)))));
            this.btnPaste.Interval = 80;
            this.btnPaste.Location = new System.Drawing.Point(327, 3);
            this.btnPaste.Name = "btnPaste";
            this.btnPaste.Size = new System.Drawing.Size(20, 20);
            this.btnPaste.TabIndex = 10;
            this.btnPaste.Text = "P";
            this.toolTip.SetToolTip(this.btnPaste, "Paste Time");
            this.btnPaste.UseVisualStyleBackColor = true;
            this.btnPaste.Click += new System.EventHandler(this.btnPaste_Click);
            // 
            // TimePicker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.flowLayoutPanel1);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.Name = "TimePicker";
            this.Size = new System.Drawing.Size(353, 31);
            this.flowLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownHH)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMM)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSS)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMS)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private CustomNumericUpDown numericUpDownHH;
        private CustomNumericUpDown numericUpDownSS;
        private CustomNumericUpDown numericUpDownMS;
        private CustomNumericUpDown numericUpDownMM;
        private System.Windows.Forms.Label lblSeperator1;
        private System.Windows.Forms.Label lblSeperator2;
        private System.Windows.Forms.Label lblSeperator3;
        private RepeatButton btnPlus;
        private RepeatButton btnMinus;
        private RepeatButton btnCopy;
        private RepeatButton btnPaste;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Label lblSign;
        private RepeatButton btnReset;
    }
}
