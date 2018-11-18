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
        public TimeTaskViewModel CurrentTask => TaskItems[0] ?? null;
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
        /// Timer refreshing stopwatch
        /// </summary>
        public Timer BreakTimer { get; set; } = new Timer();
        /// <summary>
        /// Break started time - time when user paused task
        /// </summary>
        public DateTime BreakStart { get; set; }

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
            StopCommand = new RelayCommand(() => DI.UserTimeHandler.StopTask());
            ResumeCommand = new RelayCommand(() => DI.UserTimeHandler.ResumeTask());
            FinishCommand = new RelayCommand(() => DI.UserTimeHandler.FinishTask());
            DI.UserTimeHandler.TimesUp += UserTimeHandler_TimesUp;

            LoadTaskList();
        }

        #endregion

        /// <summary>
        /// Fired when the current task has run out of time 
        /// </summary>
        private void UserTimeHandler_TimesUp()
        {
            var popupViewModel = new PopupMessageViewModel
                (
                    "Skończył się czas", 
                    "Skończył się czas/task, co chcesz teraz zrobić?", 
                    "Nastepny task", 
                    "Nie, przerwa bo nie daje rady"
                );

            var userResponse = DI.UI.DisplayPopupMessageAsync(popupViewModel);

            // TODO: Maciek
            if (userResponse.Result)
            {
                // User wants next tasks
            }
            else
            {
                // User wants a break
            }

        }

        /// <summary>
        /// Loads saved tasks from the <see cref="TimeTasksManager"/>
        /// </summary>
        public void LoadTaskList()
        {
            foreach (var e in DI.UserTimeHandler.DownloadSession())
                TaskItems.Add(DI.TimeTasksMapper.Map(e));
        }
    }
}
