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
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            WrappedTransaction = transaction;

            _connection = connection;
            _profiler   = profiler;
        }

        public override void Commit()
        {
            if (_profiler == null || !_profiler.IsEnabled)
            {
                WrappedTransaction.Commit();
                WrappedTransaction.Dispose();
                WrappedTransaction = null;
                return;
            }
            
            _profiler.OnCommitting(this);

            WrappedTransaction.Commit();
            WrappedTransaction.Dispose();
            WrappedTransaction = null;

            _profiler.OnCommitted(_connection);
        }

        public override void Rollback()
        {
            if (_profiler == null || !_profiler.IsEnabled)
            {
                WrappedTransaction.Rollback();
                WrappedTransaction.Dispose();
                WrappedTransaction = null;
                return;
            }
            
            _profiler.OnRollbacking(this);

            WrappedTransaction.Rollback();
            WrappedTransaction.Dispose();
            WrappedTransaction = null;

            _profiler.OnRollbacked(_connection);
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
            _connection  = null;

            base.Dispose(disposing);
        }
    }
}