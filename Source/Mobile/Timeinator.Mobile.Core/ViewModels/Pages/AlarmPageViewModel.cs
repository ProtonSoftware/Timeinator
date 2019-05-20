using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using Timeinator.Core;
using MvvmCross.ViewModels;

namespace Timeinator.Mobile.Core
{
    /// <summary>
    /// The view model for alarm page
    /// </summary>
    public class AlarmPageViewModel : MvxViewModel
    {
        #region Private Members

        private readonly IUserTimeHandler mUserTimeHandler;

        #endregion

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
        public bool Paused => !mUserTimeHandler.SessionRunning;

        /// <summary>
        /// Remaining time from handler
        /// </summary>
        public TimeSpan TimeRemaining {
            get
            {
                try { return CurrentTask.AssignedTime - mUserTimeHandler.TimePassed; }
                catch { return default; }
            }
        }

        /// <summary>
        /// Current session length from the start of it
        /// </summary>
        public TimeSpan SessionDuration { get; set; }

        #endregion

        #region Commands

        public ICommand StopCommand { get; private set; }
        public ICommand FinishCommand { get; private set; }

        #endregion

        #region Constructor 

        /// <summary>
        /// Default constructor
        /// </summary>
        public AlarmPageViewModel(IUserTimeHandler userTimeHandler)
        {
            // Create commands
            StopCommand = new RelayCommand(Stop);
            FinishCommand = new RelayCommand(Finish);

            // Get injected DI services
            mUserTimeHandler = userTimeHandler;
        }

        #endregion

        private void Stop()
        {
            Finish();
            StopCommand.Execute(null);
        }

        private void Finish()
        {
            mUserTimeHandler.FinishTask();
            ContinueUserTasks();
        }
        
        /// <summary>
        /// Checks if to exit to main page or start next task
        /// </summary>
        private void ContinueUserTasks()
        {
            mUserTimeHandler.CleanTasks();
            mUserTimeHandler.RefreshTasksState();
            mUserTimeHandler.StartTask();
        }
    }
}
