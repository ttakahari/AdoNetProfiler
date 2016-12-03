using System;
using System.Threading;

namespace AdoNetProfiler
{
    internal static class ReaderWriterLockSlimExtensions
    {
        internal static void ExecuteWithReadLock(this ReaderWriterLockSlim readerWriterLockSlim, Action action)
        {
            readerWriterLockSlim.EnterReadLock();

            try
            {
                action();
            }
            finally
            {
                readerWriterLockSlim.ExitReadLock();
            }
        }

        internal static T ExecuteWithReadLock<T>(this ReaderWriterLockSlim readerWriterLockSlim, Func<T> action)
        {
            readerWriterLockSlim.EnterReadLock();

            try
            {
                return action();
            }
            finally
            {
                readerWriterLockSlim.ExitReadLock();
            }
        }

        internal static void ExecuteWithWriteLock(this ReaderWriterLockSlim readerWriterLockSlim, Action action)
        {
            readerWriterLockSlim.EnterWriteLock();

            try
            {
                action();
            }
            finally
            {
                readerWriterLockSlim.ExitWriteLock();
            }
        }

        internal static T ExecuteWithWriteLock<T>(this ReaderWriterLockSlim readerWriterLockSlim, Func<T> action)
        {
            readerWriterLockSlim.EnterWriteLock();

            try
            {
                return action();
            }
            finally
            {
                readerWriterLockSlim.ExitWriteLock();
            }
        }
    }
}
