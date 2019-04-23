using MvvmCross.ViewModels;
using System;
using System.Windows.Input;
using Timeinator.Core;

namespace Timeinator.Mobile.Core
{
    /// <summary>
    /// The view model for task time page where user prepares his session
    /// </summary>
    public class TasksTimePageViewModel : MvxViewModel
    {
        #region Private Members

        private readonly ITimeTasksService mTimeTasksService;
        private readonly IUIManager mUIManager;

        #endregion

        #region Public Properties

        /// <summary>
        /// The time that user has declared to calculate tasks for
        /// </summary>
        public TimeSpan UserTime { get; set; }

        #endregion

        #region Commands

        /// <summary>
        /// The command to fire when user has picked the time and wants to calculate his session
        /// </summary>
        public ICommand CalculateSessionCommand { get; set; }

        /// <summary>
        /// The command that cancels current session and goes back to task list
        /// </summary>
        public ICommand CancelCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public TasksTimePageViewModel(ITimeTasksService timeTasksService, IUIManager uiManager)
        {
            // Create commands
            CalculateSessionCommand = new RelayCommand(CalculateSession);
            CancelCommand = new RelayCommand(Cancel);

            // Get injected DI services
            mTimeTasksService = timeTasksService;
            mUIManager = uiManager;
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// Checks if user has picked the right time and calculates new session for him
        /// Which leads to next page
        /// </summary>
        private void CalculateSession()
        {
            if (UserTime < mTimeTasksService.GetMinimumTime())
            {
                // Show user an error
                mUIManager.DisplayPopupMessageAsync(new PopupMessageViewModel("Error", "Wybrany czas jest niewystarczający, by zacząc sesję!"));
                // Don't do any further actions
                return;
            }

            // Pass it to the service so it handles it to the manager, with user free time
            mTimeTasksService.ConveyTimeToManager(UserTime);

            // Go to next page which shows a summary of calculated user session
            DI.Application.GoToPage(ApplicationPage.TasksSummary);
        }

        /// <summary>
        /// Cancels current session and goes back to task list
        /// </summary>
        private void Cancel()
        {
            // TODO: Clear task list in service when logic is done

            // Go back to task list
            mUIManager.GoBackToPreviousPage(this);
        }

        #endregion
    }
}
