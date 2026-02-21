using Xunit;

namespace Lab_2.Testing {
    public class FakerTest {
        [Fact]
        public void CheckValueGenerators() {
            Type[] types = {typeof(bool), typeof(byte), typeof(char), typeof(DateTime),
            typeof(double), typeof(float), typeof(int), typeof(long), typeof(short), typeof(string)};
            Faker faker = new Faker();

            foreach (Type type in types) {
                object obj = faker.Create(type);
                Assert.Equal(type, obj.GetType());
            }
        }

        [Fact]
        public void CheckListGenerator() {
            Faker faker = new Faker();
            var obj = faker.Create<List<int>>();

            Assert.NotNull(obj);
            Assert.NotEmpty(obj);
        }

        [Fact]
        public void CheckBasicClassGenerator() {
            Faker faker = new Faker();
            var obj = faker.Create<BasicTestClass>();

            Assert.NotNull(obj);
            Assert.False(Equals(obj.GetPrivateConstructorField(), GetDefaultValue(typeof(int))));
            Assert.False(Equals(obj.DefaultProperty, GetDefaultValue(typeof(double))));
            Assert.False(Equals(obj.defaultField, GetDefaultValue(typeof(DateTime))));
            Assert.False(Equals(obj.ReadOnlyProperty, GetDefaultValue(typeof(string))));
        }

        [Fact]
        public void CheckConstructorWithException() {
            Faker faker = new Faker();
            var obj = faker.Create<ClassWithException>();

            Assert.NotNull(obj);
            Assert.True(obj.WasCorrectConstructorCalled());
        }

        [Fact]
        public void CheckCircularDependency() {
            Faker faker = new Faker();
            var obj = faker.Create<CircularDependencyClass1>();

            Assert.NotNull(obj);
            Assert.NotNull(obj.A);
            Assert.NotNull(obj.A.B);
        }

        [Fact]
        public void CheckClassWithPrivateConstructor() {
            Faker faker = new Faker();
            var obj = faker.Create<ClassWithPrivateConstructor>();

            Assert.NotNull(obj);
        }

        [Fact]
        public void CheckStructWithParametrizedConstructor() {
            Faker faker = new Faker();
            var obj = faker.Create<StructWithParametrizedConstructor>();

            Assert.True(obj.WasParametrizedConstructorCalled());
        }

        [Fact]
        public void CheckCustomConfig() {
            FakerConfig config = new FakerConfig();
            config.Add<BasicTestClass, string, CustomGenerator>(test => test.ReadOnlyProperty);
            Faker faker = new Faker(config);
            var obj = faker.Create<BasicTestClass>();

            Assert.Equal("CustomValue", obj.ReadOnlyProperty);
        }

        private object? GetDefaultValue(Type t) {
            if (t.IsValueType)
                // Для типов-значений вызов конструктора по умолчанию даст default(T).
                return Activator.CreateInstance(t);
            else
                // Для ссылочных типов значение по умолчанию всегда null.
                return null;
        }
    }
}
