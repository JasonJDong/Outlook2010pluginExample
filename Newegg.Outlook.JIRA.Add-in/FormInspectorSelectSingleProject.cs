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
    public partial class FormInspectorSelectSingleProject : Form
    {
        public JiraProject SelectedJiraProject { get; set; }

        public List<JiraProject> DataSource { get; set; }

        public FormInspectorSelectSingleProject(List<JiraProject> dataSource)
        {
            InitializeComponent();
            Init(dataSource);
        }

        public void Init(List<JiraProject> projects)
        {
            DataSource = projects;
            cbSingleProject.DataSource = null;
            cbSingleProject.DisplayMember = "Name";
            cbSingleProject.ValueMember = "Key";
            cbSingleProject.DataSource = DataSource;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            SelectedJiraProject = cbSingleProject.SelectedItem as JiraProject;
            Close();
        }
    }
}
