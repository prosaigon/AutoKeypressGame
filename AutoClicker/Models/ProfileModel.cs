using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AutoClicker.Models
{
    public class ProfileModel : INotifyPropertyChanged
    {
        private string _name;
        private string _directoryPath;
        private DateTime _lastModified;

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public string DirectoryPath
        {
            get => _directoryPath;
            set { _directoryPath = value; OnPropertyChanged(); }
        }

        public DateTime LastModified
        {
            get => _lastModified;
            set { _lastModified = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
