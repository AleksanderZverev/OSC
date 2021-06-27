using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OSCalendar.Domain;
using OSCalendar.Domain.Entities;
using OSCalendar.Domain.Storages;
using OSCalendar.Extensions;
using WinFormsInfrastructure.Constructors;
using WinFormsInfrastructure.Containers;
using WinFormsInfrastructure.Extensions;

namespace OSCalendar.App.Views
{
    public class EventsListView : IView
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

        public bool IsReadOnly { get; private set; }
        public IReadOnlyList<EventView> EventViews => eventViews;

        private DateTime LastDate { get; set; }
        private List<EventView> eventViews = new List<EventView>();

        private readonly IFormConstructor constructor;
        private readonly IStorage<CalendarDayInfo> storage;
        private TableContainer mainTable;
        private Button addBtn;
        private TableContainer viewsTable;

        public EventsListView(IFormConstructor constructor, IStorage<CalendarDayInfo> storage, DateTime date, bool isReadOnly)
        {
            Date = date;
            IsReadOnly = isReadOnly;
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

            viewsTable = constructor.CreateTableLayoutPanel("100%");
            UpdateView();

            addBtn = constructor.CreateButton("+");
            addBtn.Dock = DockStyle.None;
            addBtn.Visible = !IsReadOnly;
            addBtn.Click += AddBtn_Click;

            mainTable.PushRow(viewsTable, SizeType.Percent, 100, 0);
            mainTable.PushRow(addBtn, SizeType.Absolute, 30, 0);
        }

        private void AddBtn_Click(object sender, EventArgs e)
        {
            var eventView = new EventView(constructor, storage, new DayEvent(Date), false, EventView.EventState.Added);
            AddEventView(eventView);
        }

        private void MainTable_Paint(object sender, PaintEventArgs e)
        {
            if (!Date.DayEquals(LastDate))
            {
                UpdateView();
                LastDate = Date;
            }
        }

        public void UpdateView()
        {
            if (viewsTable.RowCount != 0)
            {
                viewsTable.RowStyles.Clear();
            }

            if (viewsTable.Controls.Count != 0)
            {
                viewsTable.Controls.Clear();
            }

            eventViews.Clear();

            var dayInfo = storage.Get(d => d.DateEquals(Date));

            if (dayInfo == null)
            {
                return;
            }

            foreach (var dayEvent in dayInfo.Events)
            {
                var eventView = new EventView(constructor, storage, dayEvent, IsReadOnly);
                AddEventView(eventView);
            }
        }

        private void AddEventView(EventView eventView)
        {
            eventViews.Add(eventView);
            viewsTable.PushRow(eventView.GetView(), SizeType.Absolute, 15, 0);
        }
    }
}
