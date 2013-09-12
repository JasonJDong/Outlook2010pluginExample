using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Newegg.Outlook.JIRA.Add_in
{
    public class JiraIssueCreateStatus
    {
        public JiraIssueModel Issue { get; set; }

        public String IssueName
        {
            get { return Issue.Fields.Summary; }
        }

        public Boolean Success { get; set; }

        public String ErrorMessage { get; set; }

        public Bitmap Image { get { return Success ? Properties.Resources.success : Properties.Resources.fail; } }
    }
}
