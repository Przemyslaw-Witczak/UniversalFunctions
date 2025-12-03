using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfComponents.Extensions
{
    internal static class DateTimeExtensions
    {
        public static bool IsEqualMonthYear(this DateTime dt1, DateTime dt2)
        {
            return dt1.Year == dt2.Year && dt1.Month == dt2.Month;
        }
    }
}
