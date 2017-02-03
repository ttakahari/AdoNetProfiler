using System;
#if !COREFX
using System.ComponentModel;
#endif
using System.Data;
using System.Data.Common;

namespace AdoNetProfiler
{
    /// <summary>
    /// The database connection wrapped <see cref="DbConnection"/>.
    /// </summary>
#if !COREFX
    [DesignerCategory("")]
#endif
    public class AdoNetProfilerDbConnection : DbConnection
    {
        /// <inheritdoc cref="DbConnection.ConnectionString" />
        public override string ConnectionString
        {
            get { return WrappedConnection.ConnectionString; }
            set { WrappedConnection.ConnectionString = value; }
        }

        /// <inheritdoc cref="DbConnection.ConnectionTimeout" />
        public override int ConnectionTimeout => WrappedConnection.ConnectionTimeout;

        /// <inheritdoc cref="DbConnection.Database" />
        public override string Database => WrappedConnection.Database;

        /// <inheritdoc cref="DbConnection.DataSource" />
        public override string DataSource => WrappedConnection.DataSource;

        /// <inheritdoc cref="DbConnection.ServerVersion" />
        public override string ServerVersion => WrappedConnection.ServerVersion;

        /// <inheritdoc cref="DbConnection.State" />
        public override ConnectionState State => WrappedConnection.State;

        /// <summary>
        /// The original <see cref="DbConnection"/>.
        /// </summary>
        public DbConnection WrappedConnection { get; private set; }
        
        /// <summary>
        /// The instance of <see cref="IAdoNetProfiler"/> used internally.
        /// </summary>
        public IAdoNetProfiler Profiler { get; private set; }

#if !COREFX        
        /// <summary>
        /// Create a new instance of <see cref="AdoNetProfilerDbConnection" /> with recieving the instance of original <see cref="DbConnection" />.
        /// </summary>
        /// <param name="connection">The instance of original <see cref="DbConnection" />.</param>
        public AdoNetProfilerDbConnection(DbConnection connection)
            : this(connection, AdoNetProfilerFactory.GetProfiler()) { }
#endif

        /// <summary>
        /// Create a new instance of <see cref="AdoNetProfilerDbConnection"/> with recieving the instance of original <see cref="DbConnection" /> and the instance of <see cref="IAdoNetProfiler"/>.
        /// </summary>
        /// <param name="connection">The instance of original <see cref="DbConnection" />.</param>
        /// <param name="profiler">The instance of original <see cref="IAdoNetProfiler" />.</param>
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

        /// <inheritdoc cref="DbConnection.ChangeDatabase(string)" />
        public override void ChangeDatabase(string databaseName)
        {
            WrappedConnection.ChangeDatabase(databaseName);
        }

        /// <inheritdoc cref="DbConnection.Close()" />
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

#if !COREFX
        /// <inheritdoc cref="DbConnection.GetSchema()" />
        public override DataTable GetSchema()
        {
            return WrappedConnection.GetSchema();
        }
        
        /// <inheritdoc cref="DbConnection.GetSchema(string)" />
        public override DataTable GetSchema(string collectionName)
        {
            return WrappedConnection.GetSchema(collectionName);
        }
        
        /// <inheritdoc cref="DbConnection.GetSchema(string, string[])" />
        public override DataTable GetSchema(string collectionName, string[] restrictionValues)
        {
            return WrappedConnection.GetSchema(collectionName, restrictionValues);
        }
#endif

        /// <inheritdoc cref="DbConnection.Open()" />
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

        /// <inheritdoc cref="DbConnection.BeginDbTransaction(IsolationLevel)" />
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

        /// <inheritdoc cref="DbConnection.CreateDbCommand()" />
        protected override DbCommand CreateDbCommand()
        {
            return new AdoNetProfilerDbCommand(WrappedConnection.CreateCommand(), this, Profiler);
        }

        /// <summary>
        /// Free, release, or reset managed or unmanaged resources.
        /// </summary>
        /// <param name="disposing">Wether to free, release, or resetting unmanaged resources or not.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && WrappedConnection != null)
            {
                if (State != ConnectionState.Closed)
                {
                    Close();
                }

                WrappedConnection.StateChange -= StateChangeHandler;
                WrappedConnection.Dispose();
            }

            WrappedConnection = null;
            Profiler          = null;

            // corefx calls Close() in Dispose() without checking ConnectionState.
#if !COREFX
            base.Dispose(disposing);
#endif
        }

        private void StateChangeHandler(object sender, StateChangeEventArgs stateChangeEventArguments)
        {
            OnStateChange(stateChangeEventArguments);
        }
    }
}
