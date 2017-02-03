using System;
using System.Data.Common;
using Glimpse.AdoNetProfiler.TimelineMessages.Abstractions;
using Glimpse.AdoNetProfiler.Utilities;

namespace Glimpse.AdoNetProfiler.TimelineMessages
{
    /// <summary>
    /// The glimpse timeline message for database connection events.
    /// </summary>
    public class ConnectionEventTimelineMessage : TimelineMessageBase
    {
        /// <summary>
        /// The unique identity of the current connection.
        /// </summary>
        public Guid ConnectionId { get; }

        /// <summary>
        /// The database name.
        /// </summary>
        public string Database { get; }

        /// <summary>
        /// The event of <see cref="DbConnection"/>.
        /// </summary>
        public ConnectionEvent ConnectionEvent { get; }

        internal ConnectionEventTimelineMessage(DbConnection connection, Guid connectionId, ConnectionEvent connectionEvent)
        {
            ConnectionId    = connectionId;
            Database        = connection.Database;
            ConnectionEvent = connectionEvent;
        }
    }

    /// <summary>
    /// The glimpse timeline message for the database connection lifetime.
    /// </summary>
    public class ConnectionLifetimeTimelineMessage : TimelineMessageBase
    {
        /// <summary>
        /// The unique identity of the current connection.
        /// </summary>
        public Guid ConnectionId { get; }

        /// <summary>
        /// The database name.
        /// </summary>
        public string Database { get; }

        internal ConnectionLifetimeTimelineMessage(DbConnection connection, Guid connectionId)
        {
            ConnectionId = connectionId;
            Database     = connection.Database;
        }
    }
}