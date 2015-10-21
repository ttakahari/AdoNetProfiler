using System;
using System.Data;

namespace AdoNetProfiler
{
    /// <summary>
    /// The interface that defines methods profiling of database accesses.
    /// </summary>
    public interface IProfiler
    {
        /// <summary>
        /// Get the profiler is enabled or not.
        /// </summary>
        bool IsEnabled { get; }
        
        /// <summary>
        /// Execute before the connection open.
        /// </summary>
        void OnOpening();

        /// <summary>
        /// Execute after the connection open.
        /// </summary>
        void OnOpened();

        /// <summary>
        /// Execute before the connection close.
        /// </summary>
        void OnClosing();

        /// <summary>
        /// Execute after the connection close.
        /// </summary>
        void OnClosed();

        /// <summary>
        /// Execute before the connection creates the transaction.
        /// </summary>
        void OnStartingTransaction();

        /// <summary>
        /// Execute after the connection creates the transaction.
        /// </summary>
        void OnStartedTransaction();

        /// <summary>
        /// Execute before the transaction commits.
        /// </summary>
        void OnCommitting();

        /// <summary>
        /// Execute after the transaction commits.
        /// </summary>
        void OnCommitted();

        /// <summary>
        /// Execute before the transaction rollbacks.
        /// </summary>
        void OnRollbacking();

        /// <summary>
        /// Execute after the transaction rollbacks.
        /// </summary>
        void OnRollbacked();
        
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