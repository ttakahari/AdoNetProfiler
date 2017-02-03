using System;
using System.Data.Common;
using Glimpse.AdoNetProfiler.TimelineMessages;
using Glimpse.AdoNetProfiler.Timelines.Abstractions;
using Glimpse.AdoNetProfiler.Utilities;
using Glimpse.Core.Extensibility;
using Glimpse.Core.Message;

namespace Glimpse.AdoNetProfiler.Timelines
{
    internal class ConnectionEventTimeline : TimelineBase
    {
        private readonly DbConnection _connection;
        private readonly Guid _connectionId;
        private readonly ConnectionEvent _connectionEvent;

        protected override string EventName => $"Connection {_connectionEvent}:{_connection.Database}";

        protected override TimelineCategoryItem CategoryItem => new TimelineCategoryItem("Connection Event", "#681C65", "#5B1459");

        internal ConnectionEventTimeline(IInspectorContext context, DbConnection connection, Guid connectionId, ConnectionEvent connectionEvent)
            : base(context)
        {
            _connection      = connection;
            _connectionId    = connectionId;
            _connectionEvent = connectionEvent;
        }

        internal void WriteTimelineMessage()
        {
            var timelineMessage = new ConnectionEventTimelineMessage(_connection, _connectionId, _connectionEvent);

            WriteTimelineMessageCore(timelineMessage);
        }
    }

    internal class ConnectionLifetimeTimeline : TimelineBase
    {
        private readonly DbConnection _connection;

        protected override string EventName => $"Connection:{_connection.Database}";

        protected override TimelineCategoryItem CategoryItem => new TimelineCategoryItem("Connection", "#55307E", "#DEE81A");

        internal Guid ConnectionId { get; }

        internal ConnectionLifetimeTimeline(IInspectorContext context, DbConnection connection, Guid connectionId)
            : base(context)
        {
            _connection = connection;

            ConnectionId = connectionId;
        }

        internal void WriteTimelineMessage()
        {
            var timelineMessage = new ConnectionLifetimeTimelineMessage(_connection, ConnectionId);

            WriteTimelineMessageCore(timelineMessage);
        }
    }
}