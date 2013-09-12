using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Newegg.Outlook.JIRA.Add_in
{
    public class JiraProject
    {
        [JsonProperty("id")]
        public String ID { get; set; }

        [JsonProperty("key")]
        public String Key { get; set; }

        [JsonProperty("name")]
        public String Name { get; set; }

        public JiraProject Clone()
        {
            return new JiraProject
                {
                    ID = ID,
                    Key = Key,
                    Name = Name
                };
        }

        public override bool Equals(object obj)
        {
            if ((obj as JiraProject) == null)
            {
                return false;
            }
            return String.Equals((obj as JiraProject).Name, Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
