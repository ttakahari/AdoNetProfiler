#if !COREFX
using System;
using System.Data.Common;
using System.Reflection;
using System.Security;
using System.Security.Permissions;

namespace AdoNetProfiler
{
    /// <summary>
    /// The abstract class that defines methods to create a instance of data source class of a provider, using <see cref="IAdoNetProfiler"/>.
    /// </summary>
    
    public abstract class AdoNetProfilerProviderFactory : DbProviderFactory
    {
    }

    /// <summary>
    /// Defining methods to create a instance of data source class of a provider, using <see cref="IAdoNetProfiler"/>.
    /// </summary>
    /// <typeparam name="TProviderFactory">The type of the original <see cref="DbProviderFactory"/>.</typeparam>
    public class AdoNetProfilerProviderFactory<TProviderFactory> : AdoNetProfilerProviderFactory, IServiceProvider
        where TProviderFactory : DbProviderFactory
    {
        /// <summary>
        /// The current instance of <see cref="AdoNetProfilerProviderFactory{TProviderFactory}"/>.
        /// </summary>
        public static readonly AdoNetProfilerProviderFactory<TProviderFactory> Instance = new AdoNetProfilerProviderFactory<TProviderFactory>();

        /// <summary>
        /// The original <see cref="DbProviderFactory"/>.
        /// </summary>
        public TProviderFactory WrappedProviderFactory { get; }

        /// <inheritdoc cref="DbProviderFactory.CanCreateDataSourceEnumerator" />
        public override bool CanCreateDataSourceEnumerator => WrappedProviderFactory.CanCreateDataSourceEnumerator;

        /// <summary>
        /// Create a new instance of <see cref="AdoNetProfilerProviderFactory{TProviderFactory}"/>.
        /// </summary>
        public AdoNetProfilerProviderFactory()
        {
            var field = typeof(TProviderFactory).GetField("Instance", BindingFlags.Public | BindingFlags.Static);

            if (field == null)
            {
                throw new NotSupportedException("Provider doesn't have Instance property.");
            }

            WrappedProviderFactory = (TProviderFactory)field.GetValue(null);
        }

        /// <inheritdoc cref="DbProviderFactory.CreateCommand()" />
        public override DbCommand CreateCommand()
        {
            var command    = WrappedProviderFactory.CreateCommand();
            var connection = (AdoNetProfilerDbConnection)WrappedProviderFactory.CreateConnection();
            var profiler   = connection.Profiler;

            return new AdoNetProfilerDbCommand(command, connection, profiler);
        }

        /// <inheritdoc cref="DbProviderFactory.CreateCommandBuilder()" />
        public override DbCommandBuilder CreateCommandBuilder()
        {
            return WrappedProviderFactory.CreateCommandBuilder();
        }

        /// <inheritdoc cref="DbProviderFactory.CreateConnection()" />
        public override DbConnection CreateConnection()
        {
            var connection = WrappedProviderFactory.CreateConnection();

            return new AdoNetProfilerDbConnection(connection);
        }

        /// <inheritdoc cref="DbProviderFactory.CreateConnectionStringBuilder()" />
        public override DbConnectionStringBuilder CreateConnectionStringBuilder()
        {
            return WrappedProviderFactory.CreateConnectionStringBuilder();
        }

        /// <inheritdoc cref="DbProviderFactory.CreateDataAdapter()" />
        public override DbDataAdapter CreateDataAdapter()
        {
            return WrappedProviderFactory.CreateDataAdapter();
        }

        /// <inheritdoc cref="DbProviderFactory.CreateDataSourceEnumerator()" />
        public override DbDataSourceEnumerator CreateDataSourceEnumerator()
        {
            return WrappedProviderFactory.CreateDataSourceEnumerator();
        }

        /// <inheritdoc cref="DbProviderFactory.CreateParameter()" />
        public override DbParameter CreateParameter()
        {
            return WrappedProviderFactory.CreateParameter();
        }

        /// <inheritdoc cref="DbProviderFactory.CreatePermission(PermissionState)" />
        public override CodeAccessPermission CreatePermission(PermissionState state)
        {
            return WrappedProviderFactory.CreatePermission(state);
        }

        /// <inheritdoc cref="IServiceProvider.GetService(Type)" />
        public object GetService(Type serviceType)
        {
            if (serviceType == GetType())
            {
                return WrappedProviderFactory;
            }

            var service = ((IServiceProvider)WrappedProviderFactory).GetService(serviceType);
            
            return service;
        }
    }
}
#endif
