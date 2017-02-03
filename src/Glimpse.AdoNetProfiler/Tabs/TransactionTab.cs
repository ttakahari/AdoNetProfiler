using System.Collections.Generic;
using System.Linq;
using Glimpse.AdoNetProfiler.TimelineMessages;
using Glimpse.Core.Extensibility;
using Glimpse.Core.Extensions;
using Glimpse.Core.Tab.Assist;

namespace Glimpse.AdoNetProfiler.Tabs
{
    /// <summary>
    /// The glimpse tab for database transactions. 
    /// </summary>
    public class TransactionTab : TabBase, ITabSetup, ITabLayout, ILayoutControl
    {
        private static readonly object _layout;

        /// <inheritdic cref="TabBase.Name" />
        public override string Name => "Transaction";

        /// <inheritdic cref="ILayoutControl.KeysHeadings" />
        public bool KeysHeadings => true;

        static TransactionTab()
        {
            _layout = TabLayout.Create()
                .Cell(
                    "Transaction Statistics",
                    TabLayout.Create().Row(row =>
                    {
                        row.Cell(0).WithTitle("Database");
                        row.Cell(1).WithTitle("Queries");
                        row.Cell(2).Suffix(" ms").WithTitle("Total Transaction Duration");
                    })
                )
                .Cell(
                    "Transaction Events",
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
                        row.Cell(3).WithTitle("Commited");
                        row.Cell(4).Suffix(" ms").WithTitle("Total Duration");
                    })
                )
                .Build();
        }

        /// <inheritdic cref="TabBase.GetData(ITabContext)" />
        public override object GetData(ITabContext context)
        {
            var connectionLifetimes = context.GetMessages<ConnectionLifetimeTimelineMessage>().ToArray();
            var transactionLifetimes = context.GetMessages<TransactionLifetimeTimelineMessage>().ToArray();
            var transactionEvents = context.GetMessages<TransactionEventTimelineMessage>().ToArray();
            var commands = context.GetMessages<CommandTimelineMessage>().ToArray();

            var statisticsSection = new TabSection("Database", "Queries", "Total Transaction Duration");
            var eventSection = new TabSection("Database", "Events", "Queries", "IsCommited", "Total Duration");

            // Statistics
            foreach (var transactionLifetime in transactionLifetimes.OrderBy(x => x.Offset).ToArray())
            {
                statisticsSection.AddRow()
                    .Column(connectionLifetimes.First(x => x.ConnectionId == transactionLifetime.ConnectionId).Database)
                    .Column(commands.Count(x => x.TransactionId.HasValue && x.TransactionId.Value == transactionLifetime.TransactionId))
                    .Column(transactionLifetime.Duration);
            }

            // Events
            foreach (var transactionLifetime in transactionLifetimes.OrderBy(x => x.Offset).ToArray())
            {
                var queries = commands
                    .Where(x => x.TransactionId.HasValue && x.TransactionId.Value == transactionLifetime.TransactionId)
                    .ToArray();

                var events = transactionEvents
                    .Where(x => x.TransactionId == transactionLifetime.TransactionId)
                    .Select(x => new
                    {
                        EventType = "Transaction",
                        EventName = x.TransactionEvent.ToString(),
                        Duration  = x.Duration,
                        Offset    = x.Offset
                    })
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
                var duplicatedKeys = new HashSet<string>();

                foreach (var @event in events)
                {
                    var row = eventDetailSection.AddRow()
                        .Column(@event.EventType)
                        .Column(@event.EventName)
                        .Column(@event.Duration)
                        .Column(@event.Offset);

                    if (@event.EventType == "Command")
                    {
                        row.WarnIf(!duplicatedKeys.Add(@event.EventName));
                    }
                }

                eventSection.AddRow()
                    .Column(transactionLifetime.Database)
                    .Column(eventDetailSection)
                    .Column(commands.Length)
                    .Column(transactionLifetime.IsCommited)
                    .Column(transactionLifetime.Duration);
            }

            var root = new TabObject();

            root.AddRow().Key("Transaction Statistics").Value(statisticsSection);
            root.AddRow().Key("Transaction Events").Value(eventSection);

            return root.Build();
        }

        /// <inheritdic cref="ITabSetup.Setup(ITabSetupContext)" />
        public void Setup(ITabSetupContext context)
        {
            context.PersistMessages<CommandTimelineMessage>();
            context.PersistMessages<ConnectionLifetimeTimelineMessage>();
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