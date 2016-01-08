using System;
using System.Data.Common;
using Glimpse.AdoNetProfiler.Timelines;
using Glimpse.AdoNetProfiler.Utilities;
using Glimpse.Core.Extensibility;

namespace Glimpse.AdoNetProfiler.Inspectors
{
    internal class ConnectionInspector : IInspector
    {
        private static IInspectorContext _context;

        public void Setup(IInspectorContext context)
        {
            _context = context;
        }

        internal static ConnectionEventTimeline CreateEventTimeline(DbConnection connection, Guid connectionId, ConnectionEvent connectionEvent)
        {
            return new ConnectionEventTimeline(_context, connection, connectionId, connectionEvent);
        }

        internal static ConnectionLifetimeTimeline CreateLifetimeTimeline(DbConnection connection, Guid connectionId)
        {
            return new ConnectionLifetimeTimeline(_context, connection, connectionId);
        }
    }
}