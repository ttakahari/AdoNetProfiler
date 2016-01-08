using System;
using System.Data.Common;
using Glimpse.AdoNetProfiler.Timelines;
using Glimpse.Core.Extensibility;

namespace Glimpse.AdoNetProfiler.Inspectors
{
    internal class CommandInspector : IInspector
    {
        private static IInspectorContext _context;

        public void Setup(IInspectorContext context)
        {
            _context = context;
        }

        internal static CommandTimeline CreateTimeline(DbCommand command, Guid connectionId, Guid? transactionId)
        {
            return new CommandTimeline(_context, command, connectionId, transactionId);
        }
    }
}