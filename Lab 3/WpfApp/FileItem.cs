using Core;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfApp {
    class FileItem: INotifyPropertyChanged {
        private string _name;
        private long _size;
        private double _percent;
        private string _icon;

        public string Name {
            get => _name;
            set { _name = value; OnPropertyChanged(nameof(Name)); }
        }

        public long Size {
            get => _size;
            set { _size = value; OnPropertyChanged(nameof(Size)); }
        }

        public double Percent {
            get => _percent;
            set { _percent = value; OnPropertyChanged(nameof(Percent)); }
        }

        public string Icon {
            get => _icon;
            set { _icon = value; OnPropertyChanged(nameof(Icon)); }
        }

        public ObservableCollection<FileItem> Nodes { get; set; }

        public FileItem(File file) {
            _name = file.Name.Substring(file.Name.LastIndexOf('\\') + 1);
            _size = file.Size;
            _percent = file.Percent;
            Nodes = new ObservableCollection<FileItem>();
            if (file is Directory dir) {
                foreach (File entry in dir.Files) {
                    Nodes.Add(new FileItem(entry));
                }
                _icon = "E:\\SPP\\Lab 3\\WpfApp\\FolderIcon.png";
            }
            else
                _icon = "E:\\SPP\\Lab 3\\WpfApp\\FileIcon.png";
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string property = "") {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
    }
}
