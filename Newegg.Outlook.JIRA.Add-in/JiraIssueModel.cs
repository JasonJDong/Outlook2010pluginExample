using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Newegg.Outlook.JIRA.Add_in
{
    public class JiraIssueModel
    {
        [JsonProperty("fields")]
        public Fields Fields { get; set; }

        [JsonIgnore]
        public Boolean IsIssue { get; set; }

        public JiraIssueModel()
        {
            IsIssue = true;
        }
    }

    public class Fields
    {
        [JsonProperty("project")]
        public IdentfierDescriptor Project { get; set; }

        [JsonProperty("summary")]
        public String Summary { get; set; }

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public String Description { get; set; }

        [JsonProperty("issuetype")]
        public IdentfierDescriptor IssueType { get; set; }

        [JsonProperty("assignee", NullValueHandling = NullValueHandling.Ignore)]
        public IdentfierDescriptor Assignee { get; set; }

        [JsonProperty("labels", NullValueHandling = NullValueHandling.Ignore)]
        public List<String> Labels { get; set; }

        [JsonProperty("reporter", NullValueHandling = NullValueHandling.Ignore)]
        public IdentfierDescriptor Reporter { get; set; }

        [JsonProperty("priority", NullValueHandling = NullValueHandling.Ignore)]
        public IdentfierDescriptor Priority { get; set; }

        [JsonProperty("fixVersions", NullValueHandling = NullValueHandling.Ignore)]
        public List<String> FixVersions { get; set; }

        [JsonProperty("components", NullValueHandling = NullValueHandling.Ignore)]
        public List<String> Components { get; set; }
    }

    public class IdentfierDescriptor
    {
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public String Name { get; set; }

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public String ID { get; set; }

        [JsonProperty("key", NullValueHandling = NullValueHandling.Ignore)]
        public String Key { get; set; }
    }
}
