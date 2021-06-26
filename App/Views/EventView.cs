using System;
using System.Windows.Forms;
using OSCalendar.Domain;
using OSCalendar.Domain.Entities;
using OSCalendar.Domain.Storages;
using OSCalendar.EditEventWindow;
using WinFormsInfrastructure.Constructors;
using WinFormsInfrastructure.Containers;
using WinFormsInfrastructure.Extensions;

namespace OSCalendar.App.Views
{
    public class EventView : IView
    {
        private DateTime date;

        public DateTime Date
        {
            get => date;
            set
            {
                LastDate = date;
                date = value;
            }
        }
        private DateTime LastDate { get; set; }


        private readonly IFormConstructor constructor;
        private readonly IStorage<CalendarDayInfo> storage;
        private TableContainer mainTable;

        public EventView(IFormConstructor constructor, IStorage<CalendarDayInfo> storage, DateTime date)
        {
            Date = date;
            LastDate = date;
            this.constructor = constructor;
            this.storage = storage;
        }

        public Control GetView()
        {
            if (mainTable == null)
            {
                CreateView();
            }

            return mainTable;
        }

        public void CreateView()
        {
            mainTable = constructor.CreateTableLayoutPanel("100%");
            mainTable.Paint += MainTable_Paint;

            UpdateView();
        }

        private void MainTable_Paint(object sender, PaintEventArgs e)
        {
            if (!DateComparer.DaysEquals(Date, LastDate))
            {
                UpdateView();
            }
        }

        public void UpdateView()
        {
            var dayInfo = storage.Get(d => d.DateEquals(Date));

            if (dayInfo == null)
            {
                return;
            }

            if (mainTable.RowCount != 0)
            {
                mainTable.RowStyles.Clear();
            }

            foreach (var dayEvent in dayInfo.Events)
            {
                mainTable.PushRow(CreateEventView(dayEvent), SizeType.Absolute, 15, 0);
            }
        }

        public Control CreateEventView(DayEvent dayEvent)
        {
            var table = constructor.CreateTableLayoutPanel("100%");
            var titleLabel = constructor.CreateLabel(dayEvent.Title);

            table.PushRow(titleLabel, SizeType.Percent, 100, 0);

            titleLabel.Click += (s, a) => ShowEditForm((Control)s, dayEvent);

            return table;
        }

        private void ShowEditForm(Control source, DayEvent dayEvent)
        {
            var editForm = new EditEventForm(constructor, dayEvent);
            FormViewer.SetFormStartLocation(editForm, source);
            editForm.ShowDialog();

            if (editForm.IsCanceled)
                return;

            var dayInfo = storage.Get(r => r.DateEquals(dayEvent.From));
            
            dayInfo.Events.Remove(dayEvent);
            dayInfo.Events.Add(editForm.ResultDayEvent);
            
            storage.Save(dayInfo);
        }
    }
}
