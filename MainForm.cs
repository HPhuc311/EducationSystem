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
                Width = 740,
                Height = 360,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            this.Controls.Add(comboRole);
            this.Controls.Add(btnView);
            this.Controls.Add(btnAdd);
            this.Controls.Add(btnEdit);
            this.Controls.Add(btnDelete);
            this.Controls.Add(dgv);
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
    }
}