using System;
using System.Drawing;
using System.Windows.Forms;
using OSCalendar.Domain;
using OSCalendar.Domain.Entities;
using OSCalendar.Domain.Storages;
using OSCalendar.Instruments;
using WinFormsInfrastructure.Constructors;
using WinFormsInfrastructure.Containers;
using WinFormsInfrastructure.Extensions;

namespace OSCalendar.App.Views
{
    public class CellHeaderView : IView
    {
        public DateTime Date { get; set; }

        private readonly IFormConstructor constructor;
        private readonly IStorage<CalendarDayInfo> storage;
        private readonly Control parent;

        private Label dayNumberLabel;
        private Stopwatch stopwatch;
        private TableContainer mainTable;

        public CellHeaderView(IFormConstructor constructor, IStorage<CalendarDayInfo> storage, DateTime date, Control parent)
        {
            Date = date;
            this.constructor = constructor;
            this.storage = storage;
            this.parent = parent;
        }


        public Control GetView()
        {
            if (mainTable == null)
            {
                CreateView();
            }

            return mainTable;
        }

        private void CreateView()
        {
            mainTable = constructor.CreateTableLayoutPanel("100% 25px");

            dayNumberLabel = constructor.CreateLabel(GetDateString());
            dayNumberLabel.ForeColor = WhiteMainColors.Black;
            dayNumberLabel.TextAlign = ContentAlignment.MiddleRight;
            dayNumberLabel.Paint += Paint;

            var firstControl = new Control();

            var headerHeight = 20;
            if (DateComparer.DaysEquals(Date, DateTime.Now))
            {
                stopwatch = new Stopwatch(constructor, storage, Date, headerHeight);
                stopwatch.Stop += (s, a) => parent.Invalidate();
                firstControl = stopwatch.GetView();
            }

            mainTable.PushRow(firstControl, SizeType.Absolute, headerHeight, 0);
            mainTable.PushColumn(dayNumberLabel, 1);
            mainTable.Paint += MainTable_Paint;
        }

        private void MainTable_Paint(object sender, PaintEventArgs e)
        {
            if (stopwatch == null)
                return;

            var now = DateTime.Now;
            if (DateComparer.DaysEquals(Date, now))
            {
                stopwatch.SetVisible(true);
            }
            else
            {
                stopwatch.SetVisible(false);
            }
        }

        private void Paint(object sender, PaintEventArgs e)
        {
            dayNumberLabel.Text = GetDateString();
        }

        private string GetDateString()
        {
            if (Date.Day == 1)
            {
                return Date.ToString("d MMMM");
            }

            return Date.Day.ToString();
        }
    }
}
