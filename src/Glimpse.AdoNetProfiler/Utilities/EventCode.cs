namespace Glimpse.AdoNetProfiler.Utilities
{
    /// <summary>
    /// The enum that defines events of <see cref="System.Data.Common.DbConnection"/>. 
    /// </summary>
    public enum ConnectionEvent
    {
        /// <summary>Connction open.</summary>
         Open  = 0,
        /// <summary>Connction close.</summary>
        Close = 1
    }

    /// <summary>
    /// The enum that defines events of <see cref="System.Data.Common.DbTransaction"/>. 
    /// </summary>
    public enum TransactionEvent
    {
        /// <summary>Transaction begin.</summary>
        BeginTransaction = 0,
        /// <summary>Transaction commit.</summary>
        Commit           = 1,
        /// <summary>Transaction rollbak.</summary>
        Rollback         = 2
    }
}