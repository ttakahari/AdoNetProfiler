namespace Glimpse.AdoNetProfiler.Utilities
{
    public enum ConnectionEvent
    {
         Open  = 0,
         Close = 1
    }

    public enum TransactionEvent
    {
        BeginTransaction = 0,
        Commit           = 1,
        Rollback         = 2
    }
}