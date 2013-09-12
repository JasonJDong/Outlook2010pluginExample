using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Newegg.Outlook.JIRA.Add_in.Controls
{
    public partial class SearchCombox : UserControl
    {
        public Task<Object> SearchTask { get; set; }

        [Category("Extension Properties"), Description("CombBox Display Member")]
        public String DisplayMember
        {
            get { return cbMain.DisplayMember; }
            set { cbMain.DisplayMember = value; }
        }


        [Category("Extension Properties"), Description("CombBox Value Member")]
        public String ValueMember
        {
            get { return cbMain.ValueMember; }
            set { cbMain.ValueMember = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Object SelectedItem
        {
            get { return cbMain.SelectedItem; }
            set { cbMain.SelectedItem = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Object SearchResult { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public EventHandler SelectedIndexChanged { get; set; }

        public SearchCombox()
        {
            InitializeComponent();
            searchButton.Click += searchButton_Click;
            searchButton.MouseEnter += searchButton_MouseEnter;
            searchButton.MouseLeave += searchButton_MouseLeave;
            cbMain.SelectedIndexChanged += cbMain_SelectedIndexChanged;
        }

        private void cbMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SelectedIndexChanged != null)
            {
                SelectedIndexChanged(sender, e);
            }
        }

        private void searchButton_MouseLeave(object sender, EventArgs e)
        {
            searchButton.Image = Properties.Resources.search_normal;
        }

        private void searchButton_MouseEnter(object sender, EventArgs e)
        {
            searchButton.Image = Properties.Resources.search;
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            if (SearchTask != null)
            {
                SearchTask.ContinueWith(t => SafeUpdateCombBox(t.Result));
                SearchTask.Start();
            }
        }

        private void SafeUpdateCombBox(Object result)
        {
            if (InvokeRequired)
            {
                Action<Object> p = SafeUpdateCombBox;
                Invoke(p, result);
                return;
            }
            cbMain.DataSource = null;
            cbMain.DataSource = result;
            cbMain.DroppedDown = true;
        }

    }
}
