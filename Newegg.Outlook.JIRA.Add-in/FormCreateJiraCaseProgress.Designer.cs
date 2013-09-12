namespace Newegg.Outlook.JIRA.Add_in
{
    partial class FormCreateJiraCaseProgress
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCreateJiraCaseProgress));
            this.prgCreateProgress = new System.Windows.Forms.ProgressBar();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblProgress = new System.Windows.Forms.Label();
            this.gridFinishedIssueView = new System.Windows.Forms.DataGridView();
            this.colStatus = new System.Windows.Forms.DataGridViewImageColumn();
            this.colIssueName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colErrorMsg = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gridFinishedIssueView)).BeginInit();
            this.SuspendLayout();
            // 
            // prgCreateProgress
            // 
            this.prgCreateProgress.Location = new System.Drawing.Point(12, 12);
            this.prgCreateProgress.Name = "prgCreateProgress";
            this.prgCreateProgress.Size = new System.Drawing.Size(303, 14);
            this.prgCreateProgress.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(335, 7);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblProgress
            // 
            this.lblProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblProgress.AutoSize = true;
            this.lblProgress.Location = new System.Drawing.Point(12, 35);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(78, 13);
            this.lblProgress.TabIndex = 2;
            this.lblProgress.Text = "0/0  Finished...";
            // 
            // gridFinishedIssueView
            // 
            this.gridFinishedIssueView.AllowUserToAddRows = false;
            this.gridFinishedIssueView.AllowUserToDeleteRows = false;
            this.gridFinishedIssueView.AllowUserToResizeColumns = false;
            this.gridFinishedIssueView.AllowUserToResizeRows = false;
            this.gridFinishedIssueView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.gridFinishedIssueView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridFinishedIssueView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.gridFinishedIssueView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Sunken;
            this.gridFinishedIssueView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.gridFinishedIssueView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colStatus,
            this.colIssueName,
            this.colErrorMsg});
            this.gridFinishedIssueView.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.gridFinishedIssueView.Location = new System.Drawing.Point(0, 51);
            this.gridFinishedIssueView.MultiSelect = false;
            this.gridFinishedIssueView.Name = "gridFinishedIssueView";
            this.gridFinishedIssueView.ReadOnly = true;
            this.gridFinishedIssueView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.gridFinishedIssueView.RowHeadersVisible = false;
            this.gridFinishedIssueView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.gridFinishedIssueView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.gridFinishedIssueView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridFinishedIssueView.Size = new System.Drawing.Size(418, 184);
            this.gridFinishedIssueView.TabIndex = 3;
            // 
            // colStatus
            // 
            this.colStatus.DataPropertyName = "Image";
            this.colStatus.HeaderText = "Success";
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            this.colStatus.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colStatus.Width = 56;
            // 
            // colIssueName
            // 
            this.colIssueName.DataPropertyName = "IssueName";
            this.colIssueName.HeaderText = "Issue Name";
            this.colIssueName.Name = "colIssueName";
            this.colIssueName.ReadOnly = true;
            this.colIssueName.Width = 220;
            // 
            // colErrorMsg
            // 
            this.colErrorMsg.DataPropertyName = "ErrorMessage";
            this.colErrorMsg.HeaderText = "Error Message";
            this.colErrorMsg.Name = "colErrorMsg";
            this.colErrorMsg.ReadOnly = true;
            this.colErrorMsg.Width = 140;
            // 
            // FormCreateJiraCaseProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(418, 235);
            this.Controls.Add(this.gridFinishedIssueView);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.prgCreateProgress);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormCreateJiraCaseProgress";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Create Jira Case Progress";
            ((System.ComponentModel.ISupportInitialize)(this.gridFinishedIssueView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar prgCreateProgress;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.DataGridView gridFinishedIssueView;
        private System.Windows.Forms.DataGridViewImageColumn colStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIssueName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colErrorMsg;
    }
}