using System;
using System.Collections.Generic;
using System.Linq;
using OSCalendar.Instruments;

namespace OSCalendar.Domain.Entities
{
    public class CalendarDayInfo : IComparable<CalendarDayInfo>
    {
        public DateTime Date { get; set; }
        public string Text { get; set; }
        public List<StopwatchInfo> Stopwatches { get; } = new List<StopwatchInfo>();

        public int CompareTo(CalendarDayInfo other)
        {
            var otherDate = CreateNormalizedDate(other);
            var currentDate = CreateNormalizedDate(this);

            return currentDate.CompareTo(otherDate);
        }

        public static bool operator ==(CalendarDayInfo d1, CalendarDayInfo d2)
        {
            if (ReferenceEquals(d1, null) && ReferenceEquals(d2, null))
                return true;

            if (ReferenceEquals(d1, null) || ReferenceEquals(d2, null))
                return false;

            return d1.CompareTo(d2) == 0;
        }

        public static bool operator !=(CalendarDayInfo d1, CalendarDayInfo d2)
        {
            return !(d1 == d2);
        }

        private static DateTime CreateNormalizedDate(CalendarDayInfo dayInfo)
            => new DateTime(dayInfo.Date.Year, dayInfo.Date.Month, dayInfo.Date.Day);

        public override string ToString()
        {
            var stopwatchesText = Stopwatches.Count > 0
                ? string.Join(Environment.NewLine, Stopwatches.Select(r => r.ToString()))
                : null;

            var newText = Text;

            if (stopwatchesText != null)
            {
                newText += Environment.NewLine + stopwatchesText;
            }
            return newText;
        }
    }
}