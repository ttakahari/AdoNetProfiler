using System;
using System.Data.SqlClient;

namespace AdoNetProfiler.Demo.ConsoleApp.Net46
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var connection = new AdoNetProfilerDbConnection(new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=Northwind;Integrated Security=True"), new ConsoleProfiler()))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"SELECT * FROM Orders";
                        command.Transaction = transaction;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                            }
                        }
                    }

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"SELECT COUNT(1) FROM Orders";
                        command.Transaction = transaction;

                        command.ExecuteScalar();
                    }

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"UPDATE Orders SET OrderDate = @Now WHERE OrderID = @OrderId";

                        var parameter1 = command.CreateParameter();
                        parameter1.ParameterName = "@orderId";
                        parameter1.Value = 10245;

                        var parameter2 = command.CreateParameter();
                        parameter2.ParameterName = "@now";
                        parameter2.Value = DateTime.Now;

                        command.Parameters.AddRange(new[] { parameter1, parameter2 });
                        command.Transaction = transaction;

                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }

            Console.ReadLine();
        }
    }
}
