using MVVMClasses.Models;
using System;
using System.ComponentModel;

namespace WpfComponentsUITests
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private DateTimeFilter _selectedDate = new DateTimeFilter();
        public DateTimeFilter SelectedDate
        {
            get => _selectedDate;
            set
            {
                //Moze tu powinien byc warunek czy zmiana wartosci?
                _selectedDate = value;
                OnPropertyChanged(nameof(SelectedDate));
                
            }
        }

        //private bool _isChecked;
        //public bool IsChecked
        //{
        //    get => _isChecked;
        //    set
        //    {
        //        if (_isChecked != value)
        //        {
        //            _isChecked = value;                    
        //            OnPropertyChanged(nameof(IsChecked));
        //        }
        //    }
        //}

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}