using Dna;
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
    /// The view model for tasks session page
    /// </summary>
    public class TasksSessionPageViewModel : MvxViewModel
    {
        #region Private Members

        private TimeTasksMapper mTimeTasksMapper;
        private ITimeTasksService mTimeTasksService;
        private ISessionNotificationService mSessionNotificationService;
        private IUIManager mUIManager;

        /// <summary>
        /// Stores the list of already finished tasks in this session
        /// </summary>
        private List<TimeTaskContext> mFinishedTasks;

        #endregion

        #region Public Properties

        /// <summary>
        /// The list of time tasks for current session that are not the current one
        /// </summary>
        public ObservableCollection<SessionTimeTaskItemViewModel> RemainingTasks { get; set; }

        /// <summary>
        /// The current task the user is doing on the session
        /// </summary>
        public SessionTimeTaskItemViewModel CurrentTask { get; set; }

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

        /// <summary>
        /// Current break duration, displayed only when break indicator is true
        /// </summary>
        public TimeSpan BreakDuration => mTimeTasksService.CurrentBreakDuration;

        /// <summary>
        /// Indicates if user has paused current task
        /// </summary>
        public bool Paused { get; set; }

        #endregion

        #region Commands

        /// <summary>
        /// The command to initialize new session in this page
        /// </summary>
        public ICommand InitializeSessionCommand { get; private set; }

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
        public TasksSessionPageViewModel()
        {
            // Create commands
            InitializeSessionCommand = new RelayCommand(InitializeSession);
            PauseCommand = new RelayCommand(PauseTask);
            ResumeCommand = new RelayCommand(ResumeTask);
            FinishTaskCommand = new RelayCommand(FinishCurrentTask);
            EndSessionCommand = new RelayCommand(EndSessionAsync);
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
            CurrentTask.Progress = mTimeTasksService.CurrentTaskCalculatedProgress;

            // Start the break
            mTimeTasksService.StartBreak();

            // Inform the notification
            mSessionNotificationService.StopCurrentTask();

            // Set the indicator
            Paused = true;

            // Update properties immediately instead of waiting for the timer for better UX
            UpdateSessionProperties();
        }

        /// <summary>
        /// Finishes the break and resumes current task
        /// </summary>
        private void ResumeTask()
        {
            // Stop the break
            mTimeTasksService.EndBreak();

            // Set the indicator
            Paused = false;

            // If option is true
            if (DI.Settings.RecalculateTasksAfterBreak)
            {
                // Recalculate remaining tasks after break
                RecalculateTasksAfterBreak();
            }

            // If current task is already finished... (the case when user opted for break when task's time ended)
            if (TimeRemaining <= TimeSpan.Zero)
            {
                // Fire finish command
                FinishTaskCommand.Execute(null);
                return;
            }

            // Inform the notification
            mSessionNotificationService.StartNewTask(CurrentTask);

            // Update properties immediately instead of waiting for the timer for better UX
            UpdateSessionProperties();
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

            // If there are no tasks left
            if (RemainingTasks.Count <= 0)
            {
                // Session is finished at this point
                EndSession();
                return;
            }

            // Set next task on the list
            SetCurrentTask(0, RemainingTasks.ToList());

            // Get new task's context
            var newTask = mTimeTasksMapper.ReverseMap(CurrentTask);

            // And start it in the session
            mTimeTasksService.StartNextTask(newTask);

            // Inform the notification
            mSessionNotificationService.StartNewTask(CurrentTask);
        }

        /// <summary>
        /// Ends current user session, if he decides to
        /// </summary>
        private async void EndSessionAsync()
        {
            // Ask the user if he's certain to end the session
            var popupViewModel = new PopupMessageViewModel
                (
                    LocalizationResource.SessionFinished,
                    LocalizationResource.QuestionAreYouSureToFinishSession,
                    LocalizationResource.Yes,
                    LocalizationResource.No
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
            // Set default values to key properties to start fresh session
            mFinishedTasks = new List<TimeTaskContext>();
            RemainingTasks = new ObservableCollection<SessionTimeTaskItemViewModel>();

            // Get latest instances of every needed DI services
            InjectLatestDIServices();

            // Initialize notification service
            mSessionNotificationService.AttachClickCommands(NotificationButtonClick);

            // Start new session providing required actions and get all the tasks
            var contexts = mTimeTasksService.StartSession(UpdateSessionProperties, TaskTimeFinish);

            // At the start of the session, first task in the list is always current one, so set it accordingly
            SetCurrentTask(0, mTimeTasksMapper.ListMapToSession(contexts.WholeList));

            // Start the task in the notification as well
            mSessionNotificationService.StartNewTask(CurrentTask);
        }

        /// <summary>
        /// Injects latest implementations of DI services into this view model
        /// </summary>
        private void InjectLatestDIServices()
        {
            // Get every service from DI
            mTimeTasksService = Framework.Service<ITimeTasksService>();
            mSessionNotificationService = Framework.Service<ISessionNotificationService>();
            mUIManager = Framework.Service<IUIManager>();
            mTimeTasksMapper = Framework.Service<TimeTasksMapper>();
        }

        /// <summary>
        /// Recalculates remaining tasks after break to compensate for lost time
        /// </summary>
        private void RecalculateTasksAfterBreak()
        {
            // Calculate how much time we should substract from every task
            var breakDurationPerTask = BreakDuration.TotalSeconds / RemainingTasks.Count;
            var timeToSubstract = TimeSpan.FromSeconds(breakDurationPerTask);

            // For each task in the remaining list...
            foreach (var task in RemainingTasks)
            {
                // Skip tasks that would be too short after substraction, we still want to keep minimum time requirement for them 
                if ((task.AssignedTime - timeToSubstract) < TimeSpan.FromMinutes(DI.Settings.MinimumTaskTime))
                    continue;
                
                // Substract the time from task
                task.AssignedTime -= timeToSubstract;
            }
        }

        /// <summary>
        /// Called when current task's time runs out
        /// </summary>
        private void TaskTimeFinish()
        {
            // Go to alarm page that handles this action
            DI.Application.GoToPage(ApplicationPage.Alarm);
        }

        /// <summary>
        /// Updates every session property with new values
        /// </summary>
        private void UpdateSessionProperties()
        {
            // Simply use the helper to fire every property's change event, for now it works just fine
            // Potentially in the future, update only required properties, not everything
            RaiseAllPropertiesChanged();

            // Update the notification as well
            mSessionNotificationService.UpdateNotification();
        }

        /// <summary>
        /// Called when user interacted with session notification
        /// </summary>
        /// <param name="action">The action user has made</param>
        private void NotificationButtonClick(AppAction action)
        {
            // Fire proper command based on the action
            // So clicking on the notification has the exact same effect as clicking on the page
            switch (action)
            {
                case AppAction.NextSessionTask:
                    {
                        FinishTaskCommand.Execute(null);
                    } break;
                case AppAction.PauseSession:
                    {
                        PauseCommand.Execute(null);
                    } break;
                case AppAction.ResumeSession:
                    {
                        ResumeCommand.Execute(null);
                    } break;
                case AppAction.StopSession:
                    {
                        EndSessionCommand.Execute(null);
                    } break;
            }
        }

        /// <summary>
        /// Sets specified task from the list to the current one, also removing it from the remaining list
        /// </summary>
        /// <param name="index">The index of the task to set as current</param>
        /// <param name="viewModels">The list of task view models</param>
        private void SetCurrentTask(int index, List<SessionTimeTaskItemViewModel> viewModels)
        {
            try
            {
                // Set the task at specified index
                CurrentTask = viewModels.ElementAt(index);
            }
            catch
            {
                // If we get here, the index is not in the list
                // So the list is either empty...
                if (viewModels.Count <= 0)
                {
                    // Then we finish current session
                    EndSession();
                    return;
                }
                // Or something went wrong and we tried to start the task that doesn't exist
                // Debugger.Break();
            }

            // Delete current task from the list
            viewModels.Remove(CurrentTask);

            // And set the remaining list tasks
            RemainingTasks = new ObservableCollection<SessionTimeTaskItemViewModel>(viewModels);
        }

        /// <summary>
        /// Ends this session completely
        /// </summary>
        private void EndSession()
        {
            // Stop timer
            mTimeTasksService.StartBreak();

            // Send finished tasks list for removal
            mTimeTasksService.RemoveFinishedTasks(mFinishedTasks);

            // Remove the notification
            mSessionNotificationService.RemoveNotification();

            // Go to first page
            DI.Application.GoToPage(ApplicationPage.TasksList);
        }

        #endregion
    }
}
