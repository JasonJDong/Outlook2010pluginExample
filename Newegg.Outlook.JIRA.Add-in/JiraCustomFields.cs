using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Newegg.Outlook.JIRA.Add_in.Controls;

namespace Newegg.Outlook.JIRA.Add_in
{
    public abstract class JiraCustomField
    {
        public JiraField JiraField { get; set; }

        public abstract String ToFormatString();

        public abstract List<Control> ProvideControl();

        protected JiraCustomField(JiraField jiraField)
        {
            JiraField = jiraField;
            if (jiraField == null)
            {
                throw new ArgumentNullException("jiraField");
            }
        }
    }

    public class JiraCustomFields
    {
        public class CascadingSelectField : JiraCustomField
        {
            public const string PARENT_FORMAT = "{0}customfield_{1}{0}:{4}{0}value{0}:{0}{2}{0},{3}{5}";
            public const string CHILD_FORMAT = "{0}child{0}:{2}{0}value{0}:{0}{1}{0}{3}";

            private ComboBox m_Parent;
            private ComboBox m_Child;

            public CascadingSelectField(JiraField jiraField)
                : base(jiraField)
            {
            }

            public List<NameValue> ParentValues { get; set; }

            public List<NameValue> ChildValues { get; set; }

            public override string ToFormatString()
            {
                var parent = m_Parent.SelectedItem as NameValue;
                var child = m_Child.SelectedItem as NameValue;
                var parentValue = parent != null ? parent.Value : String.Empty;
                var childValue = child != null ? child.Value : String.Empty;

                var childFormat = String.Format(CHILD_FORMAT, "\"", childValue, "{", "}");

                return String.Format(PARENT_FORMAT, "\"", JiraField.Schema.CustomID, parentValue, childFormat, "{", "}");
            }

            public override List<Control> ProvideControl()
            {
                if (m_Parent == null)
                {
                    m_Parent = new ComboBox();
                    m_Parent.SelectedValueChanged += parent_SelectedValueChanged;
                }
                if (m_Child == null)
                {
                    m_Child = new ComboBox();
                }
                return new List<Control> { m_Parent, m_Child };
            }

            private void parent_SelectedValueChanged(object sender, EventArgs e)
            {

            }
        }

        public class DatePickerField : JiraCustomField
        {
            public const string FORMAT = "{0}customfield_{1}{0}:{0}{2}{0}";
            public const string DATETIME_FORMAT = "YYYY-MM-DD";

            private DateTimePicker m_Picker;

            public DateTime DateTime { get; set; }

            public DatePickerField(JiraField jiraField)
                : base(jiraField)
            {
                JiraField = jiraField;
            }

            public override string ToFormatString()
            {
                return String.Format(FORMAT, "\"", JiraField.Schema.CustomID,
                                  String.Format(DateTime.ToString(DATETIME_FORMAT)));
            }

            public override List<Control> ProvideControl()
            {
                if (m_Picker == null)
                {
                    m_Picker = new DateTimePicker();
                    m_Picker.ValueChanged += picker_ValueChanged;
                }
                return new List<Control> { m_Picker };
            }

            private void picker_ValueChanged(object sender, EventArgs e)
            {
                DateTime = m_Picker.Value;
            }
        }

        public class DateTimeField : JiraCustomField
        {
            public const string FORMAT = "{0}customfield_{1}{0}:{0}{2}{0}";
            public const string DATETIME_FORMAT = "YYYY-MM-DDThh:mm:ss.sTZD";

            private DateTimePicker m_Picker;

            public DateTime DateTime { get; set; }

            public DateTimeField(JiraField jiraField)
                : base(jiraField)
            {
                JiraField = jiraField;
            }

            public override string ToFormatString()
            {
                return String.Format(FORMAT, "\"", JiraField.Schema.CustomID,
                                  String.Format(DateTime.ToString(DATETIME_FORMAT)));
            }

            public override List<Control> ProvideControl()
            {
                if (m_Picker == null)
                {
                    m_Picker = new DateTimePicker();
                    m_Picker.Format = DateTimePickerFormat.Time;
                    m_Picker.ValueChanged += picker_ValueChanged;

                }
                return new List<Control> { m_Picker };
            }

            private void picker_ValueChanged(object sender, EventArgs e)
            {
                DateTime = m_Picker.Value;
            }
        }

        public class FreeTextField : JiraCustomField
        {
            public const string FORMAT = "{0}customfield_{1}{0}:{0}{2}{0}";

            private TextBox m_TextBox;

            public String Text { get; set; }

            public FreeTextField(JiraField jiraField)
                : base(jiraField)
            {
                Text = String.Empty;
            }

            public override string ToFormatString()
            {
                return String.Format(FORMAT, "\"", JiraField.Schema.CustomID, Text);
            }

            public override List<Control> ProvideControl()
            {
                if (m_TextBox == null)
                {
                    m_TextBox = new TextBox();
                    m_TextBox.TextChanged += m_TextBox_TextChanged;
                }
                return new List<Control>() { m_TextBox };
            }

            private void m_TextBox_TextChanged(object sender, EventArgs e)
            {
                Text = m_TextBox.Text;
            }
        }

        public class GroupPicker : JiraCustomField
        {
            public const string FORMAT = "{0}customfield_{1}{0}:{3}{0}name{0}:{0}{2}{0}{4}";

            private SearchCombox m_GroupSearch;

            public JiraGroup JiraGroup { get; set; }

            public GroupPicker(JiraField jiraField)
                : base(jiraField)
            {
            }

            public override string ToFormatString()
            {
                if (JiraGroup == null)
                {
                    return null;
                }
                return String.Format(FORMAT, "\"", JiraField.Schema.CustomID, JiraGroup.Name, "{", "}");
            }

            public override List<Control> ProvideControl()
            {
                if (m_GroupSearch == null)
                {
                    m_GroupSearch = new SearchCombox();
                    m_GroupSearch.DisplayMember = "Name";
                    m_GroupSearch.ValueMember = "Name";
                    m_GroupSearch.SelectedIndexChanged += GroupSearch_SelectedIndexChanged;
                }
                return new List<Control>() { m_GroupSearch };
            }

            private void GroupSearch_SelectedIndexChanged(object sender, EventArgs e)
            {
                JiraGroup = m_GroupSearch.SelectedItem as JiraGroup;
            }
        }

        public class Labels : JiraCustomField
        {
            public const string FORMAT = "{0}customfield_{1}{0}:[{2}]";
            public const string CHILD_FORMAT = "{0}{1}{0}";

            private TextBox m_TextBox;

            public String Text { get; set; }

            public Labels(JiraField jiraField)
                : base(jiraField)
            {
                Text = String.Empty;
            }

            public override string ToFormatString()
            {
                var labels = Text.Split(',');
                var items = labels.Select(label => String.Format(CHILD_FORMAT, "\"", label));
                var all = String.Join(",", items.ToArray());
                return String.Format(FORMAT, "\"", JiraField.Schema.CustomID, all);
            }

            public override List<Control> ProvideControl()
            {
                if (m_TextBox == null)
                {
                    m_TextBox = new TextBox();
                    m_TextBox.TextChanged += m_TextBox_TextChanged;
                }
                return new List<Control>() { m_TextBox };
            }

            private void m_TextBox_TextChanged(object sender, EventArgs e)
            {
                Text = m_TextBox.Text;
            }
        }

        public class MultiGroupPicker : JiraCustomField
        {
            public MultiGroupPicker(JiraField jiraField)
                : base(jiraField)
            {
            }

            public override string ToFormatString()
            {
                throw new NotImplementedException();
            }

            public override List<Control> ProvideControl()
            {
                throw new NotImplementedException();
            }
        }

        public class NumberField : JiraCustomField
        {
            public const string FORMAT = "{0}customfield_{1}{0}:{2}";

            private TextBox m_TextBox;

            public String Text { get; set; }

            public NumberField(JiraField jiraField)
                : base(jiraField)
            {
                Text = String.Empty;
            }

            public override string ToFormatString()
            {
                return String.Format(FORMAT, "\"", JiraField.Schema.CustomID, Text);
            }

            public override List<Control> ProvideControl()
            {
                if (m_TextBox == null)
                {
                    m_TextBox = new TextBox();
                    m_TextBox.TextChanged += m_TextBox_TextChanged;
                }
                return new List<Control>() { m_TextBox };
            }

            private void m_TextBox_TextChanged(object sender, EventArgs e)
            {
                if (!Regex.IsMatch(m_TextBox.Text, "^[\\d]+.?[\\d]*$"))
                {
                    m_TextBox.Text = m_TextBox.Text.Substring(0, m_TextBox.Text.Length - 1);
                    m_TextBox.SelectionStart = m_TextBox.Text.Length;
                    return;
                }
                Text = m_TextBox.Text.Trim('.');
            }
        }

        public class ProjectPicker : JiraCustomField
        {
            public const string FORMAT = "{0}customfield_{1}{0}:{3}{0}id{0}:{0}{2}{0}{4}";

            private SearchCombox m_GroupSearch;

            public JiraProject JiraProject { get; set; }

            public ProjectPicker(JiraField jiraField)
                : base(jiraField)
            {
            }

            public override string ToFormatString()
            {
                if (JiraProject == null)
                {
                    return null;
                }
                return String.Format(FORMAT, "\"", JiraField.Schema.CustomID, JiraProject.ID, "{", "}");
            }

            public override List<Control> ProvideControl()
            {
                if (m_GroupSearch == null)
                {
                    m_GroupSearch = new SearchCombox();
                    m_GroupSearch.DisplayMember = "Name";
                    m_GroupSearch.ValueMember = "Name";
                    m_GroupSearch.SelectedIndexChanged += GroupSearch_SelectedIndexChanged;
                }
                return new List<Control>() { m_GroupSearch };
            }

            private void GroupSearch_SelectedIndexChanged(object sender, EventArgs e)
            {
                JiraProject = m_GroupSearch.SelectedItem as JiraProject;
            }
        }

        public class RadioButtonsField : JiraCustomField
        {
            public const string FORMAT = "{0}customfield_{1}{0}:{3}{0}value{0}:{0}{2}{0}{4}";

            private TableLayoutPanel m_RadioButtons;

            public String Value { get; set; }

            public List<NameValue> RadioButtonValues { get; set; }

            public RadioButtonsField(JiraField jiraField, List<NameValue> radioButtonValues)
                : base(jiraField)
            {
                RadioButtonValues = radioButtonValues;
                Value = String.Empty;
            }

            public override string ToFormatString()
            {
                return String.Format(FORMAT, "\"", JiraField.Schema.CustomID, Value, "{", "}");
            }

            public override List<Control> ProvideControl()
            {
                if (m_RadioButtons == null)
                {
                    m_RadioButtons = new TableLayoutPanel();
                    m_RadioButtons.RowCount = 1;
                    m_RadioButtons.RowStyles[0].SizeType = SizeType.AutoSize;
                    m_RadioButtons.ColumnCount = RadioButtonValues.Count;
                    foreach (ColumnStyle columnStyle in m_RadioButtons.ColumnStyles)
                    {
                        columnStyle.SizeType = SizeType.AutoSize;
                    }
                    int i = 0;
                    foreach (NameValue nameValue in RadioButtonValues)
                    {
                        var radio = new RadioButton();
                        radio.Text = nameValue.Name;
                        radio.CheckedChanged += radio_CheckedChanged;
                        if (i == 0)
                        {
                            radio.Checked = true;
                        }
                        m_RadioButtons.Controls.Add(radio, i++, 0);
                    }
                }
                return new List<Control>() { m_RadioButtons };
            }

            private void radio_CheckedChanged(object sender, EventArgs e)
            {
                var radio = sender as RadioButton;
                if (radio != null && radio.Checked)
                {
                    Value = radio.Text;
                }
            }
        }

        public class SelectList : JiraCustomField
        {
            public const string FORMAT = "{0}customfield_{1}{0}:{3}{0}value{0}:{0}{2}{0}{4}";

            private ComboBox m_Select;

            public String Value { get; set; }

            public List<NameValue> DataSource { get; set; }

            public SelectList(JiraField jiraField, List<NameValue> dataSource)
                : base(jiraField)
            {
                DataSource = dataSource;
                Value = String.Empty;
            }

            public override string ToFormatString()
            {
                return String.Format(FORMAT, "\"", JiraField.Schema.CustomID, Value, "{", "}");
            }

            public override List<Control> ProvideControl()
            {
                if (m_Select == null)
                {
                    m_Select = new ComboBox();
                    m_Select.DataSource = DataSource;
                    m_Select.DisplayMember = "Name";
                    m_Select.ValueMember = "Value";
                    m_Select.DropDownStyle = ComboBoxStyle.DropDownList;
                    m_Select.SelectedValueChanged += m_Select_SelectedValueChanged;
                }

                return new List<Control>() { m_Select };
            }

            private void m_Select_SelectedValueChanged(object sender, EventArgs e)
            {
                var selected = m_Select.SelectedItem as NameValue;
                if (selected != null)
                {
                    Value = selected.Value;
                }
            }
        }

        public class SingleVersionPicker : JiraCustomField
        {
            public const string FORMAT = "{0}customfield_{1}{0}:{3}{0}name{0}:{0}{2}{0}{4}";

            private ComboBox m_Select;

            public String Value { get; set; }

            public List<NameValue> DataSource { get; set; }

            public SingleVersionPicker(JiraField jiraField, List<NameValue> dataSource)
                : base(jiraField)
            {
                DataSource = dataSource;
                Value = String.Empty;
            }

            public override string ToFormatString()
            {
                return String.Format(FORMAT, "\"", JiraField.Schema.CustomID, Value, "{", "}");
            }

            public override List<Control> ProvideControl()
            {
                if (m_Select == null)
                {
                    m_Select = new ComboBox();
                    m_Select.DataSource = DataSource;
                    m_Select.DisplayMember = "Name";
                    m_Select.ValueMember = "Value";
                    m_Select.DropDownStyle = ComboBoxStyle.DropDownList;
                    m_Select.SelectedValueChanged += m_Select_SelectedValueChanged;
                }

                return new List<Control>() { m_Select };
            }

            private void m_Select_SelectedValueChanged(object sender, EventArgs e)
            {
                var selected = m_Select.SelectedItem as NameValue;
                if (selected != null)
                {
                    Value = selected.Value;
                }
            }
        }

        public class TextField : JiraCustomField
        {
            public const string FORMAT = "{0}customfield_{1}{0}:{0}{2}{0}";

            private TextBox m_TextBox;

            public String Text { get; set; }

            public TextField(JiraField jiraField)
                : base(jiraField)
            {
                Text = String.Empty;
            }

            public override string ToFormatString()
            {
                return String.Format(FORMAT, "\"", JiraField.Schema.CustomID, Text);
            }

            public override List<Control> ProvideControl()
            {
                if (m_TextBox == null)
                {
                    m_TextBox = new TextBox();
                    m_TextBox.MaxLength = 255;
                    m_TextBox.TextChanged += m_TextBox_TextChanged;
                }
                return new List<Control>() { m_TextBox };
            }

            private void m_TextBox_TextChanged(object sender, EventArgs e)
            {
                Text = m_TextBox.Text;
            }
        }

        public class URLField : JiraCustomField
        {
            public const string FORMAT = "{0}customfield_{1}{0}:{0}{2}{0}";

            private TextBox m_TextBox;

            public String Text { get; set; }

            public URLField(JiraField jiraField)
                : base(jiraField)
            {
                Text = String.Empty;
            }

            public override string ToFormatString()
            {
                return String.Format(FORMAT, "\"", JiraField.Schema.CustomID, Text);
            }

            public override List<Control> ProvideControl()
            {
                if (m_TextBox == null)
                {
                    m_TextBox = new TextBox();
                    m_TextBox.LostFocus += m_TextBox_LostFocus;
                }
                return new List<Control>() { m_TextBox };
            }

            private void m_TextBox_LostFocus(object sender, EventArgs e)
            {
                if (!String.IsNullOrWhiteSpace(m_TextBox.Text) && !Uri.IsWellFormedUriString(m_TextBox.Text, UriKind.Absolute))
                {
                    m_TextBox.Text = String.Empty;
                    return;
                }
                Text = m_TextBox.Text.Trim('.');
            }
        }

        public class UserPicker : JiraCustomField
        {
            public const string FORMAT = "{0}customfield_{1}{0}:{3}{0}name{0}:{0}{2}{0}{4}";

            private SearchCombox m_UserSearch;

            public JiraUser JiraUser { get; set; }

            public UserPicker(JiraField jiraField)
                : base(jiraField)
            {
            }

            public override string ToFormatString()
            {
                if (JiraUser == null)
                {
                    return null;
                }
                return String.Format(FORMAT, "\"", JiraField.Schema.CustomID, JiraUser.Name, "{", "}");
            }

            public override List<Control> ProvideControl()
            {
                if (m_UserSearch == null)
                {
                    m_UserSearch = new SearchCombox();
                    m_UserSearch.DisplayMember = "DisplayName";
                    m_UserSearch.ValueMember = "Name";
                    m_UserSearch.SelectedIndexChanged += GroupSearch_SelectedIndexChanged;
                }
                return new List<Control>() { m_UserSearch };
            }

            private void GroupSearch_SelectedIndexChanged(object sender, EventArgs e)
            {
                JiraUser = m_UserSearch.SelectedItem as JiraUser;
            }
        }

    }

    public class NameValue
    {
        public JiraField Field { get; set; }

        public String Name { get; set; }

        public String Value { get; set; }
    }
}
