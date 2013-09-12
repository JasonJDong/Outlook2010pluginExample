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
    public partial class FormWaiting : Form
    {
        public const string WAITING_STRING = "Waiting";
        public const string WAITING_DOT = ".";

        public Task<Object> WaitingForTask { get; set; }

        public Object Result { get; set; }

        public FormWaiting(Task<Object> task)
        {
            WaitingForTask = task;
            WaitingForTask.ContinueWith(SafeClose);
            InitializeComponent();
            timer.Tick += timer_Tick;
            lblWaiting.Tag = 0;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            SafeUpdateWaitingInfo();
        }

        private void SafeClose(Task<Object> task)
        {
            if (InvokeRequired)
            {
                Action<Task<Object>> p = SafeClose;
                Invoke(p, task);
                return;
            }
            Result = task.Result;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void FormWaiting_Load(object sender, EventArgs e)
        {
            timer.Start();
            if (WaitingForTask != null)
            {
                WaitingForTask.Start();
            }
            else
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
            lblWaiting.Location = new Point((Width - lblWaiting.Width) / 2, (Height - lblWaiting.Height) / 2);
        }

        private void SafeUpdateWaitingInfo()
        {
            if (InvokeRequired)
            {
                Action p = SafeUpdateWaitingInfo;
                Invoke(p);
                return;
            }
            var dots = new StringBuilder(3);
            var count = (int)lblWaiting.Tag;
            for (int i = 0; i < count; i++)
            {
                dots.Append(WAITING_DOT);
            }
            lblWaiting.Text = WAITING_STRING + dots;
            lblWaiting.Tag = ++count > 3 ? 0 : count;
        }

        private void FormWaiting_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (timer.Enabled)
            {
                timer.Stop();
            }
        }
    }
}
