﻿using SportsComplex.Entities;
using SportsComplex.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows.Forms;

namespace SportsComplex.DesktopUI
{
    public partial class NewRentForm : Form
    {
        public NewRentForm()
        {
            InitializeComponent();
            _sportsHallTypesRepository = new SqlSportsHallTypesRepository(_connString);
            _sportsHallsRepository = new SqlSportsHallsRepository(_connString);
            _rentersRepository = new SqlRentersRepository(_connString);
            _rentsRepository = new SqlRentsRepository(_connString);
        }

        public int RentId { get; private set; }

        private void NewRentForm_Load(object sender, EventArgs e)
        {
            var halls = _sportsHallsRepository.SelectAll();
            var renters = _rentersRepository.SelectAll();

            // Fill renters combobox.
            cbRenter.Items.Clear();

            foreach (var r in renters)
            {
                cbRenter.Items.Add(r);
            }

            // Update UI.
            UpdateSportsHallsDataGridView(halls);

            nudAreaMin.Value = halls.Min((h) => h.Area);
            nudAreaMax.Value = halls.Max((h) => h.Area);
            nudRateMin.Value = halls.Min((h) => h.Rate);
            nudRateMax.Value = halls.Max((h) => h.Rate);

            dtpTimeFrom.Value = dtpTimeFrom.Value.AddMinutes(5);
            dtpTimeTo.Value = dtpTimeTo.Value.AddHours(1);
        }

        public void UpdateSportsHallsDataGridView(IEnumerable<SportsHall> halls)
        {
            dgvSportsHalls.Rows.Clear();

            foreach (var h in halls)
            {
                dgvSportsHalls.Rows.Add(
                    h.Id,
                    h.Type.Name,
                    h.Area,
                    h.Rate);
            }
        }



        private void cbClassType_DropDown(object sender, EventArgs e)
        {
            cbClassType.Items.Clear();

            var types = _sportsHallTypesRepository.SelectAll();

            foreach (var t in types)
            {
                cbClassType.Items.Add(t);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            var halls = _sportsHallsRepository.SelectAll();

            // Update UI.
            UpdateSportsHallsDataGridView(halls);

            nudAreaMin.Value = halls.Min((h) => h.Area);
            nudAreaMax.Value = halls.Max((h) => h.Area);
            nudRateMin.Value = halls.Min((h) => h.Rate);
            nudRateMax.Value = halls.Max((h) => h.Rate);

            cbClassType.Items.Clear();
        }

        private void btnNewRent_Click(object sender, EventArgs e)
        {
            if (dgvSportsHalls.SelectedRows.Count > 0)
            {
                int index = dgvSportsHalls.SelectedRows[0].Index;
                if (cbRenter.SelectedIndex > -1)
                {
                    DateTime dtFrom = dtpDate.Value.Date + dtpTimeFrom.Value.TimeOfDay;
                    DateTime dtTo = dtpDate.Value.Date + dtpTimeTo.Value.TimeOfDay;

                    RentId = _rentsRepository.MakeRent(
                        ((Renter)(cbRenter.SelectedItem)).Id,
                        (int)dgvSportsHalls[0, index].Value,
                        dtFrom,
                        dtTo,
                        Convert.ToDecimal(lblSumCash.Text)
                    );

                    this.DialogResult = DialogResult.OK;
                    return;
                }
                else
                {
                    MessageBox.Show("Please, chose a renter", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Please, chose sports hall", "Info");
            }
        }

        

        private void dtpTimeFrom_ValueChanged(object sender, EventArgs e)
        {
            if (dgvSportsHalls.SelectedRows.Count > 0)
            {
                int index = dgvSportsHalls.SelectedRows[0].Index;
                decimal rate = (decimal)dgvSportsHalls["ColumnRate", index].Value;
                TimeSpan ts = dtpTimeFrom.Value - dtpTimeTo.Value;
                _costTotal =  rate * (decimal)(ts.TotalMinutes / 60);

                lblSumCash.Text = Math.Abs(_costTotal).ToString();
            }
        }

        private decimal _costTotal = 0.0m;

        private void btnSearchSportsHalls_Click(object sender, EventArgs e)
        {
            var halls = _sportsHallsRepository.GetSportsHallsByFilter(
                        (SportsHallType)cbClassType.SelectedItem,
                        (int)nudAreaMin.Value,
                        (int)nudAreaMax.Value,
                         nudRateMin.Value,
                         nudRateMax.Value
                );

            // Update UI.
            UpdateSportsHallsDataGridView(halls);
        }

        private SqlSportsHallsRepository _sportsHallsRepository;
        private SqlSportsHallTypesRepository _sportsHallTypesRepository;
        private SqlRentersRepository _rentersRepository;
        private SqlRentsRepository _rentsRepository;

        private string _connString = ConfigurationManager.ConnectionStrings["SportsComplexConnectionString"].ConnectionString;
    }
}