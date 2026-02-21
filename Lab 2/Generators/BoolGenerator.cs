namespace Lab_2.Generators {
    internal class BoolGenerator : IValueGenerator {
        public bool CanGenerate(Type type) {
            return type == typeof(bool);
        }

        public object Generate(Type typeToGenerate, GeneratorContext context) {
            return context.Random.Next(2) != 0;
        }
    }
}
