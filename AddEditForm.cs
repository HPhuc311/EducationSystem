using System;
using System.Drawing;
using System.Windows.Forms;
using EducationSystem.Models;

namespace EducationSystem
{
    public class AddEditForm : Form
    {
        public Person PersonData { get; private set; }

        private Person editingPerson = null; // 🔥 FIX EDIT

        // Common
        TextBox txtName, txtPhone, txtEmail;
        ComboBox comboRole;
        Label errName, errEmail;

        // GroupBox
        GroupBox grpTeacher, grpStudent, grpAdmin;

        // Teacher
        TextBox tSalary, tSub1, tSub2;
        Label errSalary;

        // Student
        TextBox sSub1, sSub2, sSub3;

        // Admin
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

        // ================= UI =================
        private void InitUI()
        {
            this.Text = "Add / Edit";
            this.Width = 420;
            this.Height = 520;
            this.StartPosition = FormStartPosition.CenterScreen;

            int top = 20;

            txtName = Create("Name", ref top, out errName);
            txtPhone = Create("Phone", ref top, out _);
            txtEmail = Create("Email", ref top, out errEmail);

            Controls.Add(new Label() { Text = "Role", Left = 20, Top = top + 3 });
            comboRole = new ComboBox() { Left = 120, Top = top, Width = 150 };
            comboRole.Items.AddRange(new string[] { "Teacher", "Admin", "Student" });
            comboRole.SelectedIndexChanged += (s, e) => UpdateGroup();
            Controls.Add(comboRole);
            top += 50;

            grpTeacher = new GroupBox() { Text = "Teacher", Left = 20, Top = top, Width = 350, Height = 120 };
            tSalary = Add(grpTeacher, "Salary", 20, out errSalary);
            tSub1 = Add(grpTeacher, "Subject1", 50, out _);
            tSub2 = Add(grpTeacher, "Subject2", 80, out _);
            Controls.Add(grpTeacher);

            grpStudent = new GroupBox() { Text = "Student", Left = 20, Top = top, Width = 350, Height = 120 };
            sSub1 = Add(grpStudent, "Subject1", 20, out _);
            sSub2 = Add(grpStudent, "Subject2", 50, out _);
            sSub3 = Add(grpStudent, "Subject3", 80, out _);
            Controls.Add(grpStudent);

            grpAdmin = new GroupBox() { Text = "Admin", Left = 20, Top = top, Width = 350, Height = 120 };
            aSalary = Add(grpAdmin, "Salary", 20, out _);
            aWork = Add(grpAdmin, "WorkType", 50, out _);
            aHours = Add(grpAdmin, "Hours", 80, out errHours);
            Controls.Add(grpAdmin);

            btnSave = new Button() { Text = "Save", Left = 150, Top = top + 140, Width = 100 };
            btnSave.Click += Save;
            Controls.Add(btnSave);

            UpdateGroup();
        }

        private void UpdateGroup()
        {
            grpTeacher.Visible = comboRole.Text == "Teacher";
            grpStudent.Visible = comboRole.Text == "Student";
            grpAdmin.Visible = comboRole.Text == "Admin";
        }

        // ================= VALIDATION =================
        private void SetupValidation()
        {
            txtName.TextChanged += (s, e) => ValidateName();
            txtEmail.TextChanged += (s, e) => ValidateEmail();
            tSalary.TextChanged += (s, e) => ValidateSalary();
            aHours.TextChanged += (s, e) => ValidateHours();
        }

        private bool ValidateName()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                errName.Text = "Name is required";
                errName.Visible = true;
                txtName.BackColor = Color.MistyRose;
                return false;
            }

            errName.Visible = false;
            txtName.BackColor = Color.White;
            return true;
        }

        private bool ValidateEmail()
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(txtEmail.Text);
                errEmail.Visible = false;
                txtEmail.BackColor = Color.White;
                return true;
            }
            catch
            {
                errEmail.Text = "Invalid email";
                errEmail.Visible = true;
                txtEmail.BackColor = Color.MistyRose;
                return false;
            }
        }

        private bool ValidateSalary()
        {
            if (!double.TryParse(tSalary.Text, out _))
            {
                errSalary.Text = "Salary must be number";
                errSalary.Visible = true;
                return false;
            }

            errSalary.Visible = false;
            return true;
        }

        private bool ValidateHours()
        {
            if (!int.TryParse(aHours.Text, out _))
            {
                errHours.Text = "Must be integer";
                errHours.Visible = true;
                return false;
            }

            errHours.Visible = false;
            return true;
        }

        // ================= SAVE =================
        private void Save(object sender, EventArgs e)
        {
            bool valid = ValidateName() & ValidateEmail();

            if (comboRole.Text == "Teacher")
                valid &= ValidateSalary();

            if (comboRole.Text == "Admin")
                valid &= ValidateHours();

            if (!valid) return;

            try
            {
                // 🔥 EDIT MODE
                if (editingPerson != null)
                {
                    editingPerson.Name = txtName.Text;
                    editingPerson.Phone = txtPhone.Text;
                    editingPerson.Email = txtEmail.Text;

                    if (editingPerson is Teacher t)
                    {
                        t.Salary = double.Parse(tSalary.Text);
                        t.Subject1 = tSub1.Text;
                        t.Subject2 = tSub2.Text;
                    }
                    else if (editingPerson is Admin a)
                    {
                        a.Salary = double.Parse(aSalary.Text);
                        a.WorkType = aWork.Text;
                        a.WorkingHours = int.Parse(aHours.Text);
                    }
                    else if (editingPerson is Student s)
                    {
                        s.Subject1 = sSub1.Text;
                        s.Subject2 = sSub2.Text;
                        s.Subject3 = sSub3.Text;
                    }

                    DialogResult = DialogResult.OK;
                    return;
                }

                // ADD MODE
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

                DialogResult = DialogResult.OK;
            }
            catch
            {
                MessageBox.Show("Invalid input!");
            }
        }

        // ================= HELPERS =================
        private TextBox Create(string label, ref int top, out Label errLabel)
        {
            Controls.Add(new Label() { Text = label, Left = 20, Top = top + 3 });

            TextBox txt = new TextBox() { Left = 120, Top = top, Width = 180 };
            Controls.Add(txt);

            errLabel = new Label()
            {
                Left = 120,
                Top = top + 22,
                ForeColor = Color.Red,
                AutoSize = true,
                Visible = false
            };
            Controls.Add(errLabel);

            top += 45;
            return txt;
        }

        private TextBox Add(GroupBox g, string label, int top, out Label errLabel)
        {
            g.Controls.Add(new Label() { Text = label, Left = 10, Top = top });

            TextBox txt = new TextBox() { Left = 120, Top = top, Width = 150 };
            g.Controls.Add(txt);

            errLabel = new Label()
            {
                Left = 120,
                Top = top + 20,
                ForeColor = Color.Red,
                AutoSize = true,
                Visible = false
            };
            g.Controls.Add(errLabel);

            return txt;
        }
    }
}