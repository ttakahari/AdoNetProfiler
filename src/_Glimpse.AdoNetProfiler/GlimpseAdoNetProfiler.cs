using System;
using System.Data.Common;
using AdoNetProfiler;
using Glimpse.AdoNetProfiler.Inspectors;
using Glimpse.AdoNetProfiler.Timelines;
using Glimpse.AdoNetProfiler.Utilities;

namespace Glimpse.AdoNetProfiler
{
    public class GlimpseAdoNetProfiler : IAdoNetProfiler
    {
        private readonly Guid _connectionId;

        private ConnectionLifetimeTimeline _connectionLifetimeTimeline;
        private ConnectionEventTimeline _connectionEventTimeline;
        private TransactionLifetimeTimeline _transactionLifetimeTimeline;
        private TransactionEventTimeline _transactionEventTimeline;
        private CommandTimeline _commandTimeline;

        public bool IsEnabled => true;

        public GlimpseAdoNetProfiler()
        {
            _connectionId = Guid.NewGuid();
        }

        public void OnOpening(DbConnection connection)
        {
            _connectionLifetimeTimeline = ConnectionInspector.CreateLifetimeTimeline(connection, _connectionId);
            _connectionEventTimeline    = ConnectionInspector.CreateEventTimeline(connection, _connectionId, ConnectionEvent.Open);
        }

        public void OnOpened(DbConnection connection)
        {
            _connectionEventTimeline.WriteTimelineMessage();
            _connectionEventTimeline = null;
        }

        public void OnClosing(DbConnection connection)
        {
            _connectionEventTimeline = ConnectionInspector.CreateEventTimeline(connection, _connectionId, ConnectionEvent.Close);
        }

        public void OnClosed(DbConnection connection)
        {
            _connectionEventTimeline.WriteTimelineMessage();
            _connectionLifetimeTimeline.WriteTimelineMessage();
            _connectionEventTimeline    = null;
            _connectionLifetimeTimeline = null;
        }

        public void OnStartingTransaction(DbConnection connection)
        {
            _transactionLifetimeTimeline = TransactionInspector.CreateLifetimeTimeline(connection, _connectionId);
            _transactionEventTimeline    = TransactionInspector.CreateEventTimeline(connection, _connectionId, _transactionLifetimeTimeline.TransactionId, TransactionEvent.BeginTransaction);
        }

        public void OnStartedTransaction(DbTransaction transaction)
        {
            _transactionEventTimeline.WriteTimelineMessage();
            _transactionEventTimeline = null;
        }

        public void OnCommitting(DbTransaction transaction)
        {
            _transactionEventTimeline = TransactionInspector.CreateEventTimeline(transaction.Connection, _connectionId, _transactionLifetimeTimeline.TransactionId, TransactionEvent.Commit);
        }

        public void OnCommitted(DbConnection connection)
        {
            _transactionEventTimeline.WriteTimelineMessage();
            _transactionLifetimeTimeline.WriteTimelineMessage(true);
            _transactionEventTimeline    = null;
            _transactionLifetimeTimeline = null;
        }

        public void OnRollbacking(DbTransaction transaction)
        {
            _transactionEventTimeline = TransactionInspector.CreateEventTimeline(transaction.Connection, _connectionId, _transactionLifetimeTimeline.TransactionId, TransactionEvent.Rollback);
        }

        public void OnRollbacked(DbConnection connection)
        {
            _transactionEventTimeline.WriteTimelineMessage();
            _transactionLifetimeTimeline.WriteTimelineMessage(false);
            _transactionEventTimeline    = null;
            _transactionLifetimeTimeline = null;
        }

        public void OnExecuteReaderStart(DbCommand command)
        {
            _commandTimeline = CommandInspector.CreateTimeline(command, _connectionId, _transactionLifetimeTimeline?.TransactionId);
        }

        public void OnReaderFinish(DbDataReader reader, int records)
        {
            _commandTimeline.WriteTimelineMessage(records);
            _commandTimeline = null;
        }

        public void OnExecuteNonQueryStart(DbCommand command)
        {
            _commandTimeline = CommandInspector.CreateTimeline(command, _connectionId, _transactionLifetimeTimeline?.TransactionId);
        }

        public void OnExecuteNonQueryFinish(DbCommand command, int executionRestlt)
        {
            _commandTimeline.WriteTimelineMessage(executionRestlt);
            _commandTimeline = null;
        }

        public void OnExecuteScalarStart(DbCommand command)
        {
            _commandTimeline = CommandInspector.CreateTimeline(command, _connectionId, _transactionLifetimeTimeline?.TransactionId);
        }

        public void OnExecuteScalarFinish(DbCommand command, object executionRestlt)
        {
            // Record is always 1.
            _commandTimeline.WriteTimelineMessage(1);
            _commandTimeline = null;
        }

        public void OnCommandError(DbCommand command, Exception exception)
        {
            _commandTimeline.WriteTimelineMessage(true);
            _commandTimeline = null;
        }
    }
}