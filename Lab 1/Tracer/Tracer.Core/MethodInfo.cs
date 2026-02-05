namespace Tracer.Core {
    public class MethodInfo {
        public string Name { get; }
        public string Class { get; }
        public long Time { get; }
        public IReadOnlyList<MethodInfo> Methods { get; }
        public MethodInfo(string name, string className, long time, IReadOnlyList<MethodInfo> methods) {
            Name = name;
            Class = className;
            Time = time;
            Methods = methods;
        }
    }
}
