using System;
using System.Collections.Generic;
using System.Timers;
using Timeinator.Core;

namespace Timeinator.Mobile.Core
{
    /// <summary>
    /// User session handler
    /// </summary>
    public class SessionTimer : ISessionTimer
    {
        #region Private Members

        private readonly ITimeTasksService mTimeTasksService;
        private readonly ITimeTasksCalculator mTimeTasksCalculator;
        private readonly ISessionNotificationService mSessionNotificationService;

        /// <summary>
        /// The amount of time for every timer tick
        /// </summary>
        private readonly TimeSpan mOneTick = TimeSpan.FromSeconds(DI.Settings.TimerTickRate / 1000);

        /// <summary>
        /// The list of current tasks contexts stored in this manager
        /// </summary>
        private HeadList<TimeTaskContext> mCurrentTasks;

        /// <summary>
        /// Tasks staged to remove
        /// </summary>
        private List<TimeTaskContext> mFinishedTasks;

        /// <summary>
        /// The time that current task in session has assigned
        /// </summary>
        private TimeSpan mCurrentTaskTime;

        /// <summary>
        /// The timer that elapses every second so everything related to time can update everytime it ticks
        /// </summary>
        private Timer mSecondsTicker;

        #endregion

        #region Public Events

        /// <summary>
        /// The event that is fired any time the current task finishes
        /// </summary>
        public event Action TaskFinished = () => { };

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public SessionTimer(ITimeTasksService timeTasksService, ITimeTasksCalculator timeTasksCalculator, ISessionNotificationService sessionNotificationService)
        {
            mTimeTasksService = timeTasksService;
            mTimeTasksCalculator = timeTasksCalculator;
            mSessionNotificationService = sessionNotificationService;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Calculates the progress value of current task in session and returns it
        /// </summary>
        public double CurrentTaskCalculatedProgress
        {
            get
            {
                // Calculate the value from current time left and task time
                var calculatedValue = (mCurrentTaskTime - CurrentTimeLeft).TotalSeconds / mCurrentTaskTime.TotalSeconds;

                // Task can't be done in more than 100%
                if (calculatedValue > 1)
                    calculatedValue = 1;

                // Return as two-digit rounded value
                return Math.Round(calculatedValue, 2);
            }
        }

        /// <summary>
        /// Status of timer, indicates break mode
        /// </summary>
        public bool Paused { get; private set; }

        /// <summary>
        /// The duration of the whole current session
        /// </summary>
        public TimeSpan SessionDuration { get; private set; }

        /// <summary>
        /// The time that left in current session task
        /// </summary>
        public TimeSpan CurrentTimeLeft { get; private set; }

        /// <summary>
        /// The duration of current break
        /// </summary>
        public TimeSpan CurrentBreakDuration { get; set; }

        #endregion

        #region Interface Implementation

        /// <summary>
        /// Setups new session in the timer
        /// </summary>
        /// <param name="timerAction">The action to fire along with timer ticks</param>
        /// <param name="taskAction">The action to fire when task's time finishes</param>
        public void SetupSession(Action timerAction, Action taskAction)
        {
            // Reset any previous sessions
            SessionDuration = TimeSpan.Zero;

            // Timer ticks every second
            mSecondsTicker = new Timer(DI.Settings.TimerTickRate);

            // Run our elapsed function every time timer ticks
            mSecondsTicker.Elapsed += SecondsTicker_Elapsed;

            // Attach provided action as well
            mSecondsTicker.Elapsed += (s, e) => timerAction.Invoke();

            // Attach provided task action to the task finished event
            TaskFinished += taskAction;
        }

        /// <summary>
        /// Starts new task in current session
        /// </summary>
        /// <param name="context">The task context to start</param>
        public void StartNextTask(TimeTaskContext context)
        {
            // Set provided time
            CurrentTimeLeft = context.AssignedTime;

            // Start the timer
            mSecondsTicker.Start();

            // Save it for progress calculations
            mCurrentTaskTime = context.AssignedTime;
        }

        /// <summary>
        /// Starts the break time on current task
        /// </summary>
        public void StartBreak()
        {
            // Erase any previous break time 
            CurrentBreakDuration = TimeSpan.Zero;

            // Set the indicator
            Paused = true;
        }

        public void EndBreak()
        {
        }

        public void Resume()
        {
        }

        public void Pause()
        {
        }

        public void Finish()
        {
        }

        public void EndSession()
        {
        }

        /// <summary>
        /// Get currently queued tasks
        /// </summary>
        public HeadList<TimeTaskContext> GetTasks() => mCurrentTasks;
        
        #endregion

        #region Private Helpers

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
                        error
                    } break;
                case AppAction.PauseSession:
                    {
                        error
                    } break;
                case AppAction.ResumeSession:
                    {
                        error
                    } break;
                case AppAction.StopSession:
                    {
                        error
                    } break;
            }
        }

        /// <summary>
        /// Validates if we have proper tasks inside service
        /// </summary>
        private void ValidateTasks()
        {
            // Tasks must be set and we need at least one of them
            if (mCurrentTasks?.WholeList == null || mCurrentTasks.WholeList.Count < 1)
            {
                // Throw exception because it should not ever happen in the code, so something needs a fix
                throw new Exception(LocalizationResource.AttemptToCalculateNoTasks);
            }
        }

        /// <summary>
        /// Validates if session time is enough for current tasks
        /// </summary>
        private bool ValidateTime(TimeSpan time)
        {
            // Calculate what time we need for current session
            var neededTime = mTimeTasksCalculator.CalculateMinimumTimeForTasks(mCurrentTasks.WholeList);

            // Time must be greater or equal to minimum needed
            return time >= neededTime;
        }

        /// <summary>
        /// Runs every time the timer ticks
        /// </summary>
        private void SecondsTicker_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Add one tick to the session duration
            SessionDuration += mOneTick;

            // If break time is on...
            if (Paused)
            {
                // Add one tick to the break duration
                CurrentBreakDuration += mOneTick;

                // Don't do anything else while on break
                return;
            }

            // Substract one tick from the task time
            CurrentTimeLeft -= mOneTick;

            // If the task finished already...
            if (CurrentTimeLeft <= TimeSpan.Zero)
            {
                // Stop the timer
                mSecondsTicker.Stop();

                // Inform everyone about it
                TaskFinished.Invoke();
            }
        }

        #endregion

        /// <summary>
        /// Pauses current task and starts the break
        /// </summary>
        private void PauseTask()
        {
            // Save current task's progress
            CurrentTask.AssignedTime = CurrentTimeLeft;
            CurrentTask.Progress = CurrentTaskCalculatedProgress;

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
        /// Recalculates remaining tasks after break to compensate for lost time
        /// </summary>
        private void RecalculateTasksAfterBreak()
        {
            // If we have no remaining tasks...
            if (RemainingTasks.Count == 0)
                // Then we can't really recalculate anything, so just do nothing
                return;

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
        /// Sets provided tasks in our internal task list
        /// </summary>
        /// <param name="contexts">The tasks to set</param>
        public void SetSessionTasks(List<TimeTaskContext> contexts)
        {
            // Set our internal list with provided tasks
            mCurrentTasks = new HeadList<TimeTaskContext>(contexts);
        }

        /// <summary>
        /// Sets provided time as user session time
        /// </summary>
        /// <returns>True, if time was set successfully, or false, if time wasn't enough for the current session</returns>
        public bool SetSessionTime(TimeSpan userTime)
        {
            // Validate provided time
            if (ValidateTime(userTime))
            {
                // It is proper one, so set it
                mSessionTime = userTime;

                // And return success
                return true;
            }

            // The time isn't enough for the session, just return failure
            return false;
        }

        /// <summary>
        /// Empties the task list in manager
        /// </summary>
        public void ClearSessionTasks()
        {
            // Simply nullify the properties, so the service state is exactly the same as when before the use
            mCurrentTasks.WholeList.Clear();
            mCurrentTasks = null;
            mSessionTime = default;
        }

        /// <summary>
        /// Gets a list of calculated tasks for the session
        /// </summary>
        /// <returns>List of calculated <see cref="TimeTaskContext"/></returns>
        public List<TimeTaskContext> GetCalculatedTasks()
        {
            // Check if task list is properly set
            ValidateTasks();

            // Check if time for session is properly set
            if (mSessionTime == default || !ValidateTime(mSessionTime))
            {
                // Throw exception because it should not ever happen in the code (time should be checked before), so something needs a fix
                throw new Exception(LocalizationResource.AttemptToCalculateNoTime);
            }

            // Everything is nice and set, calculate our session
            var calculatedTasks = mTimeTasksCalculator.CalculateTasksForSession(mCurrentTasks.WholeList, mSessionTime);

            // Return the tasks
            return calculatedTasks;
        }

        /// <summary>
        /// Starts the task session
        /// </summary>
        /// <param name="timerAction">The action that will be attached to the timer elapsed event</param>
        /// <param name="taskAction">The action that will be attached to the task finished event</param>
        /// <returns>List of every task in the session we start</returns>
        public HeadList<TimeTaskContext> StartSession(Action timerAction, Action taskAction)
        {
            // Setup the timer session
            SetupSession(timerAction, taskAction);

            // Start first task
            StartNextTask(mCurrentTasks.Head);

            // Return current list of tasks
            return mCurrentTasks;
        }

        /// <summary>
        /// Ends the break time
        /// </summary>
        public void EndBreak()
        {
            // Set the indicator
            Paused = false;
        }

        public void EndSession()
        {
            // Stop timer
            StartBreak();

            // Send finished tasks list for removal
            mTimeTasksService.RemoveFinishedTasks(mFinishedTasks);

            // Remove the notification
            mSessionNotificationService.RemoveNotification();
        }
    }
}
