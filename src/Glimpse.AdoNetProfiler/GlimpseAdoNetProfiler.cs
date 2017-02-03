using System;
using System.Data.Common;
using AdoNetProfiler;
using Glimpse.AdoNetProfiler.Inspectors;
using Glimpse.AdoNetProfiler.Timelines;
using Glimpse.AdoNetProfiler.Utilities;

namespace Glimpse.AdoNetProfiler
{
    /// <summary>
    /// The profiler that implements <see cref="IAdoNetProfiler"/>, using Glimpse.
    /// </summary>
    public class GlimpseAdoNetProfiler : IAdoNetProfiler
    {
        private readonly Guid _connectionId;

        private ConnectionLifetimeTimeline _connectionLifetimeTimeline;
        private ConnectionEventTimeline _connectionEventTimeline;
        private TransactionLifetimeTimeline _transactionLifetimeTimeline;
        private TransactionEventTimeline _transactionEventTimeline;
        private CommandTimeline _commandTimeline;

        /// <inheritdoc cref="IAdoNetProfiler.IsEnabled" />
        public bool IsEnabled => true;

        /// <summary>
        /// Create a new instance of <see cref="GlimpseAdoNetProfiler"/>.
        /// </summary>
        public GlimpseAdoNetProfiler()
        {
            _connectionId = Guid.NewGuid();
        }

        /// <inheritdoc cref="IAdoNetProfiler.OnOpening(DbConnection)" />
        public void OnOpening(DbConnection connection)
        {
            _connectionLifetimeTimeline = ConnectionInspector.CreateLifetimeTimeline(connection, _connectionId);
            _connectionEventTimeline    = ConnectionInspector.CreateEventTimeline(connection, _connectionId, ConnectionEvent.Open);
        }

        /// <inheritdoc cref="IAdoNetProfiler.OnOpened(DbConnection)" />
        public void OnOpened(DbConnection connection)
        {
            _connectionEventTimeline.WriteTimelineMessage();
            _connectionEventTimeline = null;
        }

        /// <inheritdoc cref="IAdoNetProfiler.OnClosing(DbConnection)" />
        public void OnClosing(DbConnection connection)
        {
            _connectionEventTimeline = ConnectionInspector.CreateEventTimeline(connection, _connectionId, ConnectionEvent.Close);
        }

        /// <inheritdoc cref="IAdoNetProfiler.OnClosed(DbConnection)" />
        public void OnClosed(DbConnection connection)
        {
            _connectionEventTimeline.WriteTimelineMessage();
            _connectionLifetimeTimeline.WriteTimelineMessage();
            _connectionEventTimeline    = null;
            _connectionLifetimeTimeline = null;
        }

        /// <inheritdoc cref="IAdoNetProfiler.OnStartingTransaction(DbConnection)" />
        public void OnStartingTransaction(DbConnection connection)
        {
            _transactionLifetimeTimeline = TransactionInspector.CreateLifetimeTimeline(connection, _connectionId);
            _transactionEventTimeline    = TransactionInspector.CreateEventTimeline(connection, _connectionId, _transactionLifetimeTimeline.TransactionId, TransactionEvent.BeginTransaction);
        }

        /// <inheritdoc cref="IAdoNetProfiler.OnStartedTransaction(DbTransaction)" />
        public void OnStartedTransaction(DbTransaction transaction)
        {
            _transactionEventTimeline.WriteTimelineMessage();
            _transactionEventTimeline = null;
        }

        /// <inheritdoc cref="IAdoNetProfiler.OnCommitting(DbTransaction)" />
        public void OnCommitting(DbTransaction transaction)
        {
            _transactionEventTimeline = TransactionInspector.CreateEventTimeline(transaction.Connection, _connectionId, _transactionLifetimeTimeline.TransactionId, TransactionEvent.Commit);
        }

        /// <inheritdoc cref="IAdoNetProfiler.OnCommitted(DbConnection)" />
        public void OnCommitted(DbConnection connection)
        {
            _transactionEventTimeline.WriteTimelineMessage();
            _transactionLifetimeTimeline.WriteTimelineMessage(true);
            _transactionEventTimeline    = null;
            _transactionLifetimeTimeline = null;
        }

        /// <inheritdoc cref="IAdoNetProfiler.OnRollbacking(DbTransaction)" />
        public void OnRollbacking(DbTransaction transaction)
        {
            _transactionEventTimeline = TransactionInspector.CreateEventTimeline(transaction.Connection, _connectionId, _transactionLifetimeTimeline.TransactionId, TransactionEvent.Rollback);
        }

        /// <inheritdoc cref="IAdoNetProfiler.OnRollbacked(DbConnection)" />
        public void OnRollbacked(DbConnection connection)
        {
            _transactionEventTimeline.WriteTimelineMessage();
            _transactionLifetimeTimeline.WriteTimelineMessage(false);
            _transactionEventTimeline    = null;
            _transactionLifetimeTimeline = null;
        }

        /// <inheritdoc cref="IAdoNetProfiler.OnExecuteReaderStart(DbCommand)" />
        public void OnExecuteReaderStart(DbCommand command)
        {
            _commandTimeline = CommandInspector.CreateTimeline(command, _connectionId, _transactionLifetimeTimeline?.TransactionId);
        }

        /// <inheritdoc cref="IAdoNetProfiler.OnReaderFinish(DbDataReader, int)" />
        public void OnReaderFinish(DbDataReader reader, int records)
        {
            _commandTimeline.WriteTimelineMessage(records);
            _commandTimeline = null;
        }

        /// <inheritdoc cref="IAdoNetProfiler.OnExecuteNonQueryStart(DbCommand)" />
        public void OnExecuteNonQueryStart(DbCommand command)
        {
            _commandTimeline = CommandInspector.CreateTimeline(command, _connectionId, _transactionLifetimeTimeline?.TransactionId);
        }

        /// <inheritdoc cref="IAdoNetProfiler.OnExecuteNonQueryFinish(DbCommand, int)" />
        public void OnExecuteNonQueryFinish(DbCommand command, int executionRestlt)
        {
            _commandTimeline.WriteTimelineMessage(executionRestlt);
            _commandTimeline = null;
        }

        /// <inheritdoc cref="IAdoNetProfiler.OnExecuteScalarStart(DbCommand)" />
        public void OnExecuteScalarStart(DbCommand command)
        {
            _commandTimeline = CommandInspector.CreateTimeline(command, _connectionId, _transactionLifetimeTimeline?.TransactionId);
        }

        /// <inheritdoc cref="IAdoNetProfiler.OnExecuteScalarFinish(DbCommand, object)" />
        public void OnExecuteScalarFinish(DbCommand command, object executionRestlt)
        {
            // Record is always 1.
            _commandTimeline.WriteTimelineMessage(1);
            _commandTimeline = null;
        }

        /// <inheritdoc cref="IAdoNetProfiler.OnCommandError(DbCommand, Exception)" />
        public void OnCommandError(DbCommand command, Exception exception)
        {
            _commandTimeline.WriteTimelineMessage(true);
            _commandTimeline = null;
        }
    }
}