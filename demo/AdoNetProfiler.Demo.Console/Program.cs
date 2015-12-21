using System;
using System.Configuration;
using System.Data.Common;

namespace AdoNetProfiler.Demo.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            AdoNetProfilerFactory.Initialize(typeof(TraceProfiler));

            var settings = ConfigurationManager.ConnectionStrings["Northwind"];
            var factory = DbProviderFactories.GetFactory(settings.ProviderName);

            using (var connection = factory.CreateConnection())
            {
                connection.ConnectionString = settings.ConnectionString;

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
        }
    }
}
