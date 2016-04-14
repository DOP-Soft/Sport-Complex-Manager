using System.Collections.Generic;
using SportsComplex.Entities;
using System.Data.SqlClient;
using System.Data;

namespace SportsComplex.Repositories
{
    public class SqlUsersRepository : SqlBaseRepository, IUsersRepository
    {
        public SqlUsersRepository(string connectionString)
        {
            ConnectionString = connectionString;
        }

        // Returns user's ID if success. 
        public int Auth(string login, string password)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spCheckUser";

                    command.Parameters.Add(new SqlParameter("@UserId",
                                                        SqlDbType.Int,
                                                        0,
                                                        ParameterDirection.Output,
                                                        false,
                                                        0,
                                                        0,
                                                        "User",
                                                        DataRowVersion.Default,
                                                        null));
                    command.Parameters.AddWithValue("@Login", login);
                    command.Parameters.AddWithValue("@Password", password);
                    
                    command.ExecuteNonQuery();
                    int userId = (int)command.Parameters["@UserId"].Value;

                    return userId;
                }
            }
        }

        public IEnumerable<User> SelectAll()
        {
            var users = new List<User>();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string usersQuery = @"SELECT Id, LastName, FirstName, Login, Password, Disabled 
                                    FROM tblUser 
                                    ORDER BY LastName";

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = usersQuery;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            User user = new User()
                            {
                                Id = (int)reader["Id"],
                                LastName = (string) reader["LastName"],
                                FirstName = (string) reader["FirstName"],
                                Login = (string) reader["Login"],
                                Password = (string) reader["Password"],
                                Disabled = (bool) reader["Disabled"]
                            };

                            users.Add(user);
                        }
                    }
                }
            }
            return users;
        }
    }
}
