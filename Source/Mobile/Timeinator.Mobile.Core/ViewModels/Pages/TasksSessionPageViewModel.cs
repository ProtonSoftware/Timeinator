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
        private IUIManager mUIManager;
        private ISessionHandler mSessionHandler;

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
        public double TaskProgress => mSessionHandler.CurrentTaskCalculatedProgress;

        /// <summary>
        /// Current session length from the start of it
        /// </summary>
        public TimeSpan SessionDuration => mSessionHandler.SessionDuration;

        /// <summary>
        /// The remaining time left of current task
        /// </summary>
        public TimeSpan TimeRemaining => mSessionHandler.CurrentTimeLeft;

        /// <summary>
        /// Current break duration, displayed only when break indicator is true
        /// </summary>
        public TimeSpan BreakDuration => mSessionHandler.CurrentBreakDuration;

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
            EndSessionCommand = new RelayCommand(EndSessionAsync);
            PauseCommand = new RelayCommand(() => mSessionHandler.Pause());
            ResumeCommand = new RelayCommand(() => mSessionHandler.Resume());
            FinishTaskCommand = new RelayCommand(() => mSessionHandler.Finish());
        }

        #endregion

        #region Command Methods

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
                // End the session
                mSessionHandler.EndSession();
                QuitSession();
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
            RemainingTasks = new ObservableCollection<SessionTimeTaskItemViewModel>();

            // Get latest instances of every needed DI services
            InjectLatestDIServices();

            // Start new session providing required actions
            mSessionHandler.SetupSession(UpdateSessionProperties, TaskTimeFinish);

            // Begin session
            mSessionHandler.Resume();

            // Retrieve tasks
            var contexts = mSessionHandler.GetTasks();

            // At the start of the session, first task in the list is always current one, so set it accordingly
            SetCurrentTask(0, mTimeTasksMapper.ListMapToSession(contexts.WholeList));
        }

        /// <summary>
        /// Injects latest implementations of DI services into this view model
        /// </summary>
        private void InjectLatestDIServices()
        {
            // Get every service from DI
            mSessionHandler = DI.Container.GetInstance<ISessionHandler>();
            mUIManager = DI.Container.GetInstance<IUIManager>();
            mTimeTasksMapper = DI.Container.GetInstance<TimeTasksMapper>();
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
                    mSessionHandler.EndSession();
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
        private void QuitSession()
        {
            // Go to first page
            DI.Application.GoToPage(ApplicationPage.TasksList);
        }

        #endregion
    }
}
