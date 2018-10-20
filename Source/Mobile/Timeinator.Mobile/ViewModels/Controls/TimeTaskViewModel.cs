using System.Windows.Input;

namespace Timeinator.Mobile
{
    /// <summary>
    /// The view model for new time task popup
    /// </summary>
    public class TimeTaskViewModel
    {
        #region Public Properties

        /// <summary>
        /// The name of a task that is being created
        /// </summary>
        public string TaskName { get; set; }

        /// <summary>
        /// The description of a task that is being created
        /// </summary>
        public string TaskDescription { get; set; }

        /// <summary>
        /// Indicates if new task should have important flag
        /// </summary>
        public bool TaskImportance { get; set; }

        /// <summary>
        /// The priority (1-5 values) of a task that is being created
        /// </summary>
        public int TaskPriority { get; set; }

        #endregion

        #region Commands

        /// <summary>
        /// The command to add newly created task
        /// </summary>
        public ICommand AddTaskCommand { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public TimeTaskViewModel()
        {
            // Create commands
            AddTaskCommand = new RelayCommand(AddNewTask);
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// Adds newly created task to the <see cref="TimeTasksManager"/>
        /// </summary>
        public void AddNewTask()
        {
            // TODO: Create context, add it etc.
        }

        #endregion
    }
}
