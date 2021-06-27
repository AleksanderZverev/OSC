using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OSCalendar.Domain.Convertors;
using OSCalendar.Domain.Storages;
using WinFormsInfrastructure.Constructors;
using WinFormsInfrastructure.Extensions;
using WinFormsInfrastructure.ThemeManagers;

namespace OSCalendar.MainWindow
{
    public class MainForm : Form
    {
        private Font Font = new Font("Gotham", 9);
        private int TodayRow { get; set; } = 2;
        private int Rows { get; set; } = 5;
        private Color BorderColor => Color.FromArgb(230, 230, 230);
        private Color GrayText => Color.FromArgb(153, 153, 153);
        private Color BackBlack => Color.FromArgb(51, 51, 51);

        private DateTime CurrentStartDate { get; set; }

        private TableLayoutPanel mainTable;
        private List<CalendarCellView> cells = new List<CalendarCellView>();
        private List<Label> weekNumbers = new List<Label>();
        private LocalStorage localDb;

        public MainForm()
        {
            MinimumSize = new Size(800, 600);
            //Opacity = 0.9;
            //this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            //BackColor = Color.Transparent;
            
            localDb = new LocalStorage(new OSCFormatConverter());

            CurrentStartDate = CalculateStartDate(DateTime.Now);

            var constructor = new FormConstructor(new MainFormThemeManager(), Font);

            mainTable = constructor.CreateTableLayoutPanel("100%");

            var header = CreateHeader(constructor);
            var dayOfWeeks = CreateDayOfWeeks(constructor);
            var weeksTable = CreateWeeks(constructor, Rows);

            mainTable.PushRow(header, SizeType.Percent, 5, 0);
            mainTable.PushRow(dayOfWeeks, SizeType.Absolute, 25, 0);
            mainTable.PushRow(weeksTable, SizeType.Percent, 90, 0);

            Controls.Add(mainTable);
        }

        //private const int WS_EX_TRANSPARENT = 0x20;
        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        CreateParams cp = base.CreateParams;
        //        cp.ExStyle = cp.ExStyle | 0x00200000;
        //        return cp;
        //    }
        //}

        private Control CreateHeader(IFormConstructor constructor)
        {
            var table = constructor.CreateTableLayoutPanel("80% 5% 5% 5%");
           
            var title = constructor.CreateLabel("Today_Title");
            title.TextAlign = ContentAlignment.MiddleCenter;
            title.BackColor = Color.FromArgb(50, 211, 211, 211);

            var previousMonthBtn = constructor.CreateButton("B");
            previousMonthBtn.Click += PreviousMonthBtn_Click;
            var nextMonthBtn = constructor.CreateButton("N");
            nextMonthBtn.Click += NextMonthBtn_Click;
            var settingsBtn = constructor.CreateButton("S");

            table.PushRow(title, SizeType.Percent, 10, 0);
            table.PushColumn(previousMonthBtn, 1);
            table.PushColumn(nextMonthBtn, 2);
            table.PushColumn(settingsBtn, 3);

            return table;
        }

        private void NextMonthBtn_Click(object sender, EventArgs e)
        {
            CurrentStartDate = CurrentStartDate.AddDays(Rows * 7);
            UpdateCells();
            Refresh();
        }

        private void PreviousMonthBtn_Click(object sender, EventArgs e)
        {
            CurrentStartDate = CurrentStartDate.AddDays(-Rows * 7);
            UpdateCells();
            Refresh();
        }

        private void UpdateCells()
        {
            var startDate = CurrentStartDate;
            var startWeekNumber = CalculateWeekNumber(startDate);

            foreach (var calendarCell in cells)
            {
                calendarCell.Date = startDate;
                startDate = startDate.AddDays(1);
            }

            foreach (var weekNumber in weekNumbers)
            {
                weekNumber.Text = startWeekNumber++.ToString();
            }
        }

        private TableLayoutPanel CreateRowTable(IFormConstructor constructor)
        {
            var weekNumberWidth = 25;
            var percentLast = (100 - ((float)weekNumberWidth / ClientSize.Width)) / 7f;
            var args = string.Join(' ', Enumerable.Repeat(percentLast + "%", 7));

            var table = constructor.CreateTableLayoutPanel($"{weekNumberWidth}px {args}");

            return table;
        }
            

        private Control CreateDayOfWeeks(IFormConstructor constructor)
        {
            var table = CreateRowTable(constructor);
            table.PushRow(new Control(), SizeType.Percent, 100, 0);
            
            var names = new[] {"Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Вс"};

            for (var i = 1; i < 8; i++)
            {
                var label = constructor.CreateLabel(names[i - 1]);
                label.ForeColor = BackBlack;
                label.TextAlign = ContentAlignment.MiddleRight;
                label.Padding = new Padding(0, 3, 4, 3);
                table.PushColumn(label, i);
            }

            return table;
        }

        private Control CreateWeeks(IFormConstructor constructor, int rows)
        {
            var startDate = CurrentStartDate;
            var table = constructor.CreateTableLayoutPanel(SizeType.Percent, 100);
            table.BackColor = BorderColor;

            var rowHeightPercent = 100 / rows;

            for (var i = 0; i < rows; i++)
            {
                var rowTable = CreateRowTable(constructor);

                var weekNumber = constructor.CreateLabel(CalculateWeekNumber(startDate).ToString());
                weekNumber.BackColor = BackBlack;
                weekNumber.ForeColor = GrayText;
                weekNumber.TextAlign = ContentAlignment.MiddleCenter;
                rowTable.PushRow(weekNumber, SizeType.Percent, 100, 0);

                weekNumbers.Add(weekNumber);

                for (var j = 1; j < 8; j++)
                {
                    var calendarCell = new CalendarCellView(constructor, localDb, startDate);
                    var dayCell = calendarCell.GetView();

                    rowTable.PushColumn(dayCell, j);

                    startDate = startDate.AddDays(1);
                    cells.Add(calendarCell);
                }

                table.PushRow(rowTable, SizeType.Percent, rowHeightPercent, 0);
            }

            return table;
        }

        private DateTime CalculateStartDate(DateTime from)
        {
            var dayOfWeekPast = (int) from.DayOfWeek - 1;

            if (dayOfWeekPast < 0)
            {
                dayOfWeekPast = 6;
            }

            return from.AddDays(-(dayOfWeekPast + 7 * (TodayRow - 1)));
        }

        private int CalculateWeekNumber(DateTime from)
        {
            return (int) ((from - new DateTime(from.Year, 1, 1)).TotalDays / 7);
        }
            
    }
}
