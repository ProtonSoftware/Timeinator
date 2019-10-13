using MvvmCross.ViewModels;
using System;
using System.Threading.Tasks;
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

        private readonly ISessionHandler mSessionHandler;
        private readonly IUIManager mUIManager;

        #endregion

        #region Public Properties

        /// <summary>
        /// The time that user has declared to calculate tasks for
        /// </summary>
        public TimeSpan UserTime { get; set; }

        /// <summary>
        /// Flag whether to use provided value as finish time or duration
        /// </summary>
        public bool FinishMode { get; set; }

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
        public TasksTimePageViewModel(ISessionHandler sessionHandler, IUIManager uiManager)
        {
            // Create commands
            CalculateSessionCommand = new RelayCommand(async () => await CalculateSessionAsync());
            CancelCommand = new RelayCommand(Cancel);

            // Get injected DI services
            mSessionHandler = sessionHandler;
            mUIManager = uiManager;
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// Checks if user has picked the right time and calculates new session for him
        /// Which leads to next page
        /// </summary>
        private async Task CalculateSessionAsync()
        {
            var time = UserTime;
            // Alternative mode
            if (FinishMode)
            {
                // Subtract Now converted to timespan
                time -= DateTime.Now - DateTime.Today;
                // If negative then it should be above 24 hours
                if (time < TimeSpan.FromSeconds(-60))
                    time += TimeSpan.FromHours(24);
            }

            // Try to set user's selected time as session time
            var result = mSessionHandler.UpdateDuration(time);

            // If user's selected time is not enough to start a session...
            if (!result)
            {
                // Show user an error
                await mUIManager.DisplayPopupMessageAsync(new PopupMessageViewModel(LocalizationResource.Error, LocalizationResource.NotEnoughTimeForSession));

                // Don't do any further actions
                return;
            }

            // Otherwise, go to next page which shows a summary of calculated user session
            DI.Application.GoToPage(ApplicationPage.TasksSummary);
        }

        /// <summary>
        /// Cancels current session and goes back to task list
        /// </summary>
        private void Cancel()
        {
            // Clear task list in service
            mSessionHandler.ClearSessionTasks();

            // Go back to task list
            mUIManager.GoBackToPreviousPage(this);
        }

        #endregion
    }
}
