using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Timeinator.Mobile
{
    /// <summary>
    /// The view model for main tasks page
    /// </summary>
    public class TasksPreparationPageViewModel : BasePageViewModel
    {
        #region Public Properties

        /// <summary>
        /// The list of time tasks for current session to show in this page
        /// </summary>
        public ObservableCollection<CalculatedTimeTaskViewModel> TaskItems { get; set; } = new ObservableCollection<CalculatedTimeTaskViewModel>();

        #endregion

        #region Commands

        /// <summary>
        /// The command to start new tasks session
        /// </summary>
        public ICommand StartTasksCommand { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public TasksPreparationPageViewModel()
        {
            // Create commands
            StartTasksCommand = new RelayCommand(StartTaskSession);

            // Load tasks from the manager to this page
            LoadTaskList();
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// Starts new session out of current tasks
        /// </summary>
        private void StartTaskSession()
        {
            // Convert our collection to suitable list of contexts
            var taskContexts = new List<TimeTaskContext>();
            foreach (var task in TaskItems)
                taskContexts.Add(DI.TimeTasksMapper.ReverseMap(task));

            // Pass it to the time handler to start new session
            DI.TimeTasksService.ConveyTasksToTimeHandler(taskContexts);

            // Change the page afterwards
            DI.Application.GoToPage(ApplicationPage.TasksSession);
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Loads saved tasks from the <see cref="TimeTasksManager"/>
        /// </summary>
        public void LoadTaskList()
        {
            // Calculate selected tasks and get the contexts
            var contexts = DI.TimeTasksManager.GetCalculatedTasksListForSpecifiedTime();

            // Map each one as suitable view model
            foreach (var task in contexts)
                TaskItems.Add(DI.TimeTasksMapper.MapCal(task));
        }

        #endregion
    }
}
