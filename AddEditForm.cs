using EducationSystem.Models;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EducationSystem
{
    public class AddEditForm : Form
    {
        public Person PersonData { get; private set; }
        private Person editingPerson = null;

        // Common
        TextBox txtName, txtPhone, txtEmail;
        ComboBox comboRole;
        Label errName, errEmail, errPhone;
        Label errTSub1, errTSub2;
        Label errSSub1, errSSub2, errSSub3;

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
            txtPhone = Create("Phone", ref top, out errPhone);
            txtEmail = Create("Email", ref top, out errEmail);

            Controls.Add(new Label() { Text = "Role", Left = 20, Top = top + 3 });
            comboRole = new ComboBox() { Left = 120, Top = top, Width = 150 };
            comboRole.Items.AddRange(new string[] { "Teacher", "Admin", "Student" });
            comboRole.SelectedIndexChanged += (s, e) => UpdateGroup();
            Controls.Add(comboRole);
            top += 50;

            grpTeacher = new GroupBox() { Text = "Teacher", Left = 20, Top = top, Width = 350, Height = 120 };
            tSalary = Add(grpTeacher, "Salary", 20, out errSalary);
            tSub1 = Add(grpTeacher, "Subject1", 50, out errTSub1);
            tSub2 = Add(grpTeacher, "Subject2", 80, out errTSub2);
            Controls.Add(grpTeacher);

            grpStudent = new GroupBox() { Text = "Student", Left = 20, Top = top, Width = 350, Height = 120 };
            sSub1 = Add(grpStudent, "Subject1", 20, out errSSub1);
            sSub2 = Add(grpStudent, "Subject2", 50, out errSSub2);
            sSub3 = Add(grpStudent, "Subject3", 80, out errSSub3);
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
            // realtime validate
            txtName.TextChanged += (s, e) => ValidateName();
            txtEmail.TextChanged += (s, e) => ValidateEmail();
            txtPhone.TextChanged += (s, e) => ValidatePhone();
            tSalary.TextChanged += (s, e) => ValidateSalary();
            aSalary.TextChanged += (s, e) => ValidateSalary();
            aHours.TextChanged += (s, e) => ValidateHours();
            tSub1.TextChanged += (s, e) => ValidateSubjects();
            tSub2.TextChanged += (s, e) => ValidateSubjects();
            sSub1.TextChanged += (s, e) => ValidateSubjects();
            sSub2.TextChanged += (s, e) => ValidateSubjects();
            sSub3.TextChanged += (s, e) => ValidateSubjects();

            // Block incorrect input
            txtPhone.KeyPress += OnlyNumber_KeyPress;
            tSalary.KeyPress += OnlyNumber_KeyPress;
            aSalary.KeyPress += OnlyNumber_KeyPress;
            aHours.KeyPress += OnlyNumber_KeyPress;

            txtName.KeyPress += OnlyText_KeyPress;
            tSub1.KeyPress += OnlyText_KeyPress;
            tSub2.KeyPress += OnlyText_KeyPress;
            sSub1.KeyPress += OnlyText_KeyPress;
            sSub2.KeyPress += OnlyText_KeyPress;
            sSub3.KeyPress += OnlyText_KeyPress;
        }

        // ===== INPUT FILTER =====
        private void OnlyNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void OnlyText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar))
                e.Handled = true;
        }

        // ===== VALIDATE =====
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

        private bool ValidatePhone()
        {
            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                errPhone.Text = "Phone is required";
                errPhone.Visible = true;
                txtPhone.BackColor = Color.MistyRose;
                return false;
            }

            if (txtPhone.Text.Length < 9 || txtPhone.Text.Length > 11)
            {
                errPhone.Text = "Phone must be 9-11 digits";
                errPhone.Visible = true;
                txtPhone.BackColor = Color.MistyRose;
                return false;
            }

            errPhone.Visible = false;
            txtPhone.BackColor = Color.White;
            return true;
        }

        private bool ValidateSalary()
        {
            if (comboRole.Text == "Teacher")
            {
                if (!double.TryParse(tSalary.Text, out double sal) || sal <= 0)
                {
                    errSalary.Text = "Salary must be > 0";
                    errSalary.Visible = true;
                    tSalary.BackColor = Color.MistyRose;
                    return false;
                }

                errSalary.Visible = false;
                tSalary.BackColor = Color.White;
            }

            if (comboRole.Text == "Admin")
            {
                if (!double.TryParse(aSalary.Text, out double sal) || sal <= 0)
                {
                    MessageBox.Show("Admin salary must be > 0");
                    return false;
                }
            }

            return true;
        }

        private bool ValidateHours()
        {
            if (comboRole.Text == "Admin")
            {
                if (!int.TryParse(aHours.Text, out int h) || h <= 0)
                {
                    errHours.Text = "Hours must be > 0";
                    errHours.Visible = true;
                    aHours.BackColor = Color.MistyRose;
                    return false;
                }

                errHours.Visible = false;
                aHours.BackColor = Color.White;
            }

            return true;
        }

        private bool ValidateSubjects()
        {
            bool valid = true;

            // ===== TEACHER =====
            if (comboRole.Text == "Teacher")
            {
                if (string.IsNullOrWhiteSpace(tSub1.Text))
                {
                    errTSub1.Text = "Please enter the blank";
                    errTSub1.Visible = true;
                    tSub1.BackColor = Color.MistyRose;
                    valid = false;
                }
                else
                {
                    errTSub1.Visible = false;
                    tSub1.BackColor = Color.White;
                }

                if (string.IsNullOrWhiteSpace(tSub2.Text))
                {
                    errTSub2.Text = "Please enter the blank";
                    errTSub2.Visible = true;
                    tSub2.BackColor = Color.MistyRose;
                    valid = false;
                }
                else
                {
                    errTSub2.Visible = false;
                    tSub2.BackColor = Color.White;
                }
            }

            // ===== STUDENT =====
            if (comboRole.Text == "Student")
            {
                if (string.IsNullOrWhiteSpace(sSub1.Text))
                {
                    errSSub1.Text = "Please enter the blank";
                    errSSub1.Visible = true;
                    sSub1.BackColor = Color.MistyRose;
                    valid = false;
                }
                else
                {
                    errSSub1.Visible = false;
                    sSub1.BackColor = Color.White;
                }

                if (string.IsNullOrWhiteSpace(sSub2.Text))
                {
                    errSSub2.Text = "Please enter the blank";
                    errSSub2.Visible = true;
                    sSub2.BackColor = Color.MistyRose;
                    valid = false;
                }
                else
                {
                    errSSub2.Visible = false;
                    sSub2.BackColor = Color.White;
                }

                if (string.IsNullOrWhiteSpace(sSub3.Text))
                {
                    errSSub3.Text = "Please enter the blank";
                    errSSub3.Visible = true;
                    sSub3.BackColor = Color.MistyRose;
                    valid = false;
                }
                else
                {
                    errSSub3.Visible = false;
                    sSub3.BackColor = Color.White;
                }
            }

            return valid;
        }

        // ================= SAVE =================
        private void Save(object sender, EventArgs e)
        {
            bool valid = true;

            valid &= ValidateName();
            valid &= ValidateEmail();
            valid &= ValidatePhone();
            valid &= ValidateSalary();
            valid &= ValidateHours();
            valid &= ValidateSubjects();

            if (!valid) return;

            try
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