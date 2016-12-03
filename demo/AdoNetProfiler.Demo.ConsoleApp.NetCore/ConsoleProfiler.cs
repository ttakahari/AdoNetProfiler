using System;
using System.Data.Common;
using System.Diagnostics;

namespace AdoNetProfiler.Demo.ConsoleApp.NetCore
{
    public class ConsoleProfiler : IAdoNetProfiler
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
            Console.WriteLine($"Connection Open Duration - {_stopwatch.Elapsed.TotalMilliseconds} ms");
        }

        public void OnClosing(DbConnection connection)
        {
            _stopwatch = Stopwatch.StartNew();
        }

        public void OnClosed(DbConnection connection)
        {
            _stopwatch.Stop();
            _connectionStopwatch.Stop();
            Console.WriteLine($"Connection Close Duration - {_stopwatch.Elapsed.TotalMilliseconds} ms");
            Console.WriteLine($"Connection Lifetime - {_connectionStopwatch.Elapsed.TotalMilliseconds} ms");
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
            Console.WriteLine($"Transaction Start Duration - {_stopwatch.Elapsed.TotalMilliseconds} ms");
        }

        public void OnCommitting(DbTransaction transaction)
        {
            _stopwatch = Stopwatch.StartNew();
        }

        public void OnCommitted(DbConnection connection)
        {
            _stopwatch.Stop();
            _transactionStopwatch.Stop();
            Console.WriteLine($"Transaction Commit Duration - {_stopwatch.Elapsed.TotalMilliseconds} ms");
            Console.WriteLine($"Transaction Lifetime - {_transactionStopwatch.Elapsed.TotalMilliseconds} ms");
        }

        public void OnRollbacking(DbTransaction transaction)
        {
            _stopwatch = Stopwatch.StartNew();
        }

        public void OnRollbacked(DbConnection connection)
        {
            _stopwatch.Stop();
            _transactionStopwatch.Stop();
            Console.WriteLine($"Transaction Rollback Duration - {_stopwatch.Elapsed.TotalMilliseconds} ms");
            Console.WriteLine($"Transaction Lifetime - {_transactionStopwatch.Elapsed.TotalMilliseconds} ms");
        }

        private DbCommand _command;

        public void OnExecuteReaderStart(DbCommand command)
        {
            _command = command;
            _stopwatch = Stopwatch.StartNew();
        }

        public void OnReaderFinish(DbDataReader reader, int records)
        {
            _stopwatch.Stop();
            Console.WriteLine($"Command Info - Command : {_command.CommandText}, Records : {records}, Duration {_stopwatch.Elapsed.TotalMilliseconds} ms");
        }

        public void OnExecuteNonQueryStart(DbCommand command)
        {
            _command = command;
            _stopwatch = Stopwatch.StartNew();
        }

        public void OnExecuteNonQueryFinish(DbCommand command, int executionRestlt)
        {
            _stopwatch.Stop();
            Console.WriteLine($"Command Info - Command : {_command.CommandText}, Result : {executionRestlt}, Duration {_stopwatch.Elapsed.TotalMilliseconds} ms");
        }

        public void OnExecuteScalarStart(DbCommand command)
        {
            _command = command;
            _stopwatch = Stopwatch.StartNew();
        }

        public void OnExecuteScalarFinish(DbCommand command, object executionRestlt)
        {
            _stopwatch.Stop();
            Console.WriteLine($"Command Info - Command : {_command.CommandText}, Result : {executionRestlt}, Duration {_stopwatch.Elapsed.TotalMilliseconds} ms");
        }

        public void OnCommandError(DbCommand command, Exception exception)
        {
        }
    }
}
