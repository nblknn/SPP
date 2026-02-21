namespace Lab_2.Generators {
    internal class StringGenerator : IValueGenerator {
        public bool CanGenerate(Type type) {
            return type == typeof(string);
        }

        public object Generate(Type typeToGenerate, GeneratorContext context) {
            int len = context.Random.Next(1, 100);
            string result = "";
            for (int i = 0; i < len; i++) {
                result += (char)context.Random.Next(char.MaxValue);
            }
            return result;
        }
    }
}
