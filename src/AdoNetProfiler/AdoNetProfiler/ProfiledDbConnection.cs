using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;

namespace AdoNetProfiler
{
    [DesignerCategory("")]
    public class ProfiledDbConnection : DbConnection
    {
        private DbConnection _connection;
        private IProfiler _profiler;
        
        public override string ConnectionString
        {
            get { return _connection.ConnectionString; }
            set { _connection.ConnectionString = value; }
        }

        public override int ConnectionTimeout => _connection.ConnectionTimeout;

        public override string Database => _connection.Database;

        public override string DataSource => _connection.DataSource;

        public override string ServerVersion => _connection.ServerVersion;
        
        public override ConnectionState State => _connection.State;

        protected override bool CanRaiseEvents => true;

        public ProfiledDbConnection(DbConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            _connection = connection;
            _profiler   = AdoNetProfilerFactory.GetProfiler();

            _connection.StateChange += StateChangeHandler;    
        }
        
        public override void ChangeDatabase(string databaseName)
        {
            _connection.ChangeDatabase(databaseName);
        }
        
        public override void Close()
        {
            if (_profiler == null || !_profiler.IsEnabled)
            {
                _connection.Close();
                return;
            }

            _profiler.OnConnectionClose(() => _connection.Close());
        }
        
        public override DataTable GetSchema()
        {
            return _connection.GetSchema();
        }

        public override DataTable GetSchema(string collectionName)
        {
            return _connection.GetSchema(collectionName);
        }

        public override DataTable GetSchema(string collectionName, string[] restrictionValues)
        {
            return _connection.GetSchema(collectionName, restrictionValues);
        }

        public override void Open()
        {
            if (_profiler == null || !_profiler.IsEnabled)
            {
                _connection.Open();
                return;
            }

            _profiler.OnConnectionClose(() => _connection.Open());
        }
        
        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            if (_profiler == null || !_profiler.IsEnabled)
                return _connection.BeginTransaction(isolationLevel);

            var transaction = _profiler.OnTransactionBegin(() => _connection.BeginTransaction(isolationLevel));

            return new ProfiledDbTransaction(transaction, _connection, _profiler);
        }
        
        protected override DbCommand CreateDbCommand()
        {
            return new ProfiledDbCommand(_connection.CreateCommand(), this, _profiler);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _connection != null)
            {
                _connection.StateChange -= StateChangeHandler;
                _connection.Dispose();
            }

            _connection = null;
            _profiler   = null;

            base.Dispose(disposing);
        }

        private void StateChangeHandler(object sender, StateChangeEventArgs stateChangeEventArguments)
        {
            OnStateChange(stateChangeEventArguments);
        }
    }
}