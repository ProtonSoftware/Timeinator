using System;
using System.Linq;
using System.Windows.Input;
using Timeinator.Core;

namespace Timeinator.Mobile.Domain
{
    /// <summary>
    /// The view model for new time task popup
    /// </summary>
    public class AddNewTimeTaskPageViewModel : BaseModalPageViewModel
    {
        #region Private Members

        private readonly ITimeTasksService mTimeTasksService;
        private readonly IUIManager mUIManager;

        #endregion

        #region Public Properties

        /// <summary>
        /// The name of a task that is being created
        /// </summary>
        public string TaskName { get; set; } = string.Empty;

        /// <summary>
        /// The description of a task that is being created
        /// </summary>
        public string TaskDescription { get; set; } = string.Empty;

        /// <summary>
        /// The tags that are attached to this task
        /// This string is directly bound to the input field, the result tags are made by splitting this
        /// </summary>
        public string TaskTagsString { get; set; } = string.Empty;

        /// <summary>
        /// The constant amount of time provided by user for this task
        /// </summary>
        public TimeSpan TaskConstantTime { get; set; }

        /// <summary>
        /// Indicates if user wants this task to take contant amount of time instead of being calculated
        /// </summary>
        public bool TaskHasConstantTime { get; set; }

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
        public int TaskPrioritySliderValue { get; set; } = 1;

        /// <summary>
        /// The type of currently edited task
        /// </summary>
        public TimeTaskType TaskType { get; set; } = TimeTaskType.Generic;

        /// <summary>
        /// The maximum reachable progress of the task
        /// In some task types it is set automatically, in other ones its defined by user
        /// </summary>
        public int TaskMaximumProgress { get; set; }

        /// <summary>
        /// The id of a task, only set if we are editing existing one
        /// </summary>
        public int TaskId { get; set; }

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
        public AddNewTimeTaskPageViewModel(ITimeTasksService timeTasksService, IUIManager uiManager)
        {
            // Create commands
            AddTaskCommand = new RelayCommand(AddNewTask);
            GoBackCommand = new RelayCommand(CancelAndBackAsync);

            // Get injected DI services
            mTimeTasksService = timeTasksService;
            mUIManager = uiManager;
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// Adds newly created task to the <see cref="TimeTasksCalculator"/>
        /// </summary>
        public void AddNewTask()
        {
            // Check if we have everything we need for new task to create
            if (!ValidateUserInput())
            {
                // Display message to the user
                mUIManager.DisplayPopupMessageAsync(new PopupMessageViewModel(LocalizationResource.InvalidData, LocalizationResource.ProvidedTaskDataInvalid));

                // Stay at this page so user can correct his mistakes
                return;
            }

            // Data is correct, create new context out of it
            var newTask = new TimeTaskContext
            {
                Id = TaskId,
                Name = TaskName,
                Description = TaskDescription,
                Tags = TaskTagsString.SplitTagsString(),
                Type = TaskType,
                AssignedTime = TaskConstantTime,
                HasConstantTime = TaskHasConstantTime,
                IsImportant = TaskImportance,
                IsImmortal = TaskImmortality,
                Priority = (Priority)TaskPrioritySliderValue,
                CreationDate = DateTime.Now,
                Progress = 0,
                MaxProgress = TaskMaximumProgress.ConvertBasedOnType(TaskType)
            };

            // Pass it to the service to handle it
            mTimeTasksService.SaveTask(newTask);

            // Close this page
            mUIManager.GoBackToPreviousPage(this);

            // Refresh UI list so it gets new task
            TaskListHelpers.RaiseRefreshEvent();
        }

        /// <summary>
        /// Cancels current task creation and goes back to previous page
        /// </summary>
        private async void CancelAndBackAsync()
        {
            // Warn user about unsaved changes
            var vm = new PopupMessageViewModel(LocalizationResource.UnsavedChanges, LocalizationResource.AreYouSure, LocalizationResource.Yes, LocalizationResource.No);
            if (await mUIManager.DisplayPopupMessageAsync(vm))
            {
                // Go back to previous page
                await mUIManager.GoBackToPreviousPage(this);
            }
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
            if (TaskName.Length < 3 ||
            // Or if user selected constant time but didn't provide one
            TaskHasConstantTime && TaskConstantTime == TimeSpan.Zero)
                // Show an error
                return false;

            // Data is correct, return success
            return true;
        }

        #endregion
    }
}
