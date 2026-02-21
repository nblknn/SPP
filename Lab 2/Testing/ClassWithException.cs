namespace Lab_2.Testing {
    internal class ClassWithException {
        private bool wasCorrectConstructorCalled;
        public ClassWithException() {
            wasCorrectConstructorCalled = true;
        }

        public ClassWithException(string message) {
            wasCorrectConstructorCalled = false;
            throw new Exception(message);
        }

        public bool WasCorrectConstructorCalled() {
            return wasCorrectConstructorCalled;
        }
    }
}
