using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Xunit;

namespace AdoNetProfiler.Tests
{
    public class Tests
    {
        [Fact]
        public void Open_Tests()
        {
            {
                using (var connection = new TestDbConnection(new SqlConnection(@"Data Source=172.0.0.1:11433;Initial Catalog=Northwind;Integrated Security=True"), new TestProfiler()))
                {
                    Assert.Throws<SqlException>(() => connection.Open());

                    Assert.True(connection.State != ConnectionState.Open);
                    Assert.True(connection.TestProfiler.PassedEvents.Contains(nameof(IAdoNetProfiler.OnOpening)));
                    Assert.False(connection.TestProfiler.PassedEvents.Contains(nameof(IAdoNetProfiler.OnOpened)));
                }
            }

            {
                using (var connection = new TestDbConnection(new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=Northwind;Integrated Security=True"), new TestProfiler()))
                {
                    connection.Open();

                    Assert.True(connection.State == ConnectionState.Open);
                    Assert.True(connection.TestProfiler.PassedEvents.Contains(nameof(IAdoNetProfiler.OnOpening)));
                    Assert.True(connection.TestProfiler.PassedEvents.Contains(nameof(IAdoNetProfiler.OnOpened)));
                }
            }
        }

        [Fact]
        public void Close_Tests()
        {
            // ToDo: How can I throw any exceptions when closing?
            //{
            //    using (var connection = new TestDbConnection(new SqlConnection(@""), new TestProfiler()))
            //    {
            //        connection.Close();

            //        Assert.True(connection.State != ConnectionState.Open);
            //        Assert.True(connection.TestProfiler.PassedEvents.Contains(nameof(IAdoNetProfiler.OnClosing)));
            //    }
            //}

            {
                using (var connection = new TestDbConnection(new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=Northwind;Integrated Security=True"), new TestProfiler()))
                {
                    connection.Open();

                    connection.Close();

                    Assert.True(connection.State != ConnectionState.Open);
                    Assert.True(connection.TestProfiler.PassedEvents.Contains(nameof(IAdoNetProfiler.OnClosing)));
                    Assert.True(connection.TestProfiler.PassedEvents.Contains(nameof(IAdoNetProfiler.OnClosed)));
                }
            }
        }

        [Fact]
        public void BeginTransaction_Tests()
        {
            {
                using (var connection = new TestDbConnection(new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=Northwind;Integrated Security=True"), new TestProfiler()))
                {
                    Assert.Throws<InvalidOperationException>(() => connection.BeginTransaction());

                    Assert.True(connection.TestProfiler.PassedEvents.Contains(nameof(IAdoNetProfiler.OnStartingTransaction)));
                    Assert.False(connection.TestProfiler.PassedEvents.Contains(nameof(IAdoNetProfiler.OnStartedTransaction)));
                }
            }

            {
                using (var connection = new TestDbConnection(new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=Northwind;Integrated Security=True"), new TestProfiler()))
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        Assert.NotNull(transaction);
                        Assert.Equal(typeof(SqlTransaction), ((AdoNetProfilerDbTransaction)transaction).WrappedTransaction.GetType());
                        Assert.True(connection.TestProfiler.PassedEvents.Contains(nameof(IAdoNetProfiler.OnStartingTransaction)));
                        Assert.True(connection.TestProfiler.PassedEvents.Contains(nameof(IAdoNetProfiler.OnStartedTransaction)));
                    }
                }
            }
        }

        [Fact]
        public void Commit_Tests()
        {
            {
                using (var connection = new TestDbConnection(new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=Northwind;Integrated Security=True"), new TestProfiler()))
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        transaction.Rollback();

                        Assert.Throws<NullReferenceException>(() => transaction.Commit());

                        Assert.True(connection.TestProfiler.PassedEvents.Contains(nameof(IAdoNetProfiler.OnCommitting)));
                        Assert.False(connection.TestProfiler.PassedEvents.Contains(nameof(IAdoNetProfiler.OnCommitted)));
                    }
                }
            }

            {
                using (var connection = new TestDbConnection(new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=Northwind;Integrated Security=True"), new TestProfiler()))
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        transaction.Commit();

                        Assert.Null(((AdoNetProfilerDbTransaction)transaction).WrappedTransaction);
                        Assert.True(connection.TestProfiler.PassedEvents.Contains(nameof(IAdoNetProfiler.OnCommitting)));
                        Assert.True(connection.TestProfiler.PassedEvents.Contains(nameof(IAdoNetProfiler.OnCommitted)));
                    }
                }
            }
        }

        [Fact]
        public void Rollback_Tests()
        {
            {
                using (var connection = new TestDbConnection(new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=Northwind;Integrated Security=True"), new TestProfiler()))
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        transaction.Commit();

                        Assert.Throws<NullReferenceException>(() => transaction.Rollback());

                        Assert.True(connection.TestProfiler.PassedEvents.Contains(nameof(IAdoNetProfiler.OnRollbacking)));
                        Assert.False(connection.TestProfiler.PassedEvents.Contains(nameof(IAdoNetProfiler.OnRollbacked)));
                    }
                }
            }

            {
                using (var connection = new TestDbConnection(new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=Northwind;Integrated Security=True"), new TestProfiler()))
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        transaction.Rollback();

                        Assert.Null(((AdoNetProfilerDbTransaction)transaction).WrappedTransaction);
                        Assert.True(connection.TestProfiler.PassedEvents.Contains(nameof(IAdoNetProfiler.OnRollbacking)));
                        Assert.True(connection.TestProfiler.PassedEvents.Contains(nameof(IAdoNetProfiler.OnRollbacked)));
                    }
                }
            }
        }

        [Fact]
        public void ExecuteNonQuery_Tests()
        {
            {
                using (var connection = new TestDbConnection(new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=Northwind;Integrated Security=True"), new TestProfiler()))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"Dummy Query";
                        command.CommandType = CommandType.Text;

                        int? result = null;

                        Assert.Throws<SqlException>(() => result = command.ExecuteNonQuery());
                        Assert.Null(result);
                        Assert.True(connection.TestProfiler.PassedEvents.Contains(nameof(IAdoNetProfiler.OnExecuteNonQueryStart)));
                        Assert.True(connection.TestProfiler.PassedEvents.Contains(nameof(IAdoNetProfiler.OnExecuteNonQueryFinish)));
                        Assert.True(connection.TestProfiler.PassedEvents.Contains(nameof(IAdoNetProfiler.OnCommandError)));
                    }
                }

                using (var connection = new TestDbConnection(new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=Northwind;Integrated Security=True"), new TestProfiler()))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"UPDATE Orders SET EmployeeID = @employeeId WHERE OrderId = @orderId";
                        command.CommandType = CommandType.Text;

                        command.Parameters.Add(new SqlParameter("employeeId", 5));
                        command.Parameters.Add(new SqlParameter("orderId", 10248));

                        var result = command.ExecuteNonQuery();

                        Assert.Equal(1, result);
                        Assert.True(connection.TestProfiler.PassedEvents.Contains(nameof(IAdoNetProfiler.OnExecuteNonQueryStart)));
                        Assert.True(connection.TestProfiler.PassedEvents.Contains(nameof(IAdoNetProfiler.OnExecuteNonQueryFinish)));
                        Assert.False(connection.TestProfiler.PassedEvents.Contains(nameof(IAdoNetProfiler.OnCommandError)));
                    }
                }
            }
        }

        [Fact]
        public void ExecuteScalar_Tests()
        {
            {
                using (var connection = new TestDbConnection(new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=Northwind;Integrated Security=True"), new TestProfiler()))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"Dummy Query";
                        command.CommandType = CommandType.Text;

                        object result = null;

                        Assert.Throws<SqlException>(() => result = command.ExecuteScalar());
                        Assert.Null(result);
                        Assert.True(connection.TestProfiler.PassedEvents.Contains(nameof(IAdoNetProfiler.OnExecuteScalarStart)));
                        Assert.True(connection.TestProfiler.PassedEvents.Contains(nameof(IAdoNetProfiler.OnExecuteScalarFinish)));
                        Assert.True(connection.TestProfiler.PassedEvents.Contains(nameof(IAdoNetProfiler.OnCommandError)));
                    }
                }

                using (var connection = new TestDbConnection(new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=Northwind;Integrated Security=True"), new TestProfiler()))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"SELECT COUNT(1) FROM Orders";
                        command.CommandType = CommandType.Text;
                        
                        var result = command.ExecuteScalar();

                        Assert.Equal(830, result);
                        Assert.True(connection.TestProfiler.PassedEvents.Contains(nameof(IAdoNetProfiler.OnExecuteScalarStart)));
                        Assert.True(connection.TestProfiler.PassedEvents.Contains(nameof(IAdoNetProfiler.OnExecuteScalarFinish)));
                        Assert.False(connection.TestProfiler.PassedEvents.Contains(nameof(IAdoNetProfiler.OnCommandError)));
                    }
                }
            }
        }

        [Fact]
        public void ExecuteReader_Tests()
        {
            {
                using (var connection = new TestDbConnection(new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=Northwind;Integrated Security=True"), new TestProfiler()))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"Dummy Query";
                        command.CommandType = CommandType.Text;

                        var reader = default(DbDataReader);

                        Assert.Throws<SqlException>(() => reader = command.ExecuteReader());
                        Assert.Null(reader);
                        Assert.True(connection.TestProfiler.PassedEvents.Contains(nameof(IAdoNetProfiler.OnExecuteReaderStart)));
                        Assert.False(connection.TestProfiler.PassedEvents.Contains(nameof(IAdoNetProfiler.OnReaderFinish)));
                        Assert.True(connection.TestProfiler.PassedEvents.Contains(nameof(IAdoNetProfiler.OnCommandError)));
                    }
                }

                using (var connection = new TestDbConnection(new SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=Northwind;Integrated Security=True"), new TestProfiler()))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"SELECT * FROM Orders";
                        command.CommandType = CommandType.Text;

                        var count = 0;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                count++;
                            }
                        }

                        Assert.Equal(830, count);
                        Assert.True(connection.TestProfiler.PassedEvents.Contains(nameof(IAdoNetProfiler.OnExecuteReaderStart)));
                        Assert.True(connection.TestProfiler.PassedEvents.Contains(nameof(IAdoNetProfiler.OnReaderFinish)));
                        Assert.False(connection.TestProfiler.PassedEvents.Contains(nameof(IAdoNetProfiler.OnCommandError)));
                    }
                }
            }
        }
    }
}
