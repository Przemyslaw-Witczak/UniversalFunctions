using System;
using System.Windows;

namespace WpfComponentsUITests
{
    public partial class MainWindow : Window
    {
        public MainWindowViewModel ViewModel { get; } = new MainWindowViewModel();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }

        private void CheckedMonthYearPicker_SetTo2023_12_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectedDate = new DateTime(1983, 10, 29);
            ViewModel.IsChecked = true;
        }
    }
}