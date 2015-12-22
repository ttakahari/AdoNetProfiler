using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;

namespace AdoNetProfiler
{
    [DesignerCategory("")]
    public class AdoNetProfilerDbConnection : DbConnection
    {
        public override string ConnectionString
        {
            get { return WrappedConnection.ConnectionString; }
            set { WrappedConnection.ConnectionString = value; }
        }

        public override int ConnectionTimeout => WrappedConnection.ConnectionTimeout;

        public override string Database => WrappedConnection.Database;

        public override string DataSource => WrappedConnection.DataSource;

        public override string ServerVersion => WrappedConnection.ServerVersion;
        
        public override ConnectionState State => WrappedConnection.State;

        protected override bool CanRaiseEvents => true;

        public DbConnection WrappedConnection { get; private set; }

        public IAdoNetProfiler Profiler { get; private set; }

        public AdoNetProfilerDbConnection(DbConnection connection)
            : this(connection, AdoNetProfilerFactory.GetProfiler()) { }

        public AdoNetProfilerDbConnection(DbConnection connection, IAdoNetProfiler profiler)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            WrappedConnection = connection;
            Profiler          = profiler;

            WrappedConnection.StateChange += StateChangeHandler;
        }
        
        public override void ChangeDatabase(string databaseName)
        {
            WrappedConnection.ChangeDatabase(databaseName);
        }
        
        public override void Close()
        {
            if (Profiler == null || !Profiler.IsEnabled)
            {
                WrappedConnection.Close();
                return;
            }

            Profiler.OnClosing(this);

            WrappedConnection.Close();

            Profiler.OnClosed(this);
        }
        
        public override DataTable GetSchema()
        {
            return WrappedConnection.GetSchema();
        }

        public override DataTable GetSchema(string collectionName)
        {
            return WrappedConnection.GetSchema(collectionName);
        }

        public override DataTable GetSchema(string collectionName, string[] restrictionValues)
        {
            return WrappedConnection.GetSchema(collectionName, restrictionValues);
        }

        public override void Open()
        {
            if (Profiler == null || !Profiler.IsEnabled)
            {
                WrappedConnection.Open();
                return;
            }

            Profiler.OnOpening(this);


            WrappedConnection.Open();

            Profiler.OnOpened(this);
        }
        
        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            if (Profiler == null || !Profiler.IsEnabled)
            {
                return WrappedConnection.BeginTransaction(isolationLevel);
            }

            Profiler.OnStartingTransaction(this);

            var transaction = WrappedConnection.BeginTransaction(isolationLevel);

            Profiler.OnStartedTransaction(transaction);

            return new AdoNetProfilerDbTransaction(transaction, WrappedConnection, Profiler);
        }
        
        protected override DbCommand CreateDbCommand()
        {
            return new AdoNetProfilerDbCommand(WrappedConnection.CreateCommand(), this, Profiler);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && WrappedConnection != null)
            {
                Close();

                WrappedConnection.StateChange -= StateChangeHandler;
                WrappedConnection.Dispose();
            }

            WrappedConnection = null;
            Profiler          = null;

            base.Dispose(disposing);
        }

        private void StateChangeHandler(object sender, StateChangeEventArgs stateChangeEventArguments)
        {
            OnStateChange(stateChangeEventArguments);
        }
    }
}