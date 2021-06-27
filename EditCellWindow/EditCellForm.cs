using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using OSCalendar.App.Views;
using OSCalendar.Domain.Entities;
using OSCalendar.Domain.Storages;
using OSCalendar.EditEventWindow;
using OSCalendar.MainWindow;
using WinFormsInfrastructure.Constructors;
using WinFormsInfrastructure.Containers;
using WinFormsInfrastructure.Extensions;
using WinFormsInfrastructure.Maths;

namespace OSCalendar.EditCellWindow
{
    public class EditCellForm : Form
    {
        public DateTime Date { get; }
        public bool IsReadOnly { get; }

        private readonly IFormConstructor constructor;
        private readonly IStorage<CalendarDayInfo> storage;

        private TableLayoutPanel mainTable;

        private readonly List<EventHandler> cellChangedEvents = new List<EventHandler>();
        private TextBox textEditor;
        private Label headerLabel;
        private Button applyBtn;
        private Button cancelButton;
        private TableLayoutPanel confirmTable;

        private List<DayEvent> dayEvents = new List<DayEvent>();
        private EventsListView eventsListView;

        public event EventHandler CellChanged
        {
            add => cellChangedEvents.Add(value);
            remove => cellChangedEvents.Remove(value);
        }

        public EditCellForm(
            IFormConstructor constructor, 
            IStorage<CalendarDayInfo> storage, 
            DateTime date,
            bool isReadOnly,
            string startText = "")
        {
            Date = date;
            IsReadOnly = isReadOnly;
            this.constructor = constructor;
            this.storage = storage;

            FormBorderStyle = FormBorderStyle.None;

            var title = date.ToString("d MMMM");

            mainTable = constructor.CreateTableLayoutPanel("100%");
            mainTable.Padding = new Padding(15);

            headerLabel = constructor.CreateLabel(title);

            textEditor = constructor.CreateTextBox("", startText);
            textEditor.Multiline = true;

            eventsListView = new EventsListView(constructor, storage, date, isReadOnly);

            confirmTable = constructor.CreateTableLayoutPanel("50% 50%");

            applyBtn = constructor.CreateButton("Ok");
            applyBtn.Click += ApplyBtn_Click;

            cancelButton = constructor.CreateButton("Cancel");
            cancelButton.Click += CancelButton_Click;

            confirmTable.PushRow(applyBtn, SizeType.Percent, 100, 0);
            confirmTable.PushColumn(cancelButton, 1);

            mainTable.PushRow(headerLabel, SizeType.Absolute, 25, 0);
            mainTable.PushRow(textEditor, SizeType.Percent, 50, 0);
            mainTable.PushRow(eventsListView.GetView(), SizeType.Percent, 50, 0);
            mainTable.PushRow(confirmTable, SizeType.Absolute, 30, 0);

            Controls.Add(mainTable);

            var formMover = new FormMover(this);
            formMover.BindOnControls(this, mainTable, headerLabel);

            Load += (s, a) => FormViewer.MakeRoundCorners(this);
            Paint += EditCellForm_Paint;
        }

        //private void EventsTable_Paint(object sender, PaintEventArgs e)
        //{
        //    var rows = eventsTable.RowCount - 1;

        //    if (rows != dayEvents.Count)
        //    {
        //        eventsTable.Controls.Remove(addEventBtn);
        //        foreach (var dayEvent in dayEvents.Skip(rows))
        //        {
        //            eventsTable.PushRow(CreateEventView(dayEvent), SizeType.Absolute, 30, 0);
        //        }

        //        eventsTable.PushRow(addEventBtn, SizeType.Absolute, 30, 0);
        //    }
        //}

        //private Control CreateEventView(DayEvent dayEvent)
        //{
        //    var table = constructor.CreateTableLayoutPanel("100% 30px");

        //    var eventView = new EventView(constructor, storage, dayEvent);

        //    var removeButton = constructor.CreateButton("-");
        //    removeButton.Click += (s, a) => dayEvents.Remove(dayEvent);

        //    table.PushRow(eventView.GetView(), SizeType.AutoSize, 30, 0);
        //    table.PushColumn(removeButton, 1);

        //    return table;
        //}

        //private void AddEventBtn_Click(object sender, EventArgs e)
        //{
        //    var editEventForm = new EditEventForm(constructor, new DayEvent("", "", true));
        //    editEventForm.TopMost = true;
        //    FormViewer.SetFormStartLocation(editEventForm, headerLabel);
        //    editEventForm.ShowDialog();

        //    if (editEventForm.IsCanceled)
        //    {
        //        return;
        //    }

        //    var newDayEvent = editEventForm.ResultDayEvent;
        //    dayEvents.Add(newDayEvent);
        //}

        private void EditCellForm_Paint(object sender, PaintEventArgs e)
        {
            //можно в конструктор убрать
            if (IsReadOnly)
            {
                textEditor.ReadOnly = true;
                confirmTable.Visible = false;
            }
            else
            {
                textEditor.ReadOnly = false;
                confirmTable.Visible = true;
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ApplyBtn_Click(object sender, EventArgs e)
        {
            var dayInfo = new CalendarDayInfo {Date = Date, Text = textEditor.Text.Trim()};
            dayInfo.Events.AddRange(eventsListView.EventViews.Select(v => v.DayEvent));

            storage.Save(dayInfo);
            
            foreach (var eventHandler in cellChangedEvents)
            {
                eventHandler(this, EventArgs.Empty);
            }
            Close();
        }
    }
}
