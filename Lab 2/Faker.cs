using Lab_2.Generators;
using System.Reflection;

namespace Lab_2 {
    public class Faker {
        private GeneratorContext _context;
        private Dictionary<Type, object> _createdTypes;
        private List<IValueGenerator> _generators;
        private FakerConfig _config;

        public Faker() {
            _context = new GeneratorContext(new Random(DateTime.Now.Nanosecond), this);
            _createdTypes = new Dictionary<Type, object>();
            _generators = new List<IValueGenerator> { 
                new BoolGenerator(),
                new ByteGenerator(),
                new CharGenerator(),
                new DateTimeGenerator(),
                new DoubleGenerator(),
                new FloatGenerator(),
                new IntGenerator(),
                new ListGenerator(),
                new LongGenerator(),
                new ShortGenerator(),
                new StringGenerator(),
            };
            _config = new FakerConfig();
        }

        public Faker(FakerConfig config): this() {
            _config = config;
        }

        public T Create<T>() {
            return (T)Create(typeof(T));
        }

        public object Create(Type t) {
            foreach (var generator in _generators) {
                if (generator.CanGenerate(t))
                    return generator.Generate(t, _context);
            }
            return CreateClass(t);
        }

        private object CreateClass(Type t) {
            var constructors = t.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);
            Array.Sort(constructors, (a, b) => b.GetParameters().Length.CompareTo(a.GetParameters().Length));
            if (!_createdTypes.TryAdd(t, 0))
                return _createdTypes[t];
            object result = null;
            foreach (var constructor in constructors) {
                try {
                    var paramInfo = constructor.GetParameters();
                    object[] parameters = new object[paramInfo.Length];
                    for (int i = 0; i < paramInfo.Length; i++) {
                        parameters[i] = GetParameterValue(t, paramInfo[i].ParameterType, paramInfo[i].Name);
                    }
                    result = Activator.CreateInstance(t, parameters);
                }
                catch {
                    continue;
                }
                break;
            }
            if (constructors.Length == 0)
                result = Activator.CreateInstance(t, true);
            _createdTypes[t] = result;
            SetDefaultProperties(result, t);
            return result;
        }

        private void SetDefaultProperties(object result, Type t) {
            var fields = t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static | BindingFlags.SetField);
            foreach (var field in fields) {
                if (Equals(field.GetValue(result), GetDefaultValue(field.FieldType)))
                    field.SetValue(result, GetParameterValue(t, field.FieldType, field.Name));
            }
            var properties = t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static | BindingFlags.SetProperty);
            foreach (var property in properties) {
                if (Equals(property.GetValue(result), GetDefaultValue(property.PropertyType)))
                    property.SetValue(result, GetParameterValue(t, property.PropertyType, property.Name));
            }
        }

        private object GetParameterValue(Type t, Type paramType, string paramName) {
            var generator = _config.GetGenerator(t, paramName);
            if (generator != null)
                return generator.Generate(paramType, _context);
            else
                return Create(paramType);
        }

        private static object? GetDefaultValue(Type t) {
            if (t.IsValueType)
                // Для типов-значений вызов конструктора по умолчанию даст default(T).
                return Activator.CreateInstance(t);
            else
                // Для ссылочных типов значение по умолчанию всегда null.
                return null;
        }
    }
}
