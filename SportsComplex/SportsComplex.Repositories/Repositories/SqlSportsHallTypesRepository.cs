using System.Collections.Generic;
using SportsComplex.Entities;
using System.Data.SqlClient;
using System.Data;

namespace SportsComplex.Repositories
{
    public class SqlSportsHallTypesRepository : SqlBaseRepository, ISportsHallTypesRepository
    {
        public SqlSportsHallTypesRepository(string connnectionStr)
        {
            ConnectionString = connnectionStr;
        }

        public int Add(string name)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spAddSportsHallType";

                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.Add(new SqlParameter("@hallTypeId",
                                                        SqlDbType.Int,
                                                        0,
                                                        ParameterDirection.Output,
                                                        false,
                                                        0,
                                                        0,
                                                        "tblClassType",
                                                        DataRowVersion.Default,
                                                        null));
                    command.ExecuteNonQuery();

                    int hallTypeId = (int)command.Parameters["@hallTypeId"].Value;

                    return hallTypeId;
                }
            }
        }

        public void Remove(int id)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "DELETE FROM tblClassType WHERE Id = @id";

                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public IEnumerable<SportsHallType> SelectAll()
        {
            var hallsTypes = new List<SportsHallType>();

            // Get data from database.
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "SELECT * FROM tblClassType";

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var sportsHall = new SportsHallType()
                            {
                                Id = (int)reader["Id"],
                                Name = (string)reader["Name"]
                            };

                            hallsTypes.Add(sportsHall);
                        }
                    }
                }
            }

            return hallsTypes;
        }
    }
}
