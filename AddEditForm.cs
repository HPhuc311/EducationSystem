using EducationSystem.Models;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace EducationSystem
{
    public class AddEditForm : Form
    {
        public Person PersonData { get; private set; }
        private Person editingPerson = null;

        TextBox txtName, txtPhone, txtEmail;
        ComboBox comboRole;

        Label errName, errEmail, errPhone;
        Label errTSub1, errTSub2;
        Label errSSub1, errSSub2, errSSub3;
        Label errRole;
        Label errTSalary, errASalary, errWork;

        GroupBox grpTeacher, grpStudent, grpAdmin;

        TextBox tSalary, tSub1, tSub2;
        TextBox sSub1, sSub2, sSub3;
        TextBox aSalary, aWork, aHours;
        Label errHours;

        Button btnSave;

        public AddEditForm()
        {
            InitUI();
            SetupValidation();
        }

        public AddEditForm(Person p) : this()
        {
            editingPerson = p;

            txtName.Text = p.Name;
            txtPhone.Text = p.Phone;
            txtEmail.Text = p.Email;
            comboRole.Text = p.GetRole();

            if (p is Teacher t)
            {
                tSalary.Text = t.Salary.ToString();
                tSub1.Text = t.Subject1;
                tSub2.Text = t.Subject2;
            }
            else if (p is Admin a)
            {
                aSalary.Text = a.Salary.ToString();
                aWork.Text = a.WorkType;
                aHours.Text = a.WorkingHours.ToString();
            }
            else if (p is Student s)
            {
                sSub1.Text = s.Subject1;
                sSub2.Text = s.Subject2;
                sSub3.Text = s.Subject3;
            }

            UpdateGroup();
        }

        private void InitUI()
        {
            this.Text = "Add / Edit";
            this.Width = 420;
            this.Height = 520;

            int top = 20;

            txtName = Create("Name", ref top, out errName);
            txtPhone = Create("Phone", ref top, out errPhone);
            txtEmail = Create("Email", ref top, out errEmail);

            Controls.Add(new Label() { Text = "Role", Left = 20, Top = top + 3 });

            comboRole = new ComboBox() { Left = 120, Top = top, Width = 150 };
            comboRole.Items.AddRange(new string[] { "Teacher", "Admin", "Student" });
            comboRole.SelectedIndexChanged += (s, e) => UpdateGroup();

            Controls.Add(comboRole);
            top += 50;

            grpTeacher = new GroupBox() { Text = "Teacher", Left = 20, Top = top, Width = 350, Height = 120 };
            tSalary = Add(grpTeacher, "Salary", 20, out errTSalary);
            tSub1 = Add(grpTeacher, "Subject1", 50, out errTSub1);
            tSub2 = Add(grpTeacher, "Subject2", 80, out errTSub2);
            Controls.Add(grpTeacher);

            grpStudent = new GroupBox() { Text = "Student", Left = 20, Top = top, Width = 350, Height = 120 };
            sSub1 = Add(grpStudent, "Subject1", 20, out errSSub1);
            sSub2 = Add(grpStudent, "Subject2", 50, out errSSub2);
            sSub3 = Add(grpStudent, "Subject3", 80, out errSSub3);
            Controls.Add(grpStudent);

            grpAdmin = new GroupBox() { Text = "Admin", Left = 20, Top = top, Width = 350, Height = 120 };
            aSalary = Add(grpAdmin, "Salary", 20, out errASalary);
            aWork = Add(grpAdmin, "WorkType", 50, out errWork);
            aHours = Add(grpAdmin, "Hours", 80, out errHours);
            Controls.Add(grpAdmin);

            btnSave = new Button() { Text = "Save", Left = 150, Top = top + 140 };
            btnSave.Click += Save;
            Controls.Add(btnSave);

            errRole = new Label() { Left = 120, Top = comboRole.Bottom + 2, ForeColor = Color.Red, Visible = false };
            Controls.Add(errRole);

            UpdateGroup();
        }

        private void UpdateGroup()
        {
            grpTeacher.Visible = comboRole.Text == "Teacher";
            grpStudent.Visible = comboRole.Text == "Student";
            grpAdmin.Visible = comboRole.Text == "Admin";
        }

        private void SetupValidation()
        {
            txtName.TextChanged += (s, e) => ValidateName();
            txtEmail.TextChanged += (s, e) => ValidateEmail();
            txtPhone.TextChanged += (s, e) => ValidatePhone();

            tSalary.TextChanged += (s, e) => ValidateSalary();
            aSalary.TextChanged += (s, e) => ValidateSalary();
            aHours.TextChanged += (s, e) => ValidateHours();
            aWork.TextChanged += (s, e) => ValidateWorkType();

            comboRole.SelectedIndexChanged += (s, e) => ValidateRole();
        }

        private bool ValidateName()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                errName.Text = "Required";
                errName.Visible = true;
                return false;
            }
            errName.Visible = false;
            return true;
        }

        private bool ValidateEmail()
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(txtEmail.Text);
                errEmail.Visible = false;
                return true;
            }
            catch
            {
                errEmail.Text = "Invalid";
                errEmail.Visible = true;
                return false;
            }
        }

        private bool ValidatePhone()
        {
            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                errPhone.Text = "Required";
                errPhone.Visible = true;
                return false;
            }
            errPhone.Visible = false;
            return true;
        }

        private bool ValidateRole()
        {
            if (string.IsNullOrEmpty(comboRole.Text))
            {
                errRole.Text = "Select role";
                errRole.Visible = true;
                return false;
            }
            errRole.Visible = false;
            return true;
        }

        private bool ValidateSalary()
        {
            if (comboRole.Text == "Teacher")
            {
                if (!double.TryParse(tSalary.Text, out double s) || s <= 0)
                {
                    errTSalary.Text = "Invalid";
                    errTSalary.Visible = true;
                    return false;
                }
                errTSalary.Visible = false;
            }

            if (comboRole.Text == "Admin")
            {
                if (!double.TryParse(aSalary.Text, out double s) || s <= 0)
                {
                    errASalary.Text = "Invalid";
                    errASalary.Visible = true;
                    return false;
                }
                errASalary.Visible = false;
            }

            return true;
        }

        private bool ValidateHours()
        {
            if (comboRole.Text == "Admin")
            {
                if (!int.TryParse(aHours.Text, out int h) || h <= 0)
                {
                    errHours.Text = "Invalid";
                    errHours.Visible = true;
                    return false;
                }
                errHours.Visible = false;
            }
            return true;
        }

        private bool ValidateWorkType()
        {
            if (comboRole.Text == "Admin" && string.IsNullOrWhiteSpace(aWork.Text))
            {
                errWork.Text = "Required";
                errWork.Visible = true;
                return false;
            }
            errWork.Visible = false;
            return true;
        }

        private void Save(object sender, EventArgs e)
        {
            if (!(ValidateName() & ValidateEmail() & ValidatePhone() &
                  ValidateSalary() & ValidateHours() & ValidateWorkType() & ValidateRole()))
                return;

            // ===== FIX EDIT =====
            if (editingPerson != null)
            {
                if (editingPerson is Teacher t)
                {
                    t.Name = txtName.Text;
                    t.Phone = txtPhone.Text;
                    t.Email = txtEmail.Text;
                    t.Salary = double.Parse(tSalary.Text);
                    t.Subject1 = tSub1.Text;
                    t.Subject2 = tSub2.Text;
                    PersonData = t;
                }
                else if (editingPerson is Admin a)
                {
                    a.Name = txtName.Text;
                    a.Phone = txtPhone.Text;
                    a.Email = txtEmail.Text;
                    a.Salary = double.Parse(aSalary.Text);
                    a.WorkType = aWork.Text;
                    a.WorkingHours = int.Parse(aHours.Text);
                    PersonData = a;
                }
                else if (editingPerson is Student s)
                {
                    s.Name = txtName.Text;
                    s.Phone = txtPhone.Text;
                    s.Email = txtEmail.Text;
                    s.Subject1 = sSub1.Text;
                    s.Subject2 = sSub2.Text;
                    s.Subject3 = sSub3.Text;
                    PersonData = s;
                }
            }
            else
            {
                if (comboRole.Text == "Teacher")
                {
                    PersonData = new Teacher()
                    {
                        Name = txtName.Text,
                        Phone = txtPhone.Text,
                        Email = txtEmail.Text,
                        Salary = double.Parse(tSalary.Text),
                        Subject1 = tSub1.Text,
                        Subject2 = tSub2.Text
                    };
                }
                else if (comboRole.Text == "Admin")
                {
                    PersonData = new Admin()
                    {
                        Name = txtName.Text,
                        Phone = txtPhone.Text,
                        Email = txtEmail.Text,
                        Salary = double.Parse(aSalary.Text),
                        WorkType = aWork.Text,
                        WorkingHours = int.Parse(aHours.Text)
                    };
                }
                else
                {
                    PersonData = new Student()
                    {
                        Name = txtName.Text,
                        Phone = txtPhone.Text,
                        Email = txtEmail.Text,
                        Subject1 = sSub1.Text,
                        Subject2 = sSub2.Text,
                        Subject3 = sSub3.Text
                    };
                }
            }

            DialogResult = DialogResult.OK;
        }

        private TextBox Create(string label, ref int top, out Label err)
        {
            Controls.Add(new Label() { Text = label, Left = 20, Top = top });
            TextBox txt = new TextBox() { Left = 120, Top = top };
            Controls.Add(txt);

            err = new Label() { Left = 120, Top = top + 20, ForeColor = Color.Red, Visible = false };
            Controls.Add(err);

            top += 45;
            return txt;
        }

        private TextBox Add(GroupBox g, string label, int top, out Label err)
        {
            g.Controls.Add(new Label() { Text = label, Left = 10, Top = top });
            TextBox txt = new TextBox() { Left = 120, Top = top };
            g.Controls.Add(txt);

            err = new Label() { Left = 120, Top = top + 20, ForeColor = Color.Red, Visible = false };
            g.Controls.Add(err);

            return txt;
        }
    }
}