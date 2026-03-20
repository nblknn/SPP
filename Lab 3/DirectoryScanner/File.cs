namespace Core {
    public class File {
        public string Name { get; set; }
        public double Percent {  get; set; }
        public long Size { get; set; }

        public File(string name) {
            Name = name;
        }

        public virtual void CalcSize() {
            Size = new FileInfo(Name).Length;
        }

        public virtual void CalcPercent(long dirSize) {
            Percent = (Size * 1.0) / dirSize * 100;
        }
    }
}