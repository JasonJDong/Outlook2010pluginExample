using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Newegg.Outlook.JIRA.Add_in
{
    public class JiraUserAuthModel
    {
        [JsonProperty("username")]
        public String User { get; set; }

        [JsonProperty("password")]
        public String Password { get; set; }
    }
}
