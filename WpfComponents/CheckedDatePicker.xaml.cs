using System;
using System.Windows;
using System.Windows.Controls;

namespace WpfComponents
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class CheckedDatePicker : UserControl
    {
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(CheckedDatePicker), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty SelectedDateProperty =
            DependencyProperty.Register("SelectedDate", typeof(DateTime), typeof(CheckedDatePicker), new FrameworkPropertyMetadata(DateTime.Now, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public CheckedDatePicker()
        {
            InitializeComponent();
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
            get { return (DateTime)GetValue(SelectedDateProperty); }
            set 
            {
                if (SelectedDate != value)
                {
                    IsChecked = true;
                    SetValue(SelectedDateProperty, value);
                }
            }
        }

        private void DatePicker_CalendarOpened(object sender, RoutedEventArgs e)
        {
            IsChecked = true;
        }
    }
}
