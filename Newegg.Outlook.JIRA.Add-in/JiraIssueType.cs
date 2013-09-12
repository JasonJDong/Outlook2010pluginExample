using System;
using Newtonsoft.Json;

namespace Newegg.Outlook.JIRA.Add_in
{
    public class JiraIssueType
    {
        [JsonProperty("id")]
        public String ID { get; set; }

        [JsonProperty("description")]
        public String Description { get; set; }

        [JsonProperty("name")]
        public String Name { get; set; }

        [JsonProperty("subTask")]
        public bool IsSubTask { get; set; }
    }
}
