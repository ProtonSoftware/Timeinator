using MvvmCross.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Timeinator.Core;

namespace Timeinator.Mobile.Domain
{
    /// <summary>
    /// The view model for main tasks page
    /// </summary>
    public class TasksListPageViewModel : MvxViewModel
    {
        #region Private Members

        private readonly TimeTasksMapper mTimeTasksMapper;
        private readonly ITimeTasksService mTimeTasksService;
        private readonly ISessionHandler mSessionHandler;
        private readonly IUIManager mUIManager;
        private readonly IViewModelProvider mViewModelProvider;
        private readonly ApplicationViewModel mApplicationViewModel;

        /// <summary>
        /// A value of selected item in sorting combobox
        /// </summary>
        private string mSortValue;

        /// <summary>
        /// A value of checkbox for selecting all tasks
        /// By default, select all the tasks
        /// </summary>
        private bool mCheckAllBox = true;

        /// <summary>
        /// The current text input from the search bar
        /// </summary>
        public string mSearchText;

        #endregion

        #region Public Properties

        /// <summary>
        /// The list of time tasks for current session to show in this page
        /// </summary>
        public ObservableCollection<ListTimeTaskItemViewModel> TaskItems { get; set; } = new ObservableCollection<ListTimeTaskItemViewModel>();

        /// <summary>
        /// The list of every tag that are associated with current list of tasks
        /// </summary>
        public ObservableCollection<string> TaskTags { get; set; } = new ObservableCollection<string>();

        /// <summary>
        /// The list of possible task list items sorting methods
        /// </summary>
        public ObservableCollection<string> SortItems { get; set; } = new ObservableCollection<string>
        {
            LocalizationResource.Alphabetical,
            LocalizationResource.CreatedDate
        };

        /// <summary>
        /// The item choosen in sorting combobox
        /// </summary>
        public string SortValue
        {
            get => mSortValue;
            set
            {
                mSortValue = value;

                switch (SortItems.IndexOf(mSortValue))
                {
                    case 0:
                        TaskItems = new ObservableCollection<ListTimeTaskItemViewModel>(TaskItems.OrderBy(x => x.Name));
                        break;
                    case 1:
                        TaskItems = new ObservableCollection<ListTimeTaskItemViewModel>(TaskItems.OrderBy(x => x.CreationDate));
                        break;
                }
            }
        }

        /// <summary>
        /// The value of check-all-tasks checkbox
        /// </summary>
        public bool CheckAllBox
        {
            get => mCheckAllBox;
            set
            {
                mCheckAllBox = value;

                foreach (var task in TaskItems)
                    task.IsEnabled = mCheckAllBox;
            }
        }

        /// <summary>
        /// The current text input from the search bar
        /// </summary>
        public string SearchText
        {
            get => mSearchText;
            set
            {
                mSearchText = value;

                TaskListHelpers.RaiseRefreshEvent();
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

        /// <summary>
        /// The command to show a settings page
        /// </summary>
        public ICommand OpenSettingsCommand { get; private set; }

        /// <summary>
        /// The command to show an about page
        /// </summary>
        public ICommand OpenAboutCommand { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public TasksListPageViewModel(
            ITimeTasksService timeTasksService,
            ISessionHandler sessionHandler,
            IUIManager uiManager,
            IViewModelProvider viewModelProvider,
            TimeTasksMapper tasksMapper,
            ApplicationViewModel applicationViewModel)
        {
            // Create commands
            AddNewTaskCommand = new RelayCommand(() => mApplicationViewModel.GoToPage(ApplicationPage.AddNewTask));
            EditTaskCommand = new RelayParameterizedCommand(EditTask);
            DeleteTaskCommand = new RelayParameterizedCommand(async (param) => await mUIManager.ExecuteOnMainThread(async () => await DeleteTaskAsync(param)));
            UserReadyCommand = new RelayCommand(() => UserReadyAsync());
            OpenSettingsCommand = new RelayCommand(() => mApplicationViewModel.GoToPage(ApplicationPage.Settings));
            OpenAboutCommand = new RelayCommand(() => mApplicationViewModel.GoToPage(ApplicationPage.About));

            // Get injected DI services
            mTimeTasksService = timeTasksService;
            mSessionHandler = sessionHandler;
            mTimeTasksMapper = tasksMapper;
            mUIManager = uiManager;
            mViewModelProvider = viewModelProvider;
            mApplicationViewModel = applicationViewModel;

            // Remove all outdated refresh events
            TaskListHelpers.CleanRefreshEvent();

            // Attach reloading function to an event, so everytime tasks need update, it can be fired and updated
            TaskListHelpers.RefreshUITasks += ReloadTasks;

            // Initially load every task
            TaskListHelpers.RaiseRefreshEvent();

            // Initially, we want to sort tasks alphabetically by default
            SortValue = SortItems[0];

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
            var taskVM = param as ListTimeTaskItemViewModel;

            // Create edit page's view model based on that
            var pageVM = mViewModelProvider.GetInjectedPageViewModel<AddNewTimeTaskPageViewModel>();
            // TODO: Use Mapper
            pageVM.TaskId = taskVM.Id;
            pageVM.TaskName = taskVM.Name;
            pageVM.TaskDescription = taskVM.Description;
            pageVM.TaskTagsString = taskVM.Tags.CreateTagsString();
            pageVM.TaskConstantTime = taskVM.AssignedTime;
            pageVM.TaskImmortality = taskVM.IsImmortal;
            pageVM.TaskPrioritySliderValue = (int)taskVM.Priority;
            pageVM.TaskImportance = taskVM.IsImportant;

            // Show the page with filled info
            mUIManager.GoToViewModelPage(pageVM);
        }

        /// <summary>
        /// Deletes the specified task from the list and request database deletion
        /// </summary>
        /// <param name="param">The task to delete</param>
        private async Task DeleteTaskAsync(object param)
        {
            // Get the task view model
            var taskVM = param as ListTimeTaskItemViewModel;

            // Ask the user if he is certain
            var popupViewModel = new PopupMessageViewModel
                (
                    LocalizationResource.TaskDeletion,
                    string.Format(LocalizationResource.QuestionAreYouCertainToDeleteTask, taskVM.Name),
                    LocalizationResource.Yes,
                    LocalizationResource.No
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
        private async Task UserReadyAsync()
        {
            // Convert our collection to suitable list of contexts 
            var taskContexts = TaskItems
                        // Take only selected tasks
                        .Where(task => task.IsEnabled)
                        // Map them as contexts
                        .Select(task => mTimeTasksMapper.ReverseMap(task))
                        .ToList();

            // If user has picked nothing...
            if (taskContexts.Count == 0)
            {
                // Show user an error
                await mUIManager.DisplayPopupMessageAsync(new PopupMessageViewModel(LocalizationResource.Error, LocalizationResource.NoTaskSelected));

                // Don't do any further actions
                return;
            }

            // Reset handler to a clean state
            mSessionHandler.ClearSessionTasks();

            // Send task contexts to the service
            mSessionHandler.UpdateTasks(taskContexts);

            // Change the page
            mApplicationViewModel.GoToPage(ApplicationPage.TasksTime);
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Looks up in every task for it's tags and lists them as strings
        /// </summary>
        private void GetEveryTaskTags()
        {
            // For every task in the list
            foreach (var task in TaskItems)
            {
                // Skip no-tags tasks
                if (task.Tags == null || task.Tags.Count < 1)
                    continue;

                // For each task's tag...
                foreach (var tag in task.Tags)
                {
                    // If its not in the list
                    if (!TaskTags.Contains(tag))
                        // Add it
                        TaskTags.Add(tag);
                }
            }
        }

        /// <summary>
        /// Reloads main task list with whatever sits in database currently
        /// </summary>
        public void ReloadTasks()
        {
            // Load saved tasks in database
            var tasks = mTimeTasksService.LoadStoredTasks(SearchText);

            // Add them to the list as suitable view models
            TaskItems = new ObservableCollection<ListTimeTaskItemViewModel>(mTimeTasksMapper.ListMapToList(tasks));
        }

        #endregion
    }
}
