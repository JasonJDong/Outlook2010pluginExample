namespace Newegg.Outlook.JIRA.Add_in
{
    partial class FormExplorerProjects
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormExplorerProjects));
            this.lsbSourceProjects = new System.Windows.Forms.ListBox();
            this.lsbSelectedProjects = new System.Windows.Forms.ListBox();
            this.btnSelect = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.prgFechProjects = new System.Windows.Forms.ProgressBar();
            this.backworkerFetchProjecs = new System.ComponentModel.BackgroundWorker();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnUpSelected = new System.Windows.Forms.Button();
            this.btnDownSelected = new System.Windows.Forms.Button();
            this.wmtxtSearchbox = new Newegg.Outlook.JIRA.Add_in.Controls.WatermarkTextBox();
            this.SuspendLayout();
            // 
            // lsbSourceProjects
            // 
            this.lsbSourceProjects.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lsbSourceProjects.FormattingEnabled = true;
            this.lsbSourceProjects.ItemHeight = 18;
            this.lsbSourceProjects.Location = new System.Drawing.Point(12, 38);
            this.lsbSourceProjects.Name = "lsbSourceProjects";
            this.lsbSourceProjects.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lsbSourceProjects.Size = new System.Drawing.Size(245, 238);
            this.lsbSourceProjects.TabIndex = 0;
            // 
            // lsbSelectedProjects
            // 
            this.lsbSelectedProjects.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lsbSelectedProjects.FormattingEnabled = true;
            this.lsbSelectedProjects.ItemHeight = 18;
            this.lsbSelectedProjects.Location = new System.Drawing.Point(330, 38);
            this.lsbSelectedProjects.Name = "lsbSelectedProjects";
            this.lsbSelectedProjects.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lsbSelectedProjects.Size = new System.Drawing.Size(245, 238);
            this.lsbSelectedProjects.TabIndex = 1;
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point(263, 99);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(61, 22);
            this.btnSelect.TabIndex = 2;
            this.btnSelect.Text = ">>";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(263, 127);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(61, 22);
            this.btnRemove.TabIndex = 3;
            this.btnRemove.Text = "<<";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(417, 282);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(78, 28);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(514, 284);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(61, 24);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // prgFechProjects
            // 
            this.prgFechProjects.Location = new System.Drawing.Point(12, 285);
            this.prgFechProjects.MarqueeAnimationSpeed = 80;
            this.prgFechProjects.Name = "prgFechProjects";
            this.prgFechProjects.Size = new System.Drawing.Size(245, 18);
            this.prgFechProjects.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.prgFechProjects.TabIndex = 6;
            this.prgFechProjects.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Projects: ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(327, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Selected Projects: ";
            // 
            // btnUpSelected
            // 
            this.btnUpSelected.Location = new System.Drawing.Point(263, 155);
            this.btnUpSelected.Name = "btnUpSelected";
            this.btnUpSelected.Size = new System.Drawing.Size(61, 22);
            this.btnUpSelected.TabIndex = 9;
            this.btnUpSelected.Text = "↑";
            this.btnUpSelected.UseVisualStyleBackColor = true;
            this.btnUpSelected.Click += new System.EventHandler(this.btnUpSelected_Click);
            // 
            // btnDownSelected
            // 
            this.btnDownSelected.Location = new System.Drawing.Point(263, 183);
            this.btnDownSelected.Name = "btnDownSelected";
            this.btnDownSelected.Size = new System.Drawing.Size(61, 22);
            this.btnDownSelected.TabIndex = 10;
            this.btnDownSelected.Text = "↓";
            this.btnDownSelected.UseVisualStyleBackColor = true;
            this.btnDownSelected.Click += new System.EventHandler(this.btnDownSelected_Click);
            // 
            // wmtxtSearchbox
            // 
            this.wmtxtSearchbox.Location = new System.Drawing.Point(69, 6);
            this.wmtxtSearchbox.Name = "wmtxtSearchbox";
            this.wmtxtSearchbox.Size = new System.Drawing.Size(188, 20);
            this.wmtxtSearchbox.TabIndex = 11;
            this.wmtxtSearchbox.WatermarkText = "Search Projects...";
            this.wmtxtSearchbox.TextChanged += new System.EventHandler(this.wmtxtSearchbox_TextChanged);
            // 
            // FormExplorerProjects
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(587, 315);
            this.Controls.Add(this.wmtxtSearchbox);
            this.Controls.Add(this.btnDownSelected);
            this.Controls.Add(this.btnUpSelected);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.prgFechProjects);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.lsbSelectedProjects);
            this.Controls.Add(this.lsbSourceProjects);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormExplorerProjects";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Projects";
            this.Load += new System.EventHandler(this.FormExplorerProjects_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lsbSourceProjects;
        private System.Windows.Forms.ListBox lsbSelectedProjects;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ProgressBar prgFechProjects;
        private System.ComponentModel.BackgroundWorker backworkerFetchProjecs;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnUpSelected;
        private System.Windows.Forms.Button btnDownSelected;
        private Controls.WatermarkTextBox wmtxtSearchbox;
    }
}