using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Newegg.Outlook.JIRA.Add_in
{
    public class JiraPriority
    {
        [JsonProperty("id")]
        public String ID { get; set; }

        [JsonProperty("description")]
        public String Description { get; set; }

        [JsonProperty("name")]
        public String Name { get; set; }
    }
}
