namespace Core {
    public class Directory: File {
        public List<File> Files = new();

        public Directory(string name) : base(name) {

        }

        public void ProcessFiles(object? state) {
            if (state is not ThreadParam param)
                throw new Exception();
            param.Semaphore.WaitOne();
            if (!param.CancellationToken.IsCancellationRequested)
                try {
                    foreach (string entry in System.IO.Directory.GetDirectories(Name)) {
                        if (param.CancellationToken.IsCancellationRequested)
                            break;
                        if (new DirectoryInfo(entry).LinkTarget != null)
                            continue;
                        var dir = new Directory(entry);
                        Files.Add(dir);
                        ThreadPool.QueueUserWorkItem(dir.ProcessFiles, state);
                        param.count++;
                    }

                    foreach (string entry in System.IO.Directory.GetFiles(Name)) {
                        if (param.CancellationToken.IsCancellationRequested)
                            break;
                        if (new FileInfo(entry).LinkTarget != null)
                            continue;
                        var file = new File(entry);
                        Files.Add(file);
                    }
                }
                catch (Exception e) {
                }

            Interlocked.Decrement(ref param.count);
            if (param.count == 0)
                lock (param.MonitorObj) {
                    Monitor.Pulse(param.MonitorObj);
                }
            param.Semaphore.Release();
        }

        public override void CalcSize() {
            foreach (var file in Files) {
                file.CalcSize();
                Size += file.Size;
            }
        }

        public void CalcPercent() {
            CalcPercent(Size);
        }

        public override void CalcPercent(long dirSize)  {
            base.CalcPercent(dirSize);
            foreach (var file in Files) {
                file.CalcPercent(Size);
            }
        }
    }
}
