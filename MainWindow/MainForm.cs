using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WinFormsInfrastructure.Constructors;
using WinFormsInfrastructure.Extensions;
using WinFormsInfrastructure.ThemeManagers;

namespace OSCalendar.MainWindow
{
    public class MainForm : Form
    {
        private TableLayoutPanel mainTable;

        public MainForm()
        {
            MinimumSize = new Size(800, 600);
            //Opacity = 0.9;
            //this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            //BackColor = Color.Transparent;

            var constructor = new FormConstructor(new MainFormThemeManager());

            mainTable = constructor.CreateTableLayoutPanel("100%");


            var rows = 5;
            var header = CreateHeader(constructor);
            var dayOfWeeks = CreateDayOfWeeks(constructor);
            var weeksTable = CreateWeeks(constructor, rows);

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
            var nextMonthBtn = constructor.CreateButton("N");
            var settingsBtn = constructor.CreateButton("S");

            table.PushRow(title, SizeType.Percent, 10, 0);
            table.PushColumn(previousMonthBtn, 1);
            table.PushColumn(nextMonthBtn, 2);
            table.PushColumn(settingsBtn, 3);

            return table;
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

            //TODO: try use day of weeks
            var names = new[] {"Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Вс"};

            for (var i = 1; i < 8; i++)
            {
                var label = constructor.CreateLabel(names[i - 1]);
                label.BackColor = Color.LightSalmon;
                label.TextAlign = ContentAlignment.MiddleCenter;
                table.PushColumn(label, i);
            }

            return table;
        }

        private Control CreateWeeks(IFormConstructor constructor, int rows)
        {
            var table = constructor.CreateTableLayoutPanel(SizeType.Percent, 100);
            var rowHeightPercent = 100 / rows;

            for (var i = 0; i < rows; i++)
            {
                var rowTable = CreateRowTable(constructor);
                var weekNumber = constructor.CreateLabel("99");
                weekNumber.BackColor = Color.Bisque;
                rowTable.PushRow(weekNumber, SizeType.Percent, 100, 0);

                for (var j = 1; j < 8; j++)
                {
                    var calendarCell = new CalendarCell(constructor);
                    var dayCell = calendarCell.GetView();
                    dayCell.BackColor = Color.LightCoral;
                    
                    rowTable.PushColumn(dayCell, j);
                }

                table.PushRow(rowTable, SizeType.Percent, rowHeightPercent, 0);
            }

            return table;
        }
    }
}
