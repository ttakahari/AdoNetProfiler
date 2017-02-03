using System;
using Glimpse.Core.Message;

namespace Glimpse.AdoNetProfiler.TimelineMessages.Abstractions
{
    /// <summary>
    /// The abstract class that defines properties of Glimpse Timeline.
    /// </summary>
    public abstract class TimelineMessageBase : MessageBase, ITimelineMessage
    {
        /// <inheritdoc cref="ITimedMessage.Offset" />
        public TimeSpan Offset { get; set; }

        /// <inheritdoc cref="ITimedMessage.Duration" />
        public TimeSpan Duration { get; set; }

        /// <inheritdoc cref="ITimedMessage.StartTime" />
        public DateTime StartTime { get; set; }

        /// <inheritdoc cref="ITimelineMessage.EventName" />
        public string EventName { get; set; }

        /// <inheritdoc cref="ITimelineMessage.EventCategory" />
        public TimelineCategoryItem EventCategory { get; set; }

        /// <inheritdoc cref="ITimelineMessage.EventSubText" />
        public string EventSubText { get; set; }
    }
}