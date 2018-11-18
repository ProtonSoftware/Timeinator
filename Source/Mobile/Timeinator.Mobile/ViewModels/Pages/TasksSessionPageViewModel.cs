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
        public TimeTaskViewModel CurrentTask => TaskItems.ElementAt(0) ?? null;
        /// <summary>
        /// Holds current task state
        /// </summary>
        public bool Paused => !DI.UserTimeHandler.TaskTimer.Enabled;
        /// <summary>
        /// Remaining time from handler
        /// </summary>
        public string TimeRemaining => (TimeSpan.FromMilliseconds(DI.UserTimeHandler.TaskTimer.Interval)-DI.UserTimeHandler.TimePassed).ToString();
        /// <summary>
        /// Progress of current task
        /// </summary>
        public double TaskProgress => CurrentTask!=null ? CurrentTask.Progress : 0;
        /// <summary>
        /// Timer refreshing break stopwatch
        /// </summary>
        public Timer BreakTimer { get; set; } = new Timer(1000);
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
            StopCommand = new RelayCommand(() => { DI.UserTimeHandler.StopTask(); SetupStopwatch(); });
            ResumeCommand = new RelayCommand(() => { DI.UserTimeHandler.ResumeTask(); SetupStopwatch(); });
            FinishCommand = new RelayCommand(() => { DI.UserTimeHandler.FinishTask(); SetupStopwatch(); OutOfTasks(); });
            DI.UserTimeHandler.TimesUp += UserTimeHandler_TimesUp;
            BreakTimer.Elapsed += ((object s, ElapsedEventArgs e) => BreakDuration = (DateTime.Now - BreakStart));

            LoadTaskList();
        }

        #endregion

        /// <summary>
        /// Nicely switches stopwatch refreshing
        /// </summary>
        private void SetupStopwatch()
        {
            BreakStart = DateTime.Now;
            if (Paused)
                BreakTimer.Start();
            else
                BreakTimer.Stop();
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
            var tmp = new List<TimeTaskContext>();
            foreach (var t in TaskItems)
                tmp.Add(DI.TimeTasksMapper.ReverseMap(t));
            DI.TimeTasksService.RemoveFinishedTasks(tmp);
            if (CurrentTask != null && CurrentTask.Progress >= 1)
                TaskItems.Remove(CurrentTask);
            if (TaskItems.Count <= 0)
            {
                BreakTimer.Stop();
                DI.Application.GoToPage(ApplicationPage.TasksList);
            }
        }

        /// <summary>
        /// Loads saved tasks from the <see cref="TimeTasksManager"/>
        /// </summary>
        public void LoadTaskList()
        {
            TaskItems.Clear();
            foreach (var e in DI.UserTimeHandler.DownloadSession())
                TaskItems.Add(DI.TimeTasksMapper.Map(e));
        }
    }
}
