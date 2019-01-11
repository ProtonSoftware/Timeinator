using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using Xamarin.Forms;

namespace Timeinator.Mobile
{
    /// <summary>
    /// The view model for tasks session page
    /// </summary>
    public class TasksSessionPageViewModel : BasePageViewModel
    {
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
        public bool Paused => !mUserTimeHandler.TimerStateRunning();

        /// <summary>
        /// Remaining time from handler
        /// </summary>
        public TimeSpan TimeRemaining {
            get
            {
                try { return CurrentTask.AssignedTime - mUserTimeHandler.TimePassed; }
                catch { return default(TimeSpan); }
            }
        }

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

            mUserTimeHandler.TimesUp += () => Device.BeginInvokeOnMainThread(async () => await UserTimeHandler_TimesUpAsync());
            RealTimer.Elapsed += RealTimer_Elapsed;  

            LoadTaskList();
            RealTimer.Start();
        }

        #endregion

        private void Stop()
        {
            mUserTimeHandler.StopTask();
            ClickStandardAction();
        }

        private void Resume()
        {
            mUserTimeHandler.RefreshTasksState(mTimeTasksService);
            mUserTimeHandler.ResumeTask();
            LoadTaskList();
            ClickStandardAction();
        }

        private void ClickStandardAction()
        {
            if (Paused)
            {
                if (CurrentTask == null)
                    return;
                BreakStart = DateTime.Now;
                mRemainingTaskTime = new TimeSpan(TimeRemaining.Ticks);
                OnPropertyChanged(nameof(CurrentTask));
            }
            UpdateProgressBar();
            OnPropertyChanged(nameof(TaskItems));
        }

        /// <summary>
        /// Refreshes properties for UI
        /// </summary>
        private void RealTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Paused)
            {
                BreakDuration = DateTime.Now - BreakStart;
                BreakTaskTime = mRemainingTaskTime - CurrentTimeLoss;
            }
            else
            {
                UpdateProgressBar();
                OnPropertyChanged(nameof(TimeRemaining));
            }
            OnPropertyChanged(nameof(Paused));
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
                mUserTimeHandler.RefreshTasksState(mTimeTasksService);
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
            if (!userResponse && CurrentTask!=null)
                StopCommand.Execute(null);
        }
        
        /// <summary>
        /// Checks if to exit to main page or start next task
        /// </summary>
        private void ContinueUserTasks()
        {
            mUserTimeHandler.RemoveAndContinueTasks(mTimeTasksService);
            LoadTaskList();
            if (TaskItems.Count <= 0)
            {
                RealTimer.Stop();
                DI.Application.GoToPage(ApplicationPage.TasksList);
            }
        }

        /// <summary>
        /// Loads tasks from the <see cref="UserTimeHandler"/>
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
            if (CurrentTask == null)
                return;
            var recent = mUserTimeHandler.RecentProgress;
            TaskProgress = recent + (1.0 - recent) * (mUserTimeHandler.TimePassed.TotalMilliseconds / CurrentTask.AssignedTime.TotalMilliseconds);
            if (TaskProgress > 1)
                TaskProgress = 1;
        }
    }
}
