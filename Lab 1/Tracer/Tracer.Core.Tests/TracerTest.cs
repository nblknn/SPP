using System.Xml.Linq;
using Xunit;

namespace Tracer.Core.Tests {
    public class TracerTest {
        private const int _time1 = 100;
        private const int _time2 = 200;
        private void Method1(ITracer tracer) {
            tracer.StartTrace();
            Thread.Sleep(_time1);
            tracer.StopTrace();
        }

        private void Method2(ITracer tracer) {
            tracer.StartTrace();
            Thread.Sleep(_time2);
            tracer.StopTrace();
        }

        [Fact]
        public void CheckOneMethod() {
            ITracer tracer = new Tracer();
            Method1(tracer);
            TraceResult result = tracer.GetTraceResult();

            Assert.Single(result.Threads);
            Assert.Single(result.Threads[0].Methods);
            MethodInfo method = result.Threads[0].Methods[0];
            Assert.Equal(nameof(Method1), method.Name);
            Assert.Equal(nameof(TracerTest), method.Class);
            Assert.True(method.Time >= _time1);
            Assert.Empty(method.Methods);
            Assert.Equal(result.Threads[0].Time, method.Time);
        }

        [Fact]
        public void CheckTwoNotNestedMethods() {
            ITracer tracer = new Tracer();
            Method1(tracer);
            Method2(tracer);
            TraceResult result = tracer.GetTraceResult();

            Assert.Single(result.Threads);
            ThreadInfo thread = result.Threads[0];
            Assert.Equal(2, thread.Methods.Count);
            Assert.Equal(thread.Time, thread.Methods[0].Time + thread.Methods[1].Time);
        }

        [Fact]
        public void CheckNestedMethods() {
            ITracer tracer = new Tracer();
            tracer.StartTrace();
            Method1(tracer);
            Method2(tracer);
            tracer.StopTrace();
            TraceResult result = tracer.GetTraceResult();

            Assert.Single(result.Threads);
            Assert.Single(result.Threads[0].Methods);
            MethodInfo method = result.Threads[0].Methods[0];
            Assert.True(method.Time >= _time1 + _time2);
            Assert.Equal(2, method.Methods.Count);
        }

        [Fact]
        public void CheckTwoThreads() {
            ITracer tracer = new Tracer();
            Thread thread = new Thread(() => {
                Method1(tracer);
            });
            thread.Start();
            Method2(tracer);
            thread.Join();
            TraceResult result = tracer.GetTraceResult();

            Assert.Equal(2, result.Threads.Count);
        }
    }
}
