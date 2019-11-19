using MvvmCross.ViewModels;
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
        private string mSearchText;

        /// <summary>
        /// The list of all available time tasks
        /// This is the underlying list and it is not directly displayed in the view
        /// </summary>
        private ObservableCollection<ListTimeTaskItemViewModel> mAllTaskItems;

        #endregion

        #region Public Properties

        /// <summary>
        /// The list of time tasks that are eligible to be displayed in the list
        /// This is based on the <see cref="mAllTaskItems"/> list, but with functionalities like sorting and searching
        /// </summary>
        public ObservableCollection<ListTimeTaskItemViewModel> TaskItems { get; set; } = new ObservableCollection<ListTimeTaskItemViewModel>();

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
                // Set the value
                mSortValue = value;

                // Sort the tasks
                SortVisibleTasks();
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
                // Set the value
                mCheckAllBox = value;

                // Enable or disable all the tasks based on that
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
                // Set the value
                mSearchText = value;

                // Filter all the tasks based on that
                TaskItems = new ObservableCollection<ListTimeTaskItemViewModel>
                    (mAllTaskItems.Where(x => x.Name.Contains(mSearchText) || x.Tags.CreateTagsString().Contains(mSearchText)));

                // Make sure new list of tasks is sorted
                SortVisibleTasks();
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

            // Initially, we want to sort tasks alphabetically by default
            SortValue = SortItems[0];
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
            var pageVM = mTimeTasksMapper.Map(taskVM, mViewModelProvider.GetInjectedPageViewModel<AddNewTimeTaskPageViewModel>());

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

                // Refresh the tasks list
                ReloadTasks();
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

            // Send task contexts to the handler
            mSessionHandler.UpdateTasks(taskContexts);

            // Change the page
            mApplicationViewModel.GoToPage(ApplicationPage.TasksTime);
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Fired whenever this page has loaded and displayed to the user
        /// </summary>
        public override void ViewAppearing()
        {
            // Do base stuff
            base.ViewAppearing();
            
            // Reload task list so it's up to date
            ReloadTasks();

            // Clear task list in session handler for upcoming session to be initially cleared
            mSessionHandler.ClearSessionTasks();
        }

        /// <summary>
        /// Reloads main task list with whatever sits in database currently
        /// </summary>
        private void ReloadTasks()
        {
            // Load saved tasks in database
            var tasks = mTimeTasksService.LoadStoredTasks();

            // Add them to the list as suitable view models
            mAllTaskItems = new ObservableCollection<ListTimeTaskItemViewModel>(mTimeTasksMapper.ListMapToList(tasks));
            TaskItems = mAllTaskItems;
        }

        /// <summary>
        /// Sorts all the tasks that are currently visible in the list
        /// </summary>
        private void SortVisibleTasks()
        {
            // Based on sorting value...
            switch (SortItems.IndexOf(SortValue))
            {
                case 0:
                    // Sort alphabetically
                    TaskItems = new ObservableCollection<ListTimeTaskItemViewModel>(TaskItems.OrderBy(x => x.Name));
                    break;
                case 1:
                    // Sort by created date
                    TaskItems = new ObservableCollection<ListTimeTaskItemViewModel>(TaskItems.OrderBy(x => x.CreationDate));
                    break;
            }
        }

        #endregion
    }
}
