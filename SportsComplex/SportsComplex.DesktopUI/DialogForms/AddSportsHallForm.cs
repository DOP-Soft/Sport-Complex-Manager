using SportsComplex.Entities;
using SportsComplex.Repositories;
using System;
using System.Configuration;
using System.Windows.Forms;

namespace SportsComplex.DesktopUI
{
    public partial class AddSportsHallForm : Form
    {
        #region Fields
        private string _connString = ConfigurationManager.ConnectionStrings["SportsComplexConnectionString"].ConnectionString;
        private SqlSportsHallTypesRepository _sportsHallTypesRepository;
        private SqlSportsHallsRepository _sportsHallRepository;
        #endregion

        #region Constructors
        public AddSportsHallForm()
        {
            InitializeComponent();

            _sportsHallTypesRepository = new SqlSportsHallTypesRepository(_connString);
            _sportsHallRepository = new SqlSportsHallsRepository(_connString);
        }
        #endregion

        #region Methods
        private void AddSportsHallForm_Load(object sender, EventArgs e)
        {
            cbType.Items.Clear();

            var hallTypes = _sportsHallTypesRepository.SelectAll();
            foreach (var ht in hallTypes)
            {
                cbType.Items.Add(ht);
            }
        }

        private void btnSportsHall_Click(object sender, EventArgs e)
        {
            if (cbType.SelectedIndex >= 0)
            {
                _sportsHallRepository.Add(((SportsHallType)(cbType.SelectedItem)).Id, (int)nudArea.Value, nudRate.Value);
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("First, chose a type of sports hall", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        #endregion
    }
}
