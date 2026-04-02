namespace Core.Model {
    public class ClassInfo {
        public string Namespace { get; set; }
        public string Name { get; set; }
        public List<MethodInfo> PublicMethods { get; set; } = new List<MethodInfo>();
        public List<ConstructorParameterInfo> ConstructorParameters { get; set; } = new List<ConstructorParameterInfo>();
        public bool HasInterfaceDependencies { get; set; }
    }
}