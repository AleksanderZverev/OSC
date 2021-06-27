using System;
using System.Windows.Forms;
using OSCalendar.Domain.Entities;
using OSCalendar.Domain.Storages;
using OSCalendar.EditCellWindow;
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
        private EventsListView eventsListView;
        private EditCellForm editForm;

        public CellTaskAreaView(IFormConstructor constructor, IStorage<CalendarDayInfo> storage, DateTime date)
        {
            Date = date;
            this.constructor = constructor;
            this.storage = storage;
            eventsListView = new EventsListView(constructor, storage, date, true);
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
            table.MouseDown += Table_MouseDown;
            table.MouseUp += Table_MouseUp;
            table.MouseClick += Table_MouseClick;

            textLabel = constructor.CreateLabel();
            textLabel.MouseDown += Table_MouseDown;
            textLabel.MouseUp += Table_MouseUp;
            textLabel.MouseClick += Table_MouseClick;
            UpdateTextLabel();

            var eventsView = eventsListView.GetView();
            eventsView.MouseDown += Table_MouseDown;
            eventsView.MouseUp += Table_MouseUp;
            eventsView.MouseClick += Table_MouseClick;

            table.PushRow(textLabel, SizeType.Percent, 10, 0);
            table.PushRow(eventsView, SizeType.Percent, 50, 0);
        }

        public void DateChanged()
        {
            if (eventsListView != null)
                eventsListView.Date = Date;

            UpdateTextLabel();
        }

        private void UpdateTextLabel()
        {
            var dayInfo = storage?.Get(r => r.DateEquals(Date));

            if (textLabel != null)
                textLabel.Text = dayInfo?.Text;
        }

        private void Table_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                editForm?.Close();
            }
        }

        private void Table_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ShowEditForm(true);
            }
        }

        private void Table_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var contextMenu = new ContextMenuStrip();

                var menuItem = new ToolStripMenuItem("Изменить");
                menuItem.Click += ChangeText_Click;

                contextMenu.Items.Add(menuItem);
                contextMenu.Show(table, e.Location);
            }
        }

        private void ChangeText_Click(object sender, EventArgs e)
        {
            ShowEditForm();
        }

        private void ShowEditForm(bool isReadOnly = false)
        {
            editForm = new EditCellForm(constructor, storage, Date, isReadOnly, textLabel.Text);

            var location = table.PointToScreen(table.Location);
            FormViewer.SetFormStartLocation(editForm, location.X, location.Y);
            editForm.Show();
        }
    }
}
