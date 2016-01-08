using System;
using System.Data.Common;
using Glimpse.AdoNetProfiler.Timelines;
using Glimpse.AdoNetProfiler.Utilities;
using Glimpse.Core.Extensibility;

namespace Glimpse.AdoNetProfiler.Inspectors
{
    internal class TransactionInspector : IInspector
    {
        private static IInspectorContext _context;

        public void Setup(IInspectorContext context)
        {
            _context = context;
        }

        internal static TransactionEventTimeline CreateEventTimeline(DbConnection connection, Guid connectionId, Guid transactionId, TransactionEvent transactionEvent)
        {
            return new TransactionEventTimeline(_context, connection, connectionId, transactionId, transactionEvent);
        }

        internal static TransactionLifetimeTimeline CreateLifetimeTimeline(DbConnection connection, Guid connectionId)
        {
            return new TransactionLifetimeTimeline(_context, connection, connectionId);
        }
    }
}