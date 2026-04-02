namespace Core {
    public class DirectoryScanner {
        private Semaphore _semaphore;
        private CancellationTokenSource _source;
        private Directory _dir;
        private Thread _thread;
        private Action _onStopScan;

        public Directory Dir { get { return _dir; } }

        public DirectoryScanner(int poolSize, string dirPath, Action onStopScan) {
            _semaphore = new Semaphore(poolSize, poolSize);
            _dir = new(dirPath);
            _source = new CancellationTokenSource();
            _thread = new Thread(Scan) {
                IsBackground = true,
            };
            _onStopScan = onStopScan;
        }

        private void Scan() {
            ThreadParam param = new ThreadParam(_semaphore, new object(), _source.Token);
            param.Count = 1;
            ThreadPool.QueueUserWorkItem(_dir.ProcessFiles, param);
            lock (param.MonitorObj) {
                Monitor.Wait(param.MonitorObj);
            }
            _dir.CalcSize();
            _dir.CalcPercent();
            _onStopScan();
        }

        public void StartScan() {
            _thread.Start();
        }

        public void CancelScan() {
            _source.Cancel();
        }
    }
}
