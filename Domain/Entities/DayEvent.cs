using System;
using System.Collections.Generic;
using System.Text;

namespace OSCalendar.Domain.Entities
{
    public class DayEvent
    {
        public string Title { get; }
        public string Description { get; }

        public bool IsFullDay { get; }

        public DateTime From { get; }
        public DateTime To { get; }

        private DayEvent(string title, string description)
        {
            Title = title;
            Description = description;
        }

        public DayEvent() : this("unknown", "")
        { }

        public DayEvent(string title, string description, bool isFullDay) : this(title, description)
        {
            IsFullDay = isFullDay;
        }

        public DayEvent(string title, string description, DateTime from, DateTime to) : this(title, description)
        {
            From = from;
            To = to;
        }
    }
}
