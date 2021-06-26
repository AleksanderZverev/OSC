using System;
using System.Windows.Forms;
using OSCalendar.Domain.Entities;
using OSCalendar.Domain.Storages;
using WinFormsInfrastructure.Constructors;
using WinFormsInfrastructure.Containers;
using WinFormsInfrastructure.Extensions;

namespace OSCalendar.EditEventWindow
{
    public class EditEventForm : Form
    {
        public DayEvent ResultDayEvent { get; private set; }
        public bool IsCanceled { get; private set; }

        private readonly IFormConstructor constructor;
        private readonly DayEvent dayEvent;

        private TableContainer mainTable;
        private TextBox titleInput;
        private TextBox descriptionInput;
        private Button okBtn;
        private Button cancelBtn;
        
        private DateTime fromDate;
        private DateTime toDate;
        private bool isFullDay;

        public EditEventForm(IFormConstructor constructor, DayEvent dayEvent)
        {
            this.constructor = constructor;
            this.dayEvent = dayEvent;

            FormBorderStyle = FormBorderStyle.None;

            mainTable = constructor.CreateTableLayoutPanel("50% 50%");

            var formTitleLabel = constructor.CreateLabel($"Event {dayEvent.From:d MMMM}");

            var titleLabel = constructor.CreateLabel("Title");
            titleInput = constructor.CreateTextBox("", dayEvent.Title);

            var descriptionLabel = constructor.CreateLabel("Description");
            descriptionInput = constructor.CreateTextBox("", dayEvent.Description);
            descriptionInput.Multiline = true;

            var fromLabel = constructor.CreateLabel($"From {dayEvent.From:h:mm}");
            fromDate = dayEvent.From;
            var toLabel = constructor.CreateLabel($"To {dayEvent.To:h:mm}");
            toDate = dayEvent.To;

            isFullDay = dayEvent.IsFullDay;

            okBtn = constructor.CreateButton("Ok");
            okBtn.Click += OkBtn_Click;

            cancelBtn = constructor.CreateButton("Cancel");
            cancelBtn.Click += CancelBtn_Click;

            mainTable.PushRow(formTitleLabel, SizeType.Absolute, 25, 0, 2);
            
            mainTable.PushRow(titleLabel, SizeType.Absolute, 25, 0);
            mainTable.PushColumn(titleInput, 1);

            mainTable.PushRow(descriptionLabel, SizeType.Absolute, 25, 0, 2);
            mainTable.PushRow(descriptionInput, SizeType.Percent, 20, 0, 2);

            mainTable.PushRow(fromLabel, SizeType.Absolute, 25, 0, 2);
            mainTable.PushRow(toLabel, SizeType.Absolute, 25, 0, 2);

            mainTable.PushRow(okBtn, SizeType.Absolute, 25, 0);
            mainTable.PushColumn(cancelBtn, 1);

            Controls.Add(mainTable);

            Load += (s, a) => FormViewer.MakeRoundCorners(this);
        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            IsCanceled = true;
            Close();
        }

        private void OkBtn_Click(object sender, EventArgs e)
        {
            if (isFullDay)
            {
                ResultDayEvent = new DayEvent(
                    titleInput.Text.Trim(),
                    descriptionInput.Text.Trim(),
                    isFullDay
                );
            }
            else
            {
                ResultDayEvent = new DayEvent(
                    titleInput.Text.Trim(),
                    descriptionInput.Text.Trim(),
                    fromDate,
                    toDate
                );
            }

            Close();
        }
    }
}
