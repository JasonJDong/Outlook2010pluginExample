using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Newegg.Outlook.JIRA.Add_in.Controls
{
    public class WatermarkTextBox : TextBox
    {
        private readonly Label lblwaterText = new Label();

        [Category("Extension Properties"), Description("Water mark text")]
        public String WatermarkText
        {
            get { return lblwaterText.Text; }
            set { lblwaterText.Text = value; }
        }

        public WatermarkTextBox()
        {
            lblwaterText.BorderStyle = BorderStyle.None;
            lblwaterText.Enabled = false;
            lblwaterText.BackColor = Color.White;
            lblwaterText.AutoSize = false;
            lblwaterText.Top = 1;
            lblwaterText.Left = 2;
            lblwaterText.Font = new Font(lblwaterText.Font.FontFamily, 10, FontStyle.Italic);
            lblwaterText.FlatStyle = FlatStyle.System;
            Controls.Add(lblwaterText);
            GotFocus += WatermarkTextBox_GotFocus;
            LostFocus += WatermarkTextBox_LostFocus;
        }

        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                lblwaterText.Visible = String.IsNullOrWhiteSpace(value);
                base.Text = value;
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            if (Multiline && (ScrollBars == ScrollBars.Vertical || ScrollBars == ScrollBars.Both))
            {
                lblwaterText.Width = Width - 20;
            }
            else
            {
                lblwaterText.Width = Width - 24;
            }
            lblwaterText.Height = Height - 2;
            base.OnSizeChanged(e);
        }
        protected override void OnTextChanged(EventArgs e)
        {
            lblwaterText.Visible = String.IsNullOrWhiteSpace(base.Text);
            base.OnTextChanged(e);
        }

        private void WatermarkTextBox_LostFocus(object sender, EventArgs e)
        {
            lblwaterText.Visible = String.IsNullOrWhiteSpace(base.Text);
        }

        private void WatermarkTextBox_GotFocus(object sender, EventArgs e)
        {
            lblwaterText.Visible = false;
        }
    }
}
