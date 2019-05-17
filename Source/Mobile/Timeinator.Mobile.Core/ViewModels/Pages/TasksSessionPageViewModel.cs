using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

        /// <summary>
        /// Stores the list of already finished tasks in this session
        /// </summary>
        private List<TimeTaskContext> mFinishedTasks = new List<TimeTaskContext>();

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
        public double TaskProgress => mTimeTasksService.CurrentTaskCalculatedProgress;

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
        public ICommand FinishTaskCommand { get; private set; }

        /// <summary>
        /// The command to end current session
        /// </summary>
        public ICommand EndSessionCommand { get; private set; }

        #endregion

        #region Constructor 

        /// <summary>
        /// Default constructor
        /// </summary>
        public TasksSessionPageViewModel(ITimeTasksService timeTasksService, IUIManager uiManager, TimeTasksMapper tasksMapper)
        {
            // Create commands
            PauseCommand = new RelayCommand(PauseTask);
            ResumeCommand = new RelayCommand(ResumeTask);
            FinishTaskCommand = new RelayCommand(FinishTaskAsync);
            EndSessionCommand = new RelayCommand(EndSessionAsync);

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
        private void PauseTask()
        {
            // Save current task's progress
            CurrentTask.AssignedTime = mTimeTasksService.CurrentTaskTimeLeft;

            // Start the break
            mTimeTasksService.StartBreak();
        }

        /// <summary>
        /// Finishes the break and resumes current task
        /// </summary>
        private void ResumeTask()
        {
            // Stop the break
            mTimeTasksService.EndBreak();
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
                    "Na pewno chcesz zakończyć aktualne zadanie?",
                    "Tak",
                    "Nie"
                );
            var userResponse = await mUIManager.DisplayPopupMessageAsync(popupViewModel);

            // If he agreed...
            if (userResponse)
            {
                // Finish task
                FinishCurrentTask();
            }
        }

        /// <summary>
        /// Ends current user session, if he decides to
        /// </summary>
        private async void EndSessionAsync()
        {
            // Ask the user if he's certain to end the session
            var popupViewModel = new PopupMessageViewModel
                (
                    "Koniec sesji",
                    "Na pewno chcesz zakończyć sesję?",
                    "Tak",
                    "Nie"
                );
            var userResponse = await mUIManager.DisplayPopupMessageAsync(popupViewModel);

            // If he agreed...
            if (userResponse)
            {
                // Save current task as finished one
                var finishedTask = mTimeTasksMapper.ReverseMap(CurrentTask);
                mFinishedTasks.Add(finishedTask);

                // End the session
                EndSession();
            }
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
            SetCurrentTask(0, mTimeTasksMapper.ListMap(contexts));
        }

        /// <summary>
        /// Called when current task's time runs out
        /// </summary>
        private async void TaskTimeFinishAsync()
        {
            // Ask the user if he wants to finish the task or take a break
            var popupViewModel = new PopupMessageViewModel
                (
                    "Skończył się czas",
                    "Skończył się czas na zadanie, co chcesz teraz zrobić?",
                    "Następne zadanie",
                    "Nie, chcę przerwę"
                );
            var userResponse = await mUIManager.DisplayPopupMessageAsync(popupViewModel);

            // If he agreed...
            if (userResponse)
            {
                // Finish the task
                FinishCurrentTask();
            }
            // Otherwise...
            else
            {
                // Start the break
                mTimeTasksService.StartBreak();
            }
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

        /// <summary>
        /// Finishes current displayed task
        /// </summary>
        private void FinishCurrentTask()
        {
            // Get finished task's context
            var finishedTask = mTimeTasksMapper.ReverseMap(CurrentTask);

            // Add finished task to the list for future reference
            mFinishedTasks.Add(finishedTask);

            // Set next task on the list
            SetCurrentTask(0, RemainingTasks.ToList());

            // Get new task's context
            var newTask = mTimeTasksMapper.ReverseMap(CurrentTask);

            // And start it in the session
            mTimeTasksService.StartNextTask(newTask);
        }


        /// <summary>
        /// Sets specified task from the list to the current one, also removing it from the remaining list
        /// </summary>
        /// <param name="index">The index of the task to set as current</param>
        /// <param name="viewModels">The list of task view models</param>
        private void SetCurrentTask(int index, List<TimeTaskViewModel> viewModels)
        {
            try
            {
                // Set the task at specified index
                CurrentTask = viewModels.ElementAt(index);
            }
            catch (ArgumentOutOfRangeException)
            {
                // If we get here, the index is not in the list
                // So the list is either empty...
                if (viewModels.Count == 0)
                {
                    // Then we finish current session
                    EndSession();
                }
                // Or something went wrong and we tried to start the task that doesn't exist
                else
                {
                    Debugger.Break();
                }
            }

            // Delete current task from the list
            viewModels.Remove(CurrentTask);

            // And set the remaining list tasks
            RemainingTasks = new ObservableCollection<TimeTaskViewModel>(viewModels);
        }

        /// <summary>
        /// Ends this session completely
        /// </summary>
        private void EndSession()
        {
            // Send finished tasks list for removal
            mTimeTasksService.RemoveFinishedTasks(mFinishedTasks);

            // Go to first page
            DI.Application.GoToPageAsync(ApplicationPage.TasksList);
        }

        #endregion
    }
    /*
        public bool Paused => !mUserTimeHandler.SessionRunning;
        public TimeSpan TimeRemaining => CurrentTask?.AssignedTime != default ? CurrentTask.AssignedTime - mUserTimeHandler.TimePassed : new TimeSpan(0);
        public TimeSpan CurrentTimeLoss => TimeSpan.FromSeconds(BreakDuration.TotalSeconds * mCurrentTimeLoss.TotalSeconds);
        public TimeSpan BreakTaskTime { get; set; }
        public Timer RealTimer { get; set; } = new Timer(1000);
        public DateTime BreakStart { get; set; }
        public TimeSpan BreakDuration { get; set; }

        ///Constructor 
        public TasksSessionPageViewModel()
        {
            mUserTimeHandler.Updated += () => { LoadTaskList(); RefreshProperties(); };
            mUserTimeHandler.TimesUp += async () => await mUIManager.ExecuteOnMainThread(async () => await UserTimeHandler_TimesUpAsync());
            RealTimer.Elapsed += RealTimer_Elapsed;  

            LoadTaskList();
            RealTimer.Start();
        }

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

        /// Refreshes properties for UI
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

        private async Task FinishAsync()
        {
            var userResponse = await mUIManager.DisplayPopupMessageAsync(popupViewModel);
            if (userResponse)
            {
                mUserTimeHandler.FinishTask();
                ContinueUserTasks();
                LoadTaskList();
            }
        }
        private async Task UserTimeHandler_TimesUpAsync()
        {
            var userResponse = await mUIManager.DisplayPopupMessageAsync(popupViewModel);
            mUserTimeHandler.FinishTask();
            TaskProgress = 1;
            ContinueUserTasks();
            if (!userResponse && CurrentTask != null)
                StopCommand.Execute(null);
        }
        
        private void ContinueUserTasks()
        {
            mUserTimeHandler.CleanTasks();
            mUserTimeHandler.RefreshTasksState();
            LoadTaskList();
            mUserTimeHandler.StartTask();
        }
    */
}
