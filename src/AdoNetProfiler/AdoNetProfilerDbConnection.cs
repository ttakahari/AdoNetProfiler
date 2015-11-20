using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;

namespace AdoNetProfiler
{
    [DesignerCategory("")]
    public class AdoNetProfilerDbConnection : DbConnection
    {
        private IAdoNetProfiler _profiler;
        
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
                throw new ArgumentNullException(nameof(connection));

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
            if (_profiler == null || !_profiler.IsEnabled)
            {
                WrappedConnection.Close();
                return;
            }
            
            _profiler.OnClosing(this);

            WrappedConnection.Close();

            _profiler.OnClosed(this);
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
            if (_profiler == null || !_profiler.IsEnabled)
            {
                WrappedConnection.Open();
                return;
            }
            
            _profiler.OnOpening(this);

            WrappedConnection.Open();

            _profiler.OnOpened(this);
        }
        
        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            if (_profiler == null || !_profiler.IsEnabled)
                return WrappedConnection.BeginTransaction(isolationLevel);

            _profiler.OnStartingTransaction(this);

            var transaction = WrappedConnection.BeginTransaction(isolationLevel);

            _profiler.OnStartedTransaction(transaction);

            return new AdoNetProfilerDbTransaction(transaction, WrappedConnection, _profiler);
        }
        
        protected override DbCommand CreateDbCommand()
        {
            return new AdoNetProfilerDbCommand(WrappedConnection.CreateCommand(), this, _profiler);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && WrappedConnection != null)
            {
                WrappedConnection.StateChange -= StateChangeHandler;
                WrappedConnection.Dispose();
            }

            WrappedConnection = null;
            _profiler   = null;

            base.Dispose(disposing);
        }

        private void StateChangeHandler(object sender, StateChangeEventArgs stateChangeEventArguments)
        {
            OnStateChange(stateChangeEventArguments);
        }
    }
}