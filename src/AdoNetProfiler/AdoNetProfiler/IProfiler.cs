using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

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
        /// Execute when the connection opens.
        /// </summary>
        /// <param name="connectionOpenAction">The action that means the connection opens.</param>
        void OnConnectionOpen(Action connectionOpenAction);

        /// <summary>
        /// Execute when the connection opens asynchronously.
        /// </summary>
        /// <param name="connectionOpenAsyncAction">The action that means the connection opens asynchronously.</param>
        /// <returns>Task the connection opens.</returns>
        Task OnConnectionOpenAsync(Func<Task> connectionOpenAsyncAction);

        /// <summary>
        /// Execute when the connection closes.
        /// </summary>
        /// <param name="connectionCloseAction">The action that means the connection closes.</param>
        void OnConnectionClose(Action connectionCloseAction);

        /// <summary>
        /// Execute when the transaction begins.
        /// </summary>
        /// <param name="transactionBeginAction">The action that means the connection creates the transaction.</param>
        /// <returns>Created transaction.</returns>
        DbTransaction OnTransactionBegin(Func<DbTransaction> transactionBeginAction);

        /// <summary>
        /// Execute when the transaction commits.
        /// </summary>
        /// <param name="transactionCommitAction">The action that means the transaction commits.</param>
        void OnTransactionCommit(Action transactionCommitAction);

        /// <summary>
        /// Execute when the transaction rollbacks.
        /// </summary>
        /// <param name="transactionRollbackAction">The action that means the transaction rollbacks.</param>
        void OnTransactionRollback(Action transactionRollbackAction);
        
        /// <summary>
        /// Execute brefore the command executes.
        /// </summary>
        /// <param name="command">The executing command.</param>
        void OnCommandStart(IDbCommand command);

        /// <summary>
        /// Execute after the command executes.
        /// </summary>
        /// <param name="command">The executed command.</param>
        /// <param name="reader">The stream from data source.</param>
        void OnCommandFinish(IDbCommand command, DbDataReader reader);

        /// <summary>
        /// Execute when the error occurs while the command executing.
        /// </summary>
        /// <param name="command">The executing command.</param>
        /// <param name="exception">The occurred error.</param>
        void OnCommandError(IDbCommand command, Exception exception);
    }
}