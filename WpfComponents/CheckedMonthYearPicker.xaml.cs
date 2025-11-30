using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace WpfComponents
{
    public partial class CheckedMonthYearPicker : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register(nameof(IsChecked), typeof(bool), typeof(CheckedMonthYearPicker), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        public static readonly DependencyProperty SelectedDateProperty =
            DependencyProperty.Register(nameof(SelectedDate), typeof(DateTime), typeof(CheckedMonthYearPicker), new FrameworkPropertyMetadata(DateTime.Now, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        
        private ObservableCollection<string> _monthNames;

        public CheckedMonthYearPicker()
        {
            InitializeComponent();
            MonthNames = new ObservableCollection<string>(DateTimeFormatInfo.CurrentInfo.MonthNames);
            // Remove the empty string at the end (13th month for calendars with 13 months)
            if (MonthNames.Count > 0 && string.IsNullOrEmpty(MonthNames[MonthNames.Count - 1]))
                MonthNames.RemoveAt(MonthNames.Count - 1);

            SelectedMonthIndex = -1; 
            Year = DateTime.Now.Year;
            DataContext = this;
        }

        /// <summary>
        /// Czy zaznaczony checkBox przy dacie
        /// </summary>
        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }



        public DateTime SelectedDate
        {
            get => (DateTime)GetValue(SelectedDateProperty);
            set => SetValue(SelectedDateProperty, value);
        }

        public ObservableCollection<string> MonthNames
        {
            get
            {
                return _monthNames;
            }
            set
            {
                _monthNames = value;
                NotifyPropertyChanged(nameof(MonthNames));
            }
        }

        private int _year;
        public int Year
        {
            get => _year;
            set
            {
                if (_year != value)
                {
                    _year = value;
                    NotifyPropertyChanged(nameof(Year));
                    UpdateSelectedDate();
                }
            }
        }

        private int _selectedMonthIndex;
        public int SelectedMonthIndex
        {
            get => _selectedMonthIndex;
            set
            {
                if (_selectedMonthIndex != value)
                {
                    _selectedMonthIndex = value;
                    NotifyPropertyChanged(nameof(SelectedMonthIndex));
                    UpdateSelectedDate();
                }
            }
        }

        private void UpdateSelectedDate()
        {
            if (Year > 0 && SelectedMonthIndex >= 0 && SelectedMonthIndex < 12)
            {
                SelectedDate = new DateTime(Year, SelectedMonthIndex + 1, 1);
                IsChecked = true;
            }
            else
            {
                IsChecked = false;
            }
        }

        private static readonly Regex _numericRegex = new Regex("^[0-9]+$");

        private void YearTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !_numericRegex.IsMatch(e.Text);
        }

        private void YearTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (!_numericRegex.IsMatch(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }
    }

   
}