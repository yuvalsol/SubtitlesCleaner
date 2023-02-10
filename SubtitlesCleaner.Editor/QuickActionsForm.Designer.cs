
namespace SubtitlesCleaner.Editor
{
    partial class QuickActionsForm
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
            this.lstQuickActions = new System.Windows.Forms.DataGridView();
            this.ColumnFix = new System.Windows.Forms.DataGridViewButtonColumn();
            this.ColumnAction = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnExamples = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnResult = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnPreview = new System.Windows.Forms.DataGridViewButtonColumn();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnDiscardChanges = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnPreviewChanges = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.lstQuickActions)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstQuickActions
            // 
            this.lstQuickActions.AllowUserToAddRows = false;
            this.lstQuickActions.AllowUserToDeleteRows = false;
            this.lstQuickActions.AllowUserToResizeColumns = false;
            this.lstQuickActions.AllowUserToResizeRows = false;
            this.lstQuickActions.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.lstQuickActions.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.lstQuickActions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.lstQuickActions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnFix,
            this.ColumnAction,
            this.ColumnExamples,
            this.ColumnResult,
            this.ColumnPreview});
            this.lstQuickActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstQuickActions.Location = new System.Drawing.Point(0, 0);
            this.lstQuickActions.MultiSelect = false;
            this.lstQuickActions.Name = "lstQuickActions";
            this.lstQuickActions.ReadOnly = true;
            this.lstQuickActions.RowHeadersVisible = false;
            this.lstQuickActions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.lstQuickActions.Size = new System.Drawing.Size(844, 552);
            this.lstQuickActions.TabIndex = 1;
            this.lstQuickActions.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.lstQuickActions_CellContentClick);
            // 
            // ColumnFix
            // 
            this.ColumnFix.HeaderText = "";
            this.ColumnFix.Name = "ColumnFix";
            this.ColumnFix.ReadOnly = true;
            this.ColumnFix.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnFix.Text = "Fix";
            this.ColumnFix.UseColumnTextForButtonValue = true;
            this.ColumnFix.Width = 60;
            // 
            // ColumnAction
            // 
            this.ColumnAction.DataPropertyName = "Action";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.ColumnAction.DefaultCellStyle = dataGridViewCellStyle2;
            this.ColumnAction.HeaderText = "Action";
            this.ColumnAction.Name = "ColumnAction";
            this.ColumnAction.ReadOnly = true;
            this.ColumnAction.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnAction.Width = 260;
            // 
            // ColumnExamples
            // 
            this.ColumnExamples.DataPropertyName = "Examples";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.ColumnExamples.DefaultCellStyle = dataGridViewCellStyle3;
            this.ColumnExamples.HeaderText = "Examples";
            this.ColumnExamples.Name = "ColumnExamples";
            this.ColumnExamples.ReadOnly = true;
            this.ColumnExamples.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnExamples.Width = 260;
            // 
            // ColumnResult
            // 
            this.ColumnResult.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnResult.DataPropertyName = "Result";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.ColumnResult.DefaultCellStyle = dataGridViewCellStyle4;
            this.ColumnResult.HeaderText = "Result";
            this.ColumnResult.Name = "ColumnResult";
            this.ColumnResult.ReadOnly = true;
            this.ColumnResult.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // ColumnPreview
            // 
            this.ColumnPreview.HeaderText = "";
            this.ColumnPreview.Name = "ColumnPreview";
            this.ColumnPreview.ReadOnly = true;
            this.ColumnPreview.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnPreview.Text = "Preview";
            this.ColumnPreview.UseColumnTextForButtonValue = true;
            this.ColumnPreview.Width = 80;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.btnApply);
            this.flowLayoutPanel1.Controls.Add(this.btnDiscardChanges);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 9);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(164, 33);
            this.flowLayoutPanel1.TabIndex = 3;
            // 
            // btnApply
            // 
            this.btnApply.AutoSize = true;
            this.btnApply.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnApply.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnApply.Location = new System.Drawing.Point(3, 6);
            this.btnApply.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(47, 24);
            this.btnApply.TabIndex = 2;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnDiscardChanges
            // 
            this.btnDiscardChanges.AutoSize = true;
            this.btnDiscardChanges.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnDiscardChanges.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnDiscardChanges.Location = new System.Drawing.Point(56, 6);
            this.btnDiscardChanges.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.btnDiscardChanges.Name = "btnDiscardChanges";
            this.btnDiscardChanges.Size = new System.Drawing.Size(105, 24);
            this.btnDiscardChanges.TabIndex = 3;
            this.btnDiscardChanges.Text = "Discard Changes";
            this.btnDiscardChanges.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.btnPreviewChanges, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 552);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(844, 52);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // btnPreviewChanges
            // 
            this.btnPreviewChanges.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnPreviewChanges.AutoSize = true;
            this.btnPreviewChanges.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnPreviewChanges.Location = new System.Drawing.Point(731, 15);
            this.btnPreviewChanges.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.btnPreviewChanges.Name = "btnPreviewChanges";
            this.btnPreviewChanges.Size = new System.Drawing.Size(110, 24);
            this.btnPreviewChanges.TabIndex = 5;
            this.btnPreviewChanges.Text = "Preview Changes";
            this.btnPreviewChanges.UseVisualStyleBackColor = true;
            this.btnPreviewChanges.Click += new System.EventHandler(this.btnPreviewChanges_Click);
            // 
            // QuickActionsForm
            // 
            this.AcceptButton = this.btnApply;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnDiscardChanges;
            this.ClientSize = new System.Drawing.Size(844, 604);
            this.Controls.Add(this.lstQuickActions);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "QuickActionsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Quick Actions";
            ((System.ComponentModel.ISupportInitialize)(this.lstQuickActions)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataGridView lstQuickActions;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnDiscardChanges;
        private System.Windows.Forms.DataGridViewButtonColumn ColumnFix;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnAction;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnExamples;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnResult;
        private System.Windows.Forms.DataGridViewButtonColumn ColumnPreview;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnPreviewChanges;
    }
}