using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Newegg.Outlook.JIRA.Add_in
{
    public class JiraCreateIssueMetaCollection
    {
        [JsonProperty("expand")]
        public String Expand { get; set; }

        [JsonProperty("projects")]
        public List<JiraCreateIssueMeta> Projects { get; set; }
    }

    public class JiraCreateIssueMeta : JiraProject
    {
        [JsonProperty("issuetypes", NullValueHandling = NullValueHandling.Ignore)]
        public List<JiraMetaIssueType> IssueTypes { get; set; }
    }

    public class JiraMetaIssueType : JiraIssueType
    {
        [JsonProperty("fields")]
        public Dictionary<String, JiraField> Fields { get; set; }
    }
}
