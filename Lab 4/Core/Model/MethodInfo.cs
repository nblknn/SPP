namespace Core.Model {
    public class MethodInfo {
        public string Name { get; set; }
        public string ReturnType { get; set; }
        public List<ParameterInfo> Parameters { get; set; } = new List<ParameterInfo>();
        public bool IsVoid => ReturnType == "void";
    }
}
