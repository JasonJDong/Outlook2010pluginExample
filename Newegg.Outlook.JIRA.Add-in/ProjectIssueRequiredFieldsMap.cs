using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Newegg.Outlook.JIRA.Add_in
{
    public class ProjectIssueRequiredFieldsMapCollection
    {
        public List<ProjectIssueRequiredFieldsMap> Maps { get; set; }

        public ProjectIssueRequiredFieldsMapCollection()
        {
            Maps = new List<ProjectIssueRequiredFieldsMap>();
        }

        public ProjectIssueRequiredFieldsMap AddOne(JiraProject projectId)
        {
            var map = new ProjectIssueRequiredFieldsMap
                {
                    Project = new IdentfierDescriptor
                        {
                            ID = projectId.ID,
                            Key = projectId.Key,
                            Name = projectId.Name
                        }
                };
            Maps.Add(map);
            return map;
        }
    }

    public class ProjectIssueRequiredFieldsMap
    {
        public IdentfierDescriptor Project { get; set; }

        public List<IssueRequiredFieldsMap> Issues { get; set; }

        public void AddUniqueIssue(IssueRequiredFieldsMap issue)
        {
            if (Issues == null)
            {
                Issues = new List<IssueRequiredFieldsMap>();
            }
            var findIndex = Issues.FindIndex(i => String.Equals(i.IssueType.Name, issue.IssueType.Name, StringComparison.OrdinalIgnoreCase));
            if (findIndex == -1)
            {
                Issues.Add(issue);
            }
            else
            {
                Issues[findIndex] = issue;
            }
        }
    }

    public class IssueRequiredFieldsMap
    {
        public JiraIssueType IssueType { get; set; }

        public String RequiredFieldsJsonValue { get; set; }

        public Boolean NoRequiredFields { get; set; }

        public List<JiraField> SystemField { get; set; }

    }
}
