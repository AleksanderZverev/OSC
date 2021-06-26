using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using OSCalendar.Domain.Convertors;
using OSCalendar.Domain.Entities;

namespace OSCalendar.Domain.Storages
{
    public class LocalStorage : IStorage<CalendarDayInfo>
    {
        private readonly IFormatConverter converter;

        public string FileName { get; } = "osc_storage.txt";
        public string FolderPath { get; } = Directory.GetCurrentDirectory();

        public List<CalendarDayInfo> DayInfos { get; }

        public LocalStorage(IFormatConverter converter)
        {
            this.converter = converter;

            DayInfos = Read();
        }

        public void Save(CalendarDayInfo newDayInfo)
        {
            var item = DayInfos.FirstOrDefault(d => d == newDayInfo);

            if (item != null)
                DayInfos.Remove(item);

            if (!string.IsNullOrWhiteSpace(newDayInfo.Text?.Trim())
                || newDayInfo.Stopwatches.Count > 0)
                DayInfos.Add(newDayInfo);

            Write(DayInfos);
        }

        public CalendarDayInfo Get(Func<CalendarDayInfo, bool> predicate)
        {
            var item = DayInfos.FirstOrDefault(predicate);

            return item;
        }

        public CalendarDayInfo GetOrCreate(DateTime param)
        {
            var item = Get(r => r.DateEquals(param));

            if (item == null)
            {
                item = new CalendarDayInfo {Date = param};
            }

            return item;
        }

        public IEnumerable<CalendarDayInfo> GetAll() => DayInfos;

        public List<CalendarDayInfo> Read()
        {
            if (!File.Exists(GetFullPath()))
            {
                return new List<CalendarDayInfo>();
            }

            var text = File.ReadAllText(GetFullPath());
            return converter.Deserialize<List<CalendarDayInfo>>(text);
        }

        public void Write(List<CalendarDayInfo> calendarDays)
        {
            var text = converter.Serialize(calendarDays);
            File.WriteAllText(GetFullPath(), text, Encoding.UTF8);
        }

        private string GetFullPath() => Path.Combine(FolderPath, FileName);
    }
}
