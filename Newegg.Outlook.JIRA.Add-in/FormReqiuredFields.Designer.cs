namespace Newegg.Outlook.JIRA.Add_in
{
    partial class FormReqiuredFields
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormReqiuredFields));
            this.btnOK = new System.Windows.Forms.Button();
            this.panelControls = new System.Windows.Forms.TableLayoutPanel();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(304, 95);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 99;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // panelControls
            // 
            this.panelControls.ColumnCount = 2;
            this.panelControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.panelControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.panelControls.Location = new System.Drawing.Point(12, 12);
            this.panelControls.Name = "panelControls";
            this.panelControls.RowCount = 2;
            this.panelControls.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.panelControls.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.panelControls.Size = new System.Drawing.Size(367, 77);
            this.panelControls.TabIndex = 2;
            this.panelControls.ControlAdded += new System.Windows.Forms.ControlEventHandler(this.panelControls_ControlAdded);
            // 
            // FormReqiuredFields
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(391, 128);
            this.Controls.Add(this.panelControls);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormReqiuredFields";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Reqiured Fields";
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FormReqiuredFields_KeyPress);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TableLayoutPanel panelControls;
    }
}