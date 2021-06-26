using System;
using System.Windows.Forms;
using OSCalendar.Domain.Entities;
using OSCalendar.Domain.Storages;
using WinFormsInfrastructure.Constructors;
using WinFormsInfrastructure.Containers;
using WinFormsInfrastructure.Extensions;

namespace OSCalendar.App.Views
{
    public class CellTaskAreaView : IView
    {
        private DateTime date;
        public DateTime Date
        {
            get => date;
            set
            {
                date = value;
                DateChanged();
            }
        }

        private readonly IFormConstructor constructor;
        private readonly IStorage<CalendarDayInfo> storage;
        private TableContainer table;
        private Label textLabel;
        private EventView eventView;

        public CellTaskAreaView(IFormConstructor constructor, IStorage<CalendarDayInfo> storage, DateTime date)
        {
            Date = date;
            this.constructor = constructor;
            this.storage = storage;
            eventView = new EventView(constructor, storage, date);
        }

        public Control GetView()
        {
            if (table == null)
            {
                CreateView();
            }

            return table;
        }

        public void CreateView()
        {
            table = constructor.CreateTableLayoutPanel("100%");

            textLabel = constructor.CreateLabel();
            var eventsView = eventView.GetView();

            table.PushRow(textLabel, SizeType.Percent, 10, 0);
            table.PushRow(eventsView, SizeType.Percent, 50, 0);
        }

        public void DateChanged()
        {
            if (eventView != null)
                eventView.Date = Date;
        }
    }
}
