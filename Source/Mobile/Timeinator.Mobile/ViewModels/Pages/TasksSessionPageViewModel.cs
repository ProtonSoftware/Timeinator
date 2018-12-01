using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        public bool Paused => !mUserTimeHandler.TaskTimer.Enabled;

        /// <summary>
        /// Remaining time from handler
        /// </summary>
        public TimeSpan TimeRemaining => CurrentTask.AssignedTime - mUserTimeHandler.TimePassed;

        /// <summary>
        /// Remaining time from handler
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

        /// <summary>
        /// Time that task progressed before pausing
        /// </summary>
        public double RecentProgress { get; set; } = 0;

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

        ~TasksSessionPageViewModel()
        {
            RealTimer.Dispose();
            mUserTimeHandler.TaskTimer.Stop();
        }

        #endregion

        private void Stop()
        {
            mUserTimeHandler.StopTask();
            ClickStandardAction();
        }

        private void Resume()
        {
            mUserTimeHandler.ResumeTask();
            ClickStandardAction();
        }

        private void ClickStandardAction()
        {
            UpdateProgress();
            SetupBreakStopwatch();
            OnPropertyChanged(nameof(TaskItems));
        }

        /// <summary>
        /// Switches break params controlling stopwatch and progress buffer
        /// </summary>
        private void SetupBreakStopwatch()
        {
            if (Paused)
            {
                BreakStart = DateTime.Now;
                RecentProgress += mUserTimeHandler.TimePassed.Ticks / CurrentTask.AssignedTime.Ticks;
                BreakTaskTime = new TimeSpan(TimeRemaining.Ticks);
                OnPropertyChanged(nameof(CurrentTask));
            }
            else //break is over -> refresh task times
            {
                mTimeTasksService.ConveyTasksToManager(mTimeTasksMapper.ListReverseMap(TaskItems.ToList()));
                var tmp = mTimeTasksService.GetCalculatedTasksFromManager();
                mUserTimeHandler.UpdateSession(tmp);
                TaskItems = new ObservableCollection<TimeTaskViewModel>(mTimeTasksMapper.ListMap(tmp));
            }
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
                CurrentTask.Progress = 1;
                ContinueUserTasks();
            }
        }

        /// <summary>
        /// Refreshes properties for UI
        /// </summary>
        private void RealTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Paused)
                BreakDuration = DateTime.Now - BreakStart;
            UpdateProgress();
            OnPropertyChanged(nameof(Paused));
            OnPropertyChanged(nameof(TimeRemaining));
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
            UpdateProgress();
            CurrentTask.Progress = 1;
            ContinueUserTasks();
            if (!userResponse)
                StopCommand.Execute(null);
        }
        
        /// <summary>
        /// Checks if to exit to main page or start next task
        /// </summary>
        private void ContinueUserTasks()
        {
            if (CurrentTask != null && CurrentTask.Progress >= 1)
            {
                mTimeTasksService.RemoveFinishedTasks(new List<TimeTaskContext> { mTimeTasksMapper.ReverseMap(CurrentTask) });
                TaskItems.Remove(CurrentTask);
                RecentProgress = 0;
                mUserTimeHandler.StartTask();
            }
            if (TaskItems.Count <= 0)
            {
                RealTimer.Stop();
                DI.Application.GoToPage(ApplicationPage.TasksList);
            }
        }

        /// <summary>
        /// Updates progress on current task
        /// </summary>
        private void UpdateProgress()
        {
            if (CurrentTask == null)
                return;
            CurrentTask.Progress = RecentProgress + (mUserTimeHandler.TimePassed.TotalMilliseconds / CurrentTask.AssignedTime.TotalMilliseconds);
            if (CurrentTask.Progress > 1)
                CurrentTask.Progress = 1;
            TaskProgress = CurrentTask.Progress;
        }

        /// <summary>
        /// Loads saved tasks from the <see cref="UserTimeHandler"/>
        /// </summary>
        public void LoadTaskList()
        {
            var tasks = mUserTimeHandler.DownloadSession();
            TaskItems = new ObservableCollection<TimeTaskViewModel>(mTimeTasksMapper.ListMap(tasks));
        }
    }
}
