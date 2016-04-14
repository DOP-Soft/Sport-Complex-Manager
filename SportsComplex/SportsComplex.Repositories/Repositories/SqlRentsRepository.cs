using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SportsComplex.Entities;
using System.Data.SqlClient;
using System.Data;

namespace SportsComplex.Repositories
{
    public class SqlRentsRepository : SqlBaseRepository, IRentsRepository
    {
        #region Constants
        const string getRentsOnDateQuery = @"SELECT rent.[Id]
                                          ,rent.[CustomerId]
	                                      ,customer.FirstName AS [CustomerFirstName]
	                                      ,customer.LastName AS [CustomerLastName]
	                                      ,customer.Phone AS [CustomerPhone]
                                          ,rent.[ClassId]
	                                      ,ct.Id AS [ClassTypeId]
	                                      ,ct.Name AS [ClassName]
	                                      ,c.Area AS [ClassArea]
	                                      ,c.Rate AS [ClassRate]
                                          ,rent.[DateStart]
                                          ,rent.[DateEnd]
                                          ,rent.[Cost]
                                          ,rent.[Deleted]
                                          FROM [tblRent] rent
                                          INNER JOIN [tblCustomer] customer ON rent.CustomerId = customer.Id
                                          INNER JOIN [tblClass] c ON rent.ClassId = c.Id
                                          INNER JOIN [tblClassType] ct ON c.ClassTypeId = ct.Id
                                          WHERE (@Date <= rent.DateEnd 
                                                AND DATEPART(DAY, @Date) = DATEPART(DAY, rent.DateStart))
                                          AND rent.Deleted = 0";

        const string getAllRentsQuery = @"SELECT rent.[Id]
                                          ,rent.[CustomerId]
	                                      ,customer.FirstName AS [CustomerFirstName]
	                                      ,customer.LastName AS [CustomerLastName]
	                                      ,customer.Phone AS [CustomerPhone]
                                          ,rent.[ClassId]
	                                      ,ct.Id AS [ClassTypeId]
	                                      ,ct.Name AS [ClassName]
	                                      ,c.Area AS [ClassArea]
	                                      ,c.Rate AS [ClassRate]
                                          ,rent.[DateStart]
                                          ,rent.[DateEnd]
                                          ,rent.[Cost]
                                          ,rent.[Deleted]
                                          FROM [tblRent] rent
                                          INNER JOIN [tblCustomer] customer ON rent.CustomerId = customer.Id
                                          INNER JOIN [tblClass] c ON rent.ClassId = c.Id
                                          INNER JOIN [tblClassType] ct ON c.ClassTypeId = ct.Id
                                          WHERE rent.[Deleted] = 0";

        const string getRentsByCustomerIdQuery = @"SELECT rent.[Id]
                                          ,rent.[CustomerId]
	                                      ,customer.FirstName AS [CustomerFirstName]
	                                      ,customer.LastName AS [CustomerLastName]
	                                      ,customer.Phone AS [CustomerPhone]
                                          ,rent.[ClassId]
	                                      ,ct.Id AS [ClassTypeId]
	                                      ,ct.Name AS [ClassName]
	                                      ,c.Area AS [ClassArea]
	                                      ,c.Rate AS [ClassRate]
                                          ,rent.[DateStart]
                                          ,rent.[DateEnd]
                                          ,rent.[Cost]
                                          ,rent.[Deleted]
                                          FROM [tblRent] rent
                                          INNER JOIN [tblCustomer] customer ON rent.CustomerId = customer.Id
                                          INNER JOIN [tblClass] c ON rent.ClassId = c.Id
                                          INNER JOIN [tblClassType] ct ON c.ClassTypeId = ct.Id
                                          WHERE customer.Id = @customerId AND rent.[Deleted] = 0";

        const string getRentByIdQuery = @"SELECT rent.[Id]
                                          ,rent.[CustomerId]
	                                      ,customer.FirstName AS [CustomerFirstName]
	                                      ,customer.LastName AS [CustomerLastName]
	                                      ,customer.Phone AS [CustomerPhone]
                                          ,rent.[ClassId]
	                                      ,ct.Id AS [ClassTypeId]
	                                      ,ct.Name AS [ClassName]
	                                      ,c.Area AS [ClassArea]
	                                      ,c.Rate AS [ClassRate]
                                          ,rent.[DateStart]
                                          ,rent.[DateEnd]
                                          ,rent.[Cost]
                                          ,rent.[Deleted]
                                          FROM [tblRent] rent
                                          INNER JOIN [tblCustomer] customer ON rent.CustomerId = Customer.Id
                                          INNER JOIN [tblClass] c ON rent.ClassId = c.Id
                                          INNER JOIN [tblClassType] ct ON c.ClassTypeId = ct.Id
                                          WHERE rent.Id = @rentId AND rent.[Deleted] = 0";
        #endregion

        public SqlRentsRepository(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public IEnumerable<RentItem> GetRentsOnDate(DateTime inputDate)
        {
            var rents = new List<RentItem>();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spGetRents";
                    command.Parameters.AddWithValue("@Date", inputDate);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var rentItem = new RentItem()
                            {
                                Id = (int)reader["Id"],
                                Customer = new Customer()
                                {
                                    Id = (int)reader["CustomerId"],
                                    FirstName = (string)reader["CustomerFirstName"],
                                    LastName = (string)reader["CustomerLastName"],
                                    Phone = (string)reader["CustomerPhone"]
                                },
                                SportsHall = new SportsHall()
                                {
                                    Id = (int)reader["ClassId"],
                                    Type = new SportsHallType()
                                    {
                                        Id = (int)reader["ClassTypeId"],
                                        Name = (string)reader["ClassName"]
                                    },
                                    Area = (int)reader["ClassArea"],
                                    Rate = (decimal)reader["ClassRate"]
                                },
                                DateStart = (DateTime)reader["DateStart"],
                                DateEnd = (DateTime)reader["DateEnd"],
                                Cost = (decimal)reader["Cost"],
                                Deleted = (bool)reader["Deleted"]
                            };

                            rents.Add(rentItem);
                        }
                    }
                }
            }

            return rents;
        }

        public decimal Remove(int rentId)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spRemoveRent";

                    command.Parameters.Add(new SqlParameter("@moneyChange",
                                                        SqlDbType.Money,
                                                        0,
                                                        ParameterDirection.Output,
                                                        false,
                                                        0,
                                                        0,
                                                        "tblRent",
                                                        DataRowVersion.Default,
                                                        null));
                    command.Parameters.AddWithValue("@rentID", rentId);

                    command.ExecuteNonQuery();
                    decimal moneyChange = Convert.ToDecimal(command.Parameters["@moneyChange"].Value);

                    return moneyChange;
                }
            }
        }

        public IEnumerable<RentItem> SelectAll()
        {
            var rents = new List<RentItem>();

            // Get data from database.
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = getAllRentsQuery;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var rentItem = new RentItem()
                            {
                                Id = (int)reader["Id"],
                                Customer = new Customer()
                                {
                                    Id = (int)reader["CustomerId"],
                                    FirstName = (string)reader["CustomerFirstName"],
                                    LastName = (string)reader["CustomerLastName"],
                                    Phone = (string)reader["CustomerPhone"]
                                },
                                SportsHall = new SportsHall()
                                {
                                    Id = (int)reader["ClassId"],
                                    Type = new SportsHallType()
                                    {
                                        Id = (int)reader["ClassTypeId"],
                                        Name = (string)reader["ClassName"]
                                    },
                                    Area = (int)reader["ClassArea"],
                                    Rate = (decimal)reader["ClassRate"]
                                },
                                DateStart = (DateTime)reader["DateStart"],
                                DateEnd = (DateTime)reader["DateEnd"],
                                Cost = (decimal)reader["Cost"],
                                Deleted = (bool)reader["Deleted"]
                            };

                            rents.Add(rentItem);
                        }
                    }
                }
            }

            return rents;
        }

        public IEnumerable<RentItem> GetRentsByCustomerId(int customerId)
        {
            var rents = new List<RentItem>();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = getRentsByCustomerIdQuery;
                    command.Parameters.AddWithValue("@customerId", customerId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var rentItem = new RentItem()
                            {
                                Id = (int)reader["Id"],
                                Customer = new Customer()
                                {
                                    Id = (int)reader["CustomerId"],
                                    FirstName = (string)reader["CustomerFirstName"],
                                    LastName = (string)reader["CustomerLastName"],
                                    Phone = (string)reader["CustomerPhone"]
                                },
                                SportsHall = new SportsHall()
                                {
                                    Id = (int)reader["ClassId"],
                                    Type = new SportsHallType()
                                    {
                                        Id = (int)reader["ClassTypeId"],
                                        Name = (string)reader["ClassName"]
                                    },
                                    Area = (int)reader["ClassArea"],
                                    Rate = (decimal)reader["ClassRate"]
                                },
                                DateStart = (DateTime)reader["DateStart"],
                                DateEnd = (DateTime)reader["DateEnd"],
                                Cost = (decimal)reader["Cost"],
                                Deleted = (bool)reader["Deleted"]
                            };

                            rents.Add(rentItem);
                        }
                    }
                }
            }

            return rents;
        }

        /// <summary>
        /// Make rent of sports hall.
        /// </summary>
        /// <param name="customerId">Id of customer.</param>
        /// <param name="sportsHallId">Id of sports hall</param>
        /// <param name="dateStart">Rent start date</param>
        /// <param name="dateEnd">Rent finish date</param>
        /// <param name="Cost">Rent summary cost. Depends of rent time period</param>
        /// <returns>Id of new rent</returns>
        public int MakeRent(int customerId, int sportsHallId, DateTime dateStart, DateTime dateEnd, decimal Cost)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spMakeRent";

                    command.Parameters.Add(new SqlParameter("@rentId",
                                                        SqlDbType.Int,
                                                        0,
                                                        ParameterDirection.Output,
                                                        false,
                                                        0,
                                                        0,
                                                        "tblRent",
                                                        DataRowVersion.Default,
                                                        null));

                    command.Parameters.AddWithValue("@customerId", customerId);
                    command.Parameters.AddWithValue("@sportsHallId", sportsHallId);
                    command.Parameters.AddWithValue("@dateStart", dateStart);
                    command.Parameters.AddWithValue("@dateEnd", dateEnd);
                    command.Parameters.AddWithValue("@cost", Cost);

                    command.ExecuteNonQuery();
                    int rentId = (int)command.Parameters["@rentId"].Value;

                    return rentId;
                }
            }
        }

        public void ExtendRent(int rentId, DateTime newEndDate, decimal overpay)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spExtendRent";

                    command.Parameters.AddWithValue("@rentId", rentId);
                    command.Parameters.AddWithValue("@newDateEnd", newEndDate);
                    command.Parameters.AddWithValue("@overpay", overpay);

                    command.ExecuteNonQuery();
                }
            }
        }

        public RentItem GetRentById(int rentId)
        {
            var rents = new List<RentItem>();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = getRentByIdQuery;

                    command.Parameters.AddWithValue("@rentId", rentId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var rentItem = new RentItem()
                            {
                                Id = (int)reader["Id"],
                                Customer = new Customer()
                                {
                                    Id = (int)reader["CustomerId"],
                                    FirstName = (string)reader["CustomerFirstName"],
                                    LastName = (string)reader["CustomerLastName"],
                                    Phone = (string)reader["CustomerPhone"]
                                },
                                SportsHall = new SportsHall()
                                {
                                    Id = (int)reader["ClassId"],
                                    Type = new SportsHallType()
                                    {
                                        Id = (int)reader["ClassTypeId"],
                                        Name = (string)reader["ClassName"]
                                    },
                                    Area = (int)reader["ClassArea"],
                                    Rate = (decimal)reader["ClassRate"]
                                },
                                DateStart = (DateTime)reader["DateStart"],
                                DateEnd = (DateTime)reader["DateEnd"],
                                Cost = (decimal)reader["Cost"],
                                Deleted = (bool)reader["Deleted"]
                            };

                            rents.Add(rentItem);
                        }
                    }
                }
            }

            return (rents.Count > 0) ? rents[0] : null;
        }
    }
}
