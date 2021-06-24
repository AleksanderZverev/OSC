using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OSCalendar.Domain.Entities;
using OSCalendar.Domain.Storages;
using OSCalendar.Properties;
using WinFormsInfrastructure.Constructors;
using WinFormsInfrastructure.Extensions;

namespace OSCalendar.Instruments
{
    public class Stopwatch
    {
        private readonly IFormConstructor constructor;
        private readonly IStorage<CalendarDayInfo> storage;
        private readonly int height;
        private TableLayoutPanel table;
        private Button playBtn;
        private Button pauseBtn;
        private Button stopBtn;

        public Stopwatch(IFormConstructor constructor, IStorage<CalendarDayInfo> storage, int height)
        {
            this.constructor = constructor;
            this.storage = storage;
            this.height = height;
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
            table = constructor.CreateTableLayoutPanel($"{height}px {height}px {height}px 100%");

            playBtn = constructor.CreateImageButton(Resources.play, Resources.play_hover);
            playBtn.Margin = Padding.Empty;
            pauseBtn = constructor.CreateImageButton(Resources.pause, Resources.pause_hover);
            pauseBtn.Margin = Padding.Empty;
            stopBtn = constructor.CreateImageButton(Resources.stop, Resources.stop_hover);
            stopBtn.Margin = Padding.Empty;

            table.PushRow(playBtn, SizeType.Percent, 100, 0);
            table.PushColumn(pauseBtn, 1);
            table.PushColumn(stopBtn, 2);
            table.PushColumn(new Control(), 3);
        }
    }
}
