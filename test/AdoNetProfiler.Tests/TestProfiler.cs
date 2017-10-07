using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace AdoNetProfiler.Tests
{
    public class TestProfiler : IAdoNetProfiler
    {
        private readonly IList<string> _passedEvents;

        public bool IsEnabled => true;

        public string[] PassedEvents => _passedEvents.ToArray();

        public TestProfiler()
        {
            _passedEvents = new List<string>();
        }

        public void OnOpening(DbConnection connection)
        {
            _passedEvents.Add(nameof(OnOpening));
        }

        public void OnOpened(DbConnection connection)
        {
            _passedEvents.Add(nameof(OnOpened));
        }

        public void OnClosing(DbConnection connection)
        {
            _passedEvents.Add(nameof(OnClosing));
        }

        public void OnClosed(DbConnection connection)
        {
            _passedEvents.Add(nameof(OnClosed));
        }

        public void OnStartingTransaction(DbConnection connection)
        {
            _passedEvents.Add(nameof(OnStartingTransaction));
        }

        public void OnStartedTransaction(DbTransaction transaction)
        {
            _passedEvents.Add(nameof(OnStartedTransaction));
        }

        public void OnCommitting(DbTransaction transaction)
        {
            _passedEvents.Add(nameof(OnCommitting));
        }

        public void OnCommitted(DbConnection connection)
        {
            _passedEvents.Add(nameof(OnCommitted));
        }

        public void OnRollbacking(DbTransaction transaction)
        {
            _passedEvents.Add(nameof(OnRollbacking));
        }

        public void OnRollbacked(DbConnection connection)
        {
            _passedEvents.Add(nameof(OnRollbacked));
        }

        public void OnExecuteNonQueryStart(DbCommand command)
        {
            _passedEvents.Add(nameof(OnExecuteNonQueryStart));
        }

        public void OnExecuteNonQueryFinish(DbCommand command, int executionRestlt)
        {
            _passedEvents.Add(nameof(OnExecuteNonQueryFinish));
        }

        public void OnExecuteReaderStart(DbCommand command)
        {
            _passedEvents.Add(nameof(OnExecuteReaderStart));
        }

        public void OnExecuteScalarStart(DbCommand command)
        {
            _passedEvents.Add(nameof(OnExecuteScalarStart));
        }

        public void OnExecuteScalarFinish(DbCommand command, object executionRestlt)
        {
            _passedEvents.Add(nameof(OnExecuteScalarFinish));
        }

        public void OnReaderFinish(DbDataReader reader, int record)
        {
            _passedEvents.Add(nameof(OnReaderFinish));
        }

        public void OnCommandError(DbCommand command, Exception exception)
        {
            _passedEvents.Add(nameof(OnCommandError)); 
        }
    }
}
