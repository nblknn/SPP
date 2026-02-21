namespace Lab_2.Generators {
    internal class DoubleGenerator : IValueGenerator {
        public bool CanGenerate(Type type) {
            return type == typeof(double);
        }

        public object Generate(Type typeToGenerate, GeneratorContext context) {
            return -1000d + context.Random.NextDouble() * 2000d;
        }
    }
}