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
    public partial class RentersForm : Form
    {
        public RentersForm()
        {
            InitializeComponent();

            _customersRepository = new SqlCustomersRepository(connString);
        }

        private void RentersForm_Load(object sender, EventArgs e)
        {
            var customers = _customersRepository.SelectAll();

            foreach (var r in customers)
            {
                dgvCustomers.Rows.Add(r.Id, r.FirstName, r.LastName, r.Phone);
            }
        }



        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddCustomerForm frmAddRenter = new AddCustomerForm();
            if (frmAddRenter.ShowDialog() == DialogResult.OK)
            {
                dgvCustomers.Rows.Clear();
                var renters = _customersRepository.SelectAll();
                // Update customers data grid.
                foreach (var r in renters)
                {
                    dgvCustomers.Rows.Add(r.Id, r.FirstName, r.LastName, r.Phone);
                }
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (dgvCustomers.SelectedCells.Count > 0)
            {
                int row = dgvCustomers.SelectedCells[0].RowIndex;

                try
                {
                    // Remove selected renter. 
                    _customersRepository.Remove((int)dgvCustomers[0, row].Value);
                    // Remove item from dataGridView.
                    dgvCustomers.Rows.RemoveAt(dgvCustomers.SelectedCells[0].RowIndex);
                }
                catch (SqlException)
                {
                    MessageBox.Show("Selected customer cannot be removed as he has a rent", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dgvCustomers_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int id = (int)dgvCustomers[0, e.RowIndex].Value;
            string firstName = (string)dgvCustomers[1, e.RowIndex].Value;
            string lastName = (string)dgvCustomers[2, e.RowIndex].Value;
            string phone = (string)dgvCustomers[3, e.RowIndex].Value;

            try
            {
                _customersRepository.Edit(id, lastName, firstName, phone);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Edit error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private SqlCustomersRepository _customersRepository;
        private string connString = ConfigurationManager.ConnectionStrings["SportsComplexConnectionString"].ConnectionString;
    }
}
