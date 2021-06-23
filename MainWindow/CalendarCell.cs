using System;
using System.Drawing;
using System.Windows.Forms;
using WinFormsInfrastructure.Constructors;
using WinFormsInfrastructure.Extensions;

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
            table.BackColor = Color.LightCoral;

            header = constructor.CreateLabel();
            header.ForeColor = Color.Bisque;
            header.Paint += Header_Paint;
            text = constructor.CreateLabel("(text)");

            table.PushRow(header, SizeType.Absolute, 15, 0);
            table.PushRow(text, SizeType.Percent, 100, 0);

            var now = DateTime.Now;
            if (Date.Month == now.Month && Date.Day == now.Day)
            {
                table.BackColor = Color.LightBlue;
            }
        }

        private void Header_Paint(object sender, PaintEventArgs e)
        {
            header.Text = Date.Day.ToString();
        }
    }
}
