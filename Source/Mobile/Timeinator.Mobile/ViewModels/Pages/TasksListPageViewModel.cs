using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

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
            UserReadyCommand = new RelayCommand(UserReady);

            // Load saved tasks in database
            var tasks = DI.TimeTasksService.LoadStoredTasks();

            // For each of them...
            foreach (var task in tasks)
                // Add it to the page's collection as view model
                TaskItems.Add(DI.TimeTasksMapper.Map(task));
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// Fired when user wants to start new session with selected tasks
        /// </summary>
        private void UserReady()
        {
            // Convert our collection to suitable list of contexts
            var taskContexts = new List<TimeTaskContext>();
            foreach (var task in TaskItems)
            {
                if (task.IsEnabled)
                {
                    taskContexts.Add(DI.TimeTasksMapper.ReverseMap(task));
                }
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
    }
}
