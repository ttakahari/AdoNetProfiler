using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;

namespace AdoNetProfiler.Demo.Console
{
    public class TraceProfiler : IAdoNetProfiler
    {
        private Stopwatch _stopwatch;

        public bool IsEnabled => true;

        private Stopwatch _connectionStopwatch;

        public void OnOpening(DbConnection connection)
        {
            _stopwatch = Stopwatch.StartNew();
            _connectionStopwatch = Stopwatch.StartNew();
        }

        public void OnOpened(DbConnection connection)
        {
            _stopwatch.Stop();
            Trace.WriteLine($"Connection Open Duration - {_stopwatch.Elapsed.TotalMilliseconds} ms");
        }

        public void OnClosing(DbConnection connection)
        {
            _stopwatch = Stopwatch.StartNew();
        }

        public void OnClosed(DbConnection connection)
        {
            _stopwatch.Stop();
            _connectionStopwatch.Stop();
            Trace.WriteLine($"Connection Close Duration - {_stopwatch.Elapsed.TotalMilliseconds} ms");
            Trace.WriteLine($"Connection Lifetime - {_connectionStopwatch.Elapsed.TotalMilliseconds} ms");
        }

        private Stopwatch _transactionStopwatch;

        public void OnStartingTransaction(DbConnection connection)
        {
            _stopwatch = Stopwatch.StartNew();
            _transactionStopwatch = Stopwatch.StartNew();
        }

        public void OnStartedTransaction(DbTransaction transaction)
        {
            _stopwatch.Stop();
            Trace.WriteLine($"Transaction Start Duration - {_stopwatch.Elapsed.TotalMilliseconds} ms");
        }

        public void OnCommitting(DbTransaction transaction)
        {
            _stopwatch = Stopwatch.StartNew();
        }

        public void OnCommitted(DbConnection connection)
        {
            _stopwatch.Stop();
            _transactionStopwatch.Stop();
            Trace.WriteLine($"Transaction Commit Duration - {_stopwatch.Elapsed.TotalMilliseconds} ms");
            Trace.WriteLine($"Transaction Lifetime - {_transactionStopwatch.Elapsed.TotalMilliseconds} ms");
        }

        public void OnRollbacking(DbTransaction transaction)
        {
            _stopwatch = Stopwatch.StartNew();
        }

        public void OnRollbacked(DbConnection connection)
        {
            _stopwatch.Stop();
            _transactionStopwatch.Stop();
            Trace.WriteLine($"Transaction Rollback Duration - {_stopwatch.Elapsed.TotalMilliseconds} ms");
            Trace.WriteLine($"Transaction Lifetime - {_transactionStopwatch.Elapsed.TotalMilliseconds} ms");
        }

        private IDbCommand _command;

        public void OnExecuteReaderStart(DbCommand command)
        {
            _command = command;
            _stopwatch = Stopwatch.StartNew();
        }

        public void OnReaderFinish(DbDataReader reader, int record)
        {
            _stopwatch.Stop();
            Trace.WriteLine($"Command Info - Command : {_command.CommandText}, Record : {record}, Duration {_stopwatch.Elapsed.TotalMilliseconds} ms");
        }

        public void OnExecuteNonQueryStart(DbCommand command)
        {
            _command = command;
            _stopwatch = Stopwatch.StartNew();
        }

        public void OnExecuteNonQueryFinish(DbCommand command, int executionRestlt)
        {
            _stopwatch.Stop();
            Trace.WriteLine($"Command Info - Command : {_command.CommandText}, Result : {executionRestlt}, Duration {_stopwatch.Elapsed.TotalMilliseconds} ms");
        }

        public void OnExecuteScalarStart(DbCommand command)
        {
            _command = command;
            _stopwatch = Stopwatch.StartNew();
        }

        public void OnExecuteScalarFinish(DbCommand command, object executionRestlt)
        {
            _stopwatch.Stop();
            Trace.WriteLine($"Command Info - Command : {_command.CommandText}, Result : {executionRestlt}, Duration {_stopwatch.Elapsed.TotalMilliseconds} ms");
        }
        
        public void OnCommandError(DbCommand command, Exception exception)
        {
        }
    }
}