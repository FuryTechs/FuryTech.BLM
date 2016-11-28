using System;
using System.Collections.Generic;
using System.Threading;

namespace BLM
{
    public class ElevatedContext : IDisposable
    {
        private static List<int> _elevatedThreads = new List<int>();


        public static bool IsElevated()
        {
            return _elevatedThreads.Contains(Thread.CurrentThread.ManagedThreadId);
        }

        public ElevatedContext()
        {
            _elevatedThreads.Add(Thread.CurrentThread.ManagedThreadId);
            
        }

        public void Dispose()
        {
            _elevatedThreads.Remove(Thread.CurrentThread.ManagedThreadId);
        }
    }
}
