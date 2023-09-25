namespace SubtitlesCleaner.Editor
{
    partial class SubtitlesCleanerEditorForm
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle60 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle75 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle76 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle77 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle78 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle79 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle80 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle81 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle82 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SubtitlesCleanerEditorForm));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lstErrors = new System.Windows.Forms.DataGridView();
            this.ColumnNum1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnError1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStripErrors = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.fixErrorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fixAllErrorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flowLayoutPanel7 = new System.Windows.Forms.FlowLayoutPanel();
            this.chkSyncErrorsAndSubtitles = new System.Windows.Forms.CheckBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.lstEditor = new System.Windows.Forms.DataGridView();
            this.ColumnNum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnShow = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnHide = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnDuration = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnCleanText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStripEditor = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copySubtitleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyCleanTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyCleanSubtitleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.txtCleanSubtitle = new System.Windows.Forms.RichTextBox();
            this.contextMenuStripTxtCleanSubtitle = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.txtCleanSubtitle_copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.txtCleanSubtitle_selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripTxtSubtitle = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.txtSubtitle_cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.txtSubtitle_copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.txtSubtitle_pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.txtSubtitle_deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.txtSubtitle_selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flowLayoutPanel6 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnFixAndAdvance = new System.Windows.Forms.Button();
            this.btnAdvance = new System.Windows.Forms.Button();
            this.btnFix = new System.Windows.Forms.Button();
            this.lblCleanLineLengths = new System.Windows.Forms.Label();
            this.lblLineLengths = new System.Windows.Forms.Label();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.lblHearingImpairedDetection = new System.Windows.Forms.Label();
            this.rdbHIUpperCaseOnly = new System.Windows.Forms.RadioButton();
            this.rdbHIUpperLowerCases = new System.Windows.Forms.RadioButton();
            this.chkDictionaryCleaning = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnClean = new System.Windows.Forms.Button();
            this.btnQuickActions = new System.Windows.Forms.Button();
            this.btnAdjustTiming = new System.Windows.Forms.Button();
            this.btnReorder = new System.Windows.Forms.Button();
            this.btnBalanceLines = new System.Windows.Forms.Button();
            this.btnSearchAndReplace = new System.Windows.Forms.Button();
            this.btnOriginalSubtitles = new System.Windows.Forms.Button();
            this.btnTimeCalculator = new System.Windows.Forms.Button();
            this.flowLayoutPanel4 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnAddTime = new System.Windows.Forms.Button();
            this.flowLayoutPanel5 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnOpen = new System.Windows.Forms.Button();
            this.btnOpenNextSubtitle = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnSaveAs = new System.Windows.Forms.Button();
            this.chkBackupSRT = new System.Windows.Forms.CheckBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnSetShowTime = new System.Windows.Forms.Button();
            this.chkInteractiveRetiming = new System.Windows.Forms.CheckBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveAsFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.txtSubtitle = new SubtitlesCleaner.Editor.RichTextBox();
            this.diffTimePicker = new SubtitlesCleaner.Editor.TimePicker();
            this.timePicker = new SubtitlesCleaner.Editor.TimePicker();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lstErrors)).BeginInit();
            this.contextMenuStripErrors.SuspendLayout();
            this.flowLayoutPanel7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lstEditor)).BeginInit();
            this.contextMenuStripEditor.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.contextMenuStripTxtCleanSubtitle.SuspendLayout();
            this.contextMenuStripTxtSubtitle.SuspendLayout();
            this.flowLayoutPanel6.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel4.SuspendLayout();
            this.flowLayoutPanel5.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
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
            this.splitContainer1.Panel1.Controls.Add(this.flowLayoutPanel7);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1189, 494);
            this.splitContainer1.SplitterDistance = 195;
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
            this.lstErrors.ContextMenuStrip = this.contextMenuStripErrors;
            this.lstErrors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstErrors.Location = new System.Drawing.Point(0, 0);
            this.lstErrors.MultiSelect = false;
            this.lstErrors.Name = "lstErrors";
            this.lstErrors.ReadOnly = true;
            this.lstErrors.RowHeadersVisible = false;
            this.lstErrors.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.lstErrors.Size = new System.Drawing.Size(195, 470);
            this.lstErrors.TabIndex = 0;
            this.lstErrors.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.lstErrors_CellMouseDoubleClick);
            this.lstErrors.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.lstErrors_ColumnHeaderMouseClick);
            this.lstErrors.SelectionChanged += new System.EventHandler(this.lstErrors_SelectionChanged);
            this.lstErrors.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lstErrors_MouseUp);
            // 
            // ColumnNum1
            // 
            this.ColumnNum1.DataPropertyName = "Num";
            dataGridViewCellStyle60.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ColumnNum1.DefaultCellStyle = dataGridViewCellStyle60;
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
            dataGridViewCellStyle75.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.ColumnError1.DefaultCellStyle = dataGridViewCellStyle75;
            this.ColumnError1.HeaderText = "Error";
            this.ColumnError1.Name = "ColumnError1";
            this.ColumnError1.ReadOnly = true;
            this.ColumnError1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // contextMenuStripErrors
            // 
            this.contextMenuStripErrors.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fixErrorToolStripMenuItem,
            this.fixAllErrorsToolStripMenuItem});
            this.contextMenuStripErrors.Name = "contextMenuStripErrors";
            this.contextMenuStripErrors.ShowImageMargin = false;
            this.contextMenuStripErrors.Size = new System.Drawing.Size(115, 48);
            // 
            // fixErrorToolStripMenuItem
            // 
            this.fixErrorToolStripMenuItem.Name = "fixErrorToolStripMenuItem";
            this.fixErrorToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.fixErrorToolStripMenuItem.Text = "Fix Error";
            this.fixErrorToolStripMenuItem.Click += new System.EventHandler(this.fixErrorToolStripMenuItem_Click);
            // 
            // fixAllErrorsToolStripMenuItem
            // 
            this.fixAllErrorsToolStripMenuItem.Name = "fixAllErrorsToolStripMenuItem";
            this.fixAllErrorsToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.fixAllErrorsToolStripMenuItem.Text = "Fix All Errors";
            this.fixAllErrorsToolStripMenuItem.Click += new System.EventHandler(this.fixAllErrorsToolStripMenuItem_Click);
            // 
            // flowLayoutPanel7
            // 
            this.flowLayoutPanel7.AutoSize = true;
            this.flowLayoutPanel7.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel7.Controls.Add(this.chkSyncErrorsAndSubtitles);
            this.flowLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanel7.Location = new System.Drawing.Point(0, 470);
            this.flowLayoutPanel7.Name = "flowLayoutPanel7";
            this.flowLayoutPanel7.Size = new System.Drawing.Size(195, 24);
            this.flowLayoutPanel7.TabIndex = 8;
            // 
            // chkSyncErrorsAndSubtitles
            // 
            this.chkSyncErrorsAndSubtitles.AutoSize = true;
            this.chkSyncErrorsAndSubtitles.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkSyncErrorsAndSubtitles.Location = new System.Drawing.Point(3, 3);
            this.chkSyncErrorsAndSubtitles.Name = "chkSyncErrorsAndSubtitles";
            this.chkSyncErrorsAndSubtitles.Size = new System.Drawing.Size(150, 18);
            this.chkSyncErrorsAndSubtitles.TabIndex = 7;
            this.chkSyncErrorsAndSubtitles.Text = "Sync Errors && Subtitles";
            this.toolTip.SetToolTip(this.chkSyncErrorsAndSubtitles, "Clicking on an error or a subtitle will also focus on the other one");
            this.chkSyncErrorsAndSubtitles.UseVisualStyleBackColor = true;
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
            this.splitContainer2.Panel2.Controls.Add(this.tableLayoutPanel1);
            this.splitContainer2.Panel2.Controls.Add(this.flowLayoutPanel2);
            this.splitContainer2.Panel2.Controls.Add(this.flowLayoutPanel1);
            this.splitContainer2.Panel2.Controls.Add(this.flowLayoutPanel4);
            this.splitContainer2.Panel2.Controls.Add(this.flowLayoutPanel5);
            this.splitContainer2.Panel2.Controls.Add(this.flowLayoutPanel3);
            this.splitContainer2.Size = new System.Drawing.Size(989, 494);
            this.splitContainer2.SplitterDistance = 210;
            this.splitContainer2.TabIndex = 2;
            // 
            // lstEditor
            // 
            this.lstEditor.AllowUserToAddRows = false;
            this.lstEditor.AllowUserToResizeColumns = false;
            this.lstEditor.AllowUserToResizeRows = false;
            this.lstEditor.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle76.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle76.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle76.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle76.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle76.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle76.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle76.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.lstEditor.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle76;
            this.lstEditor.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.lstEditor.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnNum,
            this.ColumnShow,
            this.ColumnHide,
            this.ColumnDuration,
            this.ColumnText,
            this.ColumnCleanText});
            this.lstEditor.ContextMenuStrip = this.contextMenuStripEditor;
            this.lstEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstEditor.Location = new System.Drawing.Point(0, 0);
            this.lstEditor.MultiSelect = false;
            this.lstEditor.Name = "lstEditor";
            this.lstEditor.ReadOnly = true;
            this.lstEditor.RowHeadersVisible = false;
            this.lstEditor.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.lstEditor.Size = new System.Drawing.Size(989, 210);
            this.lstEditor.TabIndex = 1;
            this.lstEditor.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.lstEditor_CellMouseDoubleClick);
            this.lstEditor.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.lstEditor_DataBindingComplete);
            this.lstEditor.SelectionChanged += new System.EventHandler(this.lstEditor_SelectionChanged);
            this.lstEditor.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lstEditor_KeyDown);
            this.lstEditor.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lstEditor_MouseUp);
            // 
            // ColumnNum
            // 
            this.ColumnNum.DataPropertyName = "Num";
            dataGridViewCellStyle77.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ColumnNum.DefaultCellStyle = dataGridViewCellStyle77;
            this.ColumnNum.HeaderText = "#";
            this.ColumnNum.Name = "ColumnNum";
            this.ColumnNum.ReadOnly = true;
            this.ColumnNum.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnNum.Width = 50;
            // 
            // ColumnShow
            // 
            this.ColumnShow.DataPropertyName = "Show";
            dataGridViewCellStyle78.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ColumnShow.DefaultCellStyle = dataGridViewCellStyle78;
            this.ColumnShow.HeaderText = "Show";
            this.ColumnShow.Name = "ColumnShow";
            this.ColumnShow.ReadOnly = true;
            this.ColumnShow.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnShow.Width = 90;
            // 
            // ColumnHide
            // 
            this.ColumnHide.DataPropertyName = "Hide";
            dataGridViewCellStyle79.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ColumnHide.DefaultCellStyle = dataGridViewCellStyle79;
            this.ColumnHide.HeaderText = "Hide";
            this.ColumnHide.Name = "ColumnHide";
            this.ColumnHide.ReadOnly = true;
            this.ColumnHide.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnHide.Width = 90;
            // 
            // ColumnDuration
            // 
            this.ColumnDuration.DataPropertyName = "Duration";
            dataGridViewCellStyle80.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.ColumnDuration.DefaultCellStyle = dataGridViewCellStyle80;
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
            dataGridViewCellStyle81.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.ColumnText.DefaultCellStyle = dataGridViewCellStyle81;
            this.ColumnText.HeaderText = "Text";
            this.ColumnText.Name = "ColumnText";
            this.ColumnText.ReadOnly = true;
            this.ColumnText.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // ColumnCleanText
            // 
            this.ColumnCleanText.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnCleanText.DataPropertyName = "CleanText";
            dataGridViewCellStyle82.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.ColumnCleanText.DefaultCellStyle = dataGridViewCellStyle82;
            this.ColumnCleanText.HeaderText = "Clean Text";
            this.ColumnCleanText.Name = "ColumnCleanText";
            this.ColumnCleanText.ReadOnly = true;
            this.ColumnCleanText.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // contextMenuStripEditor
            // 
            this.contextMenuStripEditor.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyTextToolStripMenuItem,
            this.copySubtitleToolStripMenuItem,
            this.copyCleanTextToolStripMenuItem,
            this.copyCleanSubtitleToolStripMenuItem});
            this.contextMenuStripEditor.Name = "contextMenuStripEditor";
            this.contextMenuStripEditor.ShowImageMargin = false;
            this.contextMenuStripEditor.Size = new System.Drawing.Size(154, 92);
            // 
            // copyTextToolStripMenuItem
            // 
            this.copyTextToolStripMenuItem.Name = "copyTextToolStripMenuItem";
            this.copyTextToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.copyTextToolStripMenuItem.Text = "Copy Text";
            this.copyTextToolStripMenuItem.Click += new System.EventHandler(this.copyTextToolStripMenuItem_Click);
            // 
            // copySubtitleToolStripMenuItem
            // 
            this.copySubtitleToolStripMenuItem.Name = "copySubtitleToolStripMenuItem";
            this.copySubtitleToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.copySubtitleToolStripMenuItem.Text = "Copy Subtitle";
            this.copySubtitleToolStripMenuItem.Click += new System.EventHandler(this.copySubtitleToolStripMenuItem_Click);
            // 
            // copyCleanTextToolStripMenuItem
            // 
            this.copyCleanTextToolStripMenuItem.Name = "copyCleanTextToolStripMenuItem";
            this.copyCleanTextToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.copyCleanTextToolStripMenuItem.Text = "Copy Clean Text";
            this.copyCleanTextToolStripMenuItem.Click += new System.EventHandler(this.copyCleanTextToolStripMenuItem_Click);
            // 
            // copyCleanSubtitleToolStripMenuItem
            // 
            this.copyCleanSubtitleToolStripMenuItem.Name = "copyCleanSubtitleToolStripMenuItem";
            this.copyCleanSubtitleToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.copyCleanSubtitleToolStripMenuItem.Text = "Copy Clean Subtitle";
            this.copyCleanSubtitleToolStripMenuItem.Click += new System.EventHandler(this.copyCleanSubtitleToolStripMenuItem_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.txtCleanSubtitle, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtSubtitle, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel6, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblCleanLineLengths, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblLineLengths, 0, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 63);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(974, 102);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // txtCleanSubtitle
            // 
            this.txtCleanSubtitle.BackColor = System.Drawing.Color.White;
            this.txtCleanSubtitle.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtCleanSubtitle.ContextMenuStrip = this.contextMenuStripTxtCleanSubtitle;
            this.txtCleanSubtitle.DetectUrls = false;
            this.txtCleanSubtitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtCleanSubtitle.Font = new System.Drawing.Font("Consolas", 11.25F);
            this.txtCleanSubtitle.ForeColor = System.Drawing.Color.Black;
            this.txtCleanSubtitle.Location = new System.Drawing.Point(545, 3);
            this.txtCleanSubtitle.Name = "txtCleanSubtitle";
            this.txtCleanSubtitle.ReadOnly = true;
            this.tableLayoutPanel1.SetRowSpan(this.txtCleanSubtitle, 2);
            this.txtCleanSubtitle.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.txtCleanSubtitle.Size = new System.Drawing.Size(426, 82);
            this.txtCleanSubtitle.TabIndex = 4;
            this.txtCleanSubtitle.Text = "";
            // 
            // contextMenuStripTxtCleanSubtitle
            // 
            this.contextMenuStripTxtCleanSubtitle.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.txtCleanSubtitle_copyToolStripMenuItem,
            this.toolStripSeparator2,
            this.txtCleanSubtitle_selectAllToolStripMenuItem});
            this.contextMenuStripTxtCleanSubtitle.Name = "contextMenuStripTxtCleanSubtitle";
            this.contextMenuStripTxtCleanSubtitle.Size = new System.Drawing.Size(123, 54);
            this.contextMenuStripTxtCleanSubtitle.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripTxtCleanSubtitle_Opening);
            // 
            // txtCleanSubtitle_copyToolStripMenuItem
            // 
            this.txtCleanSubtitle_copyToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("txtCleanSubtitle_copyToolStripMenuItem.Image")));
            this.txtCleanSubtitle_copyToolStripMenuItem.Name = "txtCleanSubtitle_copyToolStripMenuItem";
            this.txtCleanSubtitle_copyToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.txtCleanSubtitle_copyToolStripMenuItem.Text = "Copy";
            this.txtCleanSubtitle_copyToolStripMenuItem.Click += new System.EventHandler(this.txtCleanSubtitle_copyToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(119, 6);
            // 
            // txtCleanSubtitle_selectAllToolStripMenuItem
            // 
            this.txtCleanSubtitle_selectAllToolStripMenuItem.Name = "txtCleanSubtitle_selectAllToolStripMenuItem";
            this.txtCleanSubtitle_selectAllToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.txtCleanSubtitle_selectAllToolStripMenuItem.Text = "Select All";
            this.txtCleanSubtitle_selectAllToolStripMenuItem.Click += new System.EventHandler(this.txtCleanSubtitle_selectAllToolStripMenuItem_Click);
            // 
            // contextMenuStripTxtSubtitle
            // 
            this.contextMenuStripTxtSubtitle.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.txtSubtitle_cutToolStripMenuItem,
            this.txtSubtitle_copyToolStripMenuItem,
            this.txtSubtitle_pasteToolStripMenuItem,
            this.txtSubtitle_deleteToolStripMenuItem,
            this.toolStripSeparator1,
            this.txtSubtitle_selectAllToolStripMenuItem});
            this.contextMenuStripTxtSubtitle.Name = "contextMenuStripTextBoxes";
            this.contextMenuStripTxtSubtitle.Size = new System.Drawing.Size(123, 120);
            this.contextMenuStripTxtSubtitle.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripTxtSubtitle_Opening);
            // 
            // txtSubtitle_cutToolStripMenuItem
            // 
            this.txtSubtitle_cutToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("txtSubtitle_cutToolStripMenuItem.Image")));
            this.txtSubtitle_cutToolStripMenuItem.Name = "txtSubtitle_cutToolStripMenuItem";
            this.txtSubtitle_cutToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.txtSubtitle_cutToolStripMenuItem.Text = "Cut";
            this.txtSubtitle_cutToolStripMenuItem.Click += new System.EventHandler(this.txtSubtitle_cutToolStripMenuItem_Click);
            // 
            // txtSubtitle_copyToolStripMenuItem
            // 
            this.txtSubtitle_copyToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("txtSubtitle_copyToolStripMenuItem.Image")));
            this.txtSubtitle_copyToolStripMenuItem.Name = "txtSubtitle_copyToolStripMenuItem";
            this.txtSubtitle_copyToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.txtSubtitle_copyToolStripMenuItem.Text = "Copy";
            this.txtSubtitle_copyToolStripMenuItem.Click += new System.EventHandler(this.txtSubtitle_copyToolStripMenuItem_Click);
            // 
            // txtSubtitle_pasteToolStripMenuItem
            // 
            this.txtSubtitle_pasteToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("txtSubtitle_pasteToolStripMenuItem.Image")));
            this.txtSubtitle_pasteToolStripMenuItem.Name = "txtSubtitle_pasteToolStripMenuItem";
            this.txtSubtitle_pasteToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.txtSubtitle_pasteToolStripMenuItem.Text = "Paste";
            this.txtSubtitle_pasteToolStripMenuItem.Click += new System.EventHandler(this.txtSubtitle_pasteToolStripMenuItem_Click);
            // 
            // txtSubtitle_deleteToolStripMenuItem
            // 
            this.txtSubtitle_deleteToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("txtSubtitle_deleteToolStripMenuItem.Image")));
            this.txtSubtitle_deleteToolStripMenuItem.Name = "txtSubtitle_deleteToolStripMenuItem";
            this.txtSubtitle_deleteToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.txtSubtitle_deleteToolStripMenuItem.Text = "Delete";
            this.txtSubtitle_deleteToolStripMenuItem.Click += new System.EventHandler(this.txtSubtitle_deleteToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(119, 6);
            // 
            // txtSubtitle_selectAllToolStripMenuItem
            // 
            this.txtSubtitle_selectAllToolStripMenuItem.Name = "txtSubtitle_selectAllToolStripMenuItem";
            this.txtSubtitle_selectAllToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.txtSubtitle_selectAllToolStripMenuItem.Text = "Select All";
            this.txtSubtitle_selectAllToolStripMenuItem.Click += new System.EventHandler(this.txtSubtitle_selectAllToolStripMenuItem_Click);
            // 
            // flowLayoutPanel6
            // 
            this.flowLayoutPanel6.AutoSize = true;
            this.flowLayoutPanel6.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel6.Controls.Add(this.btnFixAndAdvance);
            this.flowLayoutPanel6.Controls.Add(this.btnAdvance);
            this.flowLayoutPanel6.Controls.Add(this.btnFix);
            this.flowLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel6.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel6.Location = new System.Drawing.Point(431, 0);
            this.flowLayoutPanel6.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel6.Name = "flowLayoutPanel6";
            this.tableLayoutPanel1.SetRowSpan(this.flowLayoutPanel6, 2);
            this.flowLayoutPanel6.Size = new System.Drawing.Size(111, 88);
            this.flowLayoutPanel6.TabIndex = 10;
            // 
            // btnFixAndAdvance
            // 
            this.btnFixAndAdvance.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnFixAndAdvance.AutoSize = true;
            this.btnFixAndAdvance.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnFixAndAdvance.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFixAndAdvance.Location = new System.Drawing.Point(3, 3);
            this.btnFixAndAdvance.Name = "btnFixAndAdvance";
            this.btnFixAndAdvance.Size = new System.Drawing.Size(105, 23);
            this.btnFixAndAdvance.TabIndex = 7;
            this.btnFixAndAdvance.Text = "<= Fix && Advance";
            this.btnFixAndAdvance.UseVisualStyleBackColor = true;
            this.btnFixAndAdvance.Click += new System.EventHandler(this.btnFixAndAdvance_Click);
            // 
            // btnAdvance
            // 
            this.btnAdvance.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnAdvance.AutoSize = true;
            this.btnAdvance.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnAdvance.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAdvance.Location = new System.Drawing.Point(16, 32);
            this.btnAdvance.Name = "btnAdvance";
            this.btnAdvance.Size = new System.Drawing.Size(78, 23);
            this.btnAdvance.TabIndex = 9;
            this.btnAdvance.Text = "<= Advance";
            this.btnAdvance.UseVisualStyleBackColor = true;
            this.btnAdvance.Click += new System.EventHandler(this.btnAdvance_Click);
            // 
            // btnFix
            // 
            this.btnFix.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnFix.AutoSize = true;
            this.btnFix.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnFix.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFix.Location = new System.Drawing.Point(30, 61);
            this.btnFix.Name = "btnFix";
            this.btnFix.Size = new System.Drawing.Size(50, 23);
            this.btnFix.TabIndex = 8;
            this.btnFix.Text = "<= Fix";
            this.btnFix.UseVisualStyleBackColor = true;
            this.btnFix.Click += new System.EventHandler(this.btnFix_Click);
            // 
            // lblCleanLineLengths
            // 
            this.lblCleanLineLengths.AutoSize = true;
            this.lblCleanLineLengths.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCleanLineLengths.Location = new System.Drawing.Point(545, 88);
            this.lblCleanLineLengths.Name = "lblCleanLineLengths";
            this.lblCleanLineLengths.Size = new System.Drawing.Size(0, 14);
            this.lblCleanLineLengths.TabIndex = 0;
            // 
            // lblLineLengths
            // 
            this.lblLineLengths.AutoSize = true;
            this.lblLineLengths.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLineLengths.Location = new System.Drawing.Point(3, 88);
            this.lblLineLengths.Name = "lblLineLengths";
            this.lblLineLengths.Size = new System.Drawing.Size(0, 14);
            this.lblLineLengths.TabIndex = 0;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel2.Controls.Add(this.lblHearingImpairedDetection);
            this.flowLayoutPanel2.Controls.Add(this.rdbHIUpperCaseOnly);
            this.flowLayoutPanel2.Controls.Add(this.rdbHIUpperLowerCases);
            this.flowLayoutPanel2.Controls.Add(this.chkDictionaryCleaning);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 36);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(648, 24);
            this.flowLayoutPanel2.TabIndex = 2;
            // 
            // lblHearingImpairedDetection
            // 
            this.lblHearingImpairedDetection.AutoSize = true;
            this.lblHearingImpairedDetection.Location = new System.Drawing.Point(3, 4);
            this.lblHearingImpairedDetection.Margin = new System.Windows.Forms.Padding(3, 4, 3, 0);
            this.lblHearingImpairedDetection.Name = "lblHearingImpairedDetection";
            this.lblHearingImpairedDetection.Size = new System.Drawing.Size(161, 14);
            this.lblHearingImpairedDetection.TabIndex = 0;
            this.lblHearingImpairedDetection.Text = "Hearing-Impaired Detection:";
            this.toolTip.SetToolTip(this.lblHearingImpairedDetection, "Identifies hearing-impaired with only capital letters text or all-case text");
            // 
            // rdbHIUpperCaseOnly
            // 
            this.rdbHIUpperCaseOnly.AutoSize = true;
            this.rdbHIUpperCaseOnly.Checked = true;
            this.rdbHIUpperCaseOnly.Location = new System.Drawing.Point(170, 3);
            this.rdbHIUpperCaseOnly.Name = "rdbHIUpperCaseOnly";
            this.rdbHIUpperCaseOnly.Size = new System.Drawing.Size(115, 18);
            this.rdbHIUpperCaseOnly.TabIndex = 1;
            this.rdbHIUpperCaseOnly.TabStop = true;
            this.rdbHIUpperCaseOnly.Text = "Upper Case Only";
            this.toolTip.SetToolTip(this.rdbHIUpperCaseOnly, "Identifies hearing-impaired with only capital letters text or all-case text");
            this.rdbHIUpperCaseOnly.UseVisualStyleBackColor = true;
            this.rdbHIUpperCaseOnly.CheckedChanged += new System.EventHandler(this.rdbHICase_CheckedChanged);
            // 
            // rdbHIUpperLowerCases
            // 
            this.rdbHIUpperLowerCases.AutoSize = true;
            this.rdbHIUpperLowerCases.Location = new System.Drawing.Point(291, 3);
            this.rdbHIUpperLowerCases.Name = "rdbHIUpperLowerCases";
            this.rdbHIUpperLowerCases.Size = new System.Drawing.Size(142, 18);
            this.rdbHIUpperLowerCases.TabIndex = 2;
            this.rdbHIUpperLowerCases.Text = "Upper && Lower Cases";
            this.toolTip.SetToolTip(this.rdbHIUpperLowerCases, "Identifies hearing-impaired with only capital letters text or all-case text");
            this.rdbHIUpperLowerCases.UseVisualStyleBackColor = true;
            this.rdbHIUpperLowerCases.CheckedChanged += new System.EventHandler(this.rdbHICase_CheckedChanged);
            // 
            // chkDictionaryCleaning
            // 
            this.chkDictionaryCleaning.AutoSize = true;
            this.chkDictionaryCleaning.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkDictionaryCleaning.Location = new System.Drawing.Point(476, 3);
            this.chkDictionaryCleaning.Margin = new System.Windows.Forms.Padding(40, 3, 3, 3);
            this.chkDictionaryCleaning.Name = "chkDictionaryCleaning";
            this.chkDictionaryCleaning.Size = new System.Drawing.Size(169, 18);
            this.chkDictionaryCleaning.TabIndex = 3;
            this.chkDictionaryCleaning.Text = "English Dictionary Cleaning";
            this.toolTip.SetToolTip(this.chkDictionaryCleaning, "Enable English (Hunspell en-US) dictionary for cleaning misspelled words");
            this.chkDictionaryCleaning.UseVisualStyleBackColor = true;
            this.chkDictionaryCleaning.CheckedChanged += new System.EventHandler(this.chkDictionaryCleaning_CheckedChanged);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.btnClean);
            this.flowLayoutPanel1.Controls.Add(this.btnQuickActions);
            this.flowLayoutPanel1.Controls.Add(this.btnAdjustTiming);
            this.flowLayoutPanel1.Controls.Add(this.btnReorder);
            this.flowLayoutPanel1.Controls.Add(this.btnBalanceLines);
            this.flowLayoutPanel1.Controls.Add(this.btnSearchAndReplace);
            this.flowLayoutPanel1.Controls.Add(this.btnOriginalSubtitles);
            this.flowLayoutPanel1.Controls.Add(this.btnTimeCalculator);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 6);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(687, 30);
            this.flowLayoutPanel1.TabIndex = 1;
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
            this.toolTip.SetToolTip(this.btnClean, "Clean all subtitles");
            this.btnClean.UseVisualStyleBackColor = true;
            this.btnClean.Click += new System.EventHandler(this.btnClean_Click);
            // 
            // btnQuickActions
            // 
            this.btnQuickActions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQuickActions.AutoSize = true;
            this.btnQuickActions.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnQuickActions.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnQuickActions.Location = new System.Drawing.Point(55, 3);
            this.btnQuickActions.Name = "btnQuickActions";
            this.btnQuickActions.Size = new System.Drawing.Size(91, 24);
            this.btnQuickActions.TabIndex = 2;
            this.btnQuickActions.Text = "Quick Actions";
            this.toolTip.SetToolTip(this.btnQuickActions, "Perform quick selective fixes to subtitles");
            this.btnQuickActions.UseVisualStyleBackColor = true;
            this.btnQuickActions.Click += new System.EventHandler(this.btnQuickActions_Click);
            // 
            // btnAdjustTiming
            // 
            this.btnAdjustTiming.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAdjustTiming.AutoSize = true;
            this.btnAdjustTiming.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnAdjustTiming.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnAdjustTiming.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAdjustTiming.Location = new System.Drawing.Point(152, 3);
            this.btnAdjustTiming.Name = "btnAdjustTiming";
            this.btnAdjustTiming.Size = new System.Drawing.Size(92, 24);
            this.btnAdjustTiming.TabIndex = 3;
            this.btnAdjustTiming.Text = "Adjust Timing";
            this.toolTip.SetToolTip(this.btnAdjustTiming, "Adjust subtitles timing by 2 sync points");
            this.btnAdjustTiming.UseVisualStyleBackColor = true;
            this.btnAdjustTiming.Click += new System.EventHandler(this.btnAdjustTiming_Click);
            // 
            // btnReorder
            // 
            this.btnReorder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnReorder.AutoSize = true;
            this.btnReorder.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnReorder.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnReorder.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReorder.Location = new System.Drawing.Point(250, 3);
            this.btnReorder.Name = "btnReorder";
            this.btnReorder.Size = new System.Drawing.Size(60, 24);
            this.btnReorder.TabIndex = 4;
            this.btnReorder.Text = "Reorder";
            this.toolTip.SetToolTip(this.btnReorder, "Reorder subtitles based on their show time");
            this.btnReorder.UseVisualStyleBackColor = true;
            this.btnReorder.Click += new System.EventHandler(this.btnReorder_Click);
            // 
            // btnBalanceLines
            // 
            this.btnBalanceLines.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnBalanceLines.AutoSize = true;
            this.btnBalanceLines.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnBalanceLines.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnBalanceLines.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBalanceLines.Location = new System.Drawing.Point(316, 3);
            this.btnBalanceLines.Name = "btnBalanceLines";
            this.btnBalanceLines.Size = new System.Drawing.Size(89, 24);
            this.btnBalanceLines.TabIndex = 5;
            this.btnBalanceLines.Text = "Balance Lines";
            this.toolTip.SetToolTip(this.btnBalanceLines, "Merge short line with long line, or first line with its continuation in the secon" +
        "d line");
            this.btnBalanceLines.UseVisualStyleBackColor = true;
            this.btnBalanceLines.Click += new System.EventHandler(this.btnBalanceLines_Click);
            // 
            // btnSearchAndReplace
            // 
            this.btnSearchAndReplace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSearchAndReplace.AutoSize = true;
            this.btnSearchAndReplace.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSearchAndReplace.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnSearchAndReplace.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSearchAndReplace.Location = new System.Drawing.Point(411, 3);
            this.btnSearchAndReplace.Name = "btnSearchAndReplace";
            this.btnSearchAndReplace.Size = new System.Drawing.Size(54, 24);
            this.btnSearchAndReplace.TabIndex = 6;
            this.btnSearchAndReplace.Text = "Search";
            this.toolTip.SetToolTip(this.btnSearchAndReplace, "Search and replace");
            this.btnSearchAndReplace.UseVisualStyleBackColor = true;
            this.btnSearchAndReplace.Click += new System.EventHandler(this.btnSearchAndReplace_Click);
            // 
            // btnOriginalSubtitles
            // 
            this.btnOriginalSubtitles.AutoSize = true;
            this.btnOriginalSubtitles.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnOriginalSubtitles.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOriginalSubtitles.Location = new System.Drawing.Point(471, 3);
            this.btnOriginalSubtitles.Name = "btnOriginalSubtitles";
            this.btnOriginalSubtitles.Size = new System.Drawing.Size(107, 24);
            this.btnOriginalSubtitles.TabIndex = 7;
            this.btnOriginalSubtitles.Text = "Original Subtitles";
            this.toolTip.SetToolTip(this.btnOriginalSubtitles, "Load the original subtitles and discard all previous changes");
            this.btnOriginalSubtitles.UseVisualStyleBackColor = true;
            this.btnOriginalSubtitles.Click += new System.EventHandler(this.btnOriginalSubtitles_Click);
            // 
            // btnTimeCalculator
            // 
            this.btnTimeCalculator.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTimeCalculator.AutoSize = true;
            this.btnTimeCalculator.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnTimeCalculator.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTimeCalculator.Location = new System.Drawing.Point(584, 3);
            this.btnTimeCalculator.Name = "btnTimeCalculator";
            this.btnTimeCalculator.Size = new System.Drawing.Size(100, 24);
            this.btnTimeCalculator.TabIndex = 8;
            this.btnTimeCalculator.Text = "Time Calculator";
            this.toolTip.SetToolTip(this.btnTimeCalculator, "Calculate time differences");
            this.btnTimeCalculator.UseVisualStyleBackColor = true;
            this.btnTimeCalculator.Click += new System.EventHandler(this.btnTimeCalculator_Click);
            // 
            // flowLayoutPanel4
            // 
            this.flowLayoutPanel4.AutoSize = true;
            this.flowLayoutPanel4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel4.Controls.Add(this.btnAddTime);
            this.flowLayoutPanel4.Controls.Add(this.diffTimePicker);
            this.flowLayoutPanel4.Location = new System.Drawing.Point(3, 205);
            this.flowLayoutPanel4.Name = "flowLayoutPanel4";
            this.flowLayoutPanel4.Size = new System.Drawing.Size(429, 37);
            this.flowLayoutPanel4.TabIndex = 5;
            // 
            // btnAddTime
            // 
            this.btnAddTime.AutoSize = true;
            this.btnAddTime.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnAddTime.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddTime.Location = new System.Drawing.Point(3, 3);
            this.btnAddTime.Name = "btnAddTime";
            this.btnAddTime.Size = new System.Drawing.Size(70, 24);
            this.btnAddTime.TabIndex = 1;
            this.btnAddTime.Text = "Add Time";
            this.toolTip.SetToolTip(this.btnAddTime, "Add time, positive or negative, starting from the selected subtitle");
            this.btnAddTime.UseVisualStyleBackColor = true;
            this.btnAddTime.Click += new System.EventHandler(this.btnAddTime_Click);
            // 
            // flowLayoutPanel5
            // 
            this.flowLayoutPanel5.AutoSize = true;
            this.flowLayoutPanel5.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel5.Controls.Add(this.btnClear);
            this.flowLayoutPanel5.Controls.Add(this.btnOpen);
            this.flowLayoutPanel5.Controls.Add(this.btnOpenNextSubtitle);
            this.flowLayoutPanel5.Controls.Add(this.btnSave);
            this.flowLayoutPanel5.Controls.Add(this.btnSaveAs);
            this.flowLayoutPanel5.Controls.Add(this.chkBackupSRT);
            this.flowLayoutPanel5.Controls.Add(this.btnClose);
            this.flowLayoutPanel5.Location = new System.Drawing.Point(3, 242);
            this.flowLayoutPanel5.Name = "flowLayoutPanel5";
            this.flowLayoutPanel5.Size = new System.Drawing.Size(506, 30);
            this.flowLayoutPanel5.TabIndex = 6;
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClear.AutoSize = true;
            this.btnClear.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnClear.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.Location = new System.Drawing.Point(3, 3);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(43, 24);
            this.btnClear.TabIndex = 1;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnOpen
            // 
            this.btnOpen.AutoSize = true;
            this.btnOpen.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnOpen.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOpen.Location = new System.Drawing.Point(52, 3);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(47, 24);
            this.btnOpen.TabIndex = 2;
            this.btnOpen.Text = "Open";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnOpenNextSubtitle
            // 
            this.btnOpenNextSubtitle.AutoSize = true;
            this.btnOpenNextSubtitle.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnOpenNextSubtitle.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOpenNextSubtitle.Location = new System.Drawing.Point(105, 3);
            this.btnOpenNextSubtitle.Name = "btnOpenNextSubtitle";
            this.btnOpenNextSubtitle.Size = new System.Drawing.Size(123, 24);
            this.btnOpenNextSubtitle.TabIndex = 3;
            this.btnOpenNextSubtitle.Text = "Open Next Subtitle";
            this.btnOpenNextSubtitle.UseVisualStyleBackColor = true;
            this.btnOpenNextSubtitle.Click += new System.EventHandler(this.btnOpenNextSubtitle_Click);
            // 
            // btnSave
            // 
            this.btnSave.AutoSize = true;
            this.btnSave.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSave.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Location = new System.Drawing.Point(234, 3);
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
            this.btnSaveAs.Location = new System.Drawing.Point(283, 3);
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
            this.chkBackupSRT.Location = new System.Drawing.Point(361, 3);
            this.chkBackupSRT.Name = "chkBackupSRT";
            this.chkBackupSRT.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.chkBackupSRT.Size = new System.Drawing.Size(91, 22);
            this.chkBackupSRT.TabIndex = 6;
            this.chkBackupSRT.Text = "Backup SRT";
            this.chkBackupSRT.UseVisualStyleBackColor = true;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClose.AutoSize = true;
            this.btnClose.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Location = new System.Drawing.Point(458, 3);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(45, 24);
            this.btnClose.TabIndex = 7;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.AutoSize = true;
            this.flowLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel3.Controls.Add(this.btnSetShowTime);
            this.flowLayoutPanel3.Controls.Add(this.timePicker);
            this.flowLayoutPanel3.Controls.Add(this.chkInteractiveRetiming);
            this.flowLayoutPanel3.Location = new System.Drawing.Point(3, 168);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(592, 37);
            this.flowLayoutPanel3.TabIndex = 4;
            // 
            // btnSetShowTime
            // 
            this.btnSetShowTime.AutoSize = true;
            this.btnSetShowTime.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSetShowTime.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSetShowTime.Location = new System.Drawing.Point(3, 3);
            this.btnSetShowTime.Name = "btnSetShowTime";
            this.btnSetShowTime.Size = new System.Drawing.Size(102, 24);
            this.btnSetShowTime.TabIndex = 1;
            this.btnSetShowTime.Text = "Set Show Time";
            this.toolTip.SetToolTip(this.btnSetShowTime, "Set show times starting from the selected subtitle");
            this.btnSetShowTime.UseVisualStyleBackColor = true;
            this.btnSetShowTime.Click += new System.EventHandler(this.btnSetShowTime_Click);
            // 
            // chkInteractiveRetiming
            // 
            this.chkInteractiveRetiming.AutoSize = true;
            this.chkInteractiveRetiming.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkInteractiveRetiming.Location = new System.Drawing.Point(453, 3);
            this.chkInteractiveRetiming.Name = "chkInteractiveRetiming";
            this.chkInteractiveRetiming.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.chkInteractiveRetiming.Size = new System.Drawing.Size(136, 22);
            this.chkInteractiveRetiming.TabIndex = 3;
            this.chkInteractiveRetiming.Text = "Interactive Retiming";
            this.toolTip.SetToolTip(this.chkInteractiveRetiming, "Show timings will change in the subtitles panel as the show time is changed");
            this.chkInteractiveRetiming.UseVisualStyleBackColor = true;
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
            // txtSubtitle
            // 
            this.txtSubtitle.BackColor = System.Drawing.Color.White;
            this.txtSubtitle.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtSubtitle.ContextMenuStrip = this.contextMenuStripTxtSubtitle;
            this.txtSubtitle.DetectUrls = false;
            this.txtSubtitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSubtitle.Font = new System.Drawing.Font("Consolas", 11.25F);
            this.txtSubtitle.ForeColor = System.Drawing.Color.Black;
            this.txtSubtitle.Location = new System.Drawing.Point(3, 3);
            this.txtSubtitle.Name = "txtSubtitle";
            this.tableLayoutPanel1.SetRowSpan(this.txtSubtitle, 2);
            this.txtSubtitle.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.txtSubtitle.Size = new System.Drawing.Size(425, 82);
            this.txtSubtitle.TabIndex = 11;
            this.txtSubtitle.Text = "";
            this.txtSubtitle.LeaveWithChangedText += new System.EventHandler<SubtitlesCleaner.Editor.LeaveWithChangedTextEventArgs>(this.txtSubtitle_LeaveWithChangedText);
            // 
            // diffTimePicker
            // 
            this.diffTimePicker.AutoSize = true;
            this.diffTimePicker.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.diffTimePicker.DiffValue = System.TimeSpan.Parse("00:00:00");
            this.diffTimePicker.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.diffTimePicker.HH = 0;
            this.diffTimePicker.Location = new System.Drawing.Point(76, 3);
            this.diffTimePicker.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.diffTimePicker.MM = 0;
            this.diffTimePicker.MS = 0;
            this.diffTimePicker.Name = "diffTimePicker";
            this.diffTimePicker.ShowSign = true;
            this.diffTimePicker.Size = new System.Drawing.Size(353, 31);
            this.diffTimePicker.SS = 0;
            this.diffTimePicker.TabIndex = 2;
            this.diffTimePicker.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            // 
            // timePicker
            // 
            this.timePicker.AutoSize = true;
            this.timePicker.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.timePicker.DiffValue = System.TimeSpan.Parse("00:00:00");
            this.timePicker.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.timePicker.HH = 0;
            this.timePicker.Location = new System.Drawing.Point(108, 3);
            this.timePicker.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.timePicker.MM = 0;
            this.timePicker.MS = 0;
            this.timePicker.Name = "timePicker";
            this.timePicker.Size = new System.Drawing.Size(342, 31);
            this.timePicker.SS = 0;
            this.timePicker.TabIndex = 2;
            this.timePicker.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.timePicker.MillisecondsAdded += new System.EventHandler<int>(this.timePicker_MillisecondsAdded);
            // 
            // toolTip
            // 
            this.toolTip.AutomaticDelay = 1500;
            // 
            // SubtitlesCleanerEditorForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(1189, 494);
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(1205, 532);
            this.Name = "SubtitlesCleanerEditorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Subtitles Cleaner Editor";
            this.Shown += new System.EventHandler(this.SubtitlesCleanerEditorForm_Shown);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.SubtitlesCleanerEditorForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.SubtitlesCleanerEditorForm_DragEnter);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SubtitlesCleanerEditorForm_KeyDown);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lstErrors)).EndInit();
            this.contextMenuStripErrors.ResumeLayout(false);
            this.flowLayoutPanel7.ResumeLayout(false);
            this.flowLayoutPanel7.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.lstEditor)).EndInit();
            this.contextMenuStripEditor.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.contextMenuStripTxtCleanSubtitle.ResumeLayout(false);
            this.contextMenuStripTxtSubtitle.ResumeLayout(false);
            this.flowLayoutPanel6.ResumeLayout(false);
            this.flowLayoutPanel6.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.flowLayoutPanel4.ResumeLayout(false);
            this.flowLayoutPanel4.PerformLayout();
            this.flowLayoutPanel5.ResumeLayout(false);
            this.flowLayoutPanel5.PerformLayout();
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnClean;
        private System.Windows.Forms.Button btnOriginalSubtitles;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.DataGridView lstEditor;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.CheckBox chkBackupSRT;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private TimePicker timePicker;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Button btnSetShowTime;
        private System.Windows.Forms.CheckBox chkInteractiveRetiming;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel5;
        private System.Windows.Forms.DataGridView lstErrors;
        private System.Windows.Forms.Button btnSearchAndReplace;
        private System.Windows.Forms.Button btnAdjustTiming;
        private System.Windows.Forms.Button btnSaveAs;
        private System.Windows.Forms.SaveFileDialog saveAsFileDialog;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnNum1;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnError1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel4;
        private System.Windows.Forms.Button btnAddTime;
        private TimePicker diffTimePicker;
        private System.Windows.Forms.Button btnTimeCalculator;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnReorder;
        private System.Windows.Forms.Button btnBalanceLines;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Label lblHearingImpairedDetection;
        private System.Windows.Forms.RadioButton rdbHIUpperCaseOnly;
        private System.Windows.Forms.RadioButton rdbHIUpperLowerCases;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnNum;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnShow;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnHide;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnDuration;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnText;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnCleanText;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblLineLengths;
        private System.Windows.Forms.Label lblCleanLineLengths;
        private System.Windows.Forms.Button btnAdvance;
        private System.Windows.Forms.Button btnFix;
        private System.Windows.Forms.Button btnFixAndAdvance;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel6;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripErrors;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripEditor;
        private System.Windows.Forms.ToolStripMenuItem copyTextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fixErrorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fixAllErrorsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copySubtitleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyCleanSubtitleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyCleanTextToolStripMenuItem;
        private System.Windows.Forms.Button btnQuickActions;
        private System.Windows.Forms.CheckBox chkSyncErrorsAndSubtitles;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel7;
        private SubtitlesCleaner.Editor.RichTextBox txtSubtitle;
        private System.Windows.Forms.RichTextBox txtCleanSubtitle;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripTxtSubtitle;
        private System.Windows.Forms.ToolStripMenuItem txtSubtitle_cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem txtSubtitle_copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem txtSubtitle_pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem txtSubtitle_selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem txtSubtitle_deleteToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripTxtCleanSubtitle;
        private System.Windows.Forms.ToolStripMenuItem txtCleanSubtitle_copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem txtCleanSubtitle_selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Button btnOpenNextSubtitle;
        private System.Windows.Forms.CheckBox chkDictionaryCleaning;
        private System.Windows.Forms.ToolTip toolTip;
    }
}

