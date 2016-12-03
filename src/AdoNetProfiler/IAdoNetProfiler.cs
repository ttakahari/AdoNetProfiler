using System;
using System.Data.Common;

namespace AdoNetProfiler
{
    /// <summary>
    /// The interface that defines methods profiling of database accesses.
    /// </summary>
    public interface IAdoNetProfiler
    {
        /// <summary>
        /// Get the profiler is enabled or not.
        /// </summary>
        bool IsEnabled { get; }
        
        /// <summary>
        /// Execute before the connection open.
        /// </summary>
        /// <param name="connection">The current connection.</param>
        void OnOpening(DbConnection connection);

        /// <summary>
        /// Execute after the connection open.
        /// </summary>
        /// <param name="connection">The current connection.</param>
        void OnOpened(DbConnection connection);

        /// <summary>
        /// Execute before the connection close.
        /// </summary>
        /// <param name="connection">The current connection.</param>
        void OnClosing(DbConnection connection);

        /// <summary>
        /// Execute after the connection close.
        /// </summary>
        /// <param name="connection">The current connection.</param>
        void OnClosed(DbConnection connection);

        /// <summary>
        /// Execute before the connection creates the transaction.
        /// </summary>
        /// <param name="connection">The current connection.</param>
        void OnStartingTransaction(DbConnection connection);

        /// <summary>
        /// Execute after the connection creates the transaction.
        /// </summary>
        /// <param name="transaction">The created transaction.</param>
        void OnStartedTransaction(DbTransaction transaction);

        /// <summary>
        /// Execute before the transaction commits.
        /// </summary>
        /// <param name="transaction">The current transaction.</param>
        void OnCommitting(DbTransaction transaction);

        /// <summary>
        /// Execute after the transaction commits.
        /// </summary>
        /// <param name="connection">The current connection.</param>
        void OnCommitted(DbConnection connection);

        /// <summary>
        /// Execute before the transaction rollbacks.
        /// </summary>
        /// <param name="transaction">The current transaction.</param>
        void OnRollbacking(DbTransaction transaction);

        /// <summary>
        /// Execute after the transaction rollbacks.
        /// </summary>
        /// <param name="connection">The current connection.</param>
        void OnRollbacked(DbConnection connection);

        /// <summary>
        /// Execute before <see cref="DbCommand.ExecuteReader()"/> executes.
        /// </summary>
        /// <param name="command">The executing command.</param>
        void OnExecuteReaderStart(DbCommand command);

        /// <summary>
        /// Execute after <see cref="DbDataReader"/> reads.
        /// </summary>
        /// <param name="reader">The stream from data source.</param>
        /// <param name="record">The number of read data.</param>
        void OnReaderFinish(DbDataReader reader, int record);

        /// <summary>
        /// Execute before <see cref="DbCommand.ExecuteNonQuery"/> executes.
        /// </summary>
        /// <param name="command">The executing command.</param>
        void OnExecuteNonQueryStart(DbCommand command);

        /// <summary>
        /// Execute after <see cref="DbCommand.ExecuteNonQuery"/> executes.
        /// </summary>
        /// <param name="command">The executing command.</param>
        /// <param name="executionRestlt">The result of executing <see cref="DbCommand.ExecuteNonQuery"/>.</param>
        void OnExecuteNonQueryFinish(DbCommand command, int executionRestlt);

        /// <summary>
        /// Execute before <see cref="DbCommand.ExecuteScalar"/> executes.
        /// </summary>
        /// <param name="command">The executing command.</param>
        void OnExecuteScalarStart(DbCommand command);

        /// <summary>
        /// Execute after <see cref="DbCommand.ExecuteScalar"/> executes.
        /// </summary>
        /// <param name="command">The executing command.</param>
        /// <param name="executionRestlt">The result of executing <see cref="DbCommand.ExecuteScalar"/>.</param>
        void OnExecuteScalarFinish(DbCommand command, object executionRestlt);

        /// <summary>
        /// Execute when the error occurs while the command executing.
        /// </summary>
        /// <param name="command">The executing command.</param>
        /// <param name="exception">The occurred error.</param>
        void OnCommandError(DbCommand command, Exception exception);
    }
}
