using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OSCalendar.MainWindow;
using WinFormsInfrastructure.Constructors;
using WinFormsInfrastructure.Extensions;

namespace OSCalendar.EditCellWindow
{
    public class EditCellForm : Form
    {
        private TableLayoutPanel mainTable;

        private List<EventHandler<EditCellInfo>> Events = new List<EventHandler<EditCellInfo>>();
        private TextBox headerInput;
        private TextBox textEditor;

        public event EventHandler<EditCellInfo> CellChanged
        {
            add => Events.Add(value);
            remove => Events.Remove(value);
        }

        public EditCellForm(IFormConstructor constructor, CalendarCell calendarCell, string startText = "")
        {
            Text = calendarCell.Date.ToString("d MMMM");

            mainTable = constructor.CreateTableLayoutPanel("100%");

            var headerLabel = constructor.CreateLabel("Title: ");
            headerInput = constructor.CreateTextBox("Name");
            var headerTable = constructor.CreateTableLayoutPanel($"{headerLabel.Width}px 100%");
            headerTable.PushRow(headerLabel, SizeType.Percent, 100, 0);
            headerTable.PushColumn(headerInput, 1);

            textEditor = constructor.CreateTextBox("", startText);
            textEditor.Multiline = true;

            var applyBtn = constructor.CreateButton("Ok");
            applyBtn.Click += ApplyBtn_Click;

            mainTable.PushRow(headerTable, SizeType.Absolute, 25, 0);
            mainTable.PushRow(textEditor, SizeType.Percent, 100, 0);
            mainTable.PushRow(applyBtn, SizeType.Absolute, 30, 0);

            Controls.Add(mainTable);
        }

        private void ApplyBtn_Click(object sender, EventArgs e)
        {
            var cellInfo = new EditCellInfo {Title = headerInput.Text.Trim(), Text = textEditor.Text.Trim()};
            foreach (var eventHandler in Events)
            {
                eventHandler(this, cellInfo);
            }
            Close();
        }
    }

    public class EditCellInfo
    {
        public string Title { get; set; }
        public string Text { get; set; }
    }
}
