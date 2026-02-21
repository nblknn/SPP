namespace Lab_2.Generators {
    internal class ByteGenerator : IValueGenerator {
        public bool CanGenerate(Type type) {
            return type == typeof(byte);
        }

        public object Generate(Type typeToGenerate, GeneratorContext context) {
            return (byte)context.Random.Next(byte.MinValue, byte.MaxValue);
        }
    }
}
