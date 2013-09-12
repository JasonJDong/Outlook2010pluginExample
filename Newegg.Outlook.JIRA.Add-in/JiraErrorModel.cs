using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Newegg.Outlook.JIRA.Add_in
{
    public class JiraErrorModel
    {
        public List<String> ErrorMessages { get; set; }

        public Dictionary<String, String> Errors { get; set; }
    }
}
