using SportsComplex.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SportsComplex.DesktopUI
{
    public partial class SportsHallsForm : Form
    {
        public SportsHallsForm()
        {
            InitializeComponent();

            _sportsHallsRepository = new SqlSportsHallsRepository(ConfigurationManager.ConnectionStrings["SportsComplexConnectionString"].ConnectionString);
        }

        private void btnSportsHallTypes_Click(object sender, EventArgs e)
        {
            SportsHallTypesForm frmSportsHallTypes = new SportsHallTypesForm();
            frmSportsHallTypes.ShowDialog();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddSportsHallForm frmAddSportsHall = new AddSportsHallForm();
            if (frmAddSportsHall.ShowDialog() == DialogResult.OK)
            {
                UpdateSportsHalls();

            }
        }

        private void UpdateSportsHalls()
        {
            dgvSportsHalls.Rows.Clear();

            var halls = _sportsHallsRepository.SelectAll();
            foreach (var hall in halls)
            {
                dgvSportsHalls.Rows.Add(hall.Id, hall.Type.Name, hall.Area, hall.Rate);
            }
        }

        private SqlSportsHallsRepository _sportsHallsRepository;

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (dgvSportsHalls.SelectedRows.Count > 0)
            {
                try
                {
                    _sportsHallsRepository.Remove((int)dgvSportsHalls.SelectedRows[0].Cells[0].Value);

                    // Update UI.
                    UpdateSportsHalls();
                }
                catch(SqlException)
                {
                    MessageBox.Show("Selected sports hall can not be removed because it is already used in some rental item", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void SportsHallsForm_Load(object sender, EventArgs e)
        {
            // Update UI.
            UpdateSportsHalls();
        }
    }
}
