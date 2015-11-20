using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;

namespace AdoNetProfiler
{
    [DesignerCategory("")]
    internal class AdoNetProfilerDbCommand : DbCommand
    {
        private DbCommand _command;
        private DbConnection _connection;
        private DbTransaction _transaction;
        private readonly IAdoNetProfiler _profiler;

        public override string CommandText
        {
            get { return _command.CommandText; }
            set { _command.CommandText = value; }
        }

        public override int CommandTimeout
        {
            get { return _command.CommandTimeout; }
            set { _command.CommandTimeout = value; }
        }

        public override CommandType CommandType
        {
            get { return _command.CommandType; }
            set
            {
                if (value != CommandType.Text && value != CommandType.StoredProcedure && value != CommandType.TableDirect)
                    throw new ArgumentOutOfRangeException(nameof(value));

                _command.CommandType = value;
            }
        }

        protected override DbConnection DbConnection
        {
            get { return _connection; }
            set
            {
                _connection = value;
                var adoNetProfilerDbConnection = value as AdoNetProfilerDbConnection;
                _command.Connection = (adoNetProfilerDbConnection == null)
                    ? value
                    : adoNetProfilerDbConnection.WrappedConnection;
            }
        }

        protected override DbParameterCollection DbParameterCollection => _command.Parameters;

        protected override DbTransaction DbTransaction
        {
            get { return _transaction; }
            set
            {
                _transaction = value;
                var adoNetProfilerDbTransaction = value as AdoNetProfilerDbTransaction;
                _command.Transaction = (adoNetProfilerDbTransaction == null)
                    ? value
                    : adoNetProfilerDbTransaction.WrappedTransaction;
            }
        }

        public override bool DesignTimeVisible
        {
            get { return _command.DesignTimeVisible; }
            set { _command.DesignTimeVisible = value; }
        }

        public override UpdateRowSource UpdatedRowSource
        {
            get { return _command.UpdatedRowSource; }
            set { _command.UpdatedRowSource = value; }
        }

        public DbCommand WrappedCommand => _command;

        internal AdoNetProfilerDbCommand(DbCommand command, DbConnection connection, IAdoNetProfiler profiler)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            _command    = command;
            _connection = connection;
            _profiler   = profiler;
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            if (_profiler == null || !_profiler.IsEnabled)
                return _command.ExecuteReader(behavior);

            _profiler.OnCommandStart(this);

            try
            {
                var dbReader = _command.ExecuteReader(behavior);
                
                return new AdoNetProfilerDbDataReader(dbReader, _profiler);
            }
            catch (Exception ex)
            {
                _profiler.OnCommandError(this, ex);
                throw;
            }
            finally
            {
                _profiler.OnCommandFinish(this, true);
            }
        }

        public override int ExecuteNonQuery()
        {
            if (_profiler == null || !_profiler.IsEnabled)
                return _command.ExecuteNonQuery();

            _profiler.OnCommandStart(this);

            try
            {
                return _command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                _profiler.OnCommandError(this, ex);
                throw;
            }
            finally
            {
                _profiler.OnCommandFinish(this, false);
            }
        }

        public override object ExecuteScalar()
        {
            if (_profiler == null || !_profiler.IsEnabled)
                return _command.ExecuteScalar();

            _profiler.OnCommandStart(this);

            try
            {
                return _command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                _profiler.OnCommandError(this, ex);
                throw;
            }
            finally
            {
                _profiler.OnCommandFinish(this, false);
            }
        }

        public override void Cancel()
        {
            _command.Cancel();
        }

        public override void Prepare()
        {
            _command.Prepare();
        }

        protected override DbParameter CreateDbParameter()
        {
            return _command.CreateParameter();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _command?.Dispose();

            _command = null;

            base.Dispose(disposing);
        }
    }
}