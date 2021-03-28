using System;

namespace MVVMClasses.Models
{
    /// <summary>
    /// Klasa dla filtrów pól DateTime, zawiera wartość daty oraz czy filtr aktywny
    /// </summary>
    public class DateTimeFilter : ModelBase
    {
        private DateTime _selectedValue = DateTime.Now;
        private bool isChecked = false;

        /// <summary>
        /// Data wybrana w filtrze
        /// </summary>
        public DateTime SelectedValue
        {
            get => _selectedValue;
            set
            {
                if (_selectedValue != value)
                {
                    _selectedValue = value;
                    NotifyPropertyChanged(nameof(SelectedValue));
                    IsChecked = true;
                }
            }
        }

        /// <summary>
        /// Czy zaznaczono checkbox, czy filtr aktywny
        /// </summary>
        public bool IsChecked
        {
            get => isChecked; set
            {
                isChecked = value;
                NotifyPropertyChanged(nameof(IsChecked));
            }
        }
    }
}
