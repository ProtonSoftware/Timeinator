using System;
using System.Windows.Input;
using Timeinator.Core;

namespace Timeinator.Mobile
{
    /// <summary>
    /// The view model for new time task popup
    /// </summary>
    public class AddNewTimeTaskViewModel
    {
        #region Public Properties

        /// <summary>
        /// The name of a task that is being created
        /// </summary>
        public string TaskName { get; set; } = string.Empty;

        /// <summary>
        /// The description of a task that is being created
        /// </summary>
        public string TaskDescription { get; set; }

        /// <summary>
        /// The tag attached to this task
        /// </summary>
        public string TaskTag { get; set; }

        /// <summary>
        /// Constant unit of time defined by user
        /// </summary>
        public TimeSpan TaskConstantTime { get; set; }

        /// <summary>
        /// Indicates if new task should have important flag
        /// </summary>
        public bool TaskImportance { get; set; }

        /// <summary>
        /// Indicates if new task should be still saved even after its done
        /// </summary>
        public bool TaskImmortality { get; set; }

        /// <summary>
        /// The value of priority slider (1-5 values) that will be converted to priority on the task being created
        /// </summary>
        public double TaskPrioritySliderValue { get; set; } = 1f;

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
        public AddNewTimeTaskViewModel()
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
            // Check if we have everything we need for new task to create
            if (!ValidateUserInput())
            {
                // Display message to the user
                DI.UI.DisplayPopupMessageAsync(new PopupMessageViewModel("Niepoprawne dane", "Podane informacje o tasku nie spełniają wymagań."));

                // Stay at this page so user can correct his mistakes
                return;
            }

            // Data is correct, create new context out of it
            var newTask = new TimeTaskContext
            {
                Name = TaskName,
                Description = TaskDescription,
                Tag = TaskTag,
                AssignedTime = TaskConstantTime,
                HasConstantTime = TaskConstantTime != default(TimeSpan),
                IsImportant = TaskImportance,
                IsImmortal = TaskImmortality,
                Priority = (Priority)TaskPrioritySliderValue,
                Progress = 0
            };

            // Pass it to the service to handle it
            DI.TimeTasksService.SaveNewTask(newTask);

            // Close this page
            DI.UI.HideRecentModalFromCurrentNavigation();
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Validates user input about new task
        /// </summary>
        /// <returns>True if everything is correct, false otherwise</returns>
        private bool ValidateUserInput()
        {
            // If task's name is too short
            if (TaskName.Length < 4)
                // Show an error
                return false;

            // Data is correct, return success
            return true;
        }

        #endregion
    }
}
