using System;

namespace SyncContext
{
    public class Disposable : IDisposable
    {
        public static IDisposable Create(Action onDispose)
        {
            return new Disposable(onDispose);
        }

        Action _onDispose;
        private Disposable(Action onDispose)
        {
            _onDispose = onDispose;
        }

        ~Disposable()
        {
            Dispose();
        }

        bool _disposed;
        public void Dispose()
        {
            lock (this)
            {
                if (!_disposed)
                {
                    _disposed = true;
                    _onDispose();
                }
            }
        }
    }
}
