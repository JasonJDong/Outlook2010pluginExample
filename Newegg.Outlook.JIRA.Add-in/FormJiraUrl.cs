using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Newegg.Outlook.JIRA.Add_in
{
    public partial class FormJiraUrl : Form
    {
        public String JiraUrl { get; set; }

        public JiraOperator JiraOperator { get; set; }

        public FormJiraUrl(JiraOperator jiraOperator, String url)
        {
            JiraOperator = jiraOperator;
            InitializeComponent();
            ChangeJiraUrl(url);
        }

        public void ChangeJiraUrl(String url)
        {
            txtJiraUrl.Text = url;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!Uri.IsWellFormedUriString(txtJiraUrl.Text, UriKind.Absolute))
            {
                MessageBox.Show("This is not a vaild host name!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            JiraUrl = txtJiraUrl.Text.Trim().Trim('/');
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            SafeUpdateTestButtonEnable(false);
            txtJiraUrl.Text = JiraOperator.BaseJiraUrl = txtJiraUrl.Text.Trim().Trim('/');
            var task = new Task<List<JiraIssueType>>(() =>
                {
                    try
                    {
                        return JiraOperator.GetIssueTypes();
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                });
            task.ContinueWith(t =>
                {
                    SafeUpdateConnectSatatus(t.Result != null);
                    SafeUpdateTestButtonEnable(true);
                });
            task.Start();
        }

        private void SafeUpdateTestButtonEnable(bool enable)
        {
            if (InvokeRequired)
            {
                Action<bool> p = SafeUpdateTestButtonEnable;
                Invoke(p, enable);
                return;
            }
            if (IsDisposed)
            {
                return;
            }
            btnTest.Enabled = enable;
            txtJiraUrl.Enabled = enable;
        }


        private void SafeUpdateConnectSatatus(bool success)
        {
            if (InvokeRequired)
            {
                Action<bool> p = SafeUpdateConnectSatatus;
                Invoke(p, success);
                return;
            }
            if (IsDisposed)
            {
                return;
            }
            lblConnectStatus.ForeColor = success ? Color.Green : Color.Red;
            lblConnectStatus.Text = success ? "Connect successfully!" : "Connect failed!";
        }
    }
}
