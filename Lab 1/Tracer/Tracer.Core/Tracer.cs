using System.Diagnostics;
using System.Reflection;

namespace Tracer.Core {
    public class Tracer : ITracer {
        // int - thread id
        private Dictionary<int, List<PrivateMethodInfo>> _threads;
        private Dictionary<int, PrivateMethodInfo?> _currentMethods;

        public Tracer() {
            _threads = new();
            _currentMethods = new();
        }

        public void StartTrace() {
            int tid = Thread.CurrentThread.ManagedThreadId;
            _threads.TryAdd(tid, new());
            _currentMethods.TryAdd(tid, null);
            StackTrace st = new StackTrace();
            MethodBase mb = st.GetFrame(1).GetMethod();
            PrivateMethodInfo method = new PrivateMethodInfo(mb.Name, mb.DeclaringType.Name, _currentMethods[tid]);
            if (_currentMethods[tid] == null)
                _threads[tid].Add(method);
            _currentMethods[tid] = method;
            method.Stopwatch.Start();
        }

        public void StopTrace() {
            int tid = Thread.CurrentThread.ManagedThreadId;
            _currentMethods[tid].Stopwatch.Stop();
            _currentMethods[tid] = _currentMethods[tid].Parent;
        }

        public TraceResult GetTraceResult() {
            List<ThreadInfo> threads = new();
            foreach (int tid in _threads.Keys) {
                List<MethodInfo> methods = new();
                long time = 0;
                foreach (PrivateMethodInfo pmi in _threads[tid]) {
                    MethodInfo mi = pmi.ToMethodInfo();
                    methods.Add(mi);
                    time += mi.Time;
                }
                threads.Add(new ThreadInfo(tid, time, methods));
            }
            return new TraceResult(threads);
        }
    }
}
