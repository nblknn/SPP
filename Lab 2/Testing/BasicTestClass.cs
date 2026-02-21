namespace Lab_2.Testing {
    internal class BasicTestClass {
        private int privateConstructorField;
        public double DefaultProperty { get; set; }
        public DateTime defaultField;
        public string ReadOnlyProperty { get; }

        public BasicTestClass() {

        }

        public BasicTestClass(int privateConstructorField, string readOnlyProperty) {
            this.privateConstructorField = privateConstructorField;
            ReadOnlyProperty = readOnlyProperty;
        }

        public int GetPrivateConstructorField() {
            return privateConstructorField;
        }
    }
}
