using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SyncContext
{
    public static class AsyncExtensions
    {
        public static IDisposable AsDefault(this SynchronizationContext context)
        {
            var oldContext = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(context);

            return Disposable.Create(delegate
            {
                SynchronizationContext.SetSynchronizationContext(oldContext);
            });
        }
    }
}
