using System.Diagnostics;
using System.Reflection;

namespace Tracer.Core {
    public class Tracer : ITracer {
        // int - thread id
        private Dictionary<int, PrivateThreadInfo> _threads;

        public Tracer() {
            _threads = new();
        }

        public void StartTrace() {
            int tid = Thread.CurrentThread.ManagedThreadId;
            lock (_threads) {
                _threads.TryAdd(tid, new PrivateThreadInfo());
            }
            StackTrace st = new StackTrace();
            MethodBase mb = st.GetFrame(1).GetMethod();
            PrivateMethodInfo method = new PrivateMethodInfo(mb.Name, mb.DeclaringType.Name, _threads[tid].CurrentMethod);
            if (_threads[tid].CurrentMethod == null)
                _threads[tid].Methods.Add(method);
            else
                _threads[tid].CurrentMethod.Children.Add(method);
            _threads[tid].CurrentMethod = method;
            method.Stopwatch.Start();
        }

        public void StopTrace() {
            int tid = Thread.CurrentThread.ManagedThreadId;
            _threads[tid].CurrentMethod.Stopwatch.Stop();
            _threads[tid].CurrentMethod = _threads[tid].CurrentMethod.Parent;
        }

        public TraceResult GetTraceResult() {
            List<ThreadInfo> threads = new();
            foreach (int tid in _threads.Keys) {
                List<MethodInfo> methods = new();
                long time = 0;
                foreach (PrivateMethodInfo pmi in _threads[tid].Methods) {
                    MethodInfo mi = pmi.ToMethodInfo();
                    methods.Add(mi);
                    time += mi.Time;
                }
                threads.Add(new ThreadInfo(tid, time, methods.AsReadOnly()));
            }
            return new TraceResult(threads.AsReadOnly());
        }
    }
}
