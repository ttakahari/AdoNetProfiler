using System;
using System.Data.Common;
using Glimpse.AdoNetProfiler.TimelineMessages.Abstractions;
using Glimpse.AdoNetProfiler.Utilities;

namespace Glimpse.AdoNetProfiler.TimelineMessages
{
    /// <summary>
    /// The glimpse timeline message for database transaction events.
    /// </summary>
    public class TransactionEventTimelineMessage : TimelineMessageBase
    {
        /// <summary>
        /// The unique identity of the current transaction.
        /// </summary>
        public Guid TransactionId { get; }

        /// <summary>
        /// The unique identity of the current connection.
        /// </summary>
        public Guid ConnectionId { get; }

        /// <summary>
        /// The database name.
        /// </summary>
        public string Database { get; }

        /// <summary>
        /// The event of <see cref="DbTransaction"/>.
        /// </summary>
        public TransactionEvent TransactionEvent { get; }

        internal TransactionEventTimelineMessage(DbConnection connection, Guid connectionId, Guid transactionId, TransactionEvent transactionEvent)
        {
            TransactionId    = transactionId;
            ConnectionId     = connectionId;
            Database         = connection.Database;
            TransactionEvent = transactionEvent;
        }
    }

    /// <summary>
    /// The glimpse timeline message for the database transaction lifetime.
    /// </summary>
    public class TransactionLifetimeTimelineMessage : TimelineMessageBase
    {
        /// <summary>
        /// The unique identity of the current transaction.
        /// </summary>
        public Guid TransactionId { get; }

        /// <summary>
        /// The unique identity of the current connection.
        /// </summary>
        public Guid ConnectionId { get; }

        /// <summary>
        /// The database name.
        /// </summary>
        public string Database { get; }

        /// <summary>
        /// Wether the database transaction was commited, or not.
        /// </summary>
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