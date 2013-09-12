using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Newegg.Outlook.JIRA.Add_in
{
    public class JiraUserAuthUtil
    {
        public static Boolean IsJiraAuthExists()
        {
            var userAuthFolder = GetJiraUserAuthDirectoryPath();
            var userDirectory = new DirectoryInfo(userAuthFolder);
            if (!userDirectory.Exists)
            {
                return false;
            }
            var userAuthPath = GetJiraUserAuthFilePath();
            var userAuthFile = new FileInfo(userAuthPath);
            if (!userAuthFile.Exists)
            {
                return false;
            }
            using (var fs = userAuthFile.Open(FileMode.Open, FileAccess.Read))
            {
                var xmlReader = new System.Xml.Serialization.XmlSerializer(typeof(JiraUserAuthModel));
                try
                {
                    var userAuthInfo = xmlReader.Deserialize(fs) as JiraUserAuthModel;
                    if (userAuthInfo != null)
                    {
                        return !String.IsNullOrWhiteSpace(userAuthInfo.User) & !String.IsNullOrWhiteSpace(userAuthInfo.Password);
                    }
                }
                catch (Exception)
                {
                    return false;
                }

            }
            return true;
        }

        public static String GetJiraUserAuthDirectoryPath()
        {
            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var userAuthFolder = Path.Combine(userProfile, "Outlook4Jira");
            return userAuthFolder;
        }

        public static bool CreateJiraUserAuthFile()
        {
            try
            {
                var userAuthFolder = GetJiraUserAuthDirectoryPath();
                var userDirectory = new DirectoryInfo(userAuthFolder);
                if (!userDirectory.Exists)
                {
                    userDirectory.Create();
                }
                var userAuthPath = GetJiraUserAuthFilePath();
                var userAuthFile = new FileInfo(userAuthPath);
                if (!userAuthFile.Exists)
                {
                    var fs = userAuthFile.Create();
                    fs.Close();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static String GetJiraUserAuthFilePath()
        {
            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var userAuthFolder = Path.Combine(userProfile, "Outlook4Jira");
            var userAuthPath = Path.Combine(userAuthFolder, ".profile");
            return userAuthPath;
        }

        public static JiraUserAuthModel ReadJiraUserAuthInfo()
        {
            using (var fs = new FileStream(GetJiraUserAuthFilePath(), FileMode.Open, FileAccess.Read))
            {
                var xmlReader = new System.Xml.Serialization.XmlSerializer(typeof(JiraUserAuthModel));
                try
                {
                    var userAuthInfo = xmlReader.Deserialize(fs) as JiraUserAuthModel;
                    if (userAuthInfo != null)
                    {
                        userAuthInfo.Password = Utils.Decrypt(userAuthInfo.Password);
                        return userAuthInfo;
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }

        public static void WriteJiraUserAuthInfo(JiraUserAuthModel user)
        {
            using (var fs = new FileStream(GetJiraUserAuthFilePath(), FileMode.Truncate, FileAccess.Write))
            {
                var xmlReader = new System.Xml.Serialization.XmlSerializer(typeof(JiraUserAuthModel));
                try
                {
                    var writeUser = new JiraUserAuthModel {User = user.User, Password = user.Password};
                    writeUser.Password = Utils.Encrypt(writeUser.Password);
                    xmlReader.Serialize(fs, writeUser);
                }
                catch (Exception)
                {

                }
            }
        }
    }
}
