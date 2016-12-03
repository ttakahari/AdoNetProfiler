using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;

namespace AdoNetProfiler
{
    [DesignerCategory("")]
    public class AdoNetProfilerDbCommand : DbCommand
    {
        private DbConnection _connection;
        private DbTransaction _transaction;
        private readonly IAdoNetProfiler _profiler;

        public override string CommandText
        {
            get { return WrappedCommand.CommandText; }
            set { WrappedCommand.CommandText = value; }
        }

        public override int CommandTimeout
        {
            get { return WrappedCommand.CommandTimeout; }
            set { WrappedCommand.CommandTimeout = value; }
        }

        public override CommandType CommandType
        {
            get { return WrappedCommand.CommandType; }
            set
            {
                if (value != CommandType.Text &&
                    value != CommandType.StoredProcedure &&
                    value != CommandType.TableDirect)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                WrappedCommand.CommandType = value;
            }
        }

        protected override DbConnection DbConnection
        {
            get { return _connection; }
            set
            {
                _connection = value;
                var adoNetProfilerDbConnection = value as AdoNetProfilerDbConnection;
                WrappedCommand.Connection = (adoNetProfilerDbConnection == null)
                    ? value
                    : adoNetProfilerDbConnection.WrappedConnection;
            }
        }

        protected override DbParameterCollection DbParameterCollection => WrappedCommand.Parameters;

        protected override DbTransaction DbTransaction
        {
            get { return _transaction; }
            set
            {
                _transaction = value;
                var adoNetProfilerDbTransaction = value as AdoNetProfilerDbTransaction;
                WrappedCommand.Transaction = (adoNetProfilerDbTransaction == null)
                    ? value
                    : adoNetProfilerDbTransaction.WrappedTransaction;
            }
        }

        public override bool DesignTimeVisible
        {
            get { return WrappedCommand.DesignTimeVisible; }
            set { WrappedCommand.DesignTimeVisible = value; }
        }

        public override UpdateRowSource UpdatedRowSource
        {
            get { return WrappedCommand.UpdatedRowSource; }
            set { WrappedCommand.UpdatedRowSource = value; }
        }

        public DbCommand WrappedCommand { get; private set; }

        internal AdoNetProfilerDbCommand(DbCommand command, DbConnection connection, IAdoNetProfiler profiler)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            WrappedCommand = command;

            _connection = connection;
            _profiler   = profiler;
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            if (_profiler == null || !_profiler.IsEnabled)
            {
                return WrappedCommand.ExecuteReader(behavior);
            }

            _profiler.OnExecuteReaderStart(this);

            try
            {
                var dbReader = WrappedCommand.ExecuteReader(behavior);
                
                return new AdoNetProfilerDbDataReader(dbReader, _profiler);
            }
            catch (Exception ex)
            {
                _profiler.OnCommandError(this, ex);
                throw;
            }
        }

        public override int ExecuteNonQuery()
        {
            if (_profiler == null || !_profiler.IsEnabled)
            {
                return WrappedCommand.ExecuteNonQuery();
            }

            _profiler.OnExecuteNonQueryStart(this);

            var result = default(int?);
            try
            {
                result = WrappedCommand.ExecuteNonQuery();

                return result.Value;
            }
            catch (Exception ex)
            {
                _profiler.OnCommandError(this, ex);
                throw;
            }
            finally
            {
                _profiler.OnExecuteNonQueryFinish(this, result ?? 0);
            }
        }

        public override object ExecuteScalar()
        {
            if (_profiler == null || !_profiler.IsEnabled)
            {
                return WrappedCommand.ExecuteScalar();
            }

            _profiler.OnExecuteScalarStart(this);

            object result = null;
            try
            {
                result = WrappedCommand.ExecuteScalar();

                return result;
            }
            catch (Exception ex)
            {
                _profiler.OnCommandError(this, ex);
                throw;
            }
            finally
            {
                _profiler.OnExecuteScalarFinish(this, result);
            }
        }

        public override void Cancel()
        {
            WrappedCommand.Cancel();
        }

        public override void Prepare()
        {
            WrappedCommand.Prepare();
        }

        protected override DbParameter CreateDbParameter()
        {
            return WrappedCommand.CreateParameter();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                WrappedCommand?.Dispose();
            }

            WrappedCommand = null;

            base.Dispose(disposing);
        }
    }
}