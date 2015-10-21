using System;
using System.Threading;

namespace AdoNetProfiler
{
    public static class ReaderWriterLockSlimExtensions
    {
        public static void ExecuteWithReadLock(this ReaderWriterLockSlim readerWriterLockSlim, Action action)
        {
            readerWriterLockSlim.EnterReadLock();

            action();

            readerWriterLockSlim.ExitReadLock();
        }

        public static T ExecuteWithReadLock<T>(this ReaderWriterLockSlim readerWriterLockSlim, Func<T> action)
        {
            readerWriterLockSlim.EnterReadLock();

            var result = action();

            readerWriterLockSlim.ExitReadLock();

            return result;
        }

        public static void ExecuteWithWriteLock(this ReaderWriterLockSlim readerWriterLockSlim, Action action)
        {
            readerWriterLockSlim.EnterWriteLock();

            action();

            readerWriterLockSlim.ExitWriteLock();
        }

        public static T ExecuteWithWriteLock<T>(this ReaderWriterLockSlim readerWriterLockSlim, Func<T> action)
        {
            readerWriterLockSlim.EnterWriteLock();

            var result = action();

            readerWriterLockSlim.ExitWriteLock();

            return result;
        }
    }
}