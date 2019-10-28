using System;
using System.Windows.Input;
using Timeinator.Core;

namespace Timeinator.Mobile.Domain
{
    /// <summary>
    /// The view model for list item control in task list page
    /// </summary>
    public class ListTimeTaskItemViewModel : TimeTaskViewModel
    {
        #region Public Properties

        /// <summary>
        /// Indicates if task is intended to be done in the next session
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Indicates if this task has contant time assigned and should not be recalculated by session algorithm
        /// </summary>
        public bool IsAssignedTime { get; set; }

        /// <summary>
        /// Indicates if context menu for this item should be visible
        /// </summary>
        public bool IsContextMenuVisible { get; set; }

        /// <summary>
        /// Indicates if task's progress should be visible
        /// </summary>
        public bool ShouldShowProgress => Type == TimeTaskType.Reading;

        #endregion

        #region Events

        /// <summary>
        /// The event to fire whenever user wants to edit this task
        /// </summary>
        public event Action<object> OnEditRequest = (vm) => { };

        /// <summary>
        /// The event to fire whenever user wants to remove this task
        /// </summary>
        public event Action<object> OnDeleteRequest = (vm) => { };

        #endregion

        #region Commands

        /// <summary>
        /// The command to fire editing event
        /// </summary>
        public ICommand EditCommand { get; private set; }

        /// <summary>
        /// The command to fire deleting event
        /// </summary>
        public ICommand DeleteCommand { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public ListTimeTaskItemViewModel()
        {
            // Create commands
            EditCommand = new RelayCommand(() => OnEditRequest.Invoke(this));
            DeleteCommand = new RelayCommand(() => OnDeleteRequest.Invoke(this));
        }

        #endregion
    }
}
