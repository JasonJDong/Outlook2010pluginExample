using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Newegg.Outlook.JIRA.Add_in.Controls;

namespace Newegg.Outlook.JIRA.Add_in
{
    public partial class FormReqiuredFields : Form
    {
        public const string CUSTOM_FIELD_VALUE_SELECT_FORMAT = "\"customfield_{0}\" : {2}\"value\":\"{1}\"{3}";
        public const string CUSTOM_FIELD_VALUE_VERSION_SELECT_FORMAT = "\"customfield_{0}\" : {2}\"name\":\"{1}\"{3}";
        public const string CUSTOM_FIELD_VALUE_USER_FORMAT = "\"customfield_{0}\" : {2}\"name\":\"{1}\"{3}";
        public const string CUSTOM_FIELD_VALUE_TEXTBOX_COMMON_FORMAT = "\"customfield_{0}\" : \"{1}\"";
        public const string CUSTOM_FIELD_VALUE_TEXTBOX_NUMBER_FORMAT = "\"customfield_{0}\" : {1}";
        public const string CUSTOM_FIELD_VALUE_LABELS_FORMAT = "\"customfield_{0}\":{1}";
        public const string ONLY_NUMBER_PATTERN = "^[\\d]+$";

        public const string MESSAGE_REQUIRED_FIELD_VALUE_FORMAT = "Create issue of the issue type required {0}!";
        private int CurrentFieldCount = 0;
        private Regex m_OnlyNumber = new Regex(ONLY_NUMBER_PATTERN);

        public Dictionary<String, JiraField> JiraCreateIssueMetas { get; set; }

        public String FieldsValues { get; set; }

        public FormReqiuredFields(Dictionary<String, JiraField> meta)
        {
            JiraCreateIssueMetas = meta;
            InitializeComponent();
            ResolveRequiredFields();
        }

        private void ResolveRequiredFields()
        {
            foreach (var field in JiraCreateIssueMetas)
            {
                panelControls.RowCount++;
                switch (field.Value.Schema.Type)
                {
                    case "string":
                        GenerateString(field.Value);
                        break;
                    case "number":
                        GenerateNumber(field.Value);
                        break;
                    case "user":
                        GenerateUser(field.Value);
                        break;
                    case "version":
                        GenerateVersion(field.Value);
                        break;
                    case "array":
                        GenerateLabel(field.Value);
                        break;
                    default:
                        break;
                }
            }
        }

        private void GenerateString(JiraField field)
        {
            var lb = CreateCommonLabel(field.Name);
            panelControls.Controls.Add(lb, 0, CurrentFieldCount);
            if (!String.IsNullOrWhiteSpace(field.Schema.Custom)
                && field.Schema.Custom.Equals("com.atlassian.jira.plugin.system.customfieldtypes:select"))
            {
                var listValue = field.AllowedValues.Select(allowedValue => new NameValue { Field = field, Name = allowedValue.value, Value = allowedValue.id }).ToList();
                var select = CreateCombox(field, listValue);
                panelControls.Controls.Add(select, 1, CurrentFieldCount);
                CurrentFieldCount++;
            }
            else
            {
                var txt = CreateCommonTextbox(field);
                panelControls.Controls.Add(txt, 1, CurrentFieldCount);
                CurrentFieldCount++;
            }
        }

        private void select_SelectedIndexChanged(object sender, EventArgs e)
        {
            var select = sender as ComboBox;
            if (select != null && select.SelectedItem != null)
            {
                var package = select.Tag as ControlTagPackage;
                var selectItem = select.SelectedItem as NameValue;
                if (package != null && selectItem != null)
                {
                    var isVersion = "version".Equals(package.Field.Schema.Type);
                    var format = isVersion ? CUSTOM_FIELD_VALUE_VERSION_SELECT_FORMAT : CUSTOM_FIELD_VALUE_SELECT_FORMAT;

                    select.Tag = new ControlTagPackage
                        {
                            Field = package.Field,
                            ID = package.ID,
                            SerializedString = String.Format(format, package.ID, selectItem.Name, "{", "}")
                        };
                }
            }
        }

        private void GenerateNumber(JiraField field)
        {
            var lb = CreateCommonLabel(field.Name);
            panelControls.Controls.Add(lb, 0, CurrentFieldCount);
            if (!String.IsNullOrWhiteSpace(field.Schema.Custom)
                && field.Schema.Custom.Equals("com.atlassian.jira.plugin.system.customfieldtypes:float"))
            {
                var txt = CreateCommonTextbox(field);
                panelControls.Controls.Add(txt, 1, CurrentFieldCount);
                CurrentFieldCount++;
            }
        }

        private void GenerateUser(JiraField field)
        {
            var lb = CreateCommonLabel(field.Name);
            panelControls.Controls.Add(lb, 0, CurrentFieldCount);

            if (!String.IsNullOrWhiteSpace(field.Schema.Custom)
                && field.Schema.Custom.Equals("com.atlassian.jira.plugin.system.customfieldtypes:userpicker"))
            {
                var searchUser = CreateSearchCombox(field);
                panelControls.Controls.Add(searchUser, 1, CurrentFieldCount);
                CurrentFieldCount++;
            }
        }

        private void GenerateVersion(JiraField field)
        {
            var lb = CreateCommonLabel(field.Name);
            panelControls.Controls.Add(lb, 0, CurrentFieldCount);
            if (!String.IsNullOrWhiteSpace(field.Schema.Custom)
                && field.Schema.Custom.Equals("com.atlassian.jira.plugin.system.customfieldtypes:version"))
            {
                var listValue = new List<NameValue>();
                foreach (dynamic allowedvalue in field.AllowedValues)
                {
                    foreach (dynamic next in allowedvalue)
                    {
                        var nameValue = new NameValue
                            {
                                Name = next.name,
                                Value = next.id
                            };
                        listValue.Add(nameValue);
                    }
                }
                var select = CreateCombox(field, listValue, SelectType.Version);
                panelControls.Controls.Add(select, 1, CurrentFieldCount);
                CurrentFieldCount++;
            }
        }

        private void GenerateLabel(JiraField field)
        {
            var lb = CreateCommonLabel(field.Name);
            panelControls.Controls.Add(lb, 0, CurrentFieldCount);
            if (!String.IsNullOrWhiteSpace(field.Schema.Custom)
                && field.Schema.Custom.Equals("com.atlassian.jira.plugin.system.customfieldtypes:labels"))
            {
                var txt = CreateCommonTextbox(field, TextBoxType.Label);
                panelControls.Controls.Add(txt, 1, CurrentFieldCount);
                CurrentFieldCount++;
            }
        }

        private ComboBox CreateCombox(JiraField field, List<NameValue> datasource, SelectType type = SelectType.Common)
        {
            var select = new ComboBox();
            select.DropDownStyle = ComboBoxStyle.DropDownList;
            select.DisplayMember = "Name";
            select.ValueMember = "Value";
            select.Width = 230;
            select.TabIndex = 99 - CurrentFieldCount;
            var first = datasource.FirstOrDefault();
            var firstValue = first == null ? String.Empty : first.Name;
            var format = String.Empty;
            switch (type)
            {
                case SelectType.Common:
                    format = CUSTOM_FIELD_VALUE_SELECT_FORMAT;
                    break;
                case SelectType.Version:
                    format = CUSTOM_FIELD_VALUE_VERSION_SELECT_FORMAT;
                    break;
                default:
                    format = CUSTOM_FIELD_VALUE_SELECT_FORMAT;
                    break;
            }
            select.Tag = new ControlTagPackage
            {
                Field = field,
                ID = field.Schema.CustomID,
                SerializedString = String.Format(format, field.Schema.CustomID, firstValue, "{", "}")
            };
            select.DataSource = datasource;
            select.SelectedIndexChanged += select_SelectedIndexChanged;
            return select;
        }

        private TextBox CreateCommonTextbox(JiraField field, TextBoxType type = TextBoxType.Common)
        {
            var txt = new TextBox();
            txt.Font = new Font(Font.FontFamily, 11);
            txt.Width = 230;
            txt.Tag = new ControlTagPackage { Field = field, ID = field.Schema.CustomID };
            switch (type)
            {
                case TextBoxType.Common:
                case TextBoxType.Number:
                    txt.TextChanged += CommonTextBox_TextChanged;
                    break;
                case TextBoxType.Label:
                    txt.TextChanged += LabelTextBox_TextChanged;
                    break;
            }
            txt.TabIndex = CurrentFieldCount;
            return txt;
        }

        void LabelTextBox_TextChanged(object sender, EventArgs e)
        {
            var txt = sender as TextBox;
            if (txt != null)
            {
                if (txt.Text.Length == 0)
                {
                    return;
                }

                var package = txt.Tag as ControlTagPackage;
                if (package != null)
                {
                    const String itemFormat = "\"{0}\"";
                    const String finalFormat = "[{0}]";
                    var labels = txt.Text.Split(',');
                    var items = labels.Select(label => String.Format(itemFormat, label)).ToList();
                    var labelText = String.Format(finalFormat, String.Join(",", items.ToArray()));
                    txt.Tag = new ControlTagPackage
                    {
                        Field = package.Field,
                        ID = package.ID,
                        SerializedString = String.Format(CUSTOM_FIELD_VALUE_LABELS_FORMAT, package.ID, labelText)
                    };
                }
            }
        }

        private SearchCombox CreateSearchCombox(JiraField field)
        {
            var cb = new SearchCombox();
            cb.DisplayMember = "Name";
            cb.ValueMember = "Value";
            cb.TabIndex = 99 - CurrentFieldCount;
            //cb.Width = 230;
            cb.Tag = new ControlTagPackage
            {
                Field = field,
                ID = field.Schema.CustomID,
            };
            cb.SelectedIndexChanged = (sender, e) =>
                {
                    var package = cb.Tag as ControlTagPackage;
                    var selectItem = cb.SelectedItem as NameValue;
                    if (package != null && selectItem != null)
                    {
                        cb.Tag = new ControlTagPackage
                        {
                            Field = package.Field,
                            ID = package.ID,
                            SerializedString = String.Format(CUSTOM_FIELD_VALUE_USER_FORMAT, package.ID, selectItem.Value, "{", "}")
                        };
                    }
                };
            return cb;
        }

        private void CommonTextBox_TextChanged(object sender, EventArgs e)
        {
            var txt = sender as TextBox;
            if (txt != null)
            {
                if (txt.Text.Length == 0)
                {
                    return;
                }

                var package = txt.Tag as ControlTagPackage;
                if (package != null)
                {
                    var isNumber = "number".Equals(package.Field.Schema.Type);
                    var format = isNumber ? CUSTOM_FIELD_VALUE_TEXTBOX_NUMBER_FORMAT : CUSTOM_FIELD_VALUE_TEXTBOX_COMMON_FORMAT;
                    if (isNumber && !m_OnlyNumber.IsMatch(txt.Text.Trim()))
                    {
                        txt.Text = txt.Text.Substring(0, txt.Text.Length - 1);
                        txt.SelectionStart = txt.Text.Length;
                        return;
                    }
                    txt.Tag = new ControlTagPackage
                        {
                            Field = package.Field,
                            ID = package.ID,
                            SerializedString = String.Format(format, package.ID, txt.Text)
                        };
                }
            }
        }

        private Label CreateCommonLabel(String name)
        {
            var lbName = new Label();
            lbName.Text = name + ": ";
            lbName.Margin = new Padding(0, 5, 0, 5);
            return lbName;
        }

        private void panelControls_ControlAdded(object sender, ControlEventArgs e)
        {
            Label firstLabel = panelControls.Controls.OfType<Label>().Select(control => control).FirstOrDefault();
            var count = panelControls.Controls.OfType<Label>().Count();
            if (firstLabel != null)
            {
                var panelSize = new Size(panelControls.Width, (firstLabel.Size.Height + 15) * count);
                panelControls.Size = new Size(panelSize.Width, panelSize.Height + 10);
                this.Size = new Size(this.Size.Width, panelSize.Height + 95);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            var customFieldValues = new List<String>();
            foreach (Control control in panelControls.Controls)
            {
                var package = control.Tag as ControlTagPackage;
                if (package != null)
                {
                    if (String.IsNullOrWhiteSpace(package.SerializedString))
                    {
                        MessageBox.Show(String.Format(MESSAGE_REQUIRED_FIELD_VALUE_FORMAT, package.Field.Name), "Error",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    customFieldValues.Add(package.SerializedString);
                }
            }

            FieldsValues = String.Join(",\r\n", customFieldValues.ToArray());
            DialogResult = DialogResult.OK;
        }

        private void FormReqiuredFields_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                DialogResult = DialogResult.Cancel;
            }
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnOK_Click(btnOK, EventArgs.Empty);
            }
        }
    }

    class ControlTagPackage
    {
        public JiraField Field { get; set; }

        public String ID { get; set; }

        public String SerializedString { get; set; }
    }

    enum SelectType
    {
        Common,
        Version,
    }

    enum TextBoxType
    {
        Common,
        Number,
        Label
    }
}
