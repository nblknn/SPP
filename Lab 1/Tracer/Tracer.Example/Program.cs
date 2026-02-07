using Tracer.Core;
using Tracer.Serialization;

namespace Tracer.Example {
    public class C {
        private ITracer _tracer;

        public C(ITracer tracer) {
            _tracer = tracer;
        }

        public void M0() {
            M1();
            M2();
        }

        private void M1() {
            _tracer.StartTrace();
            Thread.Sleep(100);
            M2();
            _tracer.StopTrace();
        }

        private void M2() {
            _tracer.StartTrace();
            Thread.Sleep(200);
            _tracer.StopTrace();
        }
    }
    internal class Program {
        static void ThreadProc(object? data) {
            C test = new C(data as ITracer);
            test.M0();
        }

        static void Main(string[] args) {
            ITracer tracer = new Tracer.Core.Tracer();
            Thread thread = new Thread(ThreadProc);
            thread.Start(tracer);
            ThreadProc(tracer);
            thread.Join();
            TraceResult res = tracer.GetTraceResult();
            ResultSaver rs = new ResultSaver();
            rs.SaveResult(res);
        }
    }
}
