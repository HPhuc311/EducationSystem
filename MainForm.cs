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
        }

        private void InitializeUI()
        {
            this.Text = "Education System";
            this.Width = 800;
            this.Height = 500;
            this.StartPosition = FormStartPosition.CenterScreen;

            // ComboBox
            comboRole = new ComboBox()
            {
                Left = 20,
                Top = 20,
                Width = 120
            };

            // Buttons
            btnView = new Button() { Text = "View", Left = 160, Top = 20, Width = 80 };
            btnAdd = new Button() { Text = "Add", Left = 250, Top = 20, Width = 80 };
            btnEdit = new Button() { Text = "Edit", Left = 340, Top = 20, Width = 80 };
            btnDelete = new Button() { Text = "Delete", Left = 430, Top = 20, Width = 80 };

            // Events
            btnView.Click += (s, e) => LoadData();
            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;

            // DataGridView
            dgv = new DataGridView()
            {
                Left = 20,
                Top = 60,
                Width = 540,
                Height = 360,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            };

            this.Controls.Add(comboRole);
            this.Controls.Add(btnView);
            this.Controls.Add(btnAdd);
            this.Controls.Add(btnEdit);
            this.Controls.Add(btnDelete);
            this.Controls.Add(dgv);

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
                Top = 20,
                Width = 80
            };

            btnSearch.Click += BtnSearch_Click;

            this.Controls.Add(txtSearch);
            this.Controls.Add(btnSearch);

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
            this.Controls.Add(detailPanel);
        }

        private void LoadRoles()
        {
            comboRole.Items.AddRange(new string[] { "All", "Teacher", "Admin", "Student" });
            comboRole.SelectedIndex = 0;
        }

        private void LoadData()
        {
            dgv.DataSource = null;

            currentList = comboRole.Text == "All"
                ? manager.GetAll()
                : manager.GetByRole(comboRole.Text);

            dgv.DataSource = currentList.Select(p => new
            {
                p.Name,
                p.Phone,
                p.Email,
                Role = p.GetRole()
            }).ToList();
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

            Person p = currentList[dgv.CurrentRow.Index];

            AddEditForm form = new AddEditForm(p);

            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadData();
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;

            Person p = currentList[dgv.CurrentRow.Index];
            manager.Delete(p);

            LoadData();
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            dgv.DataSource = null;

            currentList = manager.Search(txtSearch.Text);

            dgv.DataSource = currentList.Select(p => new
            {
                p.Name,
                p.Phone,
                p.Email,
                Role = p.GetRole()
            }).ToList();
        }

        private void Dgv_SelectionChanged(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;

            Person p = currentList[dgv.CurrentRow.Index];

            dgvDetail.DataSource = GetDetailTable(p);
        }


        private object GetDetailTable(Person p)
        {
            var list = new List<object>();

            list.Add(new { Field = "Role", Value = p.GetRole() });
            list.Add(new { Field = "Name", Value = p.Name });
            list.Add(new { Field = "Phone", Value = p.Phone });
            list.Add(new { Field = "Email", Value = p.Email });

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