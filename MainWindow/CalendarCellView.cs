using System;
using System.Drawing;
using System.Windows.Forms;
using OSCalendar.App.Views;
using OSCalendar.Domain;
using OSCalendar.Domain.Entities;
using OSCalendar.Domain.Storages;
using OSCalendar.EditCellWindow;
using WinFormsInfrastructure.Constructors;
using WinFormsInfrastructure.Extensions;

namespace OSCalendar.MainWindow
{
    public class CalendarCellView : IView
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

        private TableLayoutPanel table;
        private Label taskPanel;
        private EditCellForm editForm;
        private CellHeaderView headerView;

        public CalendarCellView(IFormConstructor constructor, IStorage<CalendarDayInfo> storage, DateTime date)
        {
            this.constructor = constructor;
            this.storage = storage;
            Date = date;
        }

        public Control GetView()
        {
            if (table == null)
            {
                CreateView();
            }

            return table;
        }

        private void CreateView()
        {
            table = constructor.CreateTableLayoutPanel("100%");

            var headerHeight = 20;
            headerView = new CellHeaderView(constructor, storage, Date, table);
            var header = headerView.GetView();
            header.MouseClick += Table_MouseClick;

            taskPanel = constructor.CreateLabel();
            taskPanel.TextAlign = ContentAlignment.TopLeft;
            taskPanel.Paint += TaskPanelPaint;
            taskPanel.MouseClick += Table_MouseClick;

            table.PushRow(header, SizeType.Absolute, headerHeight, 0);
            table.PushRow(taskPanel, SizeType.Percent, 80, 0);
            table.Paint += Table_Paint;
            table.MouseClick += Table_MouseClick;
            table.Margin = new Padding(1);

            taskPanel.MouseDown += Table_MouseDown;
            taskPanel.MouseUp += Table_MouseUp;
        }

        private void DateChanged()
        {
            if (headerView != null)
            {
                headerView.Date = Date;
            }
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
            editForm = new EditCellForm(constructor, storage, Date, taskPanel.Text);
            editForm.TopMost = true;
            editForm.CellChanged += EditForm_CellChanged;

            editForm.ReadOnly = isReadOnly;

            var location = taskPanel.PointToScreen(taskPanel.Location);
            FormViewer.SetFormStartLocation(editForm, location.X, location.Y);
            editForm.Show();
        }

        private void EditForm_CellChanged(object sender, EditCellInfo e)
        {
            taskPanel.Text = e.Text.Trim();
            Save(taskPanel.Text);
        }

        private void Save(string newCellText)
        {
            var calendarDayInfo = new CalendarDayInfo { Text = newCellText, Date = Date.AddDays(0) };
            storage.Save(calendarDayInfo);
        }

        private void TaskPanelPaint(object sender, PaintEventArgs e)
        {
            var record = GetCurrentCalendarDayInfo();
            taskPanel.Text = record?.ToString();
        }

        private CalendarDayInfo GetCurrentCalendarDayInfo() => storage.Get(d => DateComparer.DaysEquals(Date, d.Date));

        private void Table_Paint(object sender, PaintEventArgs e)
        {
            var now = DateTime.Now;
            if (Date.Month == now.Month && Date.Day == now.Day)
            {
                table.BackColor = WhiteMainColors.LightBlue;
            }
            else if (Date.DayOfWeek == DayOfWeek.Sunday || Date.DayOfWeek == DayOfWeek.Saturday)
            {
                table.BackColor = WhiteMainColors.SuperLightGray;
            }
            else
            {
                table.BackColor = Color.White;
            }
        }
    }
}
