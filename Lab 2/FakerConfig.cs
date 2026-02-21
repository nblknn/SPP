using System.Linq.Expressions;

namespace Lab_2 {
    public class FakerConfig {
        private Dictionary<(Type TargetType, string PropertyName), IValueGenerator> _customGenerators = new();

        public void Add<T, TProperty, TGenerator>(Expression<Func<T, TProperty>> propertyExpression)
        where TGenerator : IValueGenerator, new() {
            var memberExpression = GetMemberExpression(propertyExpression);
            var propertyInfo = memberExpression.Member;
            var key = (typeof(T), propertyInfo.Name.ToLower());
            _customGenerators[key] = new TGenerator();
        }

        public IValueGenerator? GetGenerator(Type targetType, string propertyName) {
            return _customGenerators.GetValueOrDefault((targetType, propertyName.ToLower()));
        }

        private static MemberExpression GetMemberExpression<T, TProperty>(Expression<Func<T, TProperty>> expression) {
            var body = expression.Body;
            if (body is MemberExpression memberExpression)
                return memberExpression;
            throw new ArgumentException($"Выражение '{expression}' не является обращением к свойству");
        }
    }
}
