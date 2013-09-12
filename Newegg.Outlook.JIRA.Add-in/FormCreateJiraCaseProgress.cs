using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Newegg.Outlook.JIRA.Add_in
{
    public partial class FormCreateJiraCaseProgress : Form
    {
        private const String FINISHED_COUNT_FORMAT = "{0} / {1} Finished...";

        private List<JiraIssueCreateStatus> m_FinishedIssues = new List<JiraIssueCreateStatus>();
        public List<JiraIssueCreateStatus> FinishedIssues
        {
            get { return m_FinishedIssues; }
            set
            {
                m_FinishedIssues = value;
                SafeUpdateProgress(value.Count);
                ReBindingFinishedItemView();
                FinishedCount();
                if (value.Count >= TotalCount)
                {
                    SafeUpdateButton();
                    SafeUpdateProgress(-1);
                }
            }
        }

        public CancellationTokenSource CancellationTokenSource { get; set; }

        public int TotalCount { get; set; }

        public FormCreateJiraCaseProgress(int totalCount, CancellationTokenSource cancellation)
        {
            InitializeComponent();
            gridFinishedIssueView.AutoGenerateColumns = false;
            Init(totalCount, cancellation);
        }

        public void Init(int totalCount, CancellationTokenSource cancellation)
        {
            if (InvokeRequired)
            {
                Action<int, CancellationTokenSource> p = Init;
                Invoke(p, totalCount, cancellation);
                return;
            }
            TotalCount = totalCount;
            if (totalCount > 0)
            {
                prgCreateProgress.Step = 100 / totalCount;
            }
            btnCancel.Text = "Cancel";
            CancellationTokenSource = cancellation;
            FinishedIssues = new List<JiraIssueCreateStatus>();
        }

        public void AddFinishedIssue(JiraIssueCreateStatus issue)
        {
            var temp = FinishedIssues;
            temp.Insert(0, issue);
            FinishedIssues = temp;
        }

        private void ReBindingFinishedItemView()
        {
            if (gridFinishedIssueView.InvokeRequired)
            {
                Action p = ReBindingFinishedItemView;
                gridFinishedIssueView.Invoke(p);
                return;
            }

            gridFinishedIssueView.Rows.Clear();
            foreach (var issue in FinishedIssues)
            {
                var row = new object[] { issue.Image, issue.IssueName, issue.ErrorMessage };
                gridFinishedIssueView.Rows.Add(row);
            }
            //MS's Bug?
            //gridFinishedIssueView.DataSource = null;
            //gridFinishedIssueView.DataSource = FinishedIssues;
        }

        private void FinishedCount()
        {
            if (lblProgress.InvokeRequired)
            {
                Action p = FinishedCount;
                lblProgress.Invoke(p);
                return;
            }
            lblProgress.Text = String.Format(FINISHED_COUNT_FORMAT, FinishedIssues.Count, TotalCount);
        }

        private void SafeUpdateProgress(int value)
        {
            if (prgCreateProgress.InvokeRequired)
            {
                Action<int> p = SafeUpdateProgress;
                prgCreateProgress.Invoke(p, value);
                return;
            }
            prgCreateProgress.Value = prgCreateProgress.Step * value > 100 || value == -1 ? 100 : prgCreateProgress.Step * value;
        }

        private void SafeUpdateButton()
        {
            if (btnCancel.InvokeRequired)
            {
                Action p = SafeUpdateButton;
                btnCancel.Invoke(p);
                return;
            }
            btnCancel.Tag = new object();
            btnCancel.Text = "Close";
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (btnCancel.Tag == null)
            {
                CancellationTokenSource.Cancel();
                btnCancel.Tag = new object();
                btnCancel.Text = "Close";
            }
            else
            {
                Close();
            }
        }

    }
}
