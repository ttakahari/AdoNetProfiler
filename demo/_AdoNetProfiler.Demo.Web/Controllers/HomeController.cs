using System;
using System.Configuration;
using System.Data.Common;
using System.Web.Mvc;

namespace AdoNetProfiler.Demo.Web.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            var setting = ConfigurationManager.ConnectionStrings["Northwind"];
            var factory = DbProviderFactories.GetFactory(setting.ProviderName);

            using (var connection = factory.CreateConnection())
            {
                connection.ConnectionString = setting.ConnectionString;

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

            return View();
        }
    }
}