using SportsComplex.Entities;
using SportsComplex.Repositories;
using System;
using System.Configuration;
using System.Windows.Forms;

namespace SportsComplex.DesktopUI
{
    public partial class ExtendRentForm : Form
    {
        #region Fields
        private string _connString = ConfigurationManager.ConnectionStrings["SportsComplexConnectionString"].ConnectionString;

        private SqlRentsRepository _rentsRepository;
        private RentItem _rent;
        #endregion

        #region Constructors
        public ExtendRentForm(RentItem rent)
        {
            InitializeComponent();

            _rentsRepository = new SqlRentsRepository(_connString);
            
            // Accept parameter.
            _rent = rent;
        }
        #endregion

        #region Methods
        private void dtpTimeTo_ValueChanged(object sender, EventArgs e)
        {
            TimeSpan ts = dtpTimeFrom.Value - dtpTimeTo.Value;
            decimal _costTotal = _rent.SportsHall.Rate * (decimal)(ts.TotalMinutes / 60);

            lblOverpay.Text = Math.Abs(Math.Round(_costTotal, 2, MidpointRounding.AwayFromZero)).ToString();
        }

        private void ExtendRentForm_Load(object sender, EventArgs e)
        {
            dtpTimeFrom.Value = _rent.DateEnd.AddMinutes(1);
            dtpTimeTo.MinDate = _rent.DateEnd.AddMinutes(2);
            dtpTimeTo.Value = _rent.DateEnd.AddHours(1);
        }

        private void btnExtendRent_Click(object sender, EventArgs e)
        {
            _rentsRepository.ExtendRent(_rent.Id, dtpTimeTo.Value, Convert.ToDecimal(lblOverpay.Text));
            this.DialogResult = DialogResult.OK;
            return;
        }
        #endregion
    }
}
