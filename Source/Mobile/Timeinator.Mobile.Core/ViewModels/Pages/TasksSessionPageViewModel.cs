using MvvmCross.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Timeinator.Core;

namespace Timeinator.Mobile.Core
{
    /// <summary>
    /// The view model for tasks session page
    /// </summary>
    public class TasksSessionPageViewModel : MvxViewModel
    {
        #region Private Members

        private readonly TimeTasksMapper mTimeTasksMapper;
        private readonly ITimeTasksService mTimeTasksService;
        private readonly IUIManager mUIManager;

        #endregion

        #region Public Properties

        /// <summary>
        /// The list of time tasks for current session that are not the current one
        /// </summary>
        public ObservableCollection<TimeTaskViewModel> RemainingTasks { get; set; } = new ObservableCollection<TimeTaskViewModel>();

        /// <summary>
        /// The current task the user is doing on the session
        /// </summary>
        public TimeTaskViewModel CurrentTask { get; set; }

        /// <summary>
        /// Progress of current task shown in the progress bar on UI
        /// </summary>
        public double TaskProgress { get; set; }

        /// <summary>
        /// Current session length from the start of it
        /// </summary>
        public TimeSpan SessionDuration => mTimeTasksService.SessionDuration;

        /// <summary>
        /// The remaining time left of current task
        /// </summary>
        public TimeSpan TimeRemaining => mTimeTasksService.CurrentTaskTimeLeft;

        public bool Paused => false;

        #endregion

        #region Commands

        /// <summary>
        /// The command to pause current task
        /// </summary>
        public ICommand PauseCommand { get; private set; }

        /// <summary>
        /// The command to resume current task, available when task is paused
        /// </summary>
        public ICommand ResumeCommand { get; private set; }

        /// <summary>
        /// The command to finish current task and go for the next one
        /// </summary>
        public ICommand FinishCommand { get; private set; }

        #endregion

        #region Constructor 

        /// <summary>
        /// Default constructor
        /// </summary>
        public TasksSessionPageViewModel(ITimeTasksService timeTasksService, IUIManager uiManager, TimeTasksMapper tasksMapper)
        {
            // Create commands
            PauseCommand = new RelayCommand(PauseTaskAsync);
            ResumeCommand = new RelayCommand(ResumeTaskAsync);
            FinishCommand = new RelayCommand(FinishTaskAsync);

            // Get injected DI services
            mTimeTasksService = timeTasksService;
            mTimeTasksMapper = tasksMapper;
            mUIManager = uiManager;

            // Initialize this session
            InitializeSession();
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// Pauses current task and starts the break
        /// </summary>
        private async void PauseTaskAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Finishes the break and resumes current task
        /// </summary>
        private async void ResumeTaskAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Finishes the current task by removing it and goes to the next one
        /// </summary>
        private async void FinishTaskAsync()
        {
            // Ask the user if he's certain to finish the task before it ends
            var popupViewModel = new PopupMessageViewModel
                (
                    "Koniec zadania",
                    "Na pewno chcesz zakończyć zadanie?",
                    "Tak",
                    "Nie"
                );
            var userResponse = await mUIManager.DisplayPopupMessageAsync(popupViewModel);

            // If he disagreed
            if (!userResponse)
            {
                // Don't finish
                return;
            }

            // TODO: Finish task
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Initializes the session on this page
        /// </summary>
        private void InitializeSession()
        {
            // Create an action to fire whenever session timer ticks
            var updatePropertiesAction = new Action(UpdateSessionProperties);

            // Create an action to fire whenever task finishes
            var taskFinishAction = new Action(TaskTimeFinishAsync);

            // Start new session and get all the tasks
            var contexts = mTimeTasksService.StartSession(updatePropertiesAction, taskFinishAction);

            // At the start of the session, first task in the list is always current one, so set it accordingly
            var currentTask = contexts.ElementAt(0);
            CurrentTask = mTimeTasksMapper.Map(currentTask);

            // Delete current task from the contexts
            contexts.Remove(currentTask);

            // And set the remaining list tasks
            RemainingTasks = new ObservableCollection<TimeTaskViewModel>(mTimeTasksMapper.ListMap(contexts));
        }

        /// <summary>
        /// Called when current task's time runs out
        /// </summary>
        private async void TaskTimeFinishAsync()
        {
            var popupViewModel = new PopupMessageViewModel
                (
                    "Skończył się czas",
                    "Skończył się czas na zadanie, co chcesz teraz zrobić?",
                    "Następne zadanie",
                    "Nie, chcę przerwę"
                );
            var userResponse = await mUIManager.DisplayPopupMessageAsync(popupViewModel);
        }

        /// <summary>
        /// Updates every session property with new values
        /// </summary>
        private void UpdateSessionProperties()
        {
            // Simply use the helper to fire every property's change event, for now it works just fine
            // Potentially in the future, update only required properties, not everything
            RaiseAllPropertiesChanged();
        }

        #endregion
    }
    /*
        #region Private Members

        private readonly TimeTasksMapper mTimeTasksMapper;
        private readonly ITimeTasksService mTimeTasksService;
        private readonly IUserTimeHandler mUserTimeHandler;
        private readonly IUIManager mUIManager;

        /// <summary>
        /// Stores time loss of CurrentTask
        /// </summary>
        private TimeSpan mCurrentTimeLoss;

        /// <summary>
        /// Stores remaining time of Current Task when paused
        /// </summary>
        private TimeSpan mRemainingTaskTime;

        #endregion

        #region Public Properties

        /// <summary>
        /// The list of time tasks for current session to show in this page
        /// </summary>
        public ObservableCollection<TimeTaskViewModel> TaskItems { get; set; } = new ObservableCollection<TimeTaskViewModel>();

        /// <summary>
        /// Returns ViewModel of current task
        /// </summary>
        public TimeTaskViewModel CurrentTask {
            get
            {
                try { return TaskItems.ElementAt(0); }
                catch { return null; }
            }
        }

        /// <summary>
        /// Holds current task state
        /// </summary>
        public bool Paused => !mUserTimeHandler.SessionRunning;

        /// <summary>
        /// Remaining time from handler
        /// </summary>
        public TimeSpan TimeRemaining => CurrentTask?.AssignedTime != default ? CurrentTask.AssignedTime - mUserTimeHandler.TimePassed : new TimeSpan(0);

        /// <summary>
        /// Time lost since break start
        /// </summary>
        public TimeSpan CurrentTimeLoss => TimeSpan.FromSeconds(BreakDuration.TotalSeconds * mCurrentTimeLoss.TotalSeconds);

        /// <summary>
        /// Remaining task time displayed on break
        /// </summary>
        public TimeSpan BreakTaskTime { get; set; }

        /// <summary>
        /// Progress of current task
        /// </summary>
        public double TaskProgress { get; set; }

        /// <summary>
        /// Timer refreshing UI every sec
        /// </summary>
        public Timer RealTimer { get; set; } = new Timer(1000);

        /// <summary>
        /// Break started time - time when user paused task
        /// </summary>
        public DateTime BreakStart { get; set; }

        /// <summary>
        /// Current break length in a timespan
        /// </summary>
        public TimeSpan BreakDuration { get; set; }

        #endregion

        #region Commands

        public ICommand StopCommand { get; private set; }
        public ICommand ResumeCommand { get; private set; }
        public ICommand FinishCommand { get; private set; }

        #endregion

        #region Constructor 

        /// <summary>
        /// Default constructor
        /// </summary>
        public TasksSessionPageViewModel(ITimeTasksService timeTasksService, IUserTimeHandler userTimeHandler, IUIManager uiManager, TimeTasksMapper tasksMapper)
        {
            // Create commands
            StopCommand = new RelayCommand(Stop);
            ResumeCommand = new RelayCommand(Resume);
            FinishCommand = new RelayCommand(async () => await FinishAsync());

            // Get injected DI services
            mTimeTasksService = timeTasksService;
            mUserTimeHandler = userTimeHandler;
            mTimeTasksMapper = tasksMapper;
            mUIManager = uiManager;

            mUserTimeHandler.Updated += () => { LoadTaskList(); RefreshProperties(); };
            mUserTimeHandler.TimesUp += async () => await mUIManager.ExecuteOnMainThread(async () => await UserTimeHandler_TimesUpAsync());
            RealTimer.Elapsed += RealTimer_Elapsed;  

            LoadTaskList();
            RealTimer.Start();
        }

        #endregion

        private void Stop()
        {
            mUserTimeHandler.StopTask();
            RefreshProperties();
        }

        private void Resume()
        {
            mUserTimeHandler.RefreshTasksState();
            mUserTimeHandler.ResumeTask();
            LoadTaskList();
            RefreshProperties();
        }

        private void RefreshProperties()
        {
            if (CurrentTask == null)
                return;
            if (Paused)
            {
                BreakStart = DateTime.Now;
                mRemainingTaskTime = new TimeSpan(TimeRemaining.Ticks);
                RaisePropertyChanged(nameof(CurrentTask));
            }
            UpdateProgressBar();
            RaisePropertyChanged(nameof(TaskItems));
        }

        /// <summary>
        /// Refreshes properties for UI
        /// </summary>
        private void RealTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (CurrentTask == null)
            {
                RealTimer.Stop();
                DI.Application.GoToPageAsync(ApplicationPage.TasksList);
                return;
            }
            if (Paused)
            {
                BreakDuration = DateTime.Now - BreakStart;
                BreakTaskTime = mRemainingTaskTime - CurrentTimeLoss;
            }
            else
            {
                UpdateProgressBar();
                RaisePropertyChanged(nameof(TimeRemaining));
            }
            RaisePropertyChanged(nameof(Paused));
        }

        /// <summary>
        /// Prompts dialog whether to remove current task and does it
        /// </summary>
        private async Task FinishAsync()
        {
            var popupViewModel = new PopupMessageViewModel
                (
                    "Koniec zadania", 
                    "Na pewno chcesz zakończyć zadanie?",
                    "Tak", 
                    "Nie"
                );
            var userResponse = await mUIManager.DisplayPopupMessageAsync(popupViewModel);
            if (userResponse)
            {
                mUserTimeHandler.FinishTask();
                ContinueUserTasks();
                LoadTaskList();
            }
        }

        /// <summary>
        /// Fired when the current task has run out of time 
        /// </summary>
        private async Task UserTimeHandler_TimesUpAsync()
        {
            var popupViewModel = new PopupMessageViewModel
                (
                    "Skończył się czas", 
                    "Skończył się czas na zadanie, co chcesz teraz zrobić?",
                    "Następne zadanie", 
                    "Nie, chcę przerwę"
                );
            var userResponse = await mUIManager.DisplayPopupMessageAsync(popupViewModel);
            mUserTimeHandler.FinishTask();
            TaskProgress = 1;
            ContinueUserTasks();
            if (!userResponse && CurrentTask != null)
                StopCommand.Execute(null);
        }
        
        /// <summary>
        /// Checks if to exit to main page or start next task
        /// </summary>
        private void ContinueUserTasks()
        {
            mUserTimeHandler.CleanTasks();
            mUserTimeHandler.RefreshTasksState();
            LoadTaskList();
            mUserTimeHandler.StartTask();
        }

        /// <summary>
        /// Loads tasks from the current implementation of <see cref="IUserTimeHandler"/>
        /// </summary>
        public void LoadTaskList()
        {
            var tasks = mUserTimeHandler.DownloadSession();
            TaskItems = new ObservableCollection<TimeTaskViewModel>(mTimeTasksMapper.ListMap(tasks));
            mCurrentTimeLoss = mUserTimeHandler.TimeLossValue();
        }

        /// <summary>
        /// Updates progress on current task
        /// </summary>
        private void UpdateProgressBar()
        {
            var recent = mUserTimeHandler.RecentProgress;
            TaskProgress = recent + (1.0 - recent) * (mUserTimeHandler.TimePassed.TotalMilliseconds / CurrentTask.AssignedTime.TotalMilliseconds);
            if (TaskProgress > 1)
                TaskProgress = 1;
        }
    */
}
