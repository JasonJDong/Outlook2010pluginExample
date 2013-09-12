using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Newegg.Outlook.JIRA.Add_in
{
    public class JiraProjectsStoreUtil
    {
        public static JiraProjectCollection ReadProjects()
        {
            using (var fs = new FileStream(GetJiraProjectsFilePath(), FileMode.OpenOrCreate, FileAccess.Read))
            {
                var xmlReader = new System.Xml.Serialization.XmlSerializer(typeof(JiraProjectCollection));
                try
                {
                    var projects = xmlReader.Deserialize(fs) as JiraProjectCollection;
                    if (projects != null)
                    {
                        return projects;
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }

        public static void WriteProjects(JiraProjectCollection projects)
        {
            using (var fs = new FileStream(GetJiraProjectsFilePath(), FileMode.Truncate, FileAccess.Write))
            {
                var xmlReader = new System.Xml.Serialization.XmlSerializer(typeof(JiraProjectCollection));
                try
                {
                    xmlReader.Serialize(fs, projects);
                }
                catch (Exception e)
                {
                }
            }
        }

        public static bool CreateFile()
        {
            var dirPath = GetJiraProjectsDirectoryPath();
            if (!Directory.Exists(dirPath))
            {
                try
                {
                    Directory.CreateDirectory(dirPath);
                }
                catch (Exception)
                {
                    return false;
                }
            }
            var filePath = GetJiraProjectsFilePath();
            if (!File.Exists(filePath))
            {
                try
                {
                    var fs = File.Create(filePath);
                    fs.Close();
                }
                catch (Exception)
                {
                    return false;
                }
              
            }
            return true;
        }

        public static String GetJiraProjectsDirectoryPath()
        {
            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var projectsFolder = Path.Combine(userProfile, "Outlook4Jira");
            return projectsFolder;
        }

        public static String GetJiraProjectsFilePath()
        {
            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var projectsFolder = Path.Combine(userProfile, "Outlook4Jira");
            var projPath = Path.Combine(projectsFolder, "projects.xml");
            return projPath;
        }
    }

    public class JiraProjectCollection
    {
        public List<JiraProject> JiraProjects { get; set; }

        public JiraProject CurrentAssignProject { get; set; }

        public Boolean CreateJiraCaseAfterSent { get; set; }

        public JiraIssueType CurrentIssueType { get; set; }

        public String BaseJiraUrl { get; set; }

        public ProjectIssueRequiredFieldsMapCollection ProjectIssueRequiredFieldsMaps { get; set; }
    }
}
