using Core;
using Xunit;

namespace Tests {
    public class DirectoryScannerTest {

        [Fact]
        public void CheckDirWithNoSubdirs() {
            using (DirectoryCreator creator = new DirectoryCreator()) {
                creator.CreateFiles(creator.Path, 10);
                bool isCompleted = false;

                DirectoryScanner ds = new DirectoryScanner(8, creator.Path, () => { isCompleted = true; });
                ds.StartScan();
                while (!isCompleted)
                    Thread.Sleep(1000);

                Assert.NotNull(ds.Dir);
                Assert.NotEmpty(ds.Dir.Files);
                Assert.True(ds.Dir.Size > 0);
            }
        }

        [Fact]
        public void CheckDirWithSubdirs() {
            using (DirectoryCreator creator = new DirectoryCreator()) {
                creator.CreateDeepStructure(2, 10);
                bool isCompleted = false;

                DirectoryScanner ds = new DirectoryScanner(8, creator.Path, () => { isCompleted = true; });
                ds.StartScan();
                while (!isCompleted)
                    Thread.Sleep(1000);

                Assert.NotNull(ds.Dir);
                Assert.NotEmpty(ds.Dir.Files);
            }
        }

        [Fact]
        public void CheckCancel() {
            bool isCompleted = false;
            DirectoryScanner ds = new DirectoryScanner(8, "D:\\", () => { isCompleted = true; });

            ds.StartScan();
            Thread.Sleep(100);
            ds.CancelScan();
            while (!isCompleted)
                Thread.Sleep(1000);

            Assert.NotNull(ds.Dir);
            Assert.NotEmpty(ds.Dir.Files);
        }

        [Fact]
        public void CheckOneWorkingThread() {
            using (DirectoryCreator creator = new DirectoryCreator()) {
                creator.CreateDeepStructure(5, 2);
                bool isCompleted = false;

                DirectoryScanner ds = new DirectoryScanner(1, creator.Path, () => { isCompleted = true; });
                ds.StartScan();
                while (!isCompleted)
                    Thread.Sleep(1000);

                Assert.NotNull(ds.Dir);
            }
        }

        [Fact]
        public void CheckHundredThreads() {
            bool isCompleted = false;

            DirectoryScanner ds = new DirectoryScanner(100, "D:\\", () => { isCompleted = true; });
            ds.StartScan();
            while (!isCompleted)
                Thread.Sleep(1000);

            Assert.NotNull(ds.Dir);
            Assert.NotEmpty(ds.Dir.Files);
        }

        [Fact]
        public void CheckSymlinks() {
            using (DirectoryCreator creator = new DirectoryCreator()) {
                creator.CreateSymlink("D:\\");
                bool isCompleted = false;

                DirectoryScanner ds = new DirectoryScanner(8, creator.Path, () => { isCompleted = true; });
                ds.StartScan();
                while (!isCompleted)
                    Thread.Sleep(1000);

                Assert.NotNull(ds.Dir);
                Assert.Empty(ds.Dir.Files);
                Assert.True(ds.Dir.Size == 0);
            }
        }
    }
}