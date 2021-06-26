using System;
using System.Collections.Generic;

namespace OSCalendar.Domain.Storages
{
    public interface IStorage<TItem>
    {
        public void Save(TItem newDayInfo);
        public TItem Get(Func<TItem, bool> predicate);
        public TItem GetOrCreate(DateTime param);
        public IEnumerable<TItem> GetAll();
    }
}