using MvvmCross.ViewModels;
using System;

namespace Timeinator.Mobile.Core
{
    /// <summary>
    /// The view model for task time page where user prepares his session
    /// </summary>
    public class TasksTimePageViewModel : MvxViewModel
    {
        #region Private Members

        private readonly ITimeTasksService mTimeTasksService;

        #endregion

        #region Public Properties

        /// <summary>
        /// The time that user has declared to calculate tasks for
        /// </summary>
        public TimeSpan UserTime { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public TasksTimePageViewModel(ITimeTasksService timeTasksService)
        {
            mTimeTasksService = timeTasksService;
        }

        #endregion
    }
}
