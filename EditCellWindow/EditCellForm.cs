using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OSCalendar.MainWindow;
using WinFormsInfrastructure.Constructors;
using WinFormsInfrastructure.Extensions;
using WinFormsInfrastructure.Maths;

namespace OSCalendar.EditCellWindow
{
    public class EditCellForm : Form
    {
        public bool ReadOnly { get; set; }

        private TableLayoutPanel mainTable;

        private List<EventHandler<EditCellInfo>> Events = new List<EventHandler<EditCellInfo>>();
        private TextBox textEditor;
        private Label headerLabel;
        private Button applyBtn;
        private Button cancelButton;
        private TableLayoutPanel confirmTable;

        public event EventHandler<EditCellInfo> CellChanged
        {
            add => Events.Add(value);
            remove => Events.Remove(value);
        }

        public EditCellForm(IFormConstructor constructor, CalendarCell calendarCell, string startText = "")
        {
            FormBorderStyle = FormBorderStyle.None;

            var title = calendarCell.Date.ToString("d MMMM");

            mainTable = constructor.CreateTableLayoutPanel("100%");
            mainTable.Padding = new Padding(15);

            headerLabel = constructor.CreateLabel(title);

            textEditor = constructor.CreateTextBox("", startText);
            textEditor.Multiline = true;

            confirmTable = constructor.CreateTableLayoutPanel("50% 50%");

            applyBtn = constructor.CreateButton("Ok");
            applyBtn.Click += ApplyBtn_Click;

            cancelButton = constructor.CreateButton("Cancel");
            cancelButton.Click += CancelButton_Click;

            confirmTable.PushRow(applyBtn, SizeType.Percent, 100, 0);
            confirmTable.PushColumn(cancelButton, 1);

            mainTable.PushRow(headerLabel, SizeType.Absolute, 25, 0);
            mainTable.PushRow(textEditor, SizeType.Percent, 100, 0);
            mainTable.PushRow(confirmTable, SizeType.Absolute, 30, 0);

            Controls.Add(mainTable);

            var formMover = new FormMover(this);
            formMover.BindOnControls(this, mainTable, headerLabel);

            Load += (s, a) => FormViewer.MakeRoundCorners(this);
            Paint += EditCellForm_Paint;
        }

        private void EditCellForm_Paint(object sender, PaintEventArgs e)
        {
            if (ReadOnly)
            {
                textEditor.ReadOnly = true;
                confirmTable.Visible = false;
            }
            else
            {
                textEditor.ReadOnly = false;
                confirmTable.Visible = true;
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ApplyBtn_Click(object sender, EventArgs e)
        {
            var cellInfo = new EditCellInfo {Title = headerLabel.Text, Text = textEditor.Text.Trim()};
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
