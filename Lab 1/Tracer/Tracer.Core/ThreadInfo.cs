namespace Tracer.Core {
    public class ThreadInfo {
        public int Id { get; }
        public long Time { get; }
        public IReadOnlyList<MethodInfo> Methods { get; }
        public ThreadInfo(int id, long time, IReadOnlyList<MethodInfo> methods) {
            Id = id;
            Time = time;
            Methods = methods;
        }
    }
}
