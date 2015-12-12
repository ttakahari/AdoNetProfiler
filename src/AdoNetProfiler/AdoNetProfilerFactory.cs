using System;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace AdoNetProfiler
{
    /// <summary>
    /// The factory to create the object of <see cref="IAdoNetProfiler"/>.
    /// </summary>
    public class AdoNetProfilerFactory
    {
        // ConstructorInfo is faster than Func<IProfiler> when invoking.
        private static ConstructorInfo _constructor;

        private static bool _initialized = false;
        private static readonly ReaderWriterLockSlim _readerWriterLockSlim = new ReaderWriterLockSlim();

        /// <summary>
        /// Initialize the setting for profiling of database accessing with ADO.NET.
        /// </summary>
        /// <param name="profilerType">The type to implement <see cref="IAdoNetProfiler"/>.</param>
        public static void Initialize(Type profilerType)
        {
            if (profilerType == null)
            {
                throw new ArgumentNullException(nameof(profilerType));
            }

            if (profilerType.GetInterfaces().All(x => x != typeof (IAdoNetProfiler)))
            {
                throw new ArgumentException($"The type must be {typeof(IAdoNetProfiler).FullName}.", nameof(profilerType));
            }

            _readerWriterLockSlim.ExecuteWithReadLock(() =>
            {
                if (_initialized)
                {
                    throw new InvalidOperationException("This factory class has already initialized.");
                }

                // Overwrite DbProviderFactories.
                Utility.InitialzeDbProviderFactory();

                var constructor = profilerType.GetConstructor(Type.EmptyTypes);

                if (constructor == null)
                {
                    throw new InvalidOperationException("There is no default constructor. The profiler must have it.");
                }

                _constructor = constructor;

                _initialized = true;
            });
        }

        internal static IAdoNetProfiler GetProfiler()
        {
            return _readerWriterLockSlim.ExecuteWithWriteLock(() =>
            {
                if (!_initialized)
                {
                    throw new InvalidOperationException("This factory class has not initialized yet.");
                }

                return (IAdoNetProfiler)_constructor.Invoke(null);
            });
        }
    }
}