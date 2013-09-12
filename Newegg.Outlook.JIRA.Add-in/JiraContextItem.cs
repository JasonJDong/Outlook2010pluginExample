using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Office.Interop.Outlook;
using Microsoft.Office.Tools.Ribbon;
using Application = Microsoft.Office.Interop.Outlook.Application;
using Exception = System.Exception;
using Office = Microsoft.Office.Core;

// TODO:  Follow these steps to enable the Ribbon (XML) item:

// 1: Copy the following code block into the ThisAddin, ThisWorkbook, or ThisDocument class.

//  protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
//  {
//      return new JiraContextItem();
//  }

// 2. Create callback methods in the "Ribbon Callbacks" region of this class to handle user
//    actions, such as clicking a button. Note: if you have exported this Ribbon from the Ribbon designer,
//    move your code from the event handlers to the callback methods and modify the code to work with the
//    Ribbon extensibility (RibbonX) programming model.

// 3. Assign attributes to the control tags in the Ribbon XML file to identify the appropriate callback methods in your code.  

// For more information, see the Ribbon XML documentation in the Visual Studio Tools for Office Help.


namespace Newegg.Outlook.JIRA.Add_in
{
    [ComVisible(true)]
    public class JiraContextItem : Office.IRibbonExtensibility
    {
        #region Field and Properties

        public const string JIRA_CASE_DESCRIPTION_START = "##Jira Issue Description Start##";
        public const string JIRA_CASE_DESCRIPTION_END = "##Jira Issue Description End##";
#if DEBUG
        public const string JIRA_BASE_URL_NEWEGG = "http://localhost:2990/jira";
#else
        public const string JIRA_BASE_URL_NEWEGG = "http://jira";

#endif
        private readonly List<MailItem> m_MailItems = new List<MailItem>();
        private Office.IRibbonUI ribbon;

        private String m_SignatureHtml = String.Empty;
        private String m_SignaturePlainText = String.Empty;
        private bool m_UserHaveFileAccessPermisson = true;
        private readonly Regex m_RegexBodyLabel = new Regex("<div[^>]*>");
        private readonly Regex m_RegexSummary = new Regex("(?<=\\([^)]*\\)).*");
        private FormPermissonUser m_FormPermissonUser;
        private FormCreateJiraCaseProgress m_FormCreateJiraCaseProgress;
        private FormExplorerProjects m_FormExplorerProjects;
        private FormInspectorSelectSingleProject m_FormInspectorSelectSingleProject;

        private Application m_Application;
        public Application Application
        {
            get { return m_Application; }
            set
            {
                m_Application = value;
                ((ApplicationEvents_11_Event)m_Application).Quit -= Application_Quit;
                ((ApplicationEvents_11_Event)m_Application).Quit += Application_Quit;
            }
        }

        public JiraUserAuthModel JiraUserAuth { get; set; }

        public JiraOperator JiraOperator { get; set; }

        public List<JiraProject> SelectedProjects { get; private set; }

        public JiraProject CurrentAssignProject { get; set; }

        public bool IsCreateJiraCaseAfterSent { get; set; }

        public JiraIssueType CurrentSelectedIssueType { get; set; }

        public String BaseJiraUrl { get; set; }

        public ProjectIssueRequiredFieldsMapCollection ProjectIssueRequiredFieldsMaps { get; set; }

        #endregion

        #region Constructor

        public JiraContextItem()
        {
        }

        #endregion

        #region Application Events

        private void Application_Quit()
        {
            JiraProjectsStoreUtil.WriteProjects(new JiraProjectCollection()
            {
                CreateJiraCaseAfterSent = IsCreateJiraCaseAfterSent,
                CurrentAssignProject = CurrentAssignProject,
                JiraProjects = SelectedProjects,
                CurrentIssueType = CurrentSelectedIssueType,
                BaseJiraUrl = BaseJiraUrl,
                ProjectIssueRequiredFieldsMaps = ProjectIssueRequiredFieldsMaps
            });
        }

        #endregion

        #region Jira Issue Button

        public void OnJiraCaseClick(Office.IRibbonControl control)
        {
            CreateMailItem();
        }

        public Bitmap GetCustomImage(Office.IRibbonControl control)
        {
            return new Bitmap(Properties.Resources.jira);
        }

        #endregion

        #region Context Menu
        public void OnBatchCreateJiraCase(Office.IRibbonControl control)
        {
            ConfirmBatchCreateJiraCase();
        }
        #endregion

        #region Mail Events

        private void JiraContextItem_Send(ref bool Cancel)
        {
            if (IsCreateJiraCaseAfterSent)
            {
                if (!CheckVaild()) return;
                var currentMail = Application.ActiveInspector().CurrentItem as MailItem;
                if (currentMail != null)
                {
                    var issue = CookIssue(currentMail);
                    if (issue.IsIssue)
                    {
                        var task = new Task<String>(() => CreateIssueToJira(issue));
                        task.ContinueWith(t =>
                            {
                                if (String.IsNullOrWhiteSpace(t.Result))
                                {
                                    MessageBox.Show("Jira case created successfully!", "Information", MessageBoxButtons.OK,
                                                    MessageBoxIcon.Information);
                                }
                                else
                                {
                                    MessageBox.Show("Jira case created failed: " + t.Result, "Error", MessageBoxButtons.OK,
                                                   MessageBoxIcon.Error);
                                }
                            });
                        task.Start();
                    }
                }
            }
        }

        private JiraIssueModel CookIssue(MailItem currentMail)
        {
            var issue = new JiraIssueModel();
            var desc = GetBodyDescription(currentMail.Body);
            if (String.IsNullOrWhiteSpace(desc))
            {
                issue.IsIssue = false;
            }
            var match = m_RegexSummary.Match(currentMail.Subject);
            string summary = match.Success ? match.Value : currentMail.Subject;

            IdentfierDescriptor assignee = null;
            var firstRecipient = currentMail.Recipients.OfType<Recipient>().FirstOrDefault();
            if (firstRecipient != null && firstRecipient.Resolve())
            {
                try
                {
                    var addressEntry = firstRecipient.AddressEntry;
                    dynamic shortName = addressEntry.GetExchangeUser();
                    assignee = new IdentfierDescriptor { Name = shortName.Alias };
                }
                catch (Exception)
                {
                    assignee = null;
                }
               
            }

            var fields = new Fields
                {
                    Summary = summary,
                    Description = CookIssueField("description", desc),
                    Assignee = CookIssueField("assignee", assignee),
                    IssueType = new IdentfierDescriptor() { Name = CurrentSelectedIssueType == null ? "Case" : CurrentSelectedIssueType.Name },
                    Project = new IdentfierDescriptor() { ID = CurrentAssignProject.ID },
                    Reporter = CookIssueField("reporter", new IdentfierDescriptor { Name = JiraUserAuth.User }),
                    Priority = CookIssueField("priority", new IdentfierDescriptor() { ID = "3" }),
                };
            issue.Fields = fields;
            return issue;
        }

        private T CookIssueField<T>(String name, T shouldValue)
        {
            if (ProjectIssueRequiredFieldsMaps != null && CurrentAssignProject != null && CurrentSelectedIssueType != null)
            {
                var findProject =
                    ProjectIssueRequiredFieldsMaps.Maps.Find(p => String.Equals(CurrentAssignProject.ID, p.Project.ID));
                if (findProject != null)
                {
                    var findIssue = findProject.Issues.Find(i => String.Equals(i.IssueType.Name, CurrentSelectedIssueType.Name,
                                          StringComparison.OrdinalIgnoreCase));
                    if (findIssue != null && findIssue.SystemField != null)
                    {
                        var findSysField = findIssue.SystemField.Find(
                            s => String.Equals(s.Name, name, StringComparison.InvariantCultureIgnoreCase));
                        if (findSysField != null)
                        {
                            return shouldValue;
                        }
                    }
                }
            }
            return default(T);
        }

        private String GetBodyDescription(String body)
        {
            using (var sr = new StringReader(body))
            {
                var sb = new StringBuilder();
                bool begin = false;
                while (sr.Peek() != -1)
                {
                    String line = sr.ReadLine();
                    if (JIRA_CASE_DESCRIPTION_START.Equals(line))
                    {
                        begin = true;
                        continue;
                    }
                    if (JIRA_CASE_DESCRIPTION_END.Equals(line))
                    {
                        return sb.ToString();
                    }
                    if (begin)
                    {
                        sb.AppendLine(line);
                    }
                }
            }
            return String.Empty;
        }

        private void CreateJiraCase(CancellationToken cancel, MailItem mail)
        {
            if (cancel.IsCancellationRequested)
            {
                return;
            }
            if (mail != null)
            {
                var issue = CookIssue(mail);
                var errMsg = issue.IsIssue ? CreateIssueToJira(issue) : "This is not a jira issue mail.";
                var issueCreateStatus = new JiraIssueCreateStatus();
                issueCreateStatus.ErrorMessage = errMsg;
                issueCreateStatus.Success = String.IsNullOrWhiteSpace(errMsg);
                issueCreateStatus.Issue = issue;
                SafeReportProgress(issueCreateStatus);
            }
        }

        private void SingleCreateJiraCase()
        {
            if (!CheckVaild()) return;
            var cancellation = new CancellationTokenSource();
            if (m_FormCreateJiraCaseProgress == null || m_FormCreateJiraCaseProgress.IsDisposed)
            {
                m_FormCreateJiraCaseProgress = new FormCreateJiraCaseProgress(1, cancellation);
            }
            else
            {
                m_FormCreateJiraCaseProgress.Init(1, cancellation);
            }
            SingleCreateJiraCase(cancellation);
            m_FormCreateJiraCaseProgress.Show();
        }

        private void BatchCreateJiraCase()
        {
            if (!CheckVaild()) return;
            var selectionMails = Application.ActiveExplorer().Selection;
            if (selectionMails.Count > 0)
            {
                var mails = selectionMails.OfType<MailItem>().ToList();
                var cancellation = new CancellationTokenSource();
                if (m_FormCreateJiraCaseProgress == null || m_FormCreateJiraCaseProgress.IsDisposed)
                {
                    m_FormCreateJiraCaseProgress = new FormCreateJiraCaseProgress(mails.Count, cancellation);
                }
                else
                {
                    m_FormCreateJiraCaseProgress.Init(mails.Count, cancellation);
                }
                BatchCreateJiraCase(mails, cancellation);
                m_FormCreateJiraCaseProgress.Show();
                m_FormCreateJiraCaseProgress.Focus();
            }
        }

        private bool CheckVaild()
        {
            if (!CheckUserAuth())
            {
                MessageBox.Show("No user authorization infomations!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!CheckCurrentProject())
            {
                MessageBox.Show("No project be chosed!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!CheckRequiredFieldValue())
            {
                return false;
            }

            return true;
        }

        private void SingleCreateJiraCase(CancellationTokenSource cancellation)
        {
            var currentMail = Application.ActiveInspector().CurrentItem as MailItem;
            if (currentMail != null)
            {
                var task = new Task(o => CreateJiraCase(cancellation.Token, (MailItem)o), currentMail);
                task.Start();
            }
        }

        private void BatchCreateJiraCase(List<MailItem> mails, CancellationTokenSource cancellation)
        {
            if (mails.Count > 0)
            {
                var taskFactory = new TaskFactory(cancellation.Token);
                foreach (var mail in mails)
                {
                    taskFactory.StartNew(o => CreateJiraCase(cancellation.Token, (MailItem)o), mail);
                }
            }
        }

        private void SafeReportProgress(JiraIssueCreateStatus issueStatus)
        {
            if (m_FormCreateJiraCaseProgress.InvokeRequired)
            {
                Action<JiraIssueCreateStatus> p = SafeReportProgress;
                m_FormCreateJiraCaseProgress.Invoke(p, issueStatus);
                return;
            }
            if (m_FormCreateJiraCaseProgress == null)
            {
                return;
            }
            m_FormCreateJiraCaseProgress.AddFinishedIssue(issueStatus);
        }

        #endregion

        #region Private Methods
        private void ConfirmBatchCreateJiraCase()
        {
            var dialogResult = MessageBox.Show("Are you sure convert and create jira case(s)?", "Confirm",
                                               MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.OK)
            {
                BatchCreateJiraCase();
            }
        }

        private void ConfirmSingleCreateJiraCase()
        {
            var dialogResult = MessageBox.Show("Are you sure convert and create jira case?", "Confirm",
                                               MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.OK)
            {
                SingleCreateJiraCase();
            }
        }

        private void CreateMailItem()
        {
            var mailItem = (MailItem)Application.CreateItem(OlItemType.olMailItem);
            mailItem.BodyFormat = OlBodyFormat.olFormatHTML;

            if (String.IsNullOrWhiteSpace(m_SignatureHtml))
            {
                m_SignatureHtml = Utils.GetCurrentUserSignature(true);
                var match = m_RegexBodyLabel.Match(m_SignatureHtml);
                if (match.Success)
                {
                    var bodyLabel = match.Groups[0].Value;
                    var insertNewLine = bodyLabel + "<p>" +
                                        "<span style='font-size:6.0pt;mso-ascii-font-family:Calibri;mso-fareast-font-family:宋体;mso-hansi-font-family:Calibri;mso-bidi-font-family:Calibri;color:#000;mso-themecolor:text1;mso-themetint:217;mso-font-kerning:0pt'>" +
                                        JIRA_CASE_DESCRIPTION_START +
                                        "</span></p><p>&nbsp;</p><p>&nbsp;</p><p>" +
                                        "<span style='font-size:6.0pt;mso-ascii-font-family:Calibri;mso-fareast-font-family:宋体;mso-hansi-font-family:Calibri;mso-bidi-font-family:Calibri;color:#000;mso-themecolor:text1;mso-themetint:217;mso-font-kerning:0pt'>" +
                                        JIRA_CASE_DESCRIPTION_END +
                                        "</span></p>";
                    m_SignatureHtml = m_SignatureHtml.Replace(bodyLabel, insertNewLine);
                }
            }
            mailItem.HTMLBody = m_SignatureHtml;
            if (String.IsNullOrWhiteSpace(m_SignaturePlainText))
            {
                m_SignaturePlainText = mailItem.Body;
            }

            mailItem.Importance = OlImportance.olImportanceNormal;
            mailItem.Display(false);
#if DEBUG
            //TODO:Test
            mailItem.To = "Jason.J.Dong@newegg.com";
            mailItem.Subject = "(info)Test";
#endif
            ((ItemEvents_10_Event)mailItem).Send += JiraContextItem_Send;
            m_MailItems.Add(mailItem);
        }

        private String CreateIssueToJira(JiraIssueModel issue)
        {
            if (JiraUserAuth != null && issue != null)
            {
                String errMsg;
                String requiredValue;
                IsCurrentIssueRequiredFieldsValueNull(out requiredValue);
                if (!JiraOperator.CreateIssue(issue, out errMsg, requiredValue))
                {
                    return errMsg;
                }
                return String.Empty;
            }
            return "User authorization info is empty";
        }

        private void CreateJiraOperator()
        {
            if (JiraUserAuthUtil.IsJiraAuthExists())
            {
                JiraUserAuth = JiraUserAuthUtil.ReadJiraUserAuthInfo();
                if (JiraUserAuth != null)
                {
                    JiraOperator = new JiraOperator(JiraUserAuth);
                }
            }
        }

        private void LoadSetting()
        {
            var project = JiraProjectsStoreUtil.ReadProjects();
            if (project != null)
            {
                SelectedProjects = project.JiraProjects;
                CurrentAssignProject = project.CurrentAssignProject;
                IsCreateJiraCaseAfterSent = project.CreateJiraCaseAfterSent;
                CurrentSelectedIssueType = project.CurrentIssueType;
                BaseJiraUrl = project.BaseJiraUrl;
                ProjectIssueRequiredFieldsMaps = project.ProjectIssueRequiredFieldsMaps;
            }
            if (ProjectIssueRequiredFieldsMaps == null)
            {
                ProjectIssueRequiredFieldsMaps = new ProjectIssueRequiredFieldsMapCollection();
            }
            if (String.IsNullOrWhiteSpace(BaseJiraUrl))
            {
                BaseJiraUrl = JIRA_BASE_URL_NEWEGG;
            }
            if (JiraOperator != null)
            {
                JiraOperator.BaseJiraUrl = BaseJiraUrl;
            }
        }

        private void CreateRelatedFiles()
        {
            m_UserHaveFileAccessPermisson = JiraUserAuthUtil.CreateJiraUserAuthFile();
            m_UserHaveFileAccessPermisson = JiraProjectsStoreUtil.CreateFile();
        }

        private bool CheckUserAuth()
        {
            if (JiraUserAuth == null)
            {
                if (JiraUserAuthUtil.IsJiraAuthExists())
                {
                    JiraUserAuth = JiraUserAuthUtil.ReadJiraUserAuthInfo();
                }
                else
                {
                    JiraUserAuthUtil.CreateJiraUserAuthFile();
                    var permissonUser = new FormPermissonUser();
                    if (permissonUser.ShowDialog() == DialogResult.OK)
                    {
                        JiraUserAuth = permissonUser.JiraUserAuth;
                        JiraUserAuthUtil.WriteJiraUserAuthInfo(JiraUserAuth);
                        if (JiraOperator == null)
                        {
                            JiraOperator = new JiraOperator(JiraUserAuth) { BaseJiraUrl = BaseJiraUrl };
                        }
                    }
                }
                return JiraUserAuth != null &&
                       !String.IsNullOrWhiteSpace(JiraUserAuth.User) &&
                       !String.IsNullOrWhiteSpace(JiraUserAuth.Password);
            }
            return true;
        }

        private bool CheckCurrentProject()
        {
            if (SelectedProjects == null)
            {
                SelectProjects();
            }

            if (CurrentAssignProject == null)
            {
                if (m_FormInspectorSelectSingleProject == null || m_FormInspectorSelectSingleProject.IsDisposed)
                {
                    m_FormInspectorSelectSingleProject = new FormInspectorSelectSingleProject(SelectedProjects);
                }
                if (m_FormInspectorSelectSingleProject.ShowDialog() == DialogResult.OK)
                {
                    CurrentAssignProject = m_FormInspectorSelectSingleProject.SelectedJiraProject;
                    UpdateCurrentProjectUI();
                }
            }

            return CurrentAssignProject != null;
        }
        #endregion

        #region Tab Create Jira Issue

        public bool TabMailCreateJiraCase_GetVisible(Office.IRibbonControl control)
        {
            return control.Context is Explorer;
        }

        public Bitmap GetLargeCustomImage(Office.IRibbonControl control)
        {
            return new Bitmap(Properties.Resources.jira_large);
        }

        public void OnCreateJiraCaseReadMail(Office.IRibbonControl control)
        {
            ConfirmSingleCreateJiraCase();
        }
        #endregion

        #region Mail Read Jira Issue Setting

        public bool OnMailReadJiraCaseSettingVisible(Office.IRibbonControl control)
        {
            if (control.Context is Inspector)
            {
                var oInsp = control.Context as Inspector;
                if (oInsp.CurrentItem is MailItem)
                {
                    var oMail =
                        oInsp.CurrentItem as MailItem;

                    return oMail.Sent;
                }
                return false;
            }
            return false;
        }

        public bool OnMailReadJiraCaseProjectVisible(Office.IRibbonControl control)
        {
            return true;
        }

        public void OnSelectedIssueType(Office.IRibbonControl control, String selectedID, int selectedIndex)
        {
            switch (selectedIndex)
            {
                case 0:
                    CurrentSelectedIssueType = new JiraIssueType() { Name = "Case" };
                    break;
                case 1:
                    CurrentSelectedIssueType = new JiraIssueType() { Name = "Bug" };
                    break;
                case 2:
                    CurrentSelectedIssueType = new JiraIssueType() { Name = "Task" };
                    break;
                case 3:
                    CurrentSelectedIssueType = new JiraIssueType() { Name = "OnlineBug" };
                    break;
            }
            ribbon.InvalidateControl("drpExplorerSelectIssueType");
            ribbon.InvalidateControl("drpInspectorSelectIssueType");

            CheckRequiredFieldValue();
        }

        private bool CheckRequiredFieldValue()
        {
            String requiredValue;
            if (IsCurrentIssueRequiredFieldsValueNull(out requiredValue))
            {
                return GetRequiredFieldValue();
            }
            return true;
        }

        private bool IsCurrentIssueRequiredFieldsValueNull(out String value)
        {
            if (ProjectIssueRequiredFieldsMaps != null && CurrentAssignProject != null && CurrentSelectedIssueType != null)
            {
                var findProject =
                    ProjectIssueRequiredFieldsMaps.Maps.Find(p => String.Equals(CurrentAssignProject.ID, p.Project.ID));
                if (findProject != null && findProject.Issues != null)
                {
                    var findIssue = findProject.Issues.Find(i => String.Equals(i.IssueType.Name, CurrentSelectedIssueType.Name,
                                          StringComparison.OrdinalIgnoreCase));
                    if (findIssue != null)
                    {
                        if (findIssue.NoRequiredFields)
                        {
                            value = String.Empty;
                            return true;
                        }
                        value = findIssue.RequiredFieldsJsonValue;
                        return String.IsNullOrWhiteSpace(findIssue.RequiredFieldsJsonValue);
                    }
                    value = String.Empty;
                    return true;
                }
                value = String.Empty;
                return true;
            }
            value = String.Empty;
            return true;
        }

        private bool GetRequiredFieldValue()
        {
            if (ProjectIssueRequiredFieldsMaps != null && CurrentAssignProject != null && CurrentSelectedIssueType != null)
            {
                var findProjectIndex =
                    ProjectIssueRequiredFieldsMaps.Maps.FindIndex(p => String.Equals(CurrentAssignProject.ID, p.Project.ID));
                return GetRequiredFieldValue(findProjectIndex != -1
                                           ? ProjectIssueRequiredFieldsMaps.Maps[findProjectIndex]
                                           : ProjectIssueRequiredFieldsMaps.AddOne(CurrentAssignProject));
            }
            return false;
        }

        private bool GetRequiredFieldValue(ProjectIssueRequiredFieldsMap projectMap)
        {
            if (CurrentAssignProject != null && JiraOperator != null)
            {
                var waitingForm =
                    new FormWaiting(
                        new Task<object>(
                            () =>
                            {
                                try
                                {
                                    return (object)JiraOperator.GetIssueCreateMeta(CurrentAssignProject.ID,
                                                                           CurrentSelectedIssueType.Name);
                                }
                                catch (Exception)
                                {
                                    return null;
                                }
                            }));
                if (waitingForm.ShowDialog() == DialogResult.OK)
                {
                    var meta = waitingForm.Result as JiraCreateIssueMeta;
                    if (meta != null && meta.IssueTypes != null)
                    {
                        var currentMeta = meta.IssueTypes.FirstOrDefault();
                        if (currentMeta != null)
                        {
                            var issue = new IssueRequiredFieldsMap();
                            var required =
                                currentMeta.Fields.Where(
                                    m => String.IsNullOrWhiteSpace(m.Value.Schema.System) && m.Value.Required).ToDictionary(
                                        pair => pair.Key, pair => pair.Value);
                            var requiredSystemFields = currentMeta.Fields.Where(
                                    m => !String.IsNullOrWhiteSpace(m.Value.Schema.System)).ToDictionary(
                                        pair => pair.Key, pair => pair.Value);
                            if (required.Keys.Count > 0)
                            {
                                var requireField = new FormReqiuredFields(required);
                                if (requireField.ShowDialog() == DialogResult.OK)
                                {
                                    issue.IssueType = new JiraIssueType { Name = CurrentSelectedIssueType.Name };
                                    issue.RequiredFieldsJsonValue = requireField.FieldsValues;
                                    issue.SystemField = requiredSystemFields.Values.ToList();
                                    projectMap.AddUniqueIssue(issue);
                                    return true;
                                }
                            }
                            else
                            {
                                issue.IssueType = new JiraIssueType { Name = CurrentSelectedIssueType.Name };
                                issue.NoRequiredFields = true;
                                issue.SystemField = requiredSystemFields.Values.ToList();
                                projectMap.AddUniqueIssue(issue);
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public int OnGetSelectedIssueType(Office.IRibbonControl control)
        {
            var shouldBe = 0;
            if (CurrentSelectedIssueType != null)
            {
                switch (CurrentSelectedIssueType.Name)
                {
                    case "Case":
                        shouldBe = 0;
                        break;
                    case "Bug":
                        shouldBe = 1;
                        break;
                    case "Task":
                        shouldBe = 2;
                        break;
                    case "OnlineBug":
                        shouldBe = 3;
                        break;
                    default:
                        shouldBe = 0;
                        break;
                }
            }
            else
            {
                CurrentSelectedIssueType = new JiraIssueType { Name = "Case" };
            }
            return shouldBe;
        }

        public int OnGetIssueTypeItemCount(Office.IRibbonControl control)
        {
            return 4;
        }

        public String OnGetIssueTypeItemLabel(Office.IRibbonControl control, int index)
        {
            switch (index)
            {
                case 0:
                    return "Case";
                case 1:
                    return "Bug";
                case 2:
                    return "Task";
                case 3:
                    return "OnlineBug";
                default:
                    return "";
            }
        }
        #endregion

        #region Explorer Jira Issue Setting

        public void OnUserAuthSetting(Office.IRibbonControl control)
        {
            if (m_FormPermissonUser != null && m_FormPermissonUser.Visible)
            {
                return;
            }
            if (JiraUserAuth == null)
            {
                CheckUserAuth();
            }
            else
            {
                m_FormPermissonUser = new FormPermissonUser(JiraUserAuth);
                if (m_FormPermissonUser.ShowDialog() == DialogResult.OK)
                {
                    JiraUserAuth = m_FormPermissonUser.JiraUserAuth;
                    JiraUserAuthUtil.WriteJiraUserAuthInfo(JiraUserAuth);
                }
            }
        }

        public void OnProjectsSelect(Office.IRibbonControl control)
        {
            if (CheckUserAuth())
            {
                SelectProjects();
            }
        }

        private void SelectProjects()
        {
            if (m_FormExplorerProjects == null || m_FormExplorerProjects.IsDisposed)
            {
                m_FormExplorerProjects = new FormExplorerProjects(JiraOperator);
            }
            m_FormExplorerProjects.FillSelectedProject(SelectedProjects ?? new List<JiraProject>());
            if (m_FormExplorerProjects.ShowDialog() == DialogResult.OK)
            {
                SelectedProjects = m_FormExplorerProjects.SelectedJiraProjects;
                if (SelectedProjects != null)
                {
                    if (SelectedProjects.Count > 0)
                    {
                        CurrentAssignProject = SelectedProjects[0];
                        UpdateCurrentProjectUI();
                        CheckRequiredFieldValue();
                    }
                }
            }
        }

        public void OnSendWithCreateJiraCase(Office.IRibbonControl control, ref bool pressed)
        {
            IsCreateJiraCaseAfterSent = pressed;
        }

        public bool OnGetSendWithCreateJiraCasePressed(Office.IRibbonControl control)
        {
            var proj = JiraProjectsStoreUtil.ReadProjects();
            if (proj != null)
            {
                return proj.CreateJiraCaseAfterSent;
            }
            return false;
        }

        public void OnJiraUrlSetting(Office.IRibbonControl control)
        {
            if (!CheckUserAuth())
            {
                return;
            }
            var jiraUrlForm = new FormJiraUrl(JiraOperator, BaseJiraUrl);
            if (jiraUrlForm.ShowDialog() == DialogResult.OK)
            {
                if (!BaseJiraUrl.Equals(jiraUrlForm.JiraUrl))
                {
                    BaseJiraUrl = JiraOperator.BaseJiraUrl = jiraUrlForm.JiraUrl;
                    SelectedProjects.Clear();
                    CurrentAssignProject = null;
                    ProjectIssueRequiredFieldsMaps.Maps.Clear();
                    UpdateCurrentProjectUI();
                }
            }
        }

        public void OnResetFieldValue(Office.IRibbonControl control)
        {
            GetRequiredFieldValue();
        }

        public void OnRepeatUserMap(Office.IRibbonControl control)
        {

        }
        #endregion

        #region Explorer Jira Issue Create

        public void OnCreateJiraCaseBatch(Office.IRibbonControl control)
        {
            ConfirmBatchCreateJiraCase();
        }

        #endregion

        #region Project Item
        public String GetCurrentProjectLabel(Office.IRibbonControl control)
        {
            return CurrentAssignProject == null ? "No Project" : CurrentAssignProject.Name;
        }

        public void OnCurrentProjectSetting(Office.IRibbonControl control)
        {
            if (SelectedProjects == null)
            {
                return;
            }
            if (m_FormInspectorSelectSingleProject == null || m_FormInspectorSelectSingleProject.IsDisposed)
            {
                m_FormInspectorSelectSingleProject = new FormInspectorSelectSingleProject(SelectedProjects);
            }
            else
            {
                m_FormInspectorSelectSingleProject.Init(SelectedProjects);
            }

            if (m_FormInspectorSelectSingleProject.ShowDialog() == DialogResult.OK)
            {
                CurrentAssignProject = m_FormInspectorSelectSingleProject.SelectedJiraProject;
                CheckRequiredFieldValue();
            }
            UpdateCurrentProjectUI();
        }

        private void UpdateCurrentProjectUI()
        {
            if (ribbon != null)
            {
                ribbon.InvalidateControl("btnInspectorCurrentProjectSetting");
                ribbon.InvalidateControl("btnExplorerCurrentProjectSetting");
            }
        }

        #endregion

        #region IRibbonExtensibility Members

        public string GetCustomUI(string ribbonID)
        {
            return GetResourceText("Newegg.Outlook.JIRA.Add_in.JiraContextItem.xml");
        }

        #endregion

        #region Ribbon Callbacks
        //Create callback methods here. For more information about adding callback methods, select the Ribbon XML item in Solution Explorer and then press F1

        public void Ribbon_Load(Office.IRibbonUI ribbonUI)
        {
            this.ribbon = ribbonUI;

            CreateRelatedFiles();
            CreateJiraOperator();
            LoadSetting();
        }

        #endregion

        #region Helpers

        private static string GetResourceText(string resourceName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string[] resourceNames = asm.GetManifestResourceNames();
            for (int i = 0; i < resourceNames.Length; ++i)
            {
                if (string.Compare(resourceName, resourceNames[i], StringComparison.OrdinalIgnoreCase) == 0)
                {
                    using (StreamReader resourceReader = new StreamReader(asm.GetManifestResourceStream(resourceNames[i])))
                    {
                        if (resourceReader != null)
                        {
                            return resourceReader.ReadToEnd();
                        }
                    }
                }
            }
            return null;
        }

        #endregion
    }
}
