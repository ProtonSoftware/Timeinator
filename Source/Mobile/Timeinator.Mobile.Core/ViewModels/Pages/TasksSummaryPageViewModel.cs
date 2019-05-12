using MvvmCross.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Timeinator.Core;

namespace Timeinator.Mobile.Core
{
    /// <summary>
    /// The view model for tasks summary page
    /// </summary>
    public class TasksSummaryPageViewModel : MvxViewModel
    {
        #region Private Members

        private readonly TimeTasksMapper mTimeTasksMapper;
        private readonly ITimeTasksService mTimeTasksService;
        private readonly IUIManager mUIManager;

        #endregion

        #region Public Properties

        /// <summary>
        /// The list of time tasks for current session to show in this page
        /// </summary>
        public ObservableCollection<CalculatedTimeTaskViewModel> TaskItems { get; set; } = new ObservableCollection<CalculatedTimeTaskViewModel>();

        /// <summary>
        /// The time needed for current session
        /// </summary>
        public TimeSpan SessionTime { get; set; }

        #endregion

        #region Commands

        /// <summary>
        /// The command to start new tasks session
        /// </summary>
        public ICommand StartTasksCommand { get; private set; }

        /// <summary>
        /// The command that cancels current session and goes back to task list
        /// </summary>
        public ICommand CancelCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public TasksSummaryPageViewModel(ITimeTasksService timeTasksService, ITimeTasksCalculator TimeTasksCalculator, TimeTasksMapper tasksMapper, IUIManager uiManager)
        {
            // Create commands
            StartTasksCommand = new RelayCommand(StartTaskSession);
            CancelCommand = new RelayCommand(Cancel);

            // Get injected DI services
            mTimeTasksService = timeTasksService;
            mTimeTasksMapper = tasksMapper;
            mUIManager = uiManager;

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
            var taskContexts = mTimeTasksMapper.ListReverseMap(TaskItems.ToList());

            // Pass it to the time handler to start new session
            mTimeTasksService.ConveyTasksToTimeHandler(taskContexts);

            // Change the page afterwards
            DI.Application.GoToPageAsync(ApplicationPage.TasksSession);
        }

        /// <summary>
        /// Cancels current session and goes back to task list
        /// </summary>
        private void Cancel()
        {
            // TODO: Clear task list in service when logic is done

            // TODO: Find better way
            // Go back to task list
            DI.Application.GoToPageAsync(ApplicationPage.TasksList);
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Loads saved tasks from the <see cref="TimeTasksCalculator"/>
        /// </summary>
        public void LoadTaskList()
        {
            // Calculate selected tasks and get the contexts
            var contexts = mTimeTasksService.GetCalculatedTasksFromManager();

            // Map the list as suitable view models
            TaskItems = new ObservableCollection<CalculatedTimeTaskViewModel>(mTimeTasksMapper.ListMapCal(contexts));

            SessionTime = contexts.SumTimes();
        }

        #endregion
    }
}
