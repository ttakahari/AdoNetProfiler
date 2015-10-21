using System;
using System.Reflection;

namespace AdoNetProfiler
{
    /// <summary>
    /// The factory to create the object of <see cref="IProfiler"/>.
    /// </summary>
    public class AdoNetProfilerFactory
    {
        // ConstructorInfo is faster than Func<IProfiler> when invoking.
        private static ConstructorInfo _constructor;

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

            var constructor = profilerType.GetConstructor(Type.EmptyTypes);

            if (constructor == null)
                throw new InvalidOperationException("There is no default constructor. The profiler must have it.");

            _constructor = constructor;
        }

        public static IProfiler GetProfiler()
        {
            return (IProfiler)_constructor.Invoke(null);
        }
    }
}