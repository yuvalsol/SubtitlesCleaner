namespace SubtitlesEditor
{
    partial class SubtitlesEditorForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lstErrors = new System.Windows.Forms.DataGridView();
            this.ColumnNum1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnError1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.lstEditor = new System.Windows.Forms.DataGridView();
            this.txtSubtitle = new SubtitlesEditor.TextBox();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnClean = new System.Windows.Forms.Button();
            this.btnSearchAndReplace = new System.Windows.Forms.Button();
            this.btnAdjust = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnSaveAs = new System.Windows.Forms.Button();
            this.chkBackupSRT = new System.Windows.Forms.CheckBox();
            this.btnOriginalSubtitles = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnOpen = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnMoveTo = new System.Windows.Forms.Button();
            this.timePicker = new SubtitlesEditor.TimePicker();
            this.chkShift = new System.Windows.Forms.CheckBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveAsFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.ColumnNum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnShow = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnHide = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnDuration = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lstErrors)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lstEditor)).BeginInit();
            this.flowLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lstErrors);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(927, 362);
            this.splitContainer1.SplitterDistance = 190;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 0;
            // 
            // lstErrors
            // 
            this.lstErrors.AllowUserToAddRows = false;
            this.lstErrors.AllowUserToDeleteRows = false;
            this.lstErrors.AllowUserToResizeColumns = false;
            this.lstErrors.AllowUserToResizeRows = false;
            this.lstErrors.BackgroundColor = System.Drawing.Color.White;
            this.lstErrors.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.lstErrors.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnNum1,
            this.ColumnError1});
            this.lstErrors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstErrors.Location = new System.Drawing.Point(0, 0);
            this.lstErrors.MultiSelect = false;
            this.lstErrors.Name = "lstErrors";
            this.lstErrors.ReadOnly = true;
            this.lstErrors.RowHeadersVisible = false;
            this.lstErrors.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.lstErrors.Size = new System.Drawing.Size(190, 362);
            this.lstErrors.TabIndex = 0;
            this.lstErrors.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.lstErrors_CellMouseDoubleClick);
            this.lstErrors.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.lstErrors_ColumnHeaderMouseClick);
            // 
            // ColumnNum1
            // 
            this.ColumnNum1.DataPropertyName = "Num";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ColumnNum1.DefaultCellStyle = dataGridViewCellStyle1;
            this.ColumnNum1.HeaderText = "#";
            this.ColumnNum1.Name = "ColumnNum1";
            this.ColumnNum1.ReadOnly = true;
            this.ColumnNum1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnNum1.Width = 60;
            // 
            // ColumnError1
            // 
            this.ColumnError1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnError1.DataPropertyName = "Error";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.ColumnError1.DefaultCellStyle = dataGridViewCellStyle2;
            this.ColumnError1.HeaderText = "Error";
            this.ColumnError1.Name = "ColumnError1";
            this.ColumnError1.ReadOnly = true;
            this.ColumnError1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer2.IsSplitterFixed = true;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.lstEditor);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.txtSubtitle);
            this.splitContainer2.Panel2.Controls.Add(this.flowLayoutPanel2);
            this.splitContainer2.Panel2.Controls.Add(this.flowLayoutPanel1);
            this.splitContainer2.Size = new System.Drawing.Size(732, 362);
            this.splitContainer2.SplitterDistance = 240;
            this.splitContainer2.TabIndex = 2;
            // 
            // lstEditor
            // 
            this.lstEditor.AllowUserToAddRows = false;
            this.lstEditor.AllowUserToResizeColumns = false;
            this.lstEditor.AllowUserToResizeRows = false;
            this.lstEditor.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.lstEditor.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.lstEditor.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.lstEditor.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnNum,
            this.ColumnShow,
            this.ColumnHide,
            this.ColumnDuration,
            this.ColumnText});
            this.lstEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstEditor.Location = new System.Drawing.Point(0, 0);
            this.lstEditor.MultiSelect = false;
            this.lstEditor.Name = "lstEditor";
            this.lstEditor.ReadOnly = true;
            this.lstEditor.RowHeadersVisible = false;
            this.lstEditor.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.lstEditor.Size = new System.Drawing.Size(732, 240);
            this.lstEditor.TabIndex = 1;
            this.lstEditor.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.lstEditor_CellMouseDoubleClick);
            this.lstEditor.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.lstEditor_DataBindingComplete);
            this.lstEditor.SelectionChanged += new System.EventHandler(this.lstEditor_SelectionChanged);
            this.lstEditor.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lstEditor_KeyDown);
            // 
            // txtSubtitle
            // 
            this.txtSubtitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSubtitle.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSubtitle.ForeColor = System.Drawing.Color.Black;
            this.txtSubtitle.Location = new System.Drawing.Point(0, 34);
            this.txtSubtitle.Multiline = true;
            this.txtSubtitle.Name = "txtSubtitle";
            this.txtSubtitle.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtSubtitle.Size = new System.Drawing.Size(732, 54);
            this.txtSubtitle.TabIndex = 4;
            this.txtSubtitle.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtSubtitle.LeaveWithChangedText += new System.EventHandler<System.Tuple<SubtitlesEditor.EditorRow, string>>(this.txtSubtitle_LeaveWithChangedText);
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel2.Controls.Add(this.btnClean);
            this.flowLayoutPanel2.Controls.Add(this.btnSearchAndReplace);
            this.flowLayoutPanel2.Controls.Add(this.btnAdjust);
            this.flowLayoutPanel2.Controls.Add(this.btnSave);
            this.flowLayoutPanel2.Controls.Add(this.btnSaveAs);
            this.flowLayoutPanel2.Controls.Add(this.chkBackupSRT);
            this.flowLayoutPanel2.Controls.Add(this.btnOriginalSubtitles);
            this.flowLayoutPanel2.Controls.Add(this.btnClear);
            this.flowLayoutPanel2.Controls.Add(this.btnOpen);
            this.flowLayoutPanel2.Controls.Add(this.btnClose);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(0, 88);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(732, 30);
            this.flowLayoutPanel2.TabIndex = 6;
            // 
            // btnClean
            // 
            this.btnClean.AutoSize = true;
            this.btnClean.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnClean.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClean.Location = new System.Drawing.Point(3, 3);
            this.btnClean.Name = "btnClean";
            this.btnClean.Size = new System.Drawing.Size(46, 24);
            this.btnClean.TabIndex = 1;
            this.btnClean.Text = "Clean";
            this.btnClean.UseVisualStyleBackColor = true;
            this.btnClean.Click += new System.EventHandler(this.btnClean_Click);
            // 
            // btnSearchAndReplace
            // 
            this.btnSearchAndReplace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSearchAndReplace.AutoSize = true;
            this.btnSearchAndReplace.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSearchAndReplace.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnSearchAndReplace.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSearchAndReplace.Location = new System.Drawing.Point(55, 3);
            this.btnSearchAndReplace.Name = "btnSearchAndReplace";
            this.btnSearchAndReplace.Size = new System.Drawing.Size(54, 24);
            this.btnSearchAndReplace.TabIndex = 2;
            this.btnSearchAndReplace.Text = "Search";
            this.btnSearchAndReplace.UseVisualStyleBackColor = true;
            this.btnSearchAndReplace.Click += new System.EventHandler(this.btnSearchAndReplace_Click);
            // 
            // btnAdjust
            // 
            this.btnAdjust.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAdjust.AutoSize = true;
            this.btnAdjust.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnAdjust.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnAdjust.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAdjust.Location = new System.Drawing.Point(115, 3);
            this.btnAdjust.Name = "btnAdjust";
            this.btnAdjust.Size = new System.Drawing.Size(52, 24);
            this.btnAdjust.TabIndex = 3;
            this.btnAdjust.Text = "Adjust";
            this.btnAdjust.UseVisualStyleBackColor = true;
            this.btnAdjust.Click += new System.EventHandler(this.btnAdjust_Click);
            // 
            // btnSave
            // 
            this.btnSave.AutoSize = true;
            this.btnSave.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSave.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Location = new System.Drawing.Point(173, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(43, 24);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnSaveAs
            // 
            this.btnSaveAs.AutoSize = true;
            this.btnSaveAs.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSaveAs.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveAs.Location = new System.Drawing.Point(222, 3);
            this.btnSaveAs.Name = "btnSaveAs";
            this.btnSaveAs.Size = new System.Drawing.Size(72, 24);
            this.btnSaveAs.TabIndex = 5;
            this.btnSaveAs.Text = "Save As...";
            this.btnSaveAs.UseVisualStyleBackColor = true;
            this.btnSaveAs.Click += new System.EventHandler(this.btnSaveAs_Click);
            // 
            // chkBackupSRT
            // 
            this.chkBackupSRT.AutoSize = true;
            this.chkBackupSRT.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkBackupSRT.Location = new System.Drawing.Point(300, 3);
            this.chkBackupSRT.Name = "chkBackupSRT";
            this.chkBackupSRT.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.chkBackupSRT.Size = new System.Drawing.Size(91, 22);
            this.chkBackupSRT.TabIndex = 6;
            this.chkBackupSRT.Text = "Backup SRT";
            this.chkBackupSRT.UseVisualStyleBackColor = true;
            // 
            // btnOriginalSubtitles
            // 
            this.btnOriginalSubtitles.AutoSize = true;
            this.btnOriginalSubtitles.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnOriginalSubtitles.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOriginalSubtitles.Location = new System.Drawing.Point(397, 3);
            this.btnOriginalSubtitles.Name = "btnOriginalSubtitles";
            this.btnOriginalSubtitles.Size = new System.Drawing.Size(107, 24);
            this.btnOriginalSubtitles.TabIndex = 7;
            this.btnOriginalSubtitles.Text = "Original Subtitles";
            this.btnOriginalSubtitles.UseVisualStyleBackColor = true;
            this.btnOriginalSubtitles.Click += new System.EventHandler(this.btnOriginalSubtitles_Click);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClear.AutoSize = true;
            this.btnClear.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnClear.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.Location = new System.Drawing.Point(510, 3);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(43, 24);
            this.btnClear.TabIndex = 8;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnOpen
            // 
            this.btnOpen.AutoSize = true;
            this.btnOpen.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnOpen.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOpen.Location = new System.Drawing.Point(559, 3);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(47, 24);
            this.btnOpen.TabIndex = 9;
            this.btnOpen.Text = "Open";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClose.AutoSize = true;
            this.btnClose.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Location = new System.Drawing.Point(612, 3);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(45, 24);
            this.btnClose.TabIndex = 10;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btnMoveTo);
            this.flowLayoutPanel1.Controls.Add(this.timePicker);
            this.flowLayoutPanel1.Controls.Add(this.chkShift);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(732, 34);
            this.flowLayoutPanel1.TabIndex = 5;
            // 
            // btnMoveTo
            // 
            this.btnMoveTo.AutoSize = true;
            this.btnMoveTo.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnMoveTo.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMoveTo.Location = new System.Drawing.Point(3, 3);
            this.btnMoveTo.Name = "btnMoveTo";
            this.btnMoveTo.Size = new System.Drawing.Size(65, 24);
            this.btnMoveTo.TabIndex = 1;
            this.btnMoveTo.Text = "Move To";
            this.btnMoveTo.UseVisualStyleBackColor = true;
            this.btnMoveTo.Click += new System.EventHandler(this.btnMoveTo_Click);
            // 
            // timePicker
            // 
            this.timePicker.AutoSize = true;
            this.timePicker.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.timePicker.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.timePicker.HH = 0;
            this.timePicker.Location = new System.Drawing.Point(71, 3);
            this.timePicker.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.timePicker.MM = 0;
            this.timePicker.MS = 0;
            this.timePicker.Name = "timePicker";
            this.timePicker.Size = new System.Drawing.Size(316, 31);
            this.timePicker.SS = 0;
            this.timePicker.TabIndex = 2;
            this.timePicker.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.timePicker.MillisecondsAdded += new System.EventHandler<int>(this.timePicker_MillisecondsAdded);
            // 
            // chkShift
            // 
            this.chkShift.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkShift.Location = new System.Drawing.Point(390, 3);
            this.chkShift.Name = "chkShift";
            this.chkShift.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.chkShift.Size = new System.Drawing.Size(51, 24);
            this.chkShift.TabIndex = 3;
            this.chkShift.Text = "Shift";
            this.chkShift.UseVisualStyleBackColor = true;
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "SubRip files (*.srt)|*.srt";
            // 
            // saveAsFileDialog
            // 
            this.saveAsFileDialog.DefaultExt = "srt";
            this.saveAsFileDialog.Filter = "SubRip Subtitle|*.srt";
            // 
            // ColumnNum
            // 
            this.ColumnNum.DataPropertyName = "Num";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ColumnNum.DefaultCellStyle = dataGridViewCellStyle4;
            this.ColumnNum.HeaderText = "#";
            this.ColumnNum.Name = "ColumnNum";
            this.ColumnNum.ReadOnly = true;
            this.ColumnNum.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnNum.Width = 50;
            // 
            // ColumnShow
            // 
            this.ColumnShow.DataPropertyName = "Show";
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ColumnShow.DefaultCellStyle = dataGridViewCellStyle5;
            this.ColumnShow.HeaderText = "Show";
            this.ColumnShow.Name = "ColumnShow";
            this.ColumnShow.ReadOnly = true;
            this.ColumnShow.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnShow.Width = 90;
            // 
            // ColumnHide
            // 
            this.ColumnHide.DataPropertyName = "Hide";
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ColumnHide.DefaultCellStyle = dataGridViewCellStyle6;
            this.ColumnHide.HeaderText = "Hide";
            this.ColumnHide.Name = "ColumnHide";
            this.ColumnHide.ReadOnly = true;
            this.ColumnHide.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnHide.Width = 90;
            // 
            // ColumnDuration
            // 
            this.ColumnDuration.DataPropertyName = "Duration";
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ColumnDuration.DefaultCellStyle = dataGridViewCellStyle7;
            this.ColumnDuration.HeaderText = "Duration";
            this.ColumnDuration.Name = "ColumnDuration";
            this.ColumnDuration.ReadOnly = true;
            this.ColumnDuration.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnDuration.Width = 70;
            // 
            // ColumnText
            // 
            this.ColumnText.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnText.DataPropertyName = "Text";
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.ColumnText.DefaultCellStyle = dataGridViewCellStyle8;
            this.ColumnText.HeaderText = "Text";
            this.ColumnText.Name = "ColumnText";
            this.ColumnText.ReadOnly = true;
            this.ColumnText.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // SubtitlesEditorForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(927, 362);
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.KeyPreview = true;
            this.Name = "SubtitlesEditorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Subtitles Editor";
            this.Load += new System.EventHandler(this.SubtitlesEditorForm_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.SubtitlesEditorForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.SubtitlesEditorForm_DragEnter);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SubtitlesEditorForm_KeyDown);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lstErrors)).EndInit();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lstEditor)).EndInit();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnClean;
        private System.Windows.Forms.Button btnOriginalSubtitles;
        private System.Windows.Forms.Button btnClear;
        private SubtitlesEditor.TextBox txtSubtitle;
        private System.Windows.Forms.DataGridView lstEditor;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.CheckBox chkBackupSRT;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private TimePicker timePicker;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Button btnMoveTo;
        private System.Windows.Forms.CheckBox chkShift;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.DataGridView lstErrors;
        private System.Windows.Forms.Button btnSearchAndReplace;
        private System.Windows.Forms.Button btnAdjust;
        private System.Windows.Forms.Button btnSaveAs;
        private System.Windows.Forms.SaveFileDialog saveAsFileDialog;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnNum1;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnError1;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnNum;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnShow;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnHide;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnDuration;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnText;
    }
}

