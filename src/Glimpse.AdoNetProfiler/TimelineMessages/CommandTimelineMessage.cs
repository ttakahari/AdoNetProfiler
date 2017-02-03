using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using Glimpse.AdoNetProfiler.TimelineMessages.Abstractions;

namespace Glimpse.AdoNetProfiler.TimelineMessages
{
    /// <summary>
    /// The glimpse timeline message for database commands.
    /// </summary>
    public class CommandTimelineMessage : TimelineMessageBase
    {
        /// <summary>
        /// The unique identity of the current connection.
        /// </summary>
        public Guid ConnectionId { get; }

        /// <summary>
        /// The unique identity of the current transaction.
        /// </summary>
        public Guid? TransactionId { get; }

        /// <summary>
        /// The database command.
        /// </summary>
        public string CommandText { get; }

        /// <summary>
        /// The parameters of the database command.
        /// </summary>
        public DbParameter[] Parameters { get; }

        /// <summary>
        /// The type of the database command.
        /// </summary>
        public CommandType CommandType { get; }

        /// <summary>
        /// Wether the database command executed with the database transaction, or net.
        /// </summary>
        public bool WithTransaction { get; }

        /// <summary>
        /// The database name.
        /// </summary>
        public string Database { get; }

        /// <summary>
        /// The count of the result the database command execution.
        /// </summary>
        public int? Records { get; }

        /// <summary>
        /// Wether a error occurs while the database command executing, or net.
        /// </summary>
        public bool IsError { get; }

        internal CommandTimelineMessage(DbCommand command, Guid connectionId, Guid? transactionId, int records)
            : this(command, connectionId, transactionId)
        {
            Records = records;
        }

        internal CommandTimelineMessage(DbCommand command, Guid connectionId, Guid? transactionId, bool isError)
            : this(command, connectionId, transactionId)
        {
            IsError = isError;
        }

        private CommandTimelineMessage(DbCommand command, Guid connectionId, Guid? transactionId)
        {
            ConnectionId    = connectionId;
            TransactionId   = transactionId;
            CommandText     = command.CommandText;
            Parameters      = command.Parameters
                .Cast<DbParameter>()
                .ToArray();
            CommandType     = command.CommandType;
            WithTransaction = command.Transaction != null;
            Database        = command.Connection.Database;
        }
    }
}