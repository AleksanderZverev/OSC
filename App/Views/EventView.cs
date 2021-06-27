using System;
using System.Windows.Forms;
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
        public DayEvent DayEvent { get; private set; }
        public EventState State { get; private set; } = EventState.Unchanged;
        public bool IsReadOnly { get; private set; }

        private readonly IFormConstructor constructor;
        private readonly IStorage<CalendarDayInfo> storage;

        private TableContainer mainTable;
        private Label titleLabel;
        private Button removeBtn;

        public EventView(
            IFormConstructor constructor, 
            IStorage<CalendarDayInfo> storage, 
            DayEvent dayEvent, 
            bool isReadOnly, 
            EventState state = EventState.Unchanged)
        {
            DayEvent = dayEvent;
            IsReadOnly = isReadOnly;
            State = state;
            this.constructor = constructor;
            this.storage = storage;
        }

        public Control GetView()
        {
            if (mainTable == null)
            {
                CreateEventView();
            }

            return mainTable;
        }

        public Control CreateEventView()
        {
            mainTable = constructor.CreateTableLayoutPanel("100% 30px");

            titleLabel = constructor.CreateLabel(DayEvent.Title);
            titleLabel.Click += TitleLabel_Click;

            removeBtn = constructor.CreateButton("-");
            removeBtn.Visible = !IsReadOnly;
            removeBtn.Click += RemoveBtn_Click;

            mainTable.PushRow(titleLabel, SizeType.Percent, 100, 0);
            mainTable.PushColumn(removeBtn, 1);

            return mainTable;
        }

        public void SetReadOnly(bool isReadOnly)
        {
            if (IsReadOnly == isReadOnly)
                return;

            IsReadOnly = isReadOnly;
            removeBtn.Visible = !isReadOnly;
        }

        private void RemoveBtn_Click(object sender, EventArgs e)
        {
            State = EventState.Deleted;
        }

        private void TitleLabel_Click(object sender, EventArgs e)
        {
            ShowEditForm(titleLabel);
        }

        private void ShowEditForm(Control source)
        {
            if (IsReadOnly)
                return;

            var editForm = new EditEventForm(constructor, DayEvent);
            FormViewer.SetFormStartLocation(editForm, source);
            editForm.ShowDialog();

            if (editForm.IsCanceled)
                return;

            DayEvent = editForm.ResultDayEvent;
            State = EventState.Updated;
            titleLabel.Text = DayEvent.Title;
        }

        public enum EventState
        {
            Unchanged,
            Added,
            Updated,
            Deleted,
        }
    }
}
