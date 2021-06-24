using System;
using System.Drawing;
using System.Windows.Forms;
using OSCalendar.Domain.Entities;
using OSCalendar.Domain.Storages;
using OSCalendar.EditCellWindow;
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

        private Color GrayText => Color.FromArgb(153, 153, 153);
        private Color LightGray => Color.FromArgb(247, 247, 247);
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

            header = constructor.CreateLabel();
            header.ForeColor = BackBlack;
            header.Paint += Header_Paint;
            header.MouseClick += Table_MouseClick;

            text = constructor.CreateLabel();
            text.TextAlign = ContentAlignment.TopLeft;
            text.Paint += Text_Paint;
            text.MouseClick += Table_MouseClick;

            table.PushRow(header, SizeType.Absolute, 20, 0);
            table.PushRow(text, SizeType.Percent, 100, 0);
            table.Paint += Table_Paint;
            table.MouseClick += Table_MouseClick;
            table.Margin = new Padding(1);
        }

        private void Text_Paint(object sender, PaintEventArgs e)
        {
            var record = storage.Get(d => CompareDates(Date, d.Date));
            text.Text = record?.Text;
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
            var editForm = new EditCellForm(constructor, this, text.Text);
            editForm.CellChanged += EditForm_CellChanged;
            editForm.Show();
        }

        private void EditForm_CellChanged(object sender, EditCellInfo e)
        {
            text.Text = e.Text;
            var calendarDayInfo = new CalendarDayInfo {Text = e.Text, Date = Date.AddDays(0)};
            storage.Save(calendarDayInfo);
        }

        private void Table_Paint(object sender, PaintEventArgs e)
        {
            var now = DateTime.Now;
            if (Date.Month == now.Month && Date.Day == now.Day)
            {
                table.BackColor = Color.LightBlue;
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

        private void Header_Paint(object sender, PaintEventArgs e)
        {
            var day = Date.Day;
            header.Text = day == 1 ? Date.ToString("d MMMM") : day.ToString();
        }

        private bool CompareDates(DateTime d1, DateTime d2) =>
            d1.Year == d2.Year && d1.Month == d2.Month && d1.Day == d2.Day;
    }
}
