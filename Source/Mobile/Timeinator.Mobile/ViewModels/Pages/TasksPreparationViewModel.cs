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

        /// <summary>
        /// The command to show a modal to add new task to the list
        /// </summary>
        public ICommand AddNewTaskCommand { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public TasksPreparationViewModel()
        {
            // Create commands
            AddNewTaskCommand = new RelayCommand(() => DI.UI.ShowModalOnCurrentNavigation(new AddNewTimeTaskControl()));

            // Load saved tasks in database to the manager
            var taskFound = DI.TimeTasksService.LoadCurrentTasks();

            // If any tasks were found
            if (taskFound)
                // Load them to this page
                LoadTaskList();
        }

        #endregion

        /// <summary>
        /// Loads saved tasks from the <see cref="TimeTasksManager"/>
        /// </summary>
        public void LoadTaskList()
        {
            // Get tasks from a manager
            foreach (var task in DI.TimeTasksManager.TaskContexts)
                TaskItems.Add(DI.TimeTasksMapper.Map(task));
        }
    }
}
