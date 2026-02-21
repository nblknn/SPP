namespace Lab_2.Testing {
    internal class CustomGenerator : IValueGenerator {
        public bool CanGenerate(Type type) {
            return true;
        }

        public object Generate(Type typeToGenerate, GeneratorContext context) {
            return "CustomValue";
        }
    }
}
