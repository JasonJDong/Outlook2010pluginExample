using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Newegg.Outlook.JIRA.Add_in
{
    public class JiraOperator
    {
        private String m_CurrentCookie;

        private readonly Regex m_CookieRegex = new Regex("JSESSIONID=\\w+");

        public JiraUserAuthModel User { get; set; }

        public String BaseJiraUrl { get; set; }

        public JiraOperator(JiraUserAuthModel user)
        {
            User = user;
        }

        private bool UserAuth(JiraUserAuthModel userInfo, out String cookie, out String errMsg)
        {
            string jiraUri = BaseJiraUrl + "/rest/auth/1/session";
            try
            {
                var response = Utils.CreatePostHttpResponse(jiraUri, userInfo, 30000, null, Encoding.UTF8, null);
                var cookieString = response.GetResponseHeader("Set-Cookie");
                var cookieMath = m_CookieRegex.Match(cookieString);
                if (cookieMath.Success)
                {
                    cookieString = cookieMath.Value;
                }
                cookie = cookieString;
                errMsg = String.Empty;
                return true;
            }
            catch (WebException webEx)
            {
                cookie = String.Empty;
                errMsg = webEx.Message;
                return false;
            }
            catch (Exception e)
            {
                cookie = String.Empty;
                errMsg = e.Message;
                return false;
            }
        }

        public Boolean CreateIssue(JiraIssueModel issue, out String errMsg, String extraData = null)
        {
            if (String.IsNullOrWhiteSpace(m_CurrentCookie) && User != null)
            {
                if (!UserAuth(User, out m_CurrentCookie, out errMsg))
                {
                    return false;
                }
            }

            string jiraUri = BaseJiraUrl + "/rest/api/2/issue";
            try
            {
                String postData = Utils.Object2Json(issue);
                if (!String.IsNullOrWhiteSpace(extraData))
                {
                    var lastPosition = postData.LastIndexOf(',');
                    postData = postData.Insert(lastPosition, "," + extraData);
                }

                var response = Utils.CreatePostHttpResponse(jiraUri, postData, 30000, null, Encoding.UTF8, m_CurrentCookie);
                if (response != null)
                {
                    using (var stream = response.GetResponseStream())
                    {
                        if (stream != null)
                        {
                            using (var sr = new StreamReader(stream))
                            {
                                var resString = sr.ReadToEnd();
                                if (resString.Contains("id"))
                                {
                                    errMsg = String.Empty;
                                    return true;
                                }
                                var error = Utils.Json2Object<JiraErrorModel>(resString);
                                if (error != null)
                                {
                                    errMsg = String.Join(Environment.NewLine, error.ErrorMessages.ToArray());
                                    return false;
                                }
                            }
                        }
                    }
                }
                errMsg = "Connect failed.";
                return false;
            }
            catch (WebException e)
            {
                if (e.Response != null)
                {
                    using (var stream = e.Response.GetResponseStream())
                    {
                        if (stream != null)
                        {
                            using (var sr = new StreamReader(stream))
                            {
                                var resString = sr.ReadToEnd();
                                var error = Utils.Json2Object<JiraErrorModel>(resString);
                                if (error.ErrorMessages == null || error.ErrorMessages.Count == 0)
                                {
                                    errMsg = error.Errors != null ? String.Join(";", error.Errors.Values) : "Bad Request";
                                }
                                else
                                {
                                    errMsg = String.Join("\r\n", error.ErrorMessages.ToArray());
                                }
                                return false;
                            }
                        }
                    }
                }
                errMsg = e.Message;
                return false;
            }
            catch (Exception e)
            {
                errMsg = e.Message;
                return false;
            }
        }

        public List<JiraIssueType> GetIssueTypes()
        {
            if (String.IsNullOrWhiteSpace(m_CurrentCookie) && User != null)
            {
                String errMsg;
                if (!UserAuth(User, out m_CurrentCookie, out errMsg))
                {
                    throw new Exception(errMsg);
                }
            }

            string jiraUri = BaseJiraUrl + "/rest/api/2/issuetype";
            var response = Utils.CreateGetHttpResponse(jiraUri, 30000, null, m_CurrentCookie);
            List<JiraIssueType> issueTypes = null;
            ReadResponse(response, result =>
            {
                if (!String.IsNullOrWhiteSpace(result))
                {
                    issueTypes = Utils.Json2Object<List<JiraIssueType>>(result);
                }
            });
            return issueTypes;
        }

        public List<JiraPriority> GetPriorities()
        {
            if (String.IsNullOrWhiteSpace(m_CurrentCookie) && User != null)
            {
                String errMsg;
                if (!UserAuth(User, out m_CurrentCookie, out errMsg))
                {
                    throw new Exception(errMsg);
                }
            }

            string jiraUri = BaseJiraUrl + "/rest/api/2/priority";
            var response = Utils.CreateGetHttpResponse(jiraUri, 30000, null, m_CurrentCookie);
            List<JiraPriority> priorities = null;
            ReadResponse(response, result =>
            {
                if (!String.IsNullOrWhiteSpace(result))
                {
                    priorities = Utils.Json2Object<List<JiraPriority>>(result);
                }
            });
            return priorities;
        }

        public List<JiraProject> GetProjects()
        {
            if (String.IsNullOrWhiteSpace(m_CurrentCookie) && User != null)
            {
                String errMsg;
                if (!UserAuth(User, out m_CurrentCookie, out errMsg))
                {
                    throw new Exception(errMsg);
                }
            }

            string jiraUri = BaseJiraUrl + "/rest/api/2/project";
            var response = Utils.CreateGetHttpResponse(jiraUri, 30000, null, m_CurrentCookie);
            List<JiraProject> projects = null;
            ReadResponse(response, result =>
            {
                if (!String.IsNullOrWhiteSpace(result))
                {
                    projects = Utils.Json2Object<List<JiraProject>>(result);
                }
            });
            return projects;
        }

        public List<JiraUser> GetUser(String user)
        {
            if (String.IsNullOrWhiteSpace(m_CurrentCookie) && User != null)
            {
                String errMsg;
                if (!UserAuth(User, out m_CurrentCookie, out errMsg))
                {
                    throw new Exception(errMsg);
                }
            }

            string jiraUri = BaseJiraUrl + "/rest/api/2/user/search?username={0}&startAt=0&maxResults=10";
            var userUri = String.Format(jiraUri, user);
            var response = Utils.CreateGetHttpResponse(userUri, 30000, null, m_CurrentCookie);
            List<JiraUser> resUser = null;
            ReadResponse(response, result =>
            {
                if (!String.IsNullOrWhiteSpace(result))
                {
                    resUser = Utils.Json2Object<List<JiraUser>>(result);
                }
            });
            return resUser;
        }

        public JiraCreateIssueMeta GetIssueCreateMeta(String prjectId, String issueTypeName)
        {
            if (String.IsNullOrWhiteSpace(m_CurrentCookie) && User != null)
            {
                String errMsg;
                if (!UserAuth(User, out m_CurrentCookie, out errMsg))
                {
                    throw new Exception(errMsg);
                }
            }

            string jiraUri = BaseJiraUrl + "/rest/api/2/issue/createmeta?projectIds={0}&issuetypeNames={1}&expand=projects.issuetypes.fields";
            var metaUri = String.Format(jiraUri, prjectId, issueTypeName);
            var response = Utils.CreateGetHttpResponse(metaUri, 30000, null, m_CurrentCookie);
            JiraCreateIssueMetaCollection projects = null;
            ReadResponse(response, result =>
            {
                if (!String.IsNullOrWhiteSpace(result))
                {
                    projects = Utils.Json2Object<JiraCreateIssueMetaCollection>(result);
                }
            });
            if (projects != null && projects.Projects != null)
            {
                return projects.Projects.FirstOrDefault();
            }
            return null;
        }

        private void ReadResponse(HttpWebResponse response, Action<String> callback)
        {
            if (response != null)
            {
                using (var stream = response.GetResponseStream())
                {
                    if (stream != null)
                    {
                        using (var sr = new StreamReader(stream))
                        {
                            var resString = sr.ReadToEnd();
                            if (callback != null)
                            {
                                callback(resString);
                            }
                        }
                    }
                    else if (callback != null)
                    {
                        callback(String.Empty);
                    }
                }
            }
            else if (callback != null)
            {
                callback(String.Empty);
            }
        }
    }
}
