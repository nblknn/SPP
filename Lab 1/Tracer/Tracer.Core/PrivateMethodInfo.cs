using System.Diagnostics;

namespace Tracer.Core {
    internal class PrivateMethodInfo {
        public string Name { get; set; }
        public string Class { get; set; }
        public Stopwatch Stopwatch { get; set; }
        public List<PrivateMethodInfo> Children { get; set; }
        public PrivateMethodInfo? Parent { get; set; }
        public PrivateMethodInfo(string name, string className, PrivateMethodInfo? parent) {
            Name = name;
            Class = className;
            Stopwatch = new Stopwatch();
            Children = new List<PrivateMethodInfo>();
            Parent = parent;
        }
        public MethodInfo ToMethodInfo() {
            List<MethodInfo> methods = new();
            foreach (PrivateMethodInfo child in Children)
                methods.Add(child.ToMethodInfo());
            return new MethodInfo(Name, Class, Stopwatch.ElapsedMilliseconds, methods);
        }
    }
}
