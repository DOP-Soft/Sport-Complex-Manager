using System.Collections.Generic;
using SportsComplex.Entities;
using System.Data.SqlClient;
using System.Data;

namespace SportsComplex.Repositories
{
    public class SqlCustomersRepository : SqlBaseRepository, ICustomersRepository
    {
        public SqlCustomersRepository(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public int Add(string lastName, string firstName, string phone)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "spAddCustomer";

                    command.Parameters.AddWithValue("@firstName", firstName);
                    command.Parameters.AddWithValue("@lastName", lastName);
                    command.Parameters.AddWithValue("@phone", phone);
                    command.Parameters.Add(new SqlParameter("@customerId",
                                                        SqlDbType.Int,
                                                        0,
                                                        ParameterDirection.Output,
                                                        false,
                                                        0,
                                                        0,
                                                        "tblCustomer",
                                                        DataRowVersion.Default,
                                                        null));
                    command.ExecuteNonQuery();

                    int customerId = (int)command.Parameters["@customerId"].Value;

                    return customerId;
                }
            }
        }

        public void Edit(int customerId, string lastName, string firstName, string phone)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = @"UPDATE tblCustomer 
                                            SET FirstName = @firstName, LastName = @lastName, Phone = @phone
                                            WHERE Id = @customerId";

                    command.Parameters.AddWithValue("@customerId", customerId);
                    command.Parameters.AddWithValue("@firstName", firstName);
                    command.Parameters.AddWithValue("@lastName", lastName);
                    command.Parameters.AddWithValue("@phone", phone);
                    command.ExecuteNonQuery();
                }
            }
        }

        public Customer GetById(int customerId)
        {
            Customer renterRes = new Customer();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = @"SELECT Id, LastName, FirstName, Phone
                                            FROM tblCustomer
                                            WHERE Id = @rentId";
                    command.Parameters.AddWithValue("@rentId", customerId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var renter = new Customer()
                            {
                                Id = (int)reader["Id"],
                                FirstName = (string)reader["FirstName"],
                                LastName = (string)reader["LastName"],
                                Phone = (string)reader["Phone"]
                            };

                            renterRes = renter;
                            break;
                        }
                    }
                }
            }

            return renterRes;
        }

        public void Remove(int renterId)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = "DELETE FROM tblCustomer WHERE Id = @customerId";

                    command.Parameters.AddWithValue("@customerId", renterId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public IEnumerable<Customer> SelectAll()
        {
            var customers = new List<Customer>();

            // Get data from database.
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = @"SELECT Id, LastName, FirstName, Phone
                                            FROM tblCustomer";

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var customer = new Customer()
                            {
                                Id = (int)reader["Id"],
                                FirstName = (string)reader["FirstName"],
                                LastName = (string)reader["LastName"],
                                Phone = (string)reader["Phone"]
                            };

                            customers.Add(customer);
                        }
                    }
                }
            }

            return customers;
        }
    }
}
