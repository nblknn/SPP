namespace Lab_2.Generators {
    internal class FloatGenerator : IValueGenerator {
        public bool CanGenerate(Type type) {
            return type == typeof(float);
        }

        public object Generate(Type typeToGenerate, GeneratorContext context) {
            return (float)(-1000f + context.Random.NextDouble() * 2000f);
        }
    }
}
