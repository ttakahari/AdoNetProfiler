using System;
using System.Threading;

namespace AdoNetProfiler
{
    internal static class ReaderWriterLockSlimExtensions
    {
        internal static void ExecuteWithReadLock(this ReaderWriterLockSlim readerWriterLockSlim, Action action)
        {
            readerWriterLockSlim.EnterReadLock();

            action();

            readerWriterLockSlim.ExitReadLock();
        }

        internal static T ExecuteWithReadLock<T>(this ReaderWriterLockSlim readerWriterLockSlim, Func<T> action)
        {
            readerWriterLockSlim.EnterReadLock();

            var result = action();

            readerWriterLockSlim.ExitReadLock();

            return result;
        }

        internal static void ExecuteWithWriteLock(this ReaderWriterLockSlim readerWriterLockSlim, Action action)
        {
            readerWriterLockSlim.EnterWriteLock();

            action();

            readerWriterLockSlim.ExitWriteLock();
        }

        internal static T ExecuteWithWriteLock<T>(this ReaderWriterLockSlim readerWriterLockSlim, Func<T> action)
        {
            readerWriterLockSlim.EnterWriteLock();

            var result = action();

            readerWriterLockSlim.ExitWriteLock();

            return result;
        }
    }
}