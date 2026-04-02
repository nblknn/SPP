namespace Core {
    internal class ThreadParam {
        public Semaphore Semaphore { get; }
        public object MonitorObj { get; }
        public CancellationToken CancellationToken { get; }
        public volatile int Count;

        public ThreadParam(Semaphore semaphore, object monitorObj, CancellationToken token) {
            Semaphore = semaphore;
            MonitorObj = monitorObj;
            CancellationToken = token;
            Count = 0;
        }
    }
}
