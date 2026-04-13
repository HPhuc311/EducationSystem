using EducationCenterSystem;
using EducationSystem.Models;
using EducationSystem.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace EducationSystem
{
    public class MainForm : Form
    {
        private PersonManager manager = new PersonManager();
        private List<Person> currentList = new List<Person>();

        private TextBox txtSearch;
        private Button btnSearch;

        private Panel detailPanel;
        private DataGridView dgvDetail;

        private DataGridView dgv;
        private ComboBox comboRole;
        private Button btnAdd, btnView, btnEdit, btnDelete;

        public MainForm()
        {
            InitializeUI();
            LoadRoles();
            SeedData();
            LoadData();
        }

        private void InitializeUI()
        {
            this.Text = "Education System";
            this.Width = 800;
            this.Height = 500;
            this.StartPosition = FormStartPosition.CenterScreen;

            comboRole = new ComboBox()
            {
                Left = 20,
                Top = 20,
                Width = 120
            };

            btnView = new Button() { Text = "View", Left = 160, Top = 20 };
            btnAdd = new Button() { Text = "Add", Left = 250, Top = 20 };
            btnEdit = new Button() { Text = "Edit", Left = 340, Top = 20 };
            btnDelete = new Button() { Text = "Delete", Left = 430, Top = 20 };

            btnView.Click += (s, e) => LoadData();
            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;

            dgv = new DataGridView()
            {
                Left = 20,
                Top = 60,
                Width = 540,
                Height = 360,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            dgv.SelectionChanged += Dgv_SelectionChanged;

            txtSearch = new TextBox()
            {
                Left = 520,
                Top = 20,
                Width = 150
            };

            btnSearch = new Button()
            {
                Text = "Search",
                Left = 680,
                Top = 20
            };

            btnSearch.Click += BtnSearch_Click;

            detailPanel = new Panel()
            {
                Left = 580,
                Top = 60,
                Width = 200,
                Height = 360,
                BorderStyle = BorderStyle.FixedSingle
            };

            dgvDetail = new DataGridView()
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false
            };

            detailPanel.Controls.Add(dgvDetail);

            this.Controls.Add(comboRole);
            this.Controls.Add(btnView);
            this.Controls.Add(btnAdd);
            this.Controls.Add(btnEdit);
            this.Controls.Add(btnDelete);
            this.Controls.Add(dgv);
            this.Controls.Add(txtSearch);
            this.Controls.Add(btnSearch);
            this.Controls.Add(detailPanel);
        }

        private void LoadRoles()
        {
            comboRole.Items.AddRange(new string[] { "All", "Teacher", "Admin", "Student" });
            comboRole.SelectedIndex = 0;
        }

        private void LoadData()
        {
            List<Person> data;

            if (comboRole.Text == "All")
                data = manager.GetAllPeople();
            else
                data = manager.GetPeopleByRole(comboRole.Text);

            currentList = data;

            DisplayData(data);
        }

        public class DetailItem
        {
            public string Field { get; set; }
            public object Value { get; set; }
        }

        private void DisplayData(List<Person> list)
        {
            // Prevent SelectionChanged firing while we rebind the grid
            dgv.SelectionChanged -= Dgv_SelectionChanged;
            dgv.DataSource = null;
            dgv.DataSource = list;
            dgv.SelectionChanged += Dgv_SelectionChanged;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            AddEditForm form = new AddEditForm();

            if (form.ShowDialog() == DialogResult.OK)
            {
                manager.Add(form.PersonData);
                LoadData();
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;

            var p = dgv.CurrentRow.DataBoundItem as Person;
            if (p == null) return;

            AddEditForm form = new AddEditForm(p);

            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadData();
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;

            var p = dgv.CurrentRow.DataBoundItem as Person;
            if (p == null) return;

            manager.Delete(p);

            LoadData();
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            var result = manager.Search(txtSearch.Text);

            currentList = result;

            DisplayData(result);
        }

        private void Dgv_SelectionChanged(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;

            var selectedPerson = dgv.CurrentRow.DataBoundItem as Person;
            if (selectedPerson == null) return;

            ShowDetail(selectedPerson);
        }

        private void ShowDetail(Person p)
        {
            dgvDetail.DataSource = GetDetailTable(p);
        }

        private object GetDetailTable(Person p)
        {
            var list = new List<DetailItem>();

            list.Add(new DetailItem { Field = "Role", Value = p.GetRole() });
            list.Add(new DetailItem { Field = "Name", Value = p.Name });
            list.Add(new DetailItem { Field = "Phone", Value = p.Phone });
            list.Add(new DetailItem { Field = "Email", Value = p.Email });

            if (p is Teacher t)
            {
                list.Add(new DetailItem { Field = "Salary", Value = t.Salary });
                list.Add(new DetailItem { Field = "Subject 1", Value = t.Subject1 });
                list.Add(new DetailItem { Field = "Subject 2", Value = t.Subject2 });
            }
            else if (p is Admin a)
            {
                list.Add(new DetailItem { Field = "Salary", Value = a.Salary });
                list.Add(new DetailItem { Field = "Work Type", Value = a.WorkType });
                list.Add(new DetailItem { Field = "Working Hours", Value = a.WorkingHours });
            }
            else if (p is Student s)
            {
                list.Add(new DetailItem { Field = "Subject 1", Value = s.Subject1 });
                list.Add(new DetailItem { Field = "Subject 2", Value = s.Subject2 });
                list.Add(new DetailItem { Field = "Subject 3", Value = s.Subject3 });
            }

            return list;
        }

        private void SeedData()
        {
            manager.Add(new Teacher()
            {
                Name = "Nguyen Van A",
                Phone = "0987654321",
                Email = "a@gmail.com",
                Salary = 1500,
                Subject1 = "Math",
                Subject2 = "Physics"
            });

            manager.Add(new Teacher()
            {
                Name = "Tran Thi B",
                Phone = "0912345678",
                Email = "b@gmail.com",
                Salary = 1800,
                Subject1 = "English",
                Subject2 = "Literature"
            });

            manager.Add(new Admin()
            {
                Name = "Le Van C",
                Phone = "0909123456",
                Email = "c@gmail.com",
                Salary = 2000,
                WorkType = "Full-time",
                WorkingHours = 8
            });

            manager.Add(new Student()
            {
                Name = "Pham Thi D",
                Phone = "0977888999",
                Email = "d@gmail.com",
                Subject1 = "Math",
                Subject2 = "Chemistry",
                Subject3 = "Biology"
            });

            manager.Add(new Student()
            {
                Name = "Hoang Van E",
                Phone = "0966555444",
                Email = "e@gmail.com",
                Subject1 = "History",
                Subject2 = "Geography",
                Subject3 = "Civic"
            });
        }
    }
}