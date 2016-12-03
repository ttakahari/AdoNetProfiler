#if !COREFX
using System;
using System.Data.Common;
using System.Reflection;
using System.Security;
using System.Security.Permissions;

namespace AdoNetProfiler
{
    public abstract class AdoNetProfilerProviderFactory : DbProviderFactory
    {
    }

    public class AdoNetProfilerProviderFactory<TProviderFactory> : AdoNetProfilerProviderFactory, IServiceProvider
        where TProviderFactory : DbProviderFactory
    {
        public static readonly AdoNetProfilerProviderFactory<TProviderFactory> Instance = new AdoNetProfilerProviderFactory<TProviderFactory>();

        public TProviderFactory WrappedProviderFactory { get; }

        public override bool CanCreateDataSourceEnumerator => WrappedProviderFactory.CanCreateDataSourceEnumerator;

        public AdoNetProfilerProviderFactory()
        {
            var field = typeof(TProviderFactory).GetField("Instance", BindingFlags.Public | BindingFlags.Static);

            if (field == null)
            {
                throw new NotSupportedException("Provider doesn't have Instance property.");
            }

            WrappedProviderFactory = (TProviderFactory)field.GetValue(null);
        }

        public override DbCommand CreateCommand()
        {
            var command    = WrappedProviderFactory.CreateCommand();
            var connection = (AdoNetProfilerDbConnection)WrappedProviderFactory.CreateConnection();
            var profiler   = connection.Profiler;

            return new AdoNetProfilerDbCommand(command, connection, profiler);
        }

        public override DbCommandBuilder CreateCommandBuilder()
        {
            return WrappedProviderFactory.CreateCommandBuilder();
        }

        public override DbConnection CreateConnection()
        {
            var connection = WrappedProviderFactory.CreateConnection();

            return new AdoNetProfilerDbConnection(connection);
        }

        public override DbConnectionStringBuilder CreateConnectionStringBuilder()
        {
            return WrappedProviderFactory.CreateConnectionStringBuilder();
        }

        public override DbDataAdapter CreateDataAdapter()
        {
            return WrappedProviderFactory.CreateDataAdapter();
        }

        public override DbDataSourceEnumerator CreateDataSourceEnumerator()
        {
            return WrappedProviderFactory.CreateDataSourceEnumerator();
        }

        public override DbParameter CreateParameter()
        {
            return WrappedProviderFactory.CreateParameter();
        }

        public override CodeAccessPermission CreatePermission(PermissionState state)
        {
            return WrappedProviderFactory.CreatePermission(state);
        }

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
