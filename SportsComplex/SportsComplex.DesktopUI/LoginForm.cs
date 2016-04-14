using SportsComplex.Entities;
using SportsComplex.Repositories;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace SportsComplex.DesktopUI
{
    public partial class LoginForm : Form
    {
        #region Fields
        private string _connString = ConfigurationManager.ConnectionStrings["SportsComplexConnectionString"].ConnectionString;
        private SqlUsersRepository _userRepository;
        #endregion

        #region Constructors
        public LoginForm()
        {
            InitializeComponent();

            _userRepository = new SqlUsersRepository(_connString);
        }
        #endregion

        #region Properties
        // Logged user information.
        public User User { get; set; }
        #endregion

        #region Methods
        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                int userId = _userRepository.Auth(tbLogin.Text, Encryptor.MD5Hash(tbPassword.Text));

                var query = from user in _userRepository.SelectAll()
                            where user.Id.Equals(userId)
                            select user;

                this.User = query.ToList()[0];

                this.DialogResult = DialogResult.OK;
            }
            catch(SqlException ex)
            {
                MessageBox.Show(ex.Message, "Authentification error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}
