using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OSCalendar.Domain.Entities;
using OSCalendar.Domain.Storages;
using OSCalendar.EditCellWindow;
using OSCalendar.Instruments;
using WinFormsInfrastructure.Constructors;
using WinFormsInfrastructure.Extensions;

namespace OSCalendar.MainWindow
{
    public class CalendarCell
    {
        public DateTime Date { get; set; }

        private readonly IFormConstructor constructor;
        private readonly IStorage<CalendarDayInfo> storage;

        private TableLayoutPanel table;
        private Label header;
        private Label text;
        private Stopwatch stopwatch;
        private Control stopwatchView;
        private EditCellForm editForm;

        private Color GrayText => Color.FromArgb(153, 153, 153);
        private Color LightGray => Color.FromArgb(247, 247, 247);
        private Color LightBlue => Color.FromArgb(208, 234, 252);
        private Color BackBlack => Color.FromArgb(51, 51, 51);

        public CalendarCell(IFormConstructor constructor, IStorage<CalendarDayInfo> storage)
        {
            this.constructor = constructor;
            this.storage = storage;
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
            header = constructor.CreateLabel();
            header.ForeColor = BackBlack;
            header.TextAlign = ContentAlignment.MiddleRight;
            header.Paint += Header_Paint;
            header.MouseClick += Table_MouseClick;

            text = constructor.CreateLabel();
            text.TextAlign = ContentAlignment.TopLeft;
            text.Paint += Text_Paint;
            text.MouseClick += Table_MouseClick;

            if (DaysEquals(Date, DateTime.Now))
            {
                var headerTable = constructor.CreateTableLayoutPanel("100% 25px");
                
                stopwatch = new Stopwatch(constructor, storage, headerHeight);
                stopwatch.Stop += (s, a) =>
                {
                    var calendarDayInfo = GetCurrentCalendarDayInfo();
                    if (calendarDayInfo == null)
                    {
                        calendarDayInfo = new CalendarDayInfo {Date = Date.AddDays(0)};
                    }
                    calendarDayInfo.Stopwatches.Add(a);
                    storage.Save(calendarDayInfo);
                    table.Invalidate();
                };
                stopwatchView = stopwatch.GetView();
                stopwatchView.Paint += StopwatchView_Paint;

                headerTable.PushRow(stopwatchView, SizeType.Percent, 100, 0);
                headerTable.PushColumn(header, 1);

                table.PushRow(headerTable, SizeType.Absolute, headerHeight, 0);
            }
            else
            {
                table.PushRow(header, SizeType.Absolute, headerHeight, 0);
            }
            
            table.PushRow(text, SizeType.Percent, 80, 0);
            table.Paint += Table_Paint;
            table.MouseClick += Table_MouseClick;
            table.Margin = new Padding(1);

            text.MouseDown += Table_MouseDown;
            text.MouseUp += Table_MouseUp;
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

        private void StopwatchView_Paint(object sender, PaintEventArgs e)
        {
            if (stopwatchView == null)
            {
                return;
            }

            var now = DateTime.Now;

            if (Date.Month != now.Month || Date.Day != now.Day)
            {
                stopwatchView.Visible = false;
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
            editForm = new EditCellForm(constructor, this, text.Text);
            editForm.TopMost = true;
            editForm.CellChanged += EditForm_CellChanged;

            editForm.ReadOnly = isReadOnly;

            var location = text.PointToScreen(text.Location);
            FormViewer.SetFormStartLocation(editForm, location.X, location.Y);
            editForm.Show();
        }

        private void EditForm_CellChanged(object sender, EditCellInfo e)
        {
            text.Text = e.Text.Trim();
            Save(text.Text);
        }

        private void Header_Paint(object sender, PaintEventArgs e)
        {
            var day = Date.Day;
            header.Text = day == 1 ? Date.ToString("d MMMM") : day.ToString();
        }

        private void Save(string newCellText)
        {
            var calendarDayInfo = new CalendarDayInfo { Text = newCellText, Date = Date.AddDays(0) };
            storage.Save(calendarDayInfo);
        }

        private void Text_Paint(object sender, PaintEventArgs e)
        {
            var record = GetCurrentCalendarDayInfo();
            text.Text = record?.ToString();
        }

        private CalendarDayInfo GetCurrentCalendarDayInfo() => storage.Get(d => DaysEquals(Date, d.Date));

        private void Table_Paint(object sender, PaintEventArgs e)
        {
            var now = DateTime.Now;
            if (Date.Month == now.Month && Date.Day == now.Day)
            {
                table.BackColor = LightBlue;

                if (stopwatchView != null)
                {
                    stopwatchView.Visible = true;
                }
            }
            else if (Date.DayOfWeek == DayOfWeek.Sunday || Date.DayOfWeek == DayOfWeek.Saturday)
            {
                table.BackColor = LightGray;
            }
            else
            {
                table.BackColor = Color.White;
            }
        }

        private bool DaysEquals(DateTime d1, DateTime d2) =>
            d1.Year == d2.Year && d1.Month == d2.Month && d1.Day == d2.Day;
    }
}
