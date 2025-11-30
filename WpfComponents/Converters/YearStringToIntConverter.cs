using System;
using System.Globalization;
using System.Windows.Data;

namespace WpfComponents
{
    public class YearStringToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int year)
                return year.ToString();
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && int.TryParse(str, out int year))
                return year;
            return 0;
        }
    }
}