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
    public partial class PriceListForm : Form
    {
        private string _connString = ConfigurationManager.ConnectionStrings["SportsComplexConnectionString"].ConnectionString;
        private SqlSportsHallsRepository _hallsRepository;

        public PriceListForm()
        {
            InitializeComponent();

            _hallsRepository = new SqlSportsHallsRepository(_connString);
        }

        private void PriceListForm_Load(object sender, EventArgs e)
        {
            // Load price list.
            var sportsHalls = _hallsRepository.SelectAll();

            foreach (var hall in sportsHalls)
            {
                dgvPriceList.Rows.Add(hall.Id, hall.Type.Name, hall.Area, hall.Rate);
            }
        }
    }
}
