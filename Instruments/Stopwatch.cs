using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OSCalendar.App.Views;
using OSCalendar.Domain.Entities;
using OSCalendar.Domain.Storages;
using OSCalendar.Properties;
using WinFormsInfrastructure.Constructors;
using WinFormsInfrastructure.Extensions;
using WinFormsInfrastructure.Forms.PromptWindow;

namespace OSCalendar.Instruments
{
    public class Stopwatch : IView
    {
        public IReadOnlyList<StopwatchInfo> Records => records;

        public bool Visible => table?.Visible ?? false;

        public event EventHandler<StopwatchInfo> Stop
        {
            add => stopEventHandlers.Add(value);
            remove => stopEventHandlers.Remove(value);
        }

        private readonly List<EventHandler<StopwatchInfo>> stopEventHandlers = new List<EventHandler<StopwatchInfo>>();

        private readonly IFormConstructor constructor;
        private readonly IStorage<CalendarDayInfo> storage;
        private readonly DateTime date;
        private readonly int height;
        private TableLayoutPanel table;
        private Button playBtn;
        private Button pauseBtn;
        private Button stopBtn;

        private readonly List<StopwatchInfo> records = new List<StopwatchInfo>();
        private PictureBox stateBox;

        private StopwatchState State { get; set; }
        private TimeSpan CurrentSpan { get; set; }
        private DateTime Start { get; set; }

        public Stopwatch(IFormConstructor constructor, IStorage<CalendarDayInfo> storage, DateTime date, int height)
        {
            this.constructor = constructor;
            this.storage = storage;
            this.date = date;
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
            table = constructor.CreateTableLayoutPanel($"{height}px {height}px {height}px {height}px 100%");

            playBtn = constructor.CreateImageButton(Resources.play, Resources.play_hover);
            playBtn.Margin = Padding.Empty;
            playBtn.Click += PlayBtn_Click;

            pauseBtn = constructor.CreateImageButton(Resources.pause, Resources.pause_hover);
            pauseBtn.Margin = Padding.Empty;
            pauseBtn.Click += PauseBtn_Click;

            stopBtn = constructor.CreateImageButton(Resources.stop, Resources.stop_hover);
            stopBtn.Margin = Padding.Empty;
            stopBtn.Click += StopBtn_Click;

            stateBox = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.StretchImage,
            };

            table.PushRow(playBtn, SizeType.Percent, 100, 0);
            table.PushColumn(pauseBtn, 1);
            table.PushColumn(stopBtn, 2);
            table.PushColumn(stateBox, 3);
            table.PushColumn(new Control(), 4);
        }

        public void SetVisible(bool visible)
        {
            table.Visible = visible;
        }

        private void PlayBtn_Click(object sender, EventArgs e)
        {
            State = StopwatchState.Started;
            Start = DateTime.UtcNow;
            stateBox.Image = Resources.play_state;
        }

        private void PauseBtn_Click(object sender, EventArgs e)
        {
            State = StopwatchState.Paused;
            CurrentSpan += DateTime.UtcNow - Start;
            stateBox.Image = Resources.pause_state;
        }

        private void StopBtn_Click(object sender, EventArgs e)
        {
            if (State == StopwatchState.Stopped)
                return;

            CurrentSpan += DateTime.UtcNow - Start;
            
            var location = stopBtn.PointToScreen(stopBtn.Location);
            var name = PromptBox.Show("Введите название", location.X, location.Y);

            if (string.IsNullOrEmpty(name))
            {
                Start = DateTime.UtcNow;
                return;
            }

            var stopwatchInfo = new StopwatchInfo(name, CurrentSpan);
            records.Add(stopwatchInfo);

            State = StopwatchState.Stopped;
            CurrentSpan = default;
            stateBox.Image = null;

            var currentDayInfo = storage.GetOrCreate(date);
            currentDayInfo.Stopwatches.Add(stopwatchInfo);
            storage.Save(currentDayInfo);

            StopCalled(stopwatchInfo);
        }

        private void StopCalled(StopwatchInfo info)
        {
            foreach (var stopEventHandler in stopEventHandlers)
            {
                stopEventHandler(null, info);
            }
        }

        public enum StopwatchState
        {
            Stopped,
            Started,
            Paused
        }
    }

    public class StopwatchInfo
    {
        public string Name { get; }
        public TimeSpan TimeSpan { get; }

        public StopwatchInfo(string name, TimeSpan timeSpan)
        {
            Name = name;
            TimeSpan = timeSpan;
        }

        public override string ToString()
        {
            return $"{Name} {TimeSpan.Hours}:{FormatNumber(TimeSpan.Minutes)}:{FormatNumber(TimeSpan.Seconds)}";
        }

        private string FormatNumber(int n) => (n > 9 ? "" : "0") + n;
    }
}
