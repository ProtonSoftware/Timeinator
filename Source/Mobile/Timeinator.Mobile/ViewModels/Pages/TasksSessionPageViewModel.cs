using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using Xamarin.Forms;

namespace Timeinator.Mobile
{
    /// <summary>
    /// The view model for tasks session page
    /// </summary>
    public class TasksSessionPageViewModel : BasePageViewModel
    {
        #region Public Properties

        /// <summary>
        /// The list of time tasks for current session to show in this page
        /// </summary>
        public ObservableCollection<TimeTaskViewModel> TaskItems { get; set; } = new ObservableCollection<TimeTaskViewModel>();
        /// <summary>
        /// Returns ViewModel of current task
        /// </summary>
        public TimeTaskViewModel CurrentTask {
            get
            {
                try { return TaskItems.ElementAt(0); }
                catch { return null; }
            }
        }
        /// <summary>
        /// Holds current task state
        /// </summary>
        public bool Paused => !DI.UserTimeHandler.TaskTimer.Enabled;
        /// <summary>
        /// Remaining time from handler
        /// </summary>
        public TimeSpan TimeRemaining => TimeSpan.FromMilliseconds(DI.UserTimeHandler.TaskTimer.Interval) - DI.UserTimeHandler.TimePassed;
        /// <summary>
        /// Remaining time from handler
        /// </summary>
        public TimeSpan BreakTaskTime { get; set; }
        /// <summary>
        /// Progress of current task
        /// </summary>
        public double TaskProgress { get; set; }
        /// <summary>
        /// Timer refreshing UI every sec
        /// </summary>
        public Timer RealTimer { get; set; } = new Timer(1000);
        /// <summary>
        /// Break started time - time when user paused task
        /// </summary>
        public DateTime BreakStart { get; set; }
        /// <summary>
        /// Current break length in a timespan
        /// </summary>
        public TimeSpan BreakDuration { get; set; }
        /// <summary>
        /// Time that task progressed before pausing
        /// </summary>
        public TimeSpan RecentProgress { get; set; } = new TimeSpan(0);

        #endregion

        #region Commands

        public ICommand StopCommand { get; private set; }
        public ICommand ResumeCommand { get; private set; }
        public ICommand FinishCommand { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public TasksSessionPageViewModel()
        {
            // Create commands
            StopCommand = new RelayCommand(Stop);
            ResumeCommand = new RelayCommand(Resume);
            FinishCommand = new RelayCommand(async () => await FinishAsync());
            DI.UserTimeHandler.TimesUp += () => Device.BeginInvokeOnMainThread(async () => await UserTimeHandler_TimesUpAsync());
            RealTimer.Elapsed += RealTimer_Elapsed;  

            LoadTaskList();
            RealTimer.Start();
        }

        ~TasksSessionPageViewModel()
        {
            RealTimer.Dispose();
            DI.UserTimeHandler.TaskTimer.Stop();
        }

        #endregion

        private void Stop()
        {
            DI.UserTimeHandler.StopTask();
            ClickStandardAction();
        }

        private void Resume()
        {
            DI.UserTimeHandler.ResumeTask();
            ClickStandardAction();
        }

        private void ClickStandardAction()
        {
            UpdateProgress();
            SetupBreakStopwatch();
            OnPropertyChanged(nameof(CurrentTask));
        }

        private async Task FinishAsync()
        {
            var popupViewModel = new PopupMessageViewModel
                (
                    "Koniec zadania", 
                    "Na pewno chcesz zakończyć zadanie?",
                    "Tak", 
                    "Nie"
                );
            var userResponse = await DI.UI.DisplayPopupMessageAsync(popupViewModel);

            if (userResponse)
            {
                DI.UserTimeHandler.FinishTask();
                CurrentTask.Progress = 1;
                ContinueUserTasks();
                SetupBreakStopwatch();
            }
        }

        /// <summary>
        /// Nicely switches stopwatch refreshing
        /// </summary>
        private void SetupBreakStopwatch()
        {
            if (Paused)
            {
                BreakStart = DateTime.Now;
                RecentProgress += new TimeSpan(DI.UserTimeHandler.TimePassed.Ticks);
                BreakTaskTime = new TimeSpan(TimeRemaining.Ticks);
            }
        }

        /// <summary>
        /// Refreshes properties for UI
        /// </summary>
        private void RealTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Paused)
                BreakDuration = DateTime.Now - BreakStart;
            UpdateProgress();
            OnPropertyChanged(nameof(Paused));
            OnPropertyChanged(nameof(TimeRemaining));
            OnPropertyChanged(nameof(TaskItems));
        }

        /// <summary>
        /// Fired when the current task has run out of time 
        /// </summary>
        private async Task UserTimeHandler_TimesUpAsync()
        {
            var popupViewModel = new PopupMessageViewModel
                (
                    "Skończył się czas", 
                    "Skończył się czas na zadanie, co chcesz teraz zrobić?",
                    "Następne zadanie", 
                    "Nie, chcę przerwę"
                );
            var userResponse = await DI.UI.DisplayPopupMessageAsync(popupViewModel);

            UpdateProgress();
            ContinueUserTasks();
            if (!userResponse)
                StopCommand.Execute(null);
        }
        
        /// <summary>
        /// Checks if to exit to main page or start next task
        /// </summary>
        private void ContinueUserTasks()
        {
            if (CurrentTask != null && CurrentTask.Progress >= 1)
            {
                DI.TimeTasksService.RemoveFinishedTasks(new List<TimeTaskContext> { DI.TimeTasksMapper.ReverseMap(CurrentTask) });
                TaskItems.Remove(CurrentTask);
                DI.UserTimeHandler.StartTask();
                RecentProgress = new TimeSpan(0);
            }
            if (TaskItems.Count <= 0)
            {
                RealTimer.Stop();
                DI.Application.GoToPage(ApplicationPage.TasksList);
            }
        }

        /// <summary>
        /// Updates progress on current task
        /// </summary>
        private void UpdateProgress()
        {
            if (CurrentTask != null)
            {
                CurrentTask.Progress = (RecentProgress.TotalMilliseconds + DI.UserTimeHandler.TimePassed.TotalMilliseconds) / CurrentTask.AssignedTime.TotalMilliseconds;
                if (CurrentTask.Progress > 1)
                    CurrentTask.Progress = 1;
                TaskProgress = CurrentTask.Progress;
            }
        }

        /// <summary>
        /// Loads saved tasks from the <see cref="UserTimeHandler"/>
        /// </summary>
        public void LoadTaskList()
        {
            TaskItems.Clear();
            foreach (var e in DI.UserTimeHandler.DownloadSession())
                TaskItems.Add(DI.TimeTasksMapper.Map(e));
        }
    }
}
