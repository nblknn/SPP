namespace Tracer.Core {
    internal class PrivateThreadInfo {
        public List<PrivateMethodInfo> Methods;
        public PrivateMethodInfo? CurrentMethod;
        public PrivateThreadInfo() {
            Methods = new List<PrivateMethodInfo>();
            CurrentMethod = null;
        }
    }
}
