using System;
using System.Collections.Generic;
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
    public partial class CheckedMonthYearPicker : UserControl
    {        
     
    
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register(nameof(IsChecked), typeof(bool), typeof(CheckedMonthYearPicker), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        public static readonly DependencyProperty SelectedDateProperty =
            DependencyProperty.Register(
                nameof(SelectedDate),
                typeof(DateTime),
                typeof(CheckedMonthYearPicker),
                new FrameworkPropertyMetadata(
                    DateTime.Now,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSelectedDateChanged));

        private static void OnSelectedDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (CheckedMonthYearPicker)d;
            control.OnSelectedDateChanged((DateTime)e.OldValue, (DateTime)e.NewValue);
        }

        protected virtual void OnSelectedDateChanged(DateTime oldValue, DateTime newValue)
        {
            // Place your logic here. This will be called on binding and user changes.
            if (newValue.Equals(oldValue))
                return;
            YearTextBox.Text = newValue.Year.ToString();
            MonthComboBox.SelectedIndex = newValue.Month - 1;
        }

        public CheckedMonthYearPicker()
        {
            InitializeComponent();
        
            var monthNames = new List<string>(DateTimeFormatInfo.CurrentInfo.MonthNames);
            // Remove the empty string at the end (13th month for calendars with 13 months)
            if (monthNames.Count > 0 && string.IsNullOrEmpty(monthNames[monthNames.Count - 1]))
                monthNames.RemoveAt(monthNames.Count - 1);
            
            for (int i = 0; i < 12; i++)
            MonthComboBox.Items.Add(monthNames[i]);
                        
        }

        /// <summary>
        /// Czy zaznaczony checkBox przy dacie
        /// </summary>
        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set {
                    if (IsChecked != value)
                    {
                        SetValue(IsCheckedProperty, value);
                        
                    }
                }
        }

        public DateTime SelectedDate
        {
            get => (DateTime)GetValue(SelectedDateProperty);
            set
            {
                if (SelectedDate != value)
                {
                    SetValue(SelectedDateProperty, value);
                    IsChecked = true;
                    YearTextBox.Text = value.Year.ToString();
                }
                //NotifyPropertyChanged(nameof(SelectedDate));
            }
        }

        private void UpdateSelectedDate()
        {
            var selectedYear = int.TryParse(YearTextBox.Text, out int year) ? year : 0;
            var selectedMonthIndex = MonthComboBox.SelectedIndex > -1 ? MonthComboBox.SelectedIndex : -1;

            if (selectedYear > 0 && selectedMonthIndex > -1 && selectedMonthIndex < 12)
            {
                //SelectedDate = new DateTime(selectedYear, selectedMonthIndex + 1, 1);
                SetValue(SelectedDateProperty, new DateTime(selectedYear, selectedMonthIndex + 1, 1));
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

        private void YearTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            UpdateSelectedDate();
        }

        private void MonthComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelectedDate();
        }
    }

   
}