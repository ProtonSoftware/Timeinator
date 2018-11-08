using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Timeinator.Mobile
{
    /// <summary>
    /// The view model for main tasks page
    /// </summary>
    public class TasksPreparationViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// The list of time tasks for current session to show in this page
        /// </summary>
        public ObservableCollection<TimeTaskViewModel> TaskItems { get; set; } = new ObservableCollection<TimeTaskViewModel>();

        #endregion

        #region Commands

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public TasksPreparationViewModel()
        {
            // Load tasks from the manager to this page
            LoadTaskList();
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Loads saved tasks from the <see cref="TimeTasksManager"/>
        /// </summary>
        public void LoadTaskList()
        {
            // TODO: logic
        }

        #endregion
    }
}
