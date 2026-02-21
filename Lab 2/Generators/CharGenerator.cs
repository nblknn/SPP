
namespace Lab_2.Generators {
    internal class CharGenerator : IValueGenerator {
        public bool CanGenerate(Type type) {
            return type == typeof(char);
        }

        public object Generate(Type typeToGenerate, GeneratorContext context) {
            return (char)context.Random.Next(char.MaxValue);
        }
    }
}
