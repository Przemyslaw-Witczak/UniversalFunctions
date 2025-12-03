using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using WpfComponents.Extensions;

namespace WpfComponents
{
    public partial class CheckedMonthYearPicker : UserControl
    {        
    
    
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register(
                nameof(IsChecked), 
                typeof(bool), 
                typeof(CheckedMonthYearPicker), 
                new FrameworkPropertyMetadata(
                    false, 
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSelectedCheckedChanged));

        private static void OnSelectedCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {            
            var control = (CheckedMonthYearPicker)d;
            Debug.WriteLine($"{control.Name}:{nameof(OnSelectedCheckedChanged)}({(bool)e.OldValue}, {(bool)e.NewValue})");
            control.OnCheckedChanged((bool)e.OldValue, (bool)e.NewValue);
            Debug.WriteLine($"{control.Name}:{nameof(OnSelectedCheckedChanged)} completed");
        }

        protected virtual void OnCheckedChanged(bool oldValue, bool newValue)
        {
            Debug.WriteLine($"{Name}:{nameof(OnCheckedChanged)}({oldValue}, {newValue})");
            if (oldValue==newValue)
                return;
            if (!newValue)
            {
                Debug.WriteLine($"{Name}:{nameof(OnCheckedChanged)} - clearing date");
                YearTextBox.Text = string.Empty;
                MonthComboBox.SelectedIndex = -1;
            }
            else if (newValue)
            {
                Debug.WriteLine($"{Name}:{nameof(OnCheckedChanged)} - setting date to now");
                YearTextBox.Text = DateTime.Now.Year.ToString();
                MonthComboBox.SelectedIndex = DateTime.Now.Month - 1;
            }
            Debug.WriteLine($"{Name}:{nameof(OnCheckedChanged)} completed");
        }

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
            Debug.WriteLine($"{control.Name}:{nameof(OnSelectedDateChanged)}({(DateTime)e.OldValue}, {(DateTime)e.NewValue})");
            control.OnSelectedDateChanged((DateTime)e.OldValue, (DateTime)e.NewValue);
            Debug.WriteLine($"{control.Name}:{nameof(OnSelectedDateChanged)} completed");
        }

        protected virtual void OnSelectedDateChanged(DateTime oldValue, DateTime newValue)
        {
            // Place your logic here. This will be called on binding and user changes.
            Debug.WriteLine($"{Name}:{nameof(OnSelectedDateChanged)}({oldValue}, {newValue})");
            if (newValue.IsEqualMonthYear(oldValue))
                return;
            YearTextBox.Text = newValue.Year.ToString();
            MonthComboBox.SelectedIndex = newValue.Month - 1;
            Debug.WriteLine($"{Name}:Updated {nameof(YearTextBox)} to {YearTextBox.Text} and {nameof(MonthComboBox)} to {MonthComboBox.SelectedIndex+1}");
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
                        Debug.WriteLine($"{Name}:{nameof(IsChecked)} set to {value}");
                        SetValue(IsCheckedProperty, value);   
                        Debug.WriteLine($"{Name}:{nameof(IsChecked)} is now {IsChecked}");
                }
                }
        }

        public DateTime SelectedDate
        {
            get => (DateTime)GetValue(SelectedDateProperty);
            set
            {
                if (!SelectedDate.IsEqualMonthYear(value))
                {
                    Debug.WriteLine($"{Name}:{nameof(SelectedDate)} set to {value}");
                    SetValue(SelectedDateProperty, value);
                    IsChecked = true;
                    YearTextBox.Text = value.Year.ToString();
                    MonthComboBox.SelectedIndex = value.Month - 1;
                    Debug.WriteLine($"{Name}:Updated {nameof(YearTextBox)} to {YearTextBox.Text} and {nameof(MonthComboBox)} to {MonthComboBox.SelectedIndex+1}");
                }
                //NotifyPropertyChanged(nameof(SelectedDate));
            }
        }

        private void UpdateSelectedDateFromUserInput()
        {
            Debug.WriteLine($"{Name}:{nameof(UpdateSelectedDateFromUserInput)}");
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
            Debug.WriteLine($"{Name}:Updated {nameof(SelectedDate)} to {SelectedDate}, {nameof(IsChecked)}={IsChecked}");
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
            UpdateSelectedDateFromUserInput();
        }

        private void MonthComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelectedDateFromUserInput();
        }

        
    }

   
}