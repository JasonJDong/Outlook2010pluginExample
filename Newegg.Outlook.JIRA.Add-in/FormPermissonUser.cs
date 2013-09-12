using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Newegg.Outlook.JIRA.Add_in
{
    public partial class FormPermissonUser : Form
    {
        public JiraUserAuthModel JiraUserAuth { get; private set; }

        public FormPermissonUser()
        {
            JiraUserAuth = new JiraUserAuthModel();
            InitializeComponent();
        }

        public FormPermissonUser(JiraUserAuthModel userAuth)
        {
            InitializeComponent();
            JiraUserAuth = userAuth;
            txtUserName.Text = JiraUserAuth.User;
            txtPassword.Text = JiraUserAuth.Password;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtUserName.Text) || String.IsNullOrWhiteSpace(txtPassword.Text))
            {
                return;
            }
            JiraUserAuth.User = txtUserName.Text.Trim();
            JiraUserAuth.Password = txtPassword.Text.Trim();
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
