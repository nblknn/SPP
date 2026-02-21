using System.Collections;

namespace Lab_2.Generators {
    internal class ListGenerator : IValueGenerator {
        public bool CanGenerate(Type type) {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
        }

        public object Generate(Type typeToGenerate, GeneratorContext context) {
            int len = context.Random.Next(1, 100);
            var elementType = typeToGenerate.GetGenericArguments().FirstOrDefault();
            var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
            for (int i = 0; i < len; i++) {
                list.Add(context.Faker.Create(elementType));
            }
            return list;
        }
    }
}
