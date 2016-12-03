using System;
using System.Data.Common;
using Glimpse.AdoNetProfiler.TimelineMessages;
using Glimpse.AdoNetProfiler.Timelines.Abstractions;
using Glimpse.AdoNetProfiler.Utilities;
using Glimpse.Core.Extensibility;
using Glimpse.Core.Message;

namespace Glimpse.AdoNetProfiler.Timelines
{
    internal class TransactionEventTimeline : TimelineBase
    {
        private readonly DbConnection _connection;
        private readonly Guid _connectionId;
        private readonly Guid _transactionId;
        private readonly TransactionEvent _transactionEvent;

        protected override string EventName => $"Transaction {_transactionEvent}:{_connection.Database}";

        protected override TimelineCategoryItem CategoryItem => new TimelineCategoryItem("Transaction Event", "#A22C9E", "#8E1F8B");

        internal TransactionEventTimeline(IInspectorContext context, DbConnection connection, Guid connectionId, Guid transactionId, TransactionEvent transactionEvent)
            : base(context)
        {
            _connection       = connection;
            _connectionId     = connectionId;
            _transactionId    = transactionId;
            _transactionEvent = transactionEvent;
        }

        internal void WriteTimelineMessage()
        {
            var timelineMessage = new TransactionEventTimelineMessage(_connection, _connectionId, _transactionId, _transactionEvent);
            WriteTimelineMessageCore(timelineMessage);
        }
    }

    internal class TransactionLifetimeTimeline : TimelineBase
    {
        private readonly DbConnection _connection;
        private readonly Guid _connetionId;

        protected override string EventName => $"Transaction:{_connection.Database}";

        protected override TimelineCategoryItem CategoryItem => new TimelineCategoryItem($"Transaction", "#854BC5", "#DEE81A");

        internal Guid TransactionId { get; }

        internal TransactionLifetimeTimeline(IInspectorContext context, DbConnection connection, Guid connectionId)
            : base(context)
        {
            _connection  = connection;
            _connetionId = connectionId;

            TransactionId = Guid.NewGuid();
        }

        internal void WriteTimelineMessage(bool isComitted)
        {
            var timelineMessage = new TransactionLifetimeTimelineMessage(_connection, _connetionId, TransactionId, isComitted);
            WriteTimelineMessageCore(timelineMessage);
        }
    }
}