using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
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

        /// <summary>
        /// The command to reorder specified task in the list
        /// </summary>
        public ICommand ReorderCommand { get; set; }

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
            ReorderCommand = new RelayParameterizedCommand(Reorder);

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

            // Pass it to the service
            mTimeTasksService.SetSessionTasks(taskContexts);

            // Change the page afterwards
            DI.Application.GoToPageAsync(ApplicationPage.TasksSession);
        }

        /// <summary>
        /// Cancels current session and goes back to task list
        /// </summary>
        private void Cancel()
        {
            // Clear task list in service
            mTimeTasksService.ClearSessionTasks();

            // TODO: Find better way
            // Go back to task list
            DI.Application.GoToPageAsync(ApplicationPage.TasksList);
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
                throw new Exception("Attempted to reorder tasks without providing the positions correctly.");
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
            TaskItems = new ObservableCollection<CalculatedTimeTaskViewModel>(reorderedList);
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Loads saved tasks from the <see cref="TimeTasksCalculator"/>
        /// </summary>
        public void LoadTaskList()
        {
            // Calculate selected tasks and get the contexts
            var contexts = mTimeTasksService.GetCalculatedTasks();

            // Map the list as suitable view models
            TaskItems = new ObservableCollection<CalculatedTimeTaskViewModel>(mTimeTasksMapper.ListMapCal(contexts));

            SessionTime = contexts.SumTimes();
        }

        #endregion
    }
}
