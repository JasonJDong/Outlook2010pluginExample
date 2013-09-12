using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Newegg.Outlook.JIRA.Add_in
{
    public class JiraUser
    {
        [JsonProperty("name")]
        public String Name { get; set; }

        [JsonProperty("emailAddress")]
        public String EmailAddress { get; set; }

        [JsonProperty("displayName")]
        public String DisplayName { get; set; }

        [JsonProperty("active")]
        public Boolean Active { get; set; }
    }
}
