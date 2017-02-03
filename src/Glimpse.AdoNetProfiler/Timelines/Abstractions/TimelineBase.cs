using System;
using Glimpse.Core.Extensibility;
using Glimpse.Core.Message;

namespace Glimpse.AdoNetProfiler.Timelines.Abstractions
{
    internal abstract class TimelineBase
    {
        private readonly IMessageBroker _messageBroker;
        private readonly IExecutionTimer _timer;
        private readonly TimeSpan _offset;

        protected abstract string EventName { get; }

        protected abstract TimelineCategoryItem CategoryItem { get; }

        protected TimelineBase(IInspectorContext context)
        {
            _messageBroker = context.MessageBroker;
            _timer         = context.TimerStrategy();
            _offset        = _timer.Point().Offset;
        }

        protected void WriteTimelineMessageCore(ITimelineMessage timelineMessage)
        {
            var timerResult = _timer.Stop(_offset);
            var message     = timelineMessage
                .AsTimelineMessage(EventName, CategoryItem)
                .AsTimedMessage(timerResult);

            _messageBroker.Publish(message);
        }
    }
}