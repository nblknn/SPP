namespace Tests {
    public class DirectoryCreator : IDisposable {
        public string Path { get; private set; }

        public DirectoryCreator() {
            Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "DirectoryScannerTest", Guid.NewGuid().ToString());
            Directory.CreateDirectory(Path);
        }

        public void Dispose() {
            if (Directory.Exists(Path)) {
                Directory.Delete(Path, true);
            }
        }

        public void CreateFiles(string dir, int amount, long fileSize = 1024) {
            for (int i = 0; i < amount; i++) {
                var filePath = System.IO.Path.Combine(dir, $"file_{i}.txt");
                using (var fs = new FileStream(filePath, FileMode.Create))
                    fs.SetLength(fileSize);
            }
        }

        public void CreateDeepStructure(int depth, int filesPerLevel, long fileSize = 1024) {
            CreateDeepStructureRecursive(Path, depth, filesPerLevel, fileSize, 0);
        }

        private void CreateDeepStructureRecursive(string currentPath, int depth, int filesPerLevel, long fileSize, int currentDepth) {
            if (currentDepth >= depth)
                return;
            CreateFiles(currentPath, filesPerLevel, fileSize);
            for (int i = 0; i < filesPerLevel; i++) {
                var dirPath = System.IO.Path.Combine(currentPath, $"dir_{currentDepth}_{i}");
                Directory.CreateDirectory(dirPath);
                CreateDeepStructureRecursive(dirPath, depth, filesPerLevel, fileSize, currentDepth + 1);
            }
        }

        public void CreateSymlink(string target) {
            File.CreateSymbolicLink(System.IO.Path.Combine(Path, "symlink"), target);
        }
    }
}