using System;
using System.Data.Common;
using Glimpse.AdoNetProfiler.TimelineMessages;
using Glimpse.AdoNetProfiler.Timelines.Abstractions;
using Glimpse.Core.Extensibility;
using Glimpse.Core.Message;

namespace Glimpse.AdoNetProfiler.Timelines
{
    internal class CommandTimeline : TimelineBase
    {
        private readonly DbCommand _command;
        private readonly Guid _connetionId;
        private readonly Guid? _transactionId;

        protected override string EventName => $"Command:{_command.CommandText}";

        protected override TimelineCategoryItem CategoryItem => new TimelineCategoryItem("Command", "#FD45F7", "#DD31DA");

        internal CommandTimeline(IInspectorContext context, DbCommand command, Guid connectionId, Guid? transactionId)
            : base(context)
        {
            _command       = command;
            _connetionId   = connectionId;
            _transactionId = transactionId;
        }

        internal void WriteTimelineMessage(int records)
        {
            var timelineMessage = new CommandTimelineMessage(_command, _connetionId, _transactionId, records);

            WriteTimelineMessageCore(timelineMessage);
        }

        internal void WriteTimelineMessage(bool isError)
        {
            var timelineMessage = new CommandTimelineMessage(_command, _connetionId, _transactionId, isError);

            WriteTimelineMessageCore(timelineMessage);
        }
    }
}