using System;
using System.Threading;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SyncContext
{
    class MainClass
    {
        static int MainThreadId;
        static SyncContext _methodBContext, _baseContext;

        public static void Main(string[] args)
        {
            MainThreadId = Thread.CurrentThread.ManagedThreadId;

            _baseContext = new SyncContext("BaseContext");
            _methodBContext = new SyncContext("MethodBContext");

            var baseTask = _baseContext.Run(BaseMethod);

            while (!baseTask.IsCompleted)
            {
                _baseContext.RunPending();
                _methodBContext.RunPending();
                Thread.Sleep(1000);
            }
            Console.WriteLine();
            Console.WriteLine("Execution completed");
            Console.ReadLine();
        }

        static async Task BaseMethod()
        {
            Log("Base Start");
            var t = _methodBContext.Run(MethodB);
            Log("Returned to Base, about to await");
            await t;
            Log("Base about to return");
        }

        async static Task MethodB()
        {
            Log("MethodB Start, about to await Delay");
            await Task.Delay(333);
            Log("MethodB Continuation, about to await Delay again");
            await Task.Delay(333);
            Log("MethodB about to return");
        }

        #region Helpers
        static string GetCurrentContextName()
        {
            var syncContext = SynchronizationContext.Current as SyncContext;
            if (syncContext != null)
                return syncContext.Name;

            if (SynchronizationContext.Current == null)
                return "Default";

            return "Unknown";
        }

        static string GetThreadName()
        {
            if (Thread.CurrentThread.ManagedThreadId == MainThreadId)
                return "MainThread";
            if (Thread.CurrentThread.IsThreadPoolThread)
                return "ThreadPool";
            return "Thread" + Thread.CurrentThread.ManagedThreadId.ToString();
        }

        public static void Log(string format, params object[] args)
        {
            string context = GetCurrentContextName();
            string thread = GetThreadName();
            string message = string.Format(format, args);
            Console.WriteLine("[{2}] {1} - {0}", message, context, thread);
        }
        #endregion
    }
}
