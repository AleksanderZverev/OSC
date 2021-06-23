using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using WinFormsInfrastructure.Constructors;

namespace OSCalendar.MainWindow
{
    public class CalendarCell
    {
        public DateTime Date { get; set; }

        private readonly IFormConstructor constructor;
        private TableLayoutPanel table;
        private Label header;
        private Label text;

        public CalendarCell(IFormConstructor constructor)
        {
            this.constructor = constructor;
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
            header = constructor.CreateLabel(Date.Day.ToString());
            text = constructor.CreateLabel("(text)");
        }
    }
}
