using System;
using System.Collections.Generic;
using System.Text;

namespace OSCalendar.Domain
{
    public static class DateComparer
    {
        public static bool DaysEquals(DateTime d1, DateTime d2) =>
            d1.Year == d2.Year && d1.Month == d2.Month && d1.Day == d2.Day;
    }
}
