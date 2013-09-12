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
    public partial class FormExplorerProjects : Form
    {
        public JiraOperator JiraOperator { get; set; }

        public List<JiraProject> OriginalJiraProjects { get; set; }

        public List<JiraProject> ForSelectedJiraProjects { get; set; }

        public List<JiraProject> SelectedJiraProjects { get; set; }

        public List<JiraProject> SearchSource { get; set; }

        public FormExplorerProjects(JiraOperator jiraOperator)
        {
            JiraOperator = jiraOperator;
            InitializeComponent();
            RebindingSelectedDataSource();
            SelectedJiraProjects = new List<JiraProject>();
            backworkerFetchProjecs.DoWork += backworkerFetchProjecs_DoWork;
            backworkerFetchProjecs.RunWorkerCompleted += backworkerFetchProjecs_RunWorkerCompleted;
        }

        public void FillSelectedProject(List<JiraProject> selectedProjects)
        {
            SelectedJiraProjects = selectedProjects;
        }

        private void FormExplorerProjects_Load(object sender, EventArgs e)
        {
            wmtxtSearchbox.Text = String.Empty;
            OriginalJiraProjects = new List<JiraProject>();
            ForSelectedJiraProjects = new List<JiraProject>();
            SearchSource = new List<JiraProject>();
            backworkerFetchProjecs.RunWorkerAsync();
        }

        void backworkerFetchProjecs_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ResolveControlsStatus(false);
            if (OriginalJiraProjects != null)
            {
                foreach (var project in OriginalJiraProjects)
                {
                    ForSelectedJiraProjects.Add(project);
                    SearchSource.Add(project);
                }

                if (SelectedJiraProjects != null)
                {
                    foreach (var project in SelectedJiraProjects)
                    {
                        var index =
                            ForSelectedJiraProjects.FindIndex(
                                p => p.Name.Equals(project.Name, StringComparison.InvariantCultureIgnoreCase));
                        if (index > -1)
                        {
                            ForSelectedJiraProjects.RemoveAt(index);
                            SearchSource.RemoveAt(index);
                        }
                    }
                }
            }
            RebindingDataSource();
        }

        void backworkerFetchProjecs_DoWork(object sender, DoWorkEventArgs e)
        {
            ResolveControlsStatus(true);
            try
            {
                OriginalJiraProjects = JiraOperator.GetProjects();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ResolveControlsStatus(bool busy)
        {
            if (InvokeRequired)
            {
                Action<bool> p = ResolveControlsStatus;
                Invoke(p, busy);
                return;
            }
            lsbSelectedProjects.Enabled = !busy;
            lsbSourceProjects.Enabled = !busy;
            btnRemove.Enabled = !busy;
            btnSelect.Enabled = !busy;
            btnOK.Enabled = !busy;
            btnUpSelected.Enabled = !busy;
            btnDownSelected.Enabled = !busy;
            wmtxtSearchbox.Enabled = !busy;
            prgFechProjects.Visible = busy;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            var selectedItem = lsbSourceProjects.SelectedItems;
            if (selectedItem.Count > 0)
            {
                foreach (var item in selectedItem)
                {
                    var project = item as JiraProject;
                    SelectedJiraProjects.Add(project);
                    var forIndex = ForSelectedJiraProjects.IndexOf(project);
                    var searchIndex = SearchSource.IndexOf(project);
                    if (forIndex != -1)
                    {
                        ForSelectedJiraProjects.RemoveAt(forIndex);
                    }
                    if (searchIndex != -1)
                    {
                        SearchSource.RemoveAt(searchIndex);
                    }

                }
                RebindingDataSource();
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            var selectedItem = lsbSelectedProjects.SelectedItems;
            if (selectedItem.Count > 0)
            {
                foreach (var item in selectedItem)
                {
                    var project = item as JiraProject;
                    SearchSource.Add(project);
                    wmtxtSearchbox_TextChanged(wmtxtSearchbox.Text, EventArgs.Empty);
                    SelectedJiraProjects.Remove(project);
                }
                RebindingDataSource();
            }
        }

        private void btnUpSelected_Click(object sender, EventArgs e)
        {
            var selectedIndeies = lsbSelectedProjects.SelectedIndices;
            if (selectedIndeies.Count > 0)
            {
                var first = selectedIndeies[0];
                if (first > 0)
                {
                    var temp = SelectedJiraProjects[first - 1];
                    SelectedJiraProjects[first - 1] = SelectedJiraProjects[first];
                    SelectedJiraProjects[first] = temp;
                    RebindingSelectedDataSource();
                }
            }
        }

        private void btnDownSelected_Click(object sender, EventArgs e)
        {
            var selectedIndeies = lsbSelectedProjects.SelectedIndices;
            if (selectedIndeies.Count > 0)
            {
                var first = selectedIndeies[0];
                if (first < SelectedJiraProjects.Count - 1)
                {
                    var temp = SelectedJiraProjects[first + 1];
                    SelectedJiraProjects[first + 1] = SelectedJiraProjects[first];
                    SelectedJiraProjects[first] = temp;
                    RebindingSelectedDataSource();
                }
            }
        }

        private void RebindingDataSource()
        {
            if (InvokeRequired)
            {
                Action p = RebindingDataSource;
                Invoke(p);
                return;
            }
            RebindingForSelectedDataSource();
            RebindingSelectedDataSource();
        }

        private void RebindingSelectedDataSource()
        {
            lsbSelectedProjects.DataSource = null;
            lsbSelectedProjects.DataSource = SelectedJiraProjects;
            lsbSelectedProjects.DisplayMember = "Name";
            lsbSelectedProjects.ValueMember = "Key";

        }
        private void RebindingForSelectedDataSource()
        {
            lsbSourceProjects.DataSource = null;
            lsbSourceProjects.DataSource = ForSelectedJiraProjects;
            lsbSourceProjects.DisplayMember = "Name";
            lsbSourceProjects.ValueMember = "Key";
        }

        private void wmtxtSearchbox_TextChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(wmtxtSearchbox.Text))
            {
                ForSelectedJiraProjects.Clear();
                if (SearchSource != null)
                {
                    foreach (var project in SearchSource)
                    {
                        ForSelectedJiraProjects.Add(project);
                    }
                }
            }
            else
            {
                var condition = SearchSource.Where(o => o.Name.ToLower().Contains(wmtxtSearchbox.Text.ToLower())).ToList();
                ForSelectedJiraProjects = condition;
            }
            RebindingForSelectedDataSource();
        }

    }
}
