using System.Collections.Generic;
using System.Linq;
using Glimpse.AdoNetProfiler.TimelineMessages;
using Glimpse.Core.Extensibility;
using Glimpse.Core.Extensions;
using Glimpse.Core.Tab.Assist;

namespace Glimpse.AdoNetProfiler.Tabs
{
    /// <summary>
    /// The glimpse tab for database connections. 
    /// </summary>
    public class ConnectionTab : TabBase, ITabSetup, ITabLayout, ILayoutControl
    {
        private static readonly object _layout;

        /// <inheritdic cref="TabBase.Name" />
        public override string Name => "Connection";

        /// <inheritdic cref="ILayoutControl.KeysHeadings" />
        public bool KeysHeadings => true;

        static ConnectionTab()
        {
            _layout = TabLayout.Create()
                .Cell(
                    "Connection Statistics",
                    TabLayout.Create().Row(row =>
                    {
                        row.Cell(0).WithTitle("Database");
                        row.Cell(1).WithTitle("Connections");
                        row.Cell(2).WithTitle("Queries");
                        row.Cell(3).WithTitle("Tansactions");
                        row.Cell(4).Suffix(" ms").WithTitle("Total Connection Duration");
                    })
                )
                .Cell(
                    "Connection Events",
                    TabLayout.Create().Row(row =>
                    {
                        row.Cell(0).WithTitle("Database");
                        row.Cell(1).DisablePreview().WithTitle("Events").SetLayout(
                            TabLayout.Create().Row(r =>
                            {
                                r.Cell(0).WithTitle("Event Type");
                                r.Cell(1).AsCode(CodeType.Sql).WithTitle("Event Name");
                                r.Cell(2).Suffix(" ms").WithTitle("Duration");
                                r.Cell(3).Prefix("T+ ").Suffix(" ms").WithTitle("Offset");
                            })
                        );
                        row.Cell(2).WithTitle("Queries");
                        row.Cell(3).Suffix(" ms").WithTitle("Total Duration");
                    })
                )
                .Build();
        }

        /// <inheritdic cref="TabBase.GetData(ITabContext)" />
        public override object GetData(ITabContext context)
        {
            var connectionLifetimes  = context.GetMessages<ConnectionLifetimeTimelineMessage>().ToArray();
            var connectionEvents     = context.GetMessages<ConnectionEventTimelineMessage>().ToArray();
            var transactionLifetimes = context.GetMessages<TransactionLifetimeTimelineMessage>().ToArray();
            var transactionEvents    = context.GetMessages<TransactionEventTimelineMessage>().ToArray();
            var commands             = context.GetMessages<CommandTimelineMessage>().ToArray();
            
            var statisticsSection = new TabSection("Database", "Connections", "Queries", "Transactions", "Total Connection Duration");
            var eventSection      = new TabSection("Database", "Events", "Queries", "Total Duration");

            // Statistics
            foreach (var database in connectionLifetimes.OrderBy(x => x.Offset).GroupBy(x => x.Database).ToArray())
            {
                var connectionCount  = database.Count();
                var commandCount     = database.Sum(x => commands.Count(y => y.ConnectionId == x.ConnectionId));
                var transactionCount = database.Sum(x => transactionLifetimes.Count(y => y.ConnectionId == x.ConnectionId));

                var duration = database
                    .Select(x => x.Duration)
                    .Aggregate((x, y) => x.Add(y));

                var row = statisticsSection.AddRow()
                    .Column(database.Key)
                    .Column(connectionCount)
                    .Column(commandCount)
                    .Column(transactionCount)
                    .Column(duration);

                row.WarnIf(1 < connectionCount);
            }

            // Events
            foreach (var connectionLifetime in connectionLifetimes.OrderBy(x => x.Offset).GroupBy(x => x.ConnectionId).ToArray())
            {
                var queries = commands
                    .Where(x => x.ConnectionId == connectionLifetime.Key)
                    .ToArray();

                var events = connectionEvents
                    .Where(x => x.ConnectionId == connectionLifetime.Key)
                    .Select(x => new
                    {
                        EventType = "Connection",
                        EventName = x.ConnectionEvent.ToString(),
                        Duration  = x.Duration,
                        Offset    = x.Offset
                    })
                    .Concat(transactionEvents
                        .Where(x => x.ConnectionId == connectionLifetime.Key)
                        .Select(x => new
                        {
                            EventType = "Transaction",
                            EventName = x.TransactionEvent.ToString(),
                            Duration  = x.Duration,
                            Offset    = x.Offset
                        })
                    )
                    .Concat(queries
                        .Select(x => new
                        {
                            EventType = "Command",
                            EventName = x.CommandText,
                            Duration  = x.Duration,
                            Offset    = x.Offset
                        })
                    )
                    .OrderBy(x => x.Offset)
                    .ToArray();

                var eventDetailSection = new TabSection("EventType", "EventName", "Duration", "Offset");
                var duplicatedEvents   = new HashSet<string>();

                foreach (var @event in events)
                {
                    var row = eventDetailSection.AddRow()
                        .Column(@event.EventType)
                        .Column(@event.EventName)
                        .Column(@event.Duration)
                        .Column(@event.Offset);

                    if (@event.EventType != "Transaction")
                    {
                        row.WarnIf(!duplicatedEvents.Add(@event.EventName));
                    }
                }
                
                eventSection.AddRow()
                    .Column(connectionLifetime.First().Database)
                    .Column(eventDetailSection)
                    .Column(commands.Length)
                    .Column(connectionLifetime.Select(x => x.Duration).Aggregate((x, y) => x.Add(y)));
            }

            var root = new TabObject();

            root.AddRow().Key("Connection Statistics").Value(statisticsSection);
            root.AddRow().Key("Connection Events").Value(eventSection);

            return root.Build();
        }

        /// <inheritdic cref="ITabSetup.Setup(ITabSetupContext)" />
        public void Setup(ITabSetupContext context)
        {
            context.PersistMessages<CommandTimelineMessage>();
            context.PersistMessages<ConnectionLifetimeTimelineMessage>();
            context.PersistMessages<ConnectionEventTimelineMessage>();
            context.PersistMessages<TransactionLifetimeTimelineMessage>();
            context.PersistMessages<TransactionEventTimelineMessage>();
        }

        /// <inheritdic cref="ITabLayout.GetLayout()" />
        public object GetLayout()
        {
            return _layout;
        }
    }
}