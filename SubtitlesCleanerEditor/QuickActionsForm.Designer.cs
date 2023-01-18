
namespace SubtitlesCleanerEditor
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
            this.lstQuickActions = new System.Windows.Forms.DataGridView();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.ColumnFix = new System.Windows.Forms.DataGridViewButtonColumn();
            this.ColumnAction = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnResult = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnPreview = new System.Windows.Forms.DataGridViewButtonColumn();
            ((System.ComponentModel.ISupportInitialize)(this.lstQuickActions)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
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
            this.ColumnResult,
            this.ColumnPreview});
            this.lstQuickActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstQuickActions.Location = new System.Drawing.Point(0, 0);
            this.lstQuickActions.MultiSelect = false;
            this.lstQuickActions.Name = "lstQuickActions";
            this.lstQuickActions.ReadOnly = true;
            this.lstQuickActions.RowHeadersVisible = false;
            this.lstQuickActions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.lstQuickActions.Size = new System.Drawing.Size(664, 325);
            this.lstQuickActions.TabIndex = 1;
            this.lstQuickActions.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.lstQuickActions_CellContentClick);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btnOK);
            this.flowLayoutPanel1.Controls.Add(this.btnCancel);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 325);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(664, 37);
            this.flowLayoutPanel1.TabIndex = 3;
            // 
            // btnOK
            // 
            this.btnOK.AutoSize = true;
            this.btnOK.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(3, 6);
            this.btnOK.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(33, 24);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.AutoSize = true;
            this.btnCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(42, 6);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(52, 24);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
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
            this.ColumnAction.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnAction.DataPropertyName = "Action";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.ColumnAction.DefaultCellStyle = dataGridViewCellStyle2;
            this.ColumnAction.HeaderText = "Action";
            this.ColumnAction.Name = "ColumnAction";
            this.ColumnAction.ReadOnly = true;
            this.ColumnAction.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // ColumnResult
            // 
            this.ColumnResult.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnResult.DataPropertyName = "Result";
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
            // QuickActionsForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(664, 362);
            this.Controls.Add(this.lstQuickActions);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.MinimumSize = new System.Drawing.Size(680, 400);
            this.Name = "QuickActionsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Quick Actions";
            ((System.ComponentModel.ISupportInitialize)(this.lstQuickActions)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataGridView lstQuickActions;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.DataGridViewButtonColumn ColumnFix;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnAction;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnResult;
        private System.Windows.Forms.DataGridViewButtonColumn ColumnPreview;
    }
}