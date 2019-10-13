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

        /// <summary>
        /// Timestamp for current session
        /// </summary>
        private DateTime mStartTime;

        #endregion

        #region Public Events

        /// <summary>
        /// The event that is fired any time the current task finishes
        /// </summary>
        public event Action TaskFinished = () => { };

        /// <summary>
        /// Event called when all tasks are finished
        /// </summary>
        public event Action SessionFinished = () => { };

        /// <summary>
        /// Event called when new session is started
        /// </summary>
        public event Action SessionStarted = () => { };

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public SessionHandler(ITimeTasksService timeTasksService, ITimeTasksCalculator timeTasksCalculator)
        {
            mTimeTasksService = timeTasksService;
            mTimeTasksCalculator = timeTasksCalculator;

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
        public TimeSpan SessionTime { get; private set; } = default;

        /// <summary>
        /// Time since session has started
        /// </summary>
        public TimeSpan SessionDuration => DateTime.Now - mStartTime;

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
                SessionTime = userTime;

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
        public void SetupSession(Action timerAction, Action taskAction, Action sessionAction)
        {
            // Attach provided action as well
            mSecondsTicker.Elapsed += (s, e) => timerAction.Invoke();

            // Attach provided task action to the task finished event
            TaskFinished += taskAction;

            // Attach provided session action
            SessionFinished += sessionAction;
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
            if (mUserTasks != null)
            {
                // Simply nullify the properties, so the service state is exactly the same as when before the use
                mUserTasks.WholeList.Clear();
                mUserTasks = null;
            }
            SessionTime = default;

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
                // Update timestamp
                mStartTime = DateTime.Now;
                // Start first task
                StartNextTask(mCurrentTask);
                // Broadcast session is started
                SessionStarted.Invoke();
                // Do not perform any other action
                return;
            }

            // If current task is already finished... (the case when user opted for break when task's time ended)
            if (CurrentTimeLeft <= TimeSpan.Zero)
            {
                // Fire finish command
                Finish();
                return;
            }

            // Recalculate remaining tasks after break
            RecalculateTasksAfterBreak();
        }

        /// <summary>
        /// Pauses current task and starts the break
        /// </summary>
        public void Pause()
        {
            // Save current task's progress
            mCurrentTask.DynamicTime = CurrentTimeLeft;
            mCurrentTask.Progress = CurrentTaskCalculatedProgress;

            // Start the break
            StartBreak();
        }

        /// <summary>
        /// Finishes current displayed task
        /// </summary>
        public void Finish()
        {
            // Save current task's progress
            mCurrentTask.DynamicTime = CurrentTimeLeft;
            mCurrentTask.Progress = CurrentTaskCalculatedProgress;

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
            UpdateTasks(mUserTasks.RemainingList);

            // Recalculate remaining tasks after break
            RecalculateTasksAfterBreak();

            // And start it in the session
            StartNextTask(mCurrentTask);
        }

        public void EndSession()
        {
            // Stop timer
            StartBreak();

            // Send finished tasks list for removal
            mTimeTasksService.RemoveFinishedTasks(mFinishedTasks);

            // Fire event, session over
            SessionFinished.Invoke();
        }

        #endregion

        #region Private Helpers

        #region Handler logic
        /// <summary>
        /// Starts new task in current session
        /// </summary>
        /// <param name="context">The task context to start</param>
        private void StartNextTask(TimeTaskContext context)
        {
            // Set provided time
            CurrentTimeLeft = context.DynamicTime;

            // Save it for progress calculations
            mCurrentTask.DynamicTime = CurrentTimeLeft;

            // Start the timer
            mSecondsTicker.Start();
        }

        /// <summary>
        /// Reset session
        /// </summary>
        private void Reset()
        {
            // Reset events
            TaskFinished = () => { };
            SessionFinished = () => { };

            // Update timestamp
            mStartTime = DateTime.Now;

            // Reset any previous sessions
            SessionTime = default;

            // Prepare finished tasks list
            mFinishedTasks = new List<TimeTaskContext>();

            // Release old ticker
            if (mSecondsTicker != null)
                mSecondsTicker.Dispose();

            // Timer ticks every second
            mSecondsTicker = new Timer(DI.Settings.TimerTickRate);

            // Run our elapsed function every time timer ticks
            mSecondsTicker.Elapsed += SecondsTicker_Elapsed;
        }
        #endregion

        #region Calculating time
        /// <summary>
        /// Fit tasks in SessionTime
        /// </summary>
        /// <returns>List of calculated <see cref="TimeTaskContext"/></returns>
        private List<TimeTaskContext> CalculatedTasks(List<TimeTaskContext> target)
        {
            if (target.Count <= 0)
                return default;

            // Check if time for session is properly set
            if (SessionTime == default || !ValidateTime(SessionTime))
                // Throw exception because it should not ever happen in the code (time should be checked before), so something needs a fix
                throw new Exception(LocalizationResource.AttemptToCalculateNoTime);

            // Everything is nice and set, calculate our session
            var calculatedTasks = mTimeTasksCalculator.CalculateTasksForSession(target, SessionTime);

            // Return the tasks
            return calculatedTasks;
        }

        /// <summary>
        /// Optimized times update accordingly to break duration
        /// </summary>
        private void RecalculateTasksAfterBreak()
        {
            // Do work only if enabled in settings
            if (!DI.Settings.RecalculateTasksAfterBreak)
                return;

            // If nothing to calculate then exit
            if (mUserTasks.WholeList.Count <= 0)
                return;

            // Get time left
            var remaining = SessionTime - SessionDuration;

            // Abort if remaining time is incorrect
            if (remaining.TotalSeconds <= 0)
                return;

            // Update tasks using optimized algorithm
            var updatedTasks = mTimeTasksCalculator.CalculateTasksAfterResume(mUserTasks.WholeList, remaining);
            UpdateTasks(updatedTasks);

            // Update work property
            CurrentTimeLeft = mCurrentTask.DynamicTime;
        }
        #endregion

        #region UI logic
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
        /// Runs every time the timer ticks
        /// </summary>
        private void SecondsTicker_Elapsed(object sender, ElapsedEventArgs e)
        {
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
                // Stop timer to avoid reentering this code
                mSecondsTicker.Stop();

                // Finish current task, it is done
                Finish();

                // Inform everyone about it
                TaskFinished.Invoke();
            }
        }
        #endregion

        #region Validation
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
        #endregion

        #endregion
    }
}
