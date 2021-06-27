using System;
using Newtonsoft.Json;
using OSCalendar.Extensions;

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

        public DayEvent(DateTime dateTime) : this("unknown", "")
        {
            From = dateTime;
            To = dateTime;
        }

        public DayEvent(string title, string description, bool isFullDay) : this(title, description)
        {
            IsFullDay = isFullDay;
        }

        public DayEvent(string title, string description, DateTime from, DateTime to) : this(title, description)
        {
            From = from;
            To = to;
        }

        [JsonConstructor]
        public DayEvent(string title, string description, bool isFullDay, DateTime from, DateTime to) : this(title, description, from, to)
        {
            IsFullDay = isFullDay;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Title, Description, IsFullDay, From, To);
        }

        protected bool Equals(DayEvent other)
        {
            return Title == other.Title 
                   && Description == other.Description 
                   && IsFullDay == other.IsFullDay 
                   && From.TimeEquals(other.From) 
                   && To.TimeEquals(other.To);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) 
                return false;
            if (ReferenceEquals(this, obj)) 
                return true;
            if (obj.GetType() != this.GetType()) 
                return false;
            return Equals((DayEvent) obj);
        }
    }
}
