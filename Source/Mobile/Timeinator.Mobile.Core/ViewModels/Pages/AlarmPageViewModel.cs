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

        private readonly ITimeTasksService mTimeTasksService;

        #endregion

        #region Public Properties

        /// <summary>
        /// Current session length from the start of it
        /// </summary>
        public TimeSpan SessionDuration => mTimeTasksService.SessionDuration;

        /// <summary>
        /// The remaining time left of current task
        /// </summary>
        public TimeSpan TimeRemaining => mTimeTasksService.CurrentTaskTimeLeft;

        /// <summary>
        /// Current break duration, displayed only when break indicator is true
        /// </summary>
        public TimeSpan BreakDuration => mTimeTasksService.CurrentBreakDuration;

        #endregion

        #region Commands

        /// <summary>
        /// The command to pause current task
        /// </summary>
        public ICommand PauseCommand { get; private set; }

        /// <summary>
        /// The command to finish current task and go for the next one
        /// </summary>
        public ICommand FinishCommand { get; private set; }

        #endregion

        #region Constructor 

        /// <summary>
        /// Default constructor
        /// </summary>
        public AlarmPageViewModel(ITimeTasksService timeTasksService)
        {
            // Get injected DI services
            mTimeTasksService = timeTasksService;
        }

        #endregion

        public void InitializeButtons(Action pause, Action next)
        {
            PauseCommand = new RelayCommand(() => { pause.Invoke(); DI.Application.GoToPage(ApplicationPage.TasksSession); });
            FinishCommand = new RelayCommand(() => { next.Invoke(); DI.Application.GoToPage(ApplicationPage.TasksSession); });
        }

        #region Command Methods
        #endregion
    }
}
