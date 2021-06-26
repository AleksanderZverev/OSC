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
        public List<DayEvent> Events { get; } = new List<DayEvent>();
        public List<StopwatchInfo> Stopwatches { get; } = new List<StopwatchInfo>();

        public int CompareTo(CalendarDayInfo other)
        {
            var otherDate = NormalizeDate(other);
            var currentDate = NormalizeDate(this);

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

        private static DateTime NormalizeDate(CalendarDayInfo dayInfo)
            => new DateTime(dayInfo.Date.Year, dayInfo.Date.Month, dayInfo.Date.Day);

        public bool DateEquals(DateTime otherDate)
        {
            return Date.Year == otherDate.Year && Date.Month == otherDate.Month && Date.Day == otherDate.Day;
        }

        public override string ToString()
        {
            var stopwatchesText = Stopwatches.Count > 0
                ? string.Join(Environment.NewLine, Stopwatches.Select(r => r.ToString()))
                : null;

            var newText = Text;

            if (Stopwatches.Count > 1)
            {
                var total = TotalTimeSpan();
                newText += Environment.NewLine + $"{total.Hours}:{FormatNumber(total.Minutes)}:{FormatNumber(total.Seconds)}";
            }

            if (stopwatchesText != null)
            {
                newText += Environment.NewLine + stopwatchesText;
            }
            return newText;
        }

        private TimeSpan TotalTimeSpan()
        {
            var total = new TimeSpan();
            foreach (var stopwatchInfo in Stopwatches)
            {
                total += stopwatchInfo.TimeSpan;
            }

            return total;
        }

        private string FormatNumber(int n) => (n > 9 ? "" : "0") + n;
    }
}