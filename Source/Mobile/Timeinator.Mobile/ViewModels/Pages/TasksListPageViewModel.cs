using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Linq;
using Xamarin.Forms;

namespace Timeinator.Mobile
{
    /// <summary>
    /// The view model for main tasks page
    /// </summary>
    public class TasksListPageViewModel : BasePageViewModel
    {
        #region Private Members

        private readonly TimeTasksMapper mTimeTasksMapper;
        private readonly ITimeTasksService mTimeTasksService;
        private readonly IUIManager mUIManager;

        /// <summary>
        /// A value of selected index in sorting combobox
        /// </summary>
        private int mSortIndex = 10;

        #endregion

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

        /// <summary>
        /// The index choosen in sorting combobox
        /// </summary>
        public int SortIndex
        {
            get => mSortIndex;
            set
            {
                mSortIndex = value;

                switch (mSortIndex)
                {
                    case 0:
                        TaskItems = new ObservableCollection<TimeTaskViewModel>(TaskItems.OrderBy(x => x.Name));
                        break;
                    case 1:
                        TaskItems = new ObservableCollection<TimeTaskViewModel>(TaskItems.OrderBy(x => x.CreationDate));
                        break;
                }
            }
        }

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
        public TasksListPageViewModel(ITimeTasksService timeTasksService, IUIManager uiManager, TimeTasksMapper tasksMapper)
        {
            // Create commands
            AddNewTaskCommand = new RelayCommand(() => mUIManager.ShowModalOnCurrentNavigation(new AddNewTimeTaskControl()));
            EditTaskCommand = new RelayParameterizedCommand(EditTask);
            DeleteTaskCommand = new RelayParameterizedCommand((param) => Device.BeginInvokeOnMainThread(async () => await DeleteTaskAsync(param)));
            UserReadyCommand = new RelayCommand(UserReady);

            // Get injected DI services
            mTimeTasksService = timeTasksService;
            mTimeTasksMapper = tasksMapper;
            mUIManager = uiManager;

            TaskListHelpers.RefreshUITasks += ReloadTasks;

            ReloadTasks();

            // Initially, we want to sort tasks alphabetically by default
            SortIndex = 0;

            // Get every unique tag to display in the view
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
            var pageVM = DI.GetInjectedPageViewModel<AddNewTimeTaskViewModel>();
            pageVM.TaskId = taskVM.Id;
            pageVM.TaskName = taskVM.Name;
            pageVM.TaskDescription = taskVM.Description;
            pageVM.TaskTag = taskVM.Tag;
            pageVM.TaskConstantTime = taskVM.AssignedTime;
            pageVM.TaskImmortality = taskVM.IsImmortal;
            pageVM.TaskPrioritySliderValue = (double)taskVM.Priority;
            pageVM.TaskImportance = taskVM.IsImportant;

            // Show the page with filled info
            mUIManager.ShowModalOnCurrentNavigation(new AddNewTimeTaskControl(pageVM));
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
            var userResponse = await mUIManager.DisplayPopupMessageAsync(popupViewModel);

            // If he agreed...
            if (userResponse)
            {
                // Delete the task
                mTimeTasksService.RemoveTask(mTimeTasksMapper.ReverseMap(taskVM));

                // Refresh the list
                TaskListHelpers.RaiseRefreshEvent();
            }
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
                    taskContexts.Add(mTimeTasksMapper.ReverseMap(task));
            }

            // If user has picked nothing...
            if (taskContexts.Count == 0)
            {
                // Show user an error
                mUIManager.DisplayPopupMessageAsync(new PopupMessageViewModel("Error", "Nie wybrałes żadnego taska!"));

                // Don't do any further actions
                return;
            }

            // If user has not enough time to do all the tasks
            if (UserTime.TotalMinutes - taskContexts.GetConstant().SumTimes().TotalMinutes < taskContexts.GetConstant(true).SumPriorities())
            {
                // Show user an error
                mUIManager.DisplayPopupMessageAsync(new PopupMessageViewModel("Error", "Wybrany czas jest niewystarczający, by zacząc sesję!"));

                // Don't do any further actions
                return;
            }

            // Pass it to the service so it handles it to the manager, with user free time
            mTimeTasksService.ConveyTasksToManager(taskContexts, UserTime);

            // Change the page
            DI.Application.GoToPage(ApplicationPage.TasksPreparation);
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Looks up in every task for it's tag and lists them as strings
        /// </summary>
        private void GetEveryTaskTags()
        {
            // For every task in the list
            foreach (var task in TaskItems)
            {
                // Get it's tag
                var tag = task.Tag;

                // If its not in the list
                if (tag != null && !TaskTags.Contains(tag))
                    // Add it
                    TaskTags.Add(tag);
            }
        }

        /// <summary>
        /// Reloads main task list with whatever sits in database currently
        /// </summary>
        public void ReloadTasks()
        {
            // Load saved tasks in database
            var tasks = mTimeTasksService.LoadStoredTasks();

            // Add them to the list as suitable view models
            TaskItems = new ObservableCollection<TimeTaskViewModel>(mTimeTasksMapper.ListMap(tasks));
        }

        #endregion
    }
}
