using System;
using System.Data.Common;
using Glimpse.AdoNetProfiler.TimelineMessages.Abstractions;
using Glimpse.AdoNetProfiler.Utilities;

namespace Glimpse.AdoNetProfiler.TimelineMessages
{
    public class ConnectionEventTimelineMessage : TimelineMessageBase
    {
        public Guid ConnectionId { get; }

        public string Database { get; }

        public ConnectionEvent ConnectionEvent { get; }

        internal ConnectionEventTimelineMessage(DbConnection connection, Guid connectionId, ConnectionEvent connectionEvent)
        {
            ConnectionId    = connectionId;
            Database        = connection.Database;
            ConnectionEvent = connectionEvent;
        }
    }

    public class ConnectionLifetimeTimelineMessage : TimelineMessageBase
    {
        public Guid ConnectionId { get; }

        public string Database { get; }

        internal ConnectionLifetimeTimelineMessage(DbConnection connection, Guid connectionId)
        {
            ConnectionId = connectionId;
            Database     = connection.Database;
        }
    }
}