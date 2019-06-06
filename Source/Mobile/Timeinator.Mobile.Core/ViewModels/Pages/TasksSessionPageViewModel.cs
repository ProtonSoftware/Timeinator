using Dna;
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
        private readonly ISessionNotificationService mSessionNotificationService;
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
        public TasksSessionPageViewModel(ITimeTasksService timeTasksService, ISessionNotificationService sessionNotificationService, IUIManager uiManager, TimeTasksMapper tasksMapper)
        {
            // Create commands
            PauseCommand = new RelayCommand(PauseTask);
            ResumeCommand = new RelayCommand(ResumeTask);
            FinishTaskCommand = new RelayCommand(FinishTaskAsync);
            EndSessionCommand = new RelayCommand(EndSessionAsync);

            // Get injected DI services
            mTimeTasksService = timeTasksService;
            mSessionNotificationService = sessionNotificationService;
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

            // Inform the notification
            mSessionNotificationService.StartNewTask(CurrentTask);

            // Set the indicator
            Paused = false;

            // Update properties immediately instead of waiting for the timer for better UX
            UpdateSessionProperties();
        }

        /// <summary>
        /// Finishes the current task by removing it and goes to the next one
        /// </summary>
        private async void FinishTaskAsync()
        {
            // Ask the user if he's certain to finish the task before it ends
            var popupViewModel = new PopupMessageViewModel
                (
                    LocalizationResource.TaskFinished,
                    LocalizationResource.QuestionAreYouSureToFinishTask,
                    LocalizationResource.Yes,
                    LocalizationResource.No
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
            // Initialize notification service
            mSessionNotificationService.AttachClickCommands(NotificationButtonClick);

            // Start new session providing required actions and get all the tasks
            var contexts = mTimeTasksService.StartSession(UpdateSessionProperties, TaskTimeFinishAsync);

            // At the start of the session, first task in the list is always current one, so set it accordingly
            SetCurrentTask(0, mTimeTasksMapper.ListMap(contexts.WholeList));

            // Start the task in the notification as well
            mSessionNotificationService.StartNewTask(CurrentTask);
        }

        /// <summary>
        /// Called when current task's time runs out
        /// </summary>
        private async void TaskTimeFinishAsync()
        {
            var vm = Framework.Service<AlarmPageViewModel>();
            vm.InitializeButtons(PauseTask, FinishTaskAsync);
            await DI.Application.GoToPageAsync(ApplicationPage.Alarm);
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

            // Inform the notification
            mSessionNotificationService.StartNewTask(CurrentTask);
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

            // Remove the notification
            mSessionNotificationService.RemoveNotification();

            // Go to first page
            DI.Application.GoToPageAsync(ApplicationPage.TasksList);
        }

        #endregion
    }
}
