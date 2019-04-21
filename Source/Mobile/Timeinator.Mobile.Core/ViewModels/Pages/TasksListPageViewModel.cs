using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Linq;
using Timeinator.Core;
using MvvmCross.ViewModels;
using MvvmCross;
using MvvmCross.Base;

namespace Timeinator.Mobile.Core
{
    /// <summary>
    /// The view model for main tasks page
    /// </summary>
    public class TasksListPageViewModel : MvxViewModel
    {
        #region Private Members

        private readonly TimeTasksMapper mTimeTasksMapper;
        private readonly ITimeTasksService mTimeTasksService;
        private readonly IUIManager mUIManager;

        /// <summary>
        /// A value of selected item in sorting combobox
        /// </summary>
        private string mSortValue;

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

        public ObservableCollection<string> SortItems { get; set; } = new ObservableCollection<string>
        {
            "Alfabetycznie",
            "Po dacie dodania"
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
                        TaskItems = new ObservableCollection<TimeTaskViewModel>(TaskItems.OrderBy(x => x.Name));
                        break;
                    case 1:
                        TaskItems = new ObservableCollection<TimeTaskViewModel>(TaskItems.OrderBy(x => x.CreationDate));
                        break;
                }
            }
        }

        /// <summary>
        /// The value of check-all-tasks checkbox
        /// </summary>
        public bool CheckAllBox { get; set; }

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
        public TasksListPageViewModel(ITimeTasksService timeTasksService, IUIManager uiManager, TimeTasksMapper tasksMapper)
        {
            // Create commands
            AddNewTaskCommand = new RelayCommand(() => mUIManager.GoToViewModelPage(DI.GetInjectedPageViewModel<AddNewTimeTaskPageViewModel>()));
            EditTaskCommand = new RelayParameterizedCommand(EditTask);
            DeleteTaskCommand = new RelayParameterizedCommand(async (param) => await uiManager.ExecuteOnMainThread(async () => await DeleteTaskAsync(param)));
            UserReadyCommand = new RelayCommand(UserReady);
            OpenSettingsCommand = new RelayCommand(() => mUIManager.GoToViewModelPage(DI.GetInjectedPageViewModel<SettingsPageViewModel>()));
            OpenAboutCommand = new RelayCommand(() => mUIManager.GoToViewModelPage(DI.GetInjectedPageViewModel<AboutPageViewModel>()));

            // Get injected DI services
            mTimeTasksService = timeTasksService;
            mTimeTasksMapper = tasksMapper;
            mUIManager = uiManager;

            // Attach reloading function to an event, so everytime tasks need update, it can be fired and updated
            TaskListHelpers.RefreshUITasks += ReloadTasks;

            // Initially load every task
            ReloadTasks();

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
            var taskVM = param as TimeTaskViewModel;

            // Create edit page's view model based on that
            var pageVM = DI.GetInjectedPageViewModel<AddNewTimeTaskPageViewModel>();
            pageVM.TaskId = taskVM.Id;
            pageVM.TaskName = taskVM.Name;
            pageVM.TaskDescription = taskVM.Description;
            pageVM.TaskTag = taskVM.Tag;
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

            // Send taskContexts to TasksTimePage
            mTimeTasksService.ConveyTasksToManager(taskContexts, default);

            // Change the page
            DI.Application.GoToPage(ApplicationPage.TasksTime);
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
