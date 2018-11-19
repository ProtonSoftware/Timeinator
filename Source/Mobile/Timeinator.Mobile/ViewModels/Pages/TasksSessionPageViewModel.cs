using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;
using System.Windows.Input;

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
        /// Progress of current task
        /// </summary>
        public double TaskProgress => CurrentTask!=null ? CurrentTask.Progress*100 : 0;
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
            StopCommand = new RelayCommand(() => { DI.UserTimeHandler.StopTask();
                Stop(); });
            ResumeCommand = new RelayCommand(() => { DI.UserTimeHandler.ResumeTask();
                Resume(); });
            FinishCommand = new RelayCommand(() => { DI.UserTimeHandler.FinishTask();
                Finish(); });
            DI.UserTimeHandler.TimesUp += UserTimeHandler_TimesUp;
            RealTimer.Elapsed += RealTimer_Elapsed;  

            LoadTaskList();
        }

        #endregion

        private void Stop()
        {
            SetupStopwatch();
            UpdateProgress();
        }

        private void Resume()
        {
            SetupStopwatch();
            UpdateProgress();
        }

        private void Finish()
        {
            SetupStopwatch();
            OutOfTasks();
            UpdateProgress();
        }

        /// <summary>
        /// Nicely switches stopwatch refreshing
        /// </summary>
        private void SetupStopwatch()
        {
            BreakStart = DateTime.Now;
            if (Paused)
                RealTimer.Start();
            else
                RealTimer.Stop();
        }

        /// <summary>
        /// Refreshes properties for UI
        /// </summary>
        private void RealTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            BreakDuration = DateTime.Now - BreakStart;
            UpdateProgress();
        }

        /// <summary>
        /// Fired when the current task has run out of time 
        /// </summary>
        private void UserTimeHandler_TimesUp()
        {
            var popupViewModel = new PopupMessageViewModel
                (
                    "Skończył się czas", 
                    "Skończył się czas na zadanie, co chcesz teraz zrobić?", 
                    "Następne zadanie", 
                    "Nie, chcę przerwę"
                );
            var userResponse = DI.UI.DisplayPopupMessageAsync(popupViewModel);

            DI.UserTimeHandler.StartTask();
            OutOfTasks();
            if (!userResponse.Result)
                StopCommand.Execute(null);
        }
        
        /// <summary>
        /// Checks if to exit to main page
        /// </summary>
        private void OutOfTasks()
        {
            if (CurrentTask != null && CurrentTask.Progress >= 1)
            {
                DI.TimeTasksService.RemoveFinishedTasks(DI.TimeTasksMapper.ReverseMap(CurrentTask));
                TaskItems.Remove(CurrentTask);
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
            CurrentTask.Progress = DI.UserTimeHandler.TimePassed.TotalMilliseconds / CurrentTask.AssignedTime.TotalMilliseconds;
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
