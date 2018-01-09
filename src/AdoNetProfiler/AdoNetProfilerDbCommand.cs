using System;
using System.Collections;
#if !COREFX
using System.ComponentModel;
#endif
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Reflection;

namespace AdoNetProfiler
{
    /// <summary>
    /// The database command wrapped <see cref="DbCommand"/>.
    /// </summary>
#if !COREFX
    [DesignerCategory("")]
#endif
    public class AdoNetProfilerDbCommand : DbCommand
    {
        private DbConnection _connection;
        private DbTransaction _transaction;
        private readonly IAdoNetProfiler _profiler;
        private static readonly Hashtable _bindByNameGetCache = new Hashtable();
        private static readonly Hashtable _bindByNameSetCache = new Hashtable();

        /// <inheritdoc cref="DbCommand.CommandText" />
        public override string CommandText
        {
            get { return WrappedCommand.CommandText; }
            set { WrappedCommand.CommandText = value; }
        }

        /// <inheritdoc cref="DbCommand.CommandTimeout" />
        public override int CommandTimeout
        {
            get { return WrappedCommand.CommandTimeout; }
            set { WrappedCommand.CommandTimeout = value; }
        }

        /// <inheritdoc cref="DbCommand.CommandType" />
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

        /// <inheritdoc cref="DbCommand.DbConnection" />
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

        /// <inheritdoc cref="DbCommand.DbParameterCollection" />
        protected override DbParameterCollection DbParameterCollection => WrappedCommand.Parameters;

        /// <inheritdoc cref="DbCommand.DbTransaction" />
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

        /// <inheritdoc cref="DbCommand.DesignTimeVisible" />
        public override bool DesignTimeVisible
        {
            get { return WrappedCommand.DesignTimeVisible; }
            set { WrappedCommand.DesignTimeVisible = value; }
        }

        /// <inheritdoc cref="DbCommand.UpdatedRowSource" />
        public override UpdateRowSource UpdatedRowSource
        {
            get { return WrappedCommand.UpdatedRowSource; }
            set { WrappedCommand.UpdatedRowSource = value; }
        }

        /// <summary>
        /// The original <see cref="DbCommand"/>.
        /// </summary>
        public DbCommand WrappedCommand { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public bool BindByName
        {
            get
            {
                var cache = GetBindByNameGetAction(WrappedCommand.GetType());

                return cache != null ? cache.Invoke(WrappedCommand) : false;
            }
            set
            {
                var cache = GetBindByNameSetAction(WrappedCommand.GetType());

                if (cache != null)
                {
                    cache.Invoke(WrappedCommand, value);
                }
            }
        }

        internal AdoNetProfilerDbCommand(DbCommand command, DbConnection connection, IAdoNetProfiler profiler)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            WrappedCommand = command;

            _connection = connection;
            _profiler = profiler;
        }

        /// <inheritdoc cref="DbCommand.ExecuteDbDataReader(CommandBehavior)" />
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

        /// <inheritdoc cref="DbCommand.ExecuteNonQuery()" />
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

        /// <inheritdoc cref="DbCommand.ExecuteScalar()" />
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

        /// <inheritdoc cref="DbCommand.Cancel()" />
        public override void Cancel()
        {
            WrappedCommand.Cancel();
        }

        /// <inheritdoc cref="DbCommand.Prepare()" />
        public override void Prepare()
        {
            WrappedCommand.Prepare();
        }

        /// <inheritdoc cref="DbCommand.CreateDbParameter()" />
        protected override DbParameter CreateDbParameter()
        {
            return WrappedCommand.CreateParameter();
        }

        /// <summary>
        /// Free, release, or reset managed or unmanaged resources.
        /// </summary>
        /// <param name="disposing">Wether to free, release, or resetting unmanaged resources or not.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                WrappedCommand?.Dispose();
            }

            WrappedCommand = null;

            base.Dispose(disposing);
        }

        private Func<DbCommand, bool> GetBindByNameGetAction(Type commandType)
        {
            lock (_bindByNameGetCache)
            {
                if (_bindByNameGetCache[commandType] is Func<DbCommand, bool> cache)
                {
                    return cache;
                }

                var property = commandType
#if NETSTANDARD1_6
                    .GetTypeInfo()
#endif
                    .GetProperty("BindByName", BindingFlags.Public | BindingFlags.Instance);
                
                // check for only OracleCommand
                if (property != null
                    && property.CanRead
                    && property.PropertyType == typeof(bool)
                    && property.GetIndexParameters().Length == 0
                    && property.GetGetMethod() != null)
                {
                    var target = Expression.Parameter(typeof(DbCommand), "target");
                    var prop   = Expression.PropertyOrField(Expression.Convert(target, commandType), "BindByName");

                    var action = Expression.Lambda<Func<DbCommand, bool>>(Expression.Convert(prop, typeof(bool)), target).Compile();

                    _bindByNameGetCache.Add(commandType, action);

                    return action;
                }
            }

            return null;
        }

        private Action<DbCommand, bool> GetBindByNameSetAction(Type commandType)
        {
            lock (_bindByNameSetCache)
            {
                if (_bindByNameSetCache[commandType] is Action<DbCommand, bool> cache)
                {
                    return cache;
                }

                var property = commandType
#if NETSTANDARD1_6
                    .GetTypeInfo()
#endif
                    .GetProperty("BindByName", BindingFlags.Public | BindingFlags.Instance);
                
                // check for only OracleCommand
                if (property != null
                    && property.CanWrite
                    && property.PropertyType == typeof(bool)
                    && property.GetIndexParameters().Length == 0
                    && property.GetSetMethod() != null)
                {
                    var target = Expression.Parameter(typeof(DbCommand), "target");
                    var value  = Expression.Parameter(typeof(bool), "value");

                    var left  = Expression.PropertyOrField(Expression.Convert(target, commandType), "BindByName");
                    var right = Expression.Convert(value, left.Type);

                    var action = Expression.Lambda<Action<DbCommand, bool>>(Expression.Assign(left, right), target, value).Compile();

                    _bindByNameSetCache.Add(commandType, action);

                    return action;
                }
            }

            return null;
        }
    }
}
