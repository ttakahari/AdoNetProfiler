using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using Glimpse.AdoNetProfiler.TimelineMessages.Abstractions;

namespace Glimpse.AdoNetProfiler.TimelineMessages
{
    public class CommandTimelineMessage : TimelineMessageBase
    {
        public Guid ConnectionId { get; }

        public Guid? TransactionId { get; }

        public string CommandText { get; }

        public DbParameter[] Parameters { get; }

        public CommandType CommandType { get; }

        public bool WithTransaction { get; }

        public string Database { get; }

        public int? Records { get; }

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