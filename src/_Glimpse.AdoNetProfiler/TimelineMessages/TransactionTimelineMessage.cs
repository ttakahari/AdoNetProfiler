using System;
using System.Data.Common;
using Glimpse.AdoNetProfiler.TimelineMessages.Abstractions;
using Glimpse.AdoNetProfiler.Utilities;

namespace Glimpse.AdoNetProfiler.TimelineMessages
{
    public class TransactionEventTimelineMessage : TimelineMessageBase
    {
        public Guid TransactionId { get; }

        public Guid ConnectionId { get; }

        public string Database { get; }

        public TransactionEvent TransactionEvent { get; }

        internal TransactionEventTimelineMessage(DbConnection connection, Guid connectionId, Guid transactionId, TransactionEvent transactionEvent)
        {
            TransactionId    = transactionId;
            ConnectionId     = connectionId;
            Database         = connection.Database;
            TransactionEvent = transactionEvent;
        }
    }

    public class TransactionLifetimeTimelineMessage : TimelineMessageBase
    {
        public Guid TransactionId { get; }

        public Guid ConnectionId { get; }

        public string Database { get; }

        public bool IsCommited { get; }

        internal TransactionLifetimeTimelineMessage(DbConnection connection, Guid connectionId, Guid transactionId, bool isCommited)
        {
            TransactionId = transactionId;
            ConnectionId  = connectionId;
            Database      = connection.Database;
            IsCommited    = isCommited;
        }
    }
}