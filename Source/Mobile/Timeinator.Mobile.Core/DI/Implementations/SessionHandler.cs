using System;
using System.Collections.Generic;
using System.Timers;
using Timeinator.Core;

namespace Timeinator.Mobile.Core
{
    /// <summary>
    /// User session handler
    /// </summary>
    public class SessionHandler : ISessionHandler
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
        private HeadList<TimeTaskContext> mUserTasks;

        /// <summary>
        /// Tasks staged to remove
        /// </summary>
        private List<TimeTaskContext> mFinishedTasks;

        /// <summary>
        /// Params of current task
        /// </summary>
        private TimeTaskContext mCurrentTask;

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
        public SessionHandler(ITimeTasksService timeTasksService, ITimeTasksCalculator timeTasksCalculator, ISessionNotificationService sessionNotificationService)
        {
            mTimeTasksService = timeTasksService;
            mTimeTasksCalculator = timeTasksCalculator;
            mSessionNotificationService = sessionNotificationService;

            Reset();
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
                if (mCurrentTask == null)
                    return 0;

                // Calculate the value from current time left and task time
                var calculatedValue = (mCurrentTask.AssignedTime - CurrentTimeLeft).TotalSeconds / mCurrentTask.AssignedTime.TotalSeconds;

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
        public bool Paused { get; private set; } = true;

        /// <summary>
        /// The duration of the whole current session
        /// </summary>
        public TimeSpan SessionDuration { get; private set; } = default;

        /// <summary>
        /// The time that left in current session task
        /// </summary>
        public TimeSpan CurrentTimeLeft { get; private set; } = default;

        /// <summary>
        /// The duration of current break
        /// </summary>
        public TimeSpan CurrentBreakDuration { get; set; } = default;

        #endregion

        #region Interface Implementation

        /// <summary>
        /// Send new tasks to session
        /// </summary>
        public void UpdateTasks(List<TimeTaskContext> tasks)
        {
            mUserTasks = new HeadList<TimeTaskContext>(tasks);
            mCurrentTask = mUserTasks.Head;

            // Check if task list is properly set
            ValidateTasks();
        }

        /// <summary>
        /// Sets provided time as user session time
        /// </summary>
        /// <returns>True, if time was set successfully, or false, if time wasn't enough for the current session</returns>
        public bool UpdateDuration(TimeSpan userTime)
        {
            // Validate provided time
            if (ValidateTime(userTime))
            {
                // It is proper one, so set it
                SessionDuration = userTime;

                // And return success
                return true;
            }

            // The time isn't enough for the session, just return failure
            return false;
        }

        /// <summary>
        /// Setups new session in the timer
        /// </summary>
        /// <param name="timerAction">The action to fire along with timer ticks</param>
        /// <param name="taskAction">The action to fire when task's time finishes</param>
        public void SetupSession(Action timerAction, Action taskAction)
        {
            // Attach provided action as well
            mSecondsTicker.Elapsed += (s, e) => timerAction.Invoke();

            // Attach provided task action to the task finished event
            TaskFinished += taskAction;
        }

        /// <summary>
        /// Update tasks to be calculated
        /// </summary>
        public void Calculate()
        {
            UpdateTasks(CalculatedTasks(mUserTasks.WholeList));
        }

        /// <summary>
        /// Get currently queued tasks
        /// </summary>
        public HeadList<TimeTaskContext> GetTasks() => mUserTasks;

        /// <summary>
        /// Passes a work copy of current task
        /// </summary>
        public TimeTaskContext GetCurrentTask() => mCurrentTask;
        
        /// <summary>
        /// Empties the task list in session, resets handler
        /// </summary>
        public void ClearSessionTasks()
        {
            // Simply nullify the properties, so the service state is exactly the same as when before the use
            mUserTasks.WholeList.Clear();
            mUserTasks = null;
            SessionDuration = default;

            // Reset state of Handler
            Reset();
        }

        /// <summary>
        /// Finishes the break and resumes current task
        /// </summary>
        public void Resume()
        {
            // Stop the break
            EndBreak();

            // Launch session if has not been yet started
            if (!mSecondsTicker.Enabled)
            {
                StartNextTask(mCurrentTask);
                // Do not perform any other action
                return;
            }

            // If option is true
            if (DI.Settings.RecalculateTasksAfterBreak)
            {
                // Recalculate remaining tasks after break
                RecalculateTasksAfterBreak();
            }

            // If current task is already finished... (the case when user opted for break when task's time ended)
            if (CurrentTimeLeft <= TimeSpan.Zero)
            {
                // Fire finish command
                Finish();
                return;
            }

            // Inform the notification
            mSessionNotificationService.StartNewTask(mCurrentTask);
        }

        /// <summary>
        /// Pauses current task and starts the break
        /// </summary>
        public void Pause()
        {
            // Save current task's progress
            mCurrentTask.AssignedTime = CurrentTimeLeft;
            mCurrentTask.Progress = CurrentTaskCalculatedProgress;

            // Start the break
            StartBreak();

            // Inform the notification
            mSessionNotificationService.StopCurrentTask();
        }

        /// <summary>
        /// Finishes current displayed task
        /// </summary>
        public void Finish()
        {
            // Get finished task's context
            var finishedTask = mCurrentTask;

            // Add finished task to the list for future reference
            mFinishedTasks.Add(finishedTask);

            // If there are no tasks left
            if (mUserTasks.RemainingList.Count <= 0)
            {
                // Session is finished at this point
                EndSession();
                return;
            }

            // Refresh tasks
            UpdateTasks(CalculatedTasks(mUserTasks.RemainingList));

            // And start it in the session
            StartNextTask(mCurrentTask);
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

        #endregion

        #region Private Helpers

        /// <summary>
        /// Starts new task in current session
        /// </summary>
        /// <param name="context">The task context to start</param>
        private void StartNextTask(TimeTaskContext context)
        {
            // Set provided time
            CurrentTimeLeft = context.AssignedTime;

            // Start the timer
            mSecondsTicker.Start();

            // Save it for progress calculations
            mCurrentTask.AssignedTime = context.AssignedTime;

            // Inform the notification
            mSessionNotificationService.StartNewTask(mCurrentTask);
        }

        /// <summary>
        /// Reset session
        /// </summary>
        private void Reset()
        {
            // Reset any previous sessions
            SessionDuration = TimeSpan.Zero;

            // Release old ticker
            if (mSecondsTicker != null)
                mSecondsTicker.Dispose();

            // Timer ticks every second
            mSecondsTicker = new Timer(DI.Settings.TimerTickRate);

            // Run our elapsed function every time timer ticks
            mSecondsTicker.Elapsed += SecondsTicker_Elapsed;

            // Setup notification service
            mSessionNotificationService.Setup();

            // Initialize notification service
            mSessionNotificationService.AttachClickCommands(NotificationButtonClick);
        }

        /// <summary>
        /// Gets a list of calculated tasks for the session
        /// </summary>
        /// <returns>List of calculated <see cref="TimeTaskContext"/></returns>
        private List<TimeTaskContext> CalculatedTasks(List<TimeTaskContext> target)
        {
            if (target.Count <= 0)
                return default;

            // Check if time for session is properly set
            if (SessionDuration == default || !ValidateTime(SessionDuration))
            {
                // Throw exception because it should not ever happen in the code (time should be checked before), so something needs a fix
                throw new Exception(LocalizationResource.AttemptToCalculateNoTime);
            }

            // Everything is nice and set, calculate our session
            var calculatedTasks = mTimeTasksCalculator.CalculateTasksForSession(target, SessionDuration);

            // Return the tasks
            return calculatedTasks;
        }

        /// <summary>
        /// Recalculates remaining tasks after break, subtracts lost time
        /// </summary>
        private void RecalculateTasksAfterBreak()
        {
            // TODO:
            // Verify and rework to get desired behavoiur

            var tasksLeft = mUserTasks.WholeList.Count;

            // If we have no remaining tasks...
            if (tasksLeft <= 0)
                // Then we can't really recalculate anything, so just do nothing
                return;

            // TODO: this should not subtract time evenly but use priority parameter instead
            // Calculate how much time we should substract from every task
            var breakDurationPerTask = CurrentBreakDuration.TotalSeconds / tasksLeft;
            var timeToSubstract = TimeSpan.FromSeconds(breakDurationPerTask);

            // There was no break or it was so short we can ignore it
            if (breakDurationPerTask <= 0.001)
                return;

            // For each task in the remaining list...
            foreach (var task in mUserTasks.WholeList)
            {
                // Skip tasks that would be too short after substraction, we still want to keep minimum time requirement for them 
                if ((task.AssignedTime - timeToSubstract) < TimeSpan.FromMinutes(DI.Settings.MinimumTaskTime))
                    continue;
                
                // Substract the time from task
                task.AssignedTime -= timeToSubstract;
            }
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
                        Finish();
                    } break;
                case AppAction.PauseSession:
                    {
                        Pause();
                    } break;
                case AppAction.ResumeSession:
                    {
                        Resume();
                    } break;
                case AppAction.StopSession:
                    {
                        EndSession();
                    } break;
            }
        }

        /// <summary>
        /// Starts the break time on current task
        /// </summary>
        private void StartBreak()
        {
            // Erase any previous break time 
            CurrentBreakDuration = TimeSpan.Zero;

            // Set the indicator
            Paused = true;
        }

        /// <summary>
        /// Ends the break time
        /// </summary>
        private void EndBreak()
        {
            // Set the indicator
            Paused = false;
        }

        /// <summary>
        /// Validates if we have proper tasks inside service
        /// </summary>
        private void ValidateTasks()
        {
            // Tasks must be set and we need at least one of them
            if (mUserTasks?.WholeList == null || mUserTasks.WholeList.Count < 1)
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
            var neededTime = mTimeTasksCalculator.CalculateMinimumTimeForTasks(mUserTasks.WholeList);

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
    }
}
