using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Newegg.Outlook.JIRA.Add_in
{
    public class JiraField
    {
        [JsonProperty("required")]
        public Boolean Required { get; set; }

        [JsonProperty("schema")]
        public JiraFieldSchema Schema { get; set; }

        [JsonProperty("name")]
        public String Name { get; set; }

        [JsonProperty("allowedValues")]
        [System.Xml.Serialization.XmlIgnore]
        public List<dynamic> AllowedValues { get; set; }
    }

    public class JiraFieldSchema
    {
        [JsonProperty("type")]
        public String Type { get; set; }

        [JsonProperty("system")]
        public String System { get; set; }

        [JsonProperty("custom")]
        public String Custom { get; set; }

        [JsonProperty("customId")]
        public String CustomID { get; set; }
    }

    public class JiraAllowedValue
    {
        [JsonProperty("value")]
        public String Value { get; set; }

        [JsonProperty("id")]
        public String ID { get; set; }
    }
}
