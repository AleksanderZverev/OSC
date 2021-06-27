using System;

namespace OSCalendar.Extensions
{
    public static class DateTimeExtensions
    {
        public static bool DayEquals(this DateTime dateTime, DateTime other)
        {
            return dateTime.Day == other.Day 
                   && dateTime.Month == other.Month 
                   && dateTime.Year == other.Year;
        }

        public static bool TimeEquals(this DateTime dateTime, DateTime other)
        {
            return dateTime.Second == other.Second
                   && dateTime.Minute == other.Minute
                   && dateTime.Hour == other.Hour;
        }
    }
}
