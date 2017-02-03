using System.Collections.Generic;
using System.Linq;
using Glimpse.AdoNetProfiler.TimelineMessages;
using Glimpse.Core.Extensibility;
using Glimpse.Core.Extensions;
using Glimpse.Core.Tab.Assist;

namespace Glimpse.AdoNetProfiler.Tabs
{
    /// <summary>
    /// The glimpse tab for database commands. 
    /// </summary>
    public class CommandTab : TabBase, ITabSetup, ITabLayout, ILayoutControl
    {
        private static readonly object _layout;

        /// <inheritdic cref="TabBase.Name" />
        public override string Name => "Command";

        /// <inheritdic cref="ILayoutControl.KeysHeadings" />
        public bool KeysHeadings => true;

        static CommandTab()
        {
            _layout = TabLayout.Create()
                .Cell(
                    "SQL Statistics",
                    TabLayout.Create().Row(row =>
                    {
                        row.Cell(0).WithTitle("Queries");
                        row.Cell(1).WithTitle("Duplications");
                        row.Cell(2).Suffix(" ms").WithTitle("Total Query Duration");
                    })
                )
                .Cell(
                    "Queries",
                    TabLayout.Create().Row(row =>
                    {
                        row.Cell(0).WithTitle("Ordinal");
                        row.Cell(1).WithTitle("Database");
                        row.Cell(2).AsCode(CodeType.Sql).WithTitle("Command");
                        row.Cell(3).DisablePreview().WithTitle("Parameters");
                        row.Cell(4).WithTitle("CommandType");
                        row.Cell(5).WithTitle("With Transaction");
                        row.Cell(6).WithTitle("Records");
                        row.Cell(7).WithTitle("IsError");
                        row.Cell(8).Suffix(" ms").WithTitle("Duration");
                        row.Cell(9).Prefix("T+ ").Suffix(" ms").WithTitle("Offset");
                    })
                )
                .Build();
        }

        /// <inheritdic cref="TabBase.GetData(ITabContext)" />
        public override object GetData(ITabContext context)
        {
            var commands = context.GetMessages<CommandTimelineMessage>().ToArray();

            if (!commands.Any())
            {
                return null;
            }

            var ordinal = 1;
            var duplicationKeys = new HashSet<string>();
            var duplicationCount = 0;
            var queries = new TabSection("Ordinal", "Database", "Command", "Parameters", "CommandType", "With Transaction", "Records", "IsError", "Duration", "Offset");
            foreach (var command in commands)
            {
                var parameters = new TabSection("Name", "Value", "DbType", "Direction");
                foreach (var parameter in command.Parameters)
                {
                    parameters.AddRow()
                        .Column(parameter.ParameterName)
                        .Column(parameter.Value)
                        .Column(parameter.DbType.ToString())
                        .Column(parameter.Direction.ToString());
                }

                var row = queries.AddRow()
                    .Column(ordinal)
                    .Column(command.Database)
                    .Column(command.CommandText)
                    .Column(command.Parameters.Any() ? parameters : null)
                    .Column(command.CommandType.ToString())
                    .Column(command.WithTransaction)
                    .Column(command.Records ?? 0)
                    .Column(command.IsError)
                    .Column(command.Duration)
                    .Column(command.Offset);

                var isDuplicated = !duplicationKeys.Add(command.CommandText);
                if (isDuplicated)
                {
                    row.WarnIf(true);
                    duplicationCount++;
                }

                ordinal++;
            }

            var sqlStatistics = new TabSection("Queries", "Duplication", "Total Query Duration");

            sqlStatistics.AddRow()
                .Column(commands.Length)
                .Column(duplicationCount)
                .Column(commands.Select(x => x.Duration).Aggregate((x, y) => x.Add(y)));

            var root = new TabObject();

            root.AddRow().Key("SQL Statistics").Value(sqlStatistics);
            root.AddRow().Key("Queries").Value(queries);

            return root.Build();
        }

        /// <inheritdic cref="ITabSetup.Setup(ITabSetupContext)" />
        public void Setup(ITabSetupContext context)
        {
            context.PersistMessages<CommandTimelineMessage>();
        }

        /// <inheritdic cref="ITabLayout.GetLayout()" />
        public object GetLayout()
        {
            return _layout;
        }
    }
}