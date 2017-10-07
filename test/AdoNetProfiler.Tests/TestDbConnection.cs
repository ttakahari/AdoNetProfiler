using System.Data.Common;

namespace AdoNetProfiler.Tests
{
    public class TestDbConnection : AdoNetProfilerDbConnection
    {
        public TestProfiler TestProfiler { get; }

        public TestDbConnection(DbConnection connection, IAdoNetProfiler profiler)
            : base(connection, profiler)
        {
            TestProfiler = (TestProfiler)profiler;
        }
    }
}
