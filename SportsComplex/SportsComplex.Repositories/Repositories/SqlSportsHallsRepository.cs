using System.Collections.Generic;
using SportsComplex.Entities;
using System.Data.SqlClient;
using System.Data;

namespace SportsComplex.Repositories
{
    public class SqlSportsHallsRepository : SqlBaseRepository, ISportsHallsRepository
    {
        #region Constants
        private const string _getHallsByFilter = @"SELECT c.Id, c.ClassTypeId as [TypeId], ct.Name as [TypeName], c.Area,                                            c.Rate
	                                                FROM [tblClass] c
	                                                INNER JOIN [tblClassType] ct  ON c.ClassTypeId = ct.Id
	                                                WHERE (c.Area BETWEEN @areaMin AND @areaMax)
	                                                AND (c.Rate BETWEEN @rateMin AND @rateMax)";
        #endregion

        public SqlSportsHallsRepository(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public IEnumerable<SportsHall> GetFreeSportsHalls()
        {
            var halls = new List<SportsHall>();

            // Get data from database.
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "SELECT * FROM vFreeSportsHalls";

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var sportsHall = new SportsHall()
                            {
                                Id = (int)reader["Id"],
                                Type = new SportsHallType()
                                {
                                    Id = (int)reader["TypeId"],
                                    Name = (string)reader["TypeName"]
                                },
                                Area = (int)reader["Area"],
                                Rate = (decimal)reader["Rate"],
                            };

                            halls.Add(sportsHall);
                        }
                    }
                }
            }

            return halls;
        }

        public IEnumerable<SportsHall> GetSportsHallsByFilter(SportsHallType sportsHallType, int minArea, int maxArea, decimal minRate, decimal maxRate)
        {
            var sportsHalls = new List<SportsHall>();

            // Get data from database.
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = _getHallsByFilter;

                    command.Parameters.AddWithValue("@areaMin", minArea);
                    command.Parameters.AddWithValue("@areaMax", maxArea);
                    command.Parameters.AddWithValue("@rateMin", minRate);
                    command.Parameters.AddWithValue("@rateMax", maxRate);

                    if (sportsHallType != null)
                    {
                        command.Parameters.AddWithValue("@hallTypeId", sportsHallType.Id);
                        command.CommandText += "AND c.ClassTypeId = @hallTypeId";
                    }

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var sportsHall = new SportsHall()
                            {
                                Id = (int)reader["Id"],
                                Type = new SportsHallType()
                                {
                                    Id = (int)reader["TypeId"],
                                    Name = (string)reader["TypeName"]
                                },
                                Area = (int)reader["Area"],
                                Rate = (decimal)reader["Rate"]
                            };
                            sportsHalls.Add(sportsHall);
                        }
                    }
                }
            }

            return sportsHalls;
        }

        public IEnumerable<SportsHall> SelectAll()
        {
            var sportsHalls = new List<SportsHall>();

            // Get data from database.
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "SELECT * FROM vPriceList";

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var sportsHall = new SportsHall()
                            {
                                Id = (int)reader["Id"],
                                Type = new SportsHallType()
                                {
                                    Id = (int)reader["TypeId"],
                                    Name = (string)reader["TypeName"]
                                },
                                Area = (int)reader["Area"],
                                Rate = (decimal)reader["Rate"]
                            };

                            sportsHalls.Add(sportsHall);
                        }
                    }
                }
            }

            return sportsHalls;
        }

        public int Add(int hallTypeId, int area, decimal rate)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spAddSportsHall";

                    command.Parameters.AddWithValue("@hallTypeId", hallTypeId);
                    command.Parameters.AddWithValue("@area", area);
                    command.Parameters.AddWithValue("@rate", rate);
                    command.Parameters.Add(new SqlParameter("@sportsHallId",
                                                        SqlDbType.Int,
                                                        0,
                                                        ParameterDirection.Output,
                                                        false,
                                                        0,
                                                        0,
                                                        "tblClass",
                                                        DataRowVersion.Default,
                                                        null));
                    command.ExecuteNonQuery();

                    int hallId = (int)command.Parameters["@sportsHallId"].Value;

                    return hallId;
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
                    command.CommandText = "DELETE FROM tblClass WHERE Id = @hallId";
                    command.Parameters.AddWithValue("@hallId", id);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
