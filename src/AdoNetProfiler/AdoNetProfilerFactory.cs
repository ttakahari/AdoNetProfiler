using System;
using System.Reflection;
using System.Threading;

namespace AdoNetProfiler
{
    /// <summary>
    /// The factory to create the object of <see cref="IProfiler"/>.
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
        /// <param name="profilerType">The type to implement <see cref="IProfiler"/>.</param>
        public static void Initialize(Type profilerType)
        {
            if (profilerType == null)
                throw new ArgumentNullException(nameof(profilerType));

            if (profilerType != typeof(IProfiler))
                throw new ArgumentException($"The type must be {typeof(IProfiler).FullName}.", nameof(profilerType));

            _readerWriterLockSlim.ExecuteWithReadLock(() =>
            {
                if (_initialized)
                    throw new InvalidOperationException("This factory class has already initialized.");

                var constructor = profilerType.GetConstructor(Type.EmptyTypes);

                if (constructor == null)
                    throw new InvalidOperationException("There is no default constructor. The profiler must have it.");

                _constructor = constructor;

                _initialized = true;
            });
        }

        public static IProfiler GetProfiler()
        {
            return _readerWriterLockSlim.ExecuteWithWriteLock(() =>
            {
                if (!_initialized)
                    throw new InvalidOperationException("This factory class has not initialized yet.");

                return (IProfiler)_constructor.Invoke(null);
            });
        }
    }
}