using System.Collections.Generic;
using System.Threading;

namespace iConnect.Common
{
    public static class ThreadManager
    {
        private static readonly Dictionary<int, ThreadInfo> Threads = new Dictionary<int, ThreadInfo>();

        private static void ManageThread(Thread thread, Thread parentThread)
        {
            int id = thread.ManagedThreadId;
            ThreadInfo t;
            if (Threads.TryGetValue(id, out t))
            {
                Threads.Remove(id);
            }
            int parentThreadId = 0;
            if (parentThread != null)
            {
                parentThreadId = parentThread.ManagedThreadId;
            }
            Threads.Add(id, new ThreadInfo(id, thread, parentThreadId, parentThread));
        }

        public static Thread CreateThread(ThreadStart threadStart)
        {
            var t = new Thread(threadStart);
            ManageThread(t, Thread.CurrentThread);
            return t;
        }

        public static Thread GetParentThread(Thread thread)
        {
            if(thread == null)
            {
                return null;
            }
            int threadId = thread.ManagedThreadId;
            ThreadInfo threadInfo;
            if (!GetThreadInfo(threadId, out threadInfo))
            {
                return null;
            }
            return threadInfo.ParentThread;
        }

        private static bool GetThreadInfo(int threadId, out ThreadInfo threadInfo)
        {
            int id = threadId;
            if (!Threads.TryGetValue(id, out threadInfo))
            {
                return false;
            }

            return true;
        }

        private static bool GetThreadContext(int threadId, out ThreadContext threadContext)
        {
            threadContext = null;
            ThreadInfo threadInfo;
            if (!GetThreadInfo(threadId, out threadInfo))
            {
                return false;
            }
            threadContext = threadInfo.ThreadContext;
            return true;
        }

        public static ThreadContext GetThreadContext(Thread thread)
        {
            if(thread == null)
            {
                return null;
            }
            int threadId = thread.ManagedThreadId;
            ThreadContext context;
            if (!GetThreadContext(threadId, out context))
            {
                ManageThread(thread, null);

                if (!GetThreadContext(threadId, out context))
                {
                    return null;
                }
            }

            return context;
        }

        private class ThreadInfo
        {
            private int _threadId;

            private Thread _thread;

            private int _parentThreadId;

            public Thread ParentThread { get; private set; }

            public ThreadContext ThreadContext { get; private set; }

            internal ThreadInfo(int threadId, Thread thread, int parentThreadId, Thread parentThread)
            {
                _threadId = threadId;
                _thread = thread;
                _parentThreadId = parentThreadId;
                ParentThread = parentThread;
                ThreadContext = new ThreadContext();
            }
        }
    }

    public class ThreadContext
    {
        private readonly Dictionary<object, object> _properties;
        public ThreadContext()
        {
            _properties = new Dictionary<object, object>();
        }

        public Dictionary<object, object> Properties { get { return _properties; }}
    }
}
