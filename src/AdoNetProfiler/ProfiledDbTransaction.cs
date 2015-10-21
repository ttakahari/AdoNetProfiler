using System;
using System.Data;
using System.Data.Common;

namespace AdoNetProfiler
{
    public class ProfiledDbTransaction : DbTransaction
    {
        private DbConnection _connection;
        private DbTransaction _transaction;
        private readonly IProfiler _profiler;

        protected override DbConnection DbConnection => _connection;

        public override IsolationLevel IsolationLevel => _transaction.IsolationLevel;

        internal DbTransaction WrappedDbTransaction => _transaction;

        public ProfiledDbTransaction(DbTransaction transaction, DbConnection connection, IProfiler profiler)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            _transaction = transaction;
            _connection  = connection;
            _profiler    = profiler;
        }

        public override void Commit()
        {
            if (_profiler == null || !_profiler.IsEnabled)
            {
                _transaction.Commit();
                return;
            }
            
            _profiler.OnCommitting(this);

            _transaction.Commit();

            _profiler.OnCommitted(_connection);
        }

        public override void Rollback()
        {
            if (_profiler == null || !_profiler.IsEnabled)
            {
                _transaction.Rollback();
                return;
            }
            
            _profiler.OnRollbacking(this);

            _transaction.Rollback();

            _profiler.OnRollbacked(_connection);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _transaction?.Dispose();

            _transaction = null;
            _connection  = null;

            base.Dispose(disposing);
        }
    }
}