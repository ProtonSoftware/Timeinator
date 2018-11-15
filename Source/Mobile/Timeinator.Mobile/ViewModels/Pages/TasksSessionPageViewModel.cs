using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Timeinator.Mobile
{
    /// <summary>
    /// The view model for tasks session page
    /// </summary>
    public class TasksSessionPageViewModel : BaseViewModel
    {
        #region PublicProperties

        /// <summary>
        /// The list of time tasks for current session to show in this page
        /// </summary>
        public ObservableCollection<TimeTaskViewModel> TaskItems { get; set; } = new ObservableCollection<TimeTaskViewModel>();
        /// <summary>
        /// Holds current task state
        /// </summary>
        public bool Paused => !DI.UserTimeHandler.TaskTimer.Enabled;
        /// <summary>
        /// Remaining time from handler
        /// </summary>
        public string TimeRemaining => (TimeSpan.FromMilliseconds(DI.UserTimeHandler.TaskTimer.Interval)-DI.UserTimeHandler.TimePassed).ToString();

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

        private void UserTimeHandler_TimesUp()
        {
            DI.UI.DisplayPopupMessage();
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
