using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Timeinator.Mobile
{
    /// <summary>
    /// The view model for main tasks page
    /// </summary>
    public class TasksListPageViewModel : BasePageViewModel
    {
        #region Public Properties

        /// <summary>
        /// The list of time tasks for current session to show in this page
        /// </summary>
        public ObservableCollection<TimeTaskViewModel> TaskItems { get; set; } = new ObservableCollection<TimeTaskViewModel>();

        /// <summary>
        /// The list of every tag that are associated with current list of tags
        /// </summary>
        public ObservableCollection<string> TaskTags { get; set; } = new ObservableCollection<string>();

        /// <summary>
        /// The time that user has declared to calculate tasks for
        /// </summary>
        public TimeSpan UserTime { get; set; }

        #endregion

        #region Commands

        /// <summary>
        /// The command to show a modal to add new task to the list
        /// </summary>
        public ICommand AddNewTaskCommand { get; private set; }

        /// <summary>
        /// The command to edit specified task as a parameter
        /// </summary>
        public ICommand EditTaskCommand { get; private set; }

        /// <summary>
        /// The command to delete specified task as a parameter
        /// </summary>
        public ICommand DeleteTaskCommand { get; private set; }

        /// <summary>
        /// The command to fire when user is ready and wants to begin new session with selected tasks
        /// </summary>
        public ICommand UserReadyCommand { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public TasksListPageViewModel()
        {
            // Create commands
            AddNewTaskCommand = new RelayCommand(() => DI.UI.ShowModalOnCurrentNavigation(new AddNewTimeTaskControl()));
            EditTaskCommand = new RelayParameterizedCommand(EditTask);
            DeleteTaskCommand = new RelayParameterizedCommand((param) => Device.BeginInvokeOnMainThread(async () => await DeleteTaskAsync(param)));
            UserReadyCommand = new RelayCommand(UserReady);

            // Load saved tasks in database
            var tasks = DI.TimeTasksService.LoadStoredTasks();

            // For each of them...
            foreach (var task in tasks)
                // Add it to the page's collection as view model
                TaskItems.Add(DI.TimeTasksMapper.Map(task));

            // Get every tag to display in the view
            GetEveryTaskTags();
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// Shows a page where specified task can be edited
        /// </summary>
        /// <param name="param">The task to edit</param>
        private void EditTask(object param)
        {
            // Get the task view model
            var taskVM = param as TimeTaskViewModel;

            // Create edit page's view model based on that
            var pageVM = new AddNewTimeTaskViewModel
            {
                TaskId = taskVM.Id,
                TaskName = taskVM.Name,
                TaskDescription = taskVM.Description,
                TaskTag = taskVM.Tag,
                TaskConstantTime = taskVM.AssignedTime,
                TaskImmortality = taskVM.IsImmortal,
                TaskPrioritySliderValue = (double)taskVM.Priority,
                TaskImportance = taskVM.IsImportant
            };

            // Show the page with filled info
            DI.UI.ShowModalOnCurrentNavigation(new AddNewTimeTaskControl(pageVM));
        }

        /// <summary>
        /// Deletes the specified task from the list and request database deletion
        /// </summary>
        /// <param name="param">The task to delete</param>
        private async Task DeleteTaskAsync(object param)
        {
            // Get the task view model
            var taskVM = param as TimeTaskViewModel;

            // Ask the user if he is certain
            var popupViewModel = new PopupMessageViewModel
                (
                    "Usuwanie zadania",
                    "Czy na pewno chcesz usunąc zadanie: " + taskVM.Name + "?",
                    "Tak",
                    "Nie"
                );
            var userResponse = await DI.UI.DisplayPopupMessageAsync(popupViewModel);

            // If he agreed...
            if (userResponse)
                // Delete the task
                DI.TimeTasksService.RemoveTask(DI.TimeTasksMapper.ReverseMap(taskVM));
        }

        /// <summary>
        /// Fired when user wants to start new session with selected tasks
        /// </summary>
        private void UserReady()
        {
            // Convert our collection to suitable list of contexts
            var taskContexts = new List<TimeTaskContext>();
            foreach (var task in TaskItems)
            {
                // Only add enabled tasks for this session
                if (task.IsEnabled)
                    taskContexts.Add(DI.TimeTasksMapper.ReverseMap(task));
            }

            // If user has picked nothing...
            if (taskContexts.Count == 0)
            {
                // Show him an error
                DI.UI.DisplayPopupMessageAsync(new PopupMessageViewModel("Error", "Nie wybrałes żadnego taska!"));

                // Don't do any further actions with no tasks
                return;
            }

            // Pass it to the service so it handles it to the manager, with user free time
            DI.TimeTasksService.ConveyTasksToManager(taskContexts, UserTime);

            // Change the page
            DI.Application.GoToPage(ApplicationPage.TasksPreparation);
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Looks up in every task for its tag and lists them as strings
        /// </summary>
        private void GetEveryTaskTags()
        {
            // For every task in the list
            foreach (var task in TaskItems)
            {
                // Get it's tag
                var tag = task.Tag;

                // If its not in the list
                if (!TaskTags.Contains(tag))
                    // Add it
                    TaskTags.Add(tag);
            }
        }

        #endregion
    }
}
