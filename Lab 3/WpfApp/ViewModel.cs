using Core;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace WpfApp {
    class ViewModel : INotifyPropertyChanged {
        private int _threadAmount = 1;
        private string _dir = "";
        private DirectoryScanner _ds;

        public int ThreadAmount {
            get => _threadAmount;
            set { _threadAmount = value; OnPropertyChanged(nameof(ThreadAmount)); }
        }

        public string Dir {
            get => _dir;
            set { _dir = value; OnPropertyChanged(nameof(Dir)); }
        }

        public ObservableCollection<FileItem> Files { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        public RelayCommand GetDir { get; set; }
        public RelayCommand StartScan { get; set; }
        public RelayCommand CancelScan { get; set; }

        public ViewModel() {
            GetDir = new RelayCommand(ExecuteGetDir, (obj) => true);
            StartScan = new RelayCommand(ExecuteStartScan, (obj) => true);
            CancelScan = new RelayCommand(ExecuteCancelScan, (obj) => true);
            Files = new();
        }

        public void OnPropertyChanged([CallerMemberName] string property = "") {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        public void ExecuteGetDir(object param) {
            OpenFolderDialog ofd = new OpenFolderDialog();
            if (ofd.ShowDialog() == true) {
                Dir = ofd.FolderName;
            }
        }

        public void ExecuteStartScan(object param) {
            _ds = new(_threadAmount, _dir, OnStopScan);
            _ds.StartScan();
        }

        private void OnStopScan() {
            Application.Current.Dispatcher.Invoke(() => {
                Files.Clear();
                Files.Add(new FileItem(_ds.Dir));
            });
        }

        public void ExecuteCancelScan(object param) {
            _ds.CancelScan();
        }
    }
}
