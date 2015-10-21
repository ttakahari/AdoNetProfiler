using System;
using System.Data;
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
        /// Execute brefore the command executes.
        /// </summary>
        /// <param name="command">The executing command.</param>
        void OnCommandStart(IDbCommand command);

        /// <summary>
        /// Execute after the command executes.
        /// </summary>
        /// <param name="command">The executed command.</param>
        /// <param name="isReader">Called by ExecuteReader or not.</param>
        void OnCommandFinish(IDbCommand command, bool isReader);

        /// <summary>
        /// Execute after the stream finishs be read.
        /// </summary>
        /// <param name="reader">The stream from data source.</param>
        void OnReaderFinish(IDataReader reader);

        /// <summary>
        /// Execute when the error occurs while the command executing.
        /// </summary>
        /// <param name="command">The executing command.</param>
        /// <param name="exception">The occurred error.</param>
        void OnCommandError(IDbCommand command, Exception exception);
    }
}