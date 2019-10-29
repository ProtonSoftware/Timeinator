using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Timeinator.Core;

namespace Timeinator.Mobile.Domain
{
    /// <summary>
    /// The view model for tasks summary page
    /// </summary>
    public class TasksSummaryPageViewModel : MvxViewModel
    {
        #region Private Members

        private readonly TimeTasksMapper mTimeTasksMapper;
        private readonly ISessionHandler mSessionHandler;
        private readonly IViewModelProvider mViewModelProvider;
        private readonly ApplicationViewModel mApplicationViewModel;

        #endregion

        #region Public Properties

        /// <summary>
        /// The list of time tasks for current session to show in this page
        /// </summary>
        public ObservableCollection<SummaryTimeTaskItemViewModel> TaskItems { get; set; } = new ObservableCollection<SummaryTimeTaskItemViewModel>();

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

        /// <summary>
        /// The command to reorder specified task in the list
        /// </summary>
        public ICommand ReorderCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public TasksSummaryPageViewModel(
            ISessionHandler sessionHandler,
            TimeTasksMapper tasksMapper,
            ApplicationViewModel applicationViewModel,
            IViewModelProvider viewModelProvider)
        {
            // Create commands
            StartTasksCommand = new RelayCommand(StartTaskSession);
            CancelCommand = new RelayCommand(Cancel);
            ReorderCommand = new RelayParameterizedCommand(Reorder);

            // Get injected DI services
            mSessionHandler = sessionHandler;
            mTimeTasksMapper = tasksMapper;
            mApplicationViewModel = applicationViewModel;
            mViewModelProvider = viewModelProvider;

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
            // Get tasks from Handler sorted like TaskItems
            var taskContexts = mTimeTasksMapper.SortLike(TaskItems.Select(vm => vm as TimeTaskViewModel).ToList(), mSessionHandler.GetTasks().WholeList);

            // Pass it to the service
            mSessionHandler.UpdateTasks(taskContexts);

            // Create brand-new view model for session page
            var sessionViewModel = mViewModelProvider.GetInjectedPageViewModel<TasksSessionPageViewModel>();
            sessionViewModel.InitializeSessionCommand.Execute(null);

            // Change to session page
            mApplicationViewModel.GoToPage(ApplicationPage.TasksSession, sessionViewModel);
        }

        /// <summary>
        /// Cancels current session and goes back to task list
        /// </summary>
        private void Cancel()
        {
            // Clear task list in service
            mSessionHandler.ClearSessionTasks();

            // TODO: Find better way
            // Go back to task list
            mApplicationViewModel.GoToPage(ApplicationPage.TasksList);
        }

        /// <summary>
        /// Reorders the task list when an item is moved
        /// </summary>
        /// <param name="param">The from-to positions provided as Tuple of two ints</param>
        private void Reorder(object param)
        {
            // Get tuple from our parameter
            if (!(param is Tuple<int, int> positions))
            {
                // Throw an exception because we explicitely say its Tuple in the code, so something must've gone seriously wrong
                throw new Exception(LocalizationResource.AttemptToReorderTasksNoPosition);
            }

            // Extract from and to positions from the tuple
            var posFrom = positions.Item1;
            var posTo = positions.Item2;

            // Prepare list of the new order
            var orderList = new List<int>();

            // Loop each index
            for (var i = 0; i < TaskItems.Count; i++)
            {
                // If its not posFrom
                if (i != posFrom)
                {
                    // Just add the index
                    orderList.Add(i);
                }
            }

            // Now we have the whole list in the order without posFrom item, so add it in the proper place
            orderList.Insert(posTo, posFrom);

            // Reorder the task items by the order list we prepared
            var reorderedList = orderList.Select(item => TaskItems.ElementAt(item)).ToList();

            // Replace old list with reordered one
            TaskItems = new ObservableCollection<SummaryTimeTaskItemViewModel>(reorderedList);
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Loads calculated task list
        /// </summary>
        private void LoadTaskList()
        {
            // Update tasks times
            mSessionHandler.Calculate();

            // Calculate selected tasks and get the contexts
            var contexts = mSessionHandler.GetTasks().WholeList;

            // Map the list as suitable view models
            TaskItems = new ObservableCollection<SummaryTimeTaskItemViewModel>(mTimeTasksMapper.ListMapToSummary(contexts));

            // Calculate session time
            SessionTime = new TimeSpan(contexts.Select(x => x.AssignedTime.Ticks).Sum());
        }

        #endregion
    }
}
