namespace Core.Model {
    public class ParseResult {
        public List<string> Usings { get; set; } = new List<string>();
        public List<ClassInfo> Classes { get; set; } = new List<ClassInfo>();
    }
}