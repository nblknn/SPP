namespace Lab_2.Testing {
    internal struct StructWithParametrizedConstructor {
        private bool X = false;

        public StructWithParametrizedConstructor(int x) {
            X = true;
        }

        public bool WasParametrizedConstructorCalled() {
            return X;
        }
    }
}
