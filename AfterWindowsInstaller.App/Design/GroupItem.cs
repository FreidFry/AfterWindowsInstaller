using AfterWindowsInstaller.infrastructure.Persistance.Models;

using System.Collections.ObjectModel;
using System.ComponentModel;

namespace AfterWindowsInstaller.App.Design
{
    public class GroupItem : INotifyPropertyChanged
    {
        public string Key { get; set; }
        public ObservableCollection<KeyValuePair<string, DownloadUrlModel>> Items { get; set; }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    OnPropertyChanged(nameof(IsExpanded));
                    if (_isExpanded)
                        Expanded?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler Expanded;
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
