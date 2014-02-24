using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace SyncContext
{
    public class SyncContext : SynchronizationContext
    {
        ConcurrentQueue<Tuple<SendOrPostCallback, object>> _queue;
        public readonly string Name;

        public SyncContext(string name)
        {
            _queue = new ConcurrentQueue<Tuple<SendOrPostCallback, object>>();
            Name = name;
        }


        public void Post(Action action)
        {
            _queue.Enqueue(new Tuple<SendOrPostCallback, object>(s => action(), null));
        }

        public override void Post(SendOrPostCallback d, object state)
        {
            _queue.Enqueue(new Tuple<SendOrPostCallback, object>(d, state));
        }

        public void RunPending()
        {
            Tuple<SendOrPostCallback, object> next;
            if (_queue.TryDequeue(out next))
            {
                //MainClass.Log("Starting {0} Context Execution", Name);

                do
                {
                    // Restoring the Synchronization.Current before continuation
                    using (this.AsDefault())
                        next.Item1(next.Item2);
                } while (_queue.TryDequeue(out next));

                //MainClass.Log("Completed {0} Context Execution", Name);
            }
        }

        public Task Run(Func<Task> asyncMethod)
        {
            Task t;
            using (this.AsDefault())
                t = asyncMethod();
            return t;
        }
    }
}
