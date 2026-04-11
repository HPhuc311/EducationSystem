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
        // Manager handles all data operations
        private PersonManager manager = new PersonManager();

        // Current list displayed on DataGridView
        private List<Person> currentList = new List<Person>();

        // Search controls
        private TextBox txtSearch;
        private Button btnSearch;

        // Detail panel (right side)
        private Panel detailPanel;
        private DataGridView dgvDetail;

        // Main table and controls
        private DataGridView dgv;
        private ComboBox comboRole;
        private Button btnAdd, btnView, btnEdit, btnDelete;

        public MainForm()
        {
            InitializeUI(); // Setup UI
            LoadRoles();    // Load role options
        }

        // ================= UI =================
        private void InitializeUI()
        {
            this.Text = "Education System";
            this.Width = 800;
            this.Height = 500;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Dropdown to filter by role
            comboRole = new ComboBox()
            {
                Left = 20,
                Top = 20,
                Width = 120
            };

            // Buttons for CRUD operations
            btnView = new Button() { Text = "View", Left = 160, Top = 20 };
            btnAdd = new Button() { Text = "Add", Left = 250, Top = 20 };
            btnEdit = new Button() { Text = "Edit", Left = 340, Top = 20 };
            btnDelete = new Button() { Text = "Delete", Left = 430, Top = 20 };

            // Assign button events
            btnView.Click += (s, e) => LoadData();
            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;

            // Main DataGridView (list of users)
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

            // When selecting a row → show detail
            dgv.SelectionChanged += Dgv_SelectionChanged;

            // Search textbox
            txtSearch = new TextBox()
            {
                Left = 520,
                Top = 20,
                Width = 150
            };

            // Search button
            btnSearch = new Button()
            {
                Text = "Search",
                Left = 680,
                Top = 20
            };

            btnSearch.Click += BtnSearch_Click;

            // Detail panel on the right
            detailPanel = new Panel()
            {
                Left = 580,
                Top = 60,
                Width = 200,
                Height = 360,
                BorderStyle = BorderStyle.FixedSingle
            };

            // DataGridView to show detailed information
            dgvDetail = new DataGridView()
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false
            };

            detailPanel.Controls.Add(dgvDetail);

            // Add controls to form
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

        // Load roles into ComboBox
        private void LoadRoles()
        {
            comboRole.Items.AddRange(new string[] { "All", "Teacher", "Admin", "Student" });
            comboRole.SelectedIndex = 0;
        }

        // ================= DATA =================

        // Load data from manager based on selected role
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

        // Display list on DataGridView
        private void DisplayData(List<Person> list)
        {
            dgv.DataSource = null;

            dgv.DataSource = list.Select(p => new
            {
                Name = p.Name,
                Phone = p.Phone,
                Email = p.Email,
                Role = p.GetRole()
            }).ToList();
        }

        // ================= BUTTON EVENTS =================

        // Add new person
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            AddEditForm form = new AddEditForm();

            if (form.ShowDialog() == DialogResult.OK)
            {
                manager.Add(form.PersonData);
                LoadData();
            }
        }

        // Edit selected person
        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;

            Person p = currentList[dgv.CurrentRow.Index];

            AddEditForm form = new AddEditForm(p);

            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadData();
            }
        }

        // Delete selected person
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;

            Person p = currentList[dgv.CurrentRow.Index];
            manager.Delete(p);

            LoadData();
        }

        // Search function
        private void BtnSearch_Click(object sender, EventArgs e)
        {
            var result = manager.Search(txtSearch.Text);

            currentList = result;

            DisplayData(result);
        }

        // ================= DETAIL =================

        // When user selects a row → show detail
        private void Dgv_SelectionChanged(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;

            Person selectedPerson = currentList[dgv.CurrentRow.Index];

            ShowDetail(selectedPerson);
        }

        // Display detail in right panel
        private void ShowDetail(Person p)
        {
            dgvDetail.DataSource = GetDetailTable(p);
        }

        // Convert Person object into table format (Field - Value)
        private object GetDetailTable(Person p)
        {
            var list = new List<object>();

            list.Add(new { Field = "Role", Value = p.GetRole() });
            list.Add(new { Field = "Name", Value = p.Name });
            list.Add(new { Field = "Phone", Value = p.Phone });
            list.Add(new { Field = "Email", Value = p.Email });

            // Add extra fields depending on type
            if (p is Teacher t)
            {
                list.Add(new { Field = "Salary", Value = t.Salary });
                list.Add(new { Field = "Subject 1", Value = t.Subject1 });
                list.Add(new { Field = "Subject 2", Value = t.Subject2 });
            }
            else if (p is Admin a)
            {
                list.Add(new { Field = "Salary", Value = a.Salary });
                list.Add(new { Field = "Work Type", Value = a.WorkType });
                list.Add(new { Field = "Working Hours", Value = a.WorkingHours });
            }
            else if (p is Student s)
            {
                list.Add(new { Field = "Subject 1", Value = s.Subject1 });
                list.Add(new { Field = "Subject 2", Value = s.Subject2 });
                list.Add(new { Field = "Subject 3", Value = s.Subject3 });
            }

            return list;
        }
    }
}