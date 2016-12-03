using System;
using System.Data;
using System.Data.Common;

namespace AdoNetProfiler
{
    public class AdoNetProfilerDbTransaction : DbTransaction
    {
        private DbConnection _connection;
        private readonly IAdoNetProfiler _profiler;

        protected override DbConnection DbConnection => _connection;

        public override IsolationLevel IsolationLevel => WrappedTransaction.IsolationLevel;

        public DbTransaction WrappedTransaction { get; private set; }

        internal AdoNetProfilerDbTransaction(DbTransaction transaction, DbConnection connection, IAdoNetProfiler profiler)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (connection  == null) throw new ArgumentNullException(nameof(connection));

            WrappedTransaction = transaction;

            _connection = connection;
            _profiler   = profiler;
        }

        public override void Commit()
        {
            if (_profiler == null || !_profiler.IsEnabled)
            {
                CommitWrappedTransaction();

                return;
            }
            
            _profiler.OnCommitting(this);

            CommitWrappedTransaction();

            _profiler.OnCommitted(_connection);
        }

        private void CommitWrappedTransaction()
        {
            WrappedTransaction.Commit();
            WrappedTransaction.Dispose();
            WrappedTransaction = null;
        }

        public override void Rollback()
        {
            if (_profiler == null || !_profiler.IsEnabled)
            {
                RollbackWrappedTransaction();

                return;
            }
            
            _profiler.OnRollbacking(this);

            RollbackWrappedTransaction();

            _profiler.OnRollbacked(_connection);
        }

        private void RollbackWrappedTransaction()
        {
            WrappedTransaction.Rollback();
            WrappedTransaction.Dispose();
            WrappedTransaction = null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (WrappedTransaction != null)
                {
                    Rollback();
                }
            }

            WrappedTransaction = null;

            _connection = null;

            base.Dispose(disposing);
        }
    }
}
