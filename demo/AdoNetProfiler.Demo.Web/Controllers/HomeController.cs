using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdoNetProfiler.Demo.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var connectionSetting = ConfigurationManager.ConnectionStrings["Northwind"];
            var factory           = DbProviderFactories.GetFactory(connectionSetting.ProviderName);

            using (var connection = factory.CreateConnection())
            {
                connection.ConnectionString = connectionSetting.ConnectionString;

                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"SELECT * FROM Orders";
                    command.CommandType = CommandType.Text;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                        }
                    }
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"SELECT * FROM Orders";
                    command.CommandText = @"SELECT COUNT(1) FROM OrderDetails";

                    var result = command.ExecuteScalar();
                }

                using (var transaction = connection.BeginTransaction())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"UPDATE Orders SET OrderDate = @orderDate WHERE OrderID = @orderId";
                        command.CommandType = CommandType.Text;
                        command.Transaction = transaction;

                        var p1 = command.CreateParameter();

                        p1.ParameterName = "@orderDate";
                        p1.Value         = DateTime.Now;
                        p1.DbType        = DbType.DateTime;

                        var p2 = command.CreateParameter();

                        p2.ParameterName = "@orderId";
                        p2.Value         = 10245;
                        p2.DbType        = DbType.Int32;

                        command.Parameters.Add(p1);
                        command.Parameters.Add(p2);

                        var result = command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}