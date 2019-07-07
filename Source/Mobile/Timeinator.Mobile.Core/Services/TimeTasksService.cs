using System;
using System.Collections.Generic;
using System.Linq;
using Timeinator.Core;
using Timeinator.Mobile.DataAccess;

namespace Timeinator.Mobile.Core
{
    /// <summary>
    /// The service that handles all the logic associated with the time tasks
    /// </summary>
    public class TimeTasksService : ITimeTasksService
    {
        #region Private Members

        private readonly TimeTasksMapper mTimeTasksMapper;
        private readonly ITimeTasksCalculator mTimeTasksCalculator;
        private readonly ITimeTasksRepository mTimeTasksRepository;
        private readonly ISessionTimer mSessionTimer;

        /// <summary>
        /// The list of current tasks contexts stored in this manager
        /// </summary>
        private HeadList<TimeTaskContext> mCurrentTasks;

        /// <summary>
        /// The time for the current session
        /// </summary>
        private TimeSpan mSessionTime;

        /// <summary>
        /// The time that current task in session has assigned
        /// </summary>
        private TimeSpan mCurrentTaskTime;

        #endregion

        #region Public Properties

        /// <summary>
        /// The session duration time from session timer
        /// </summary>
        public TimeSpan SessionDuration => mSessionTimer.SessionDuration;

        /// <summary>
        /// The time left to complete current task from session timer
        /// </summary>
        public TimeSpan CurrentTaskTimeLeft => mSessionTimer.CurrentTaskTimeLeft;

        /// <summary>
        /// The current break duration from session timer
        /// </summary>
        public TimeSpan CurrentBreakDuration => mSessionTimer.CurrentBreakDuration;

        /// <summary>
        /// Calculates the progress value of current task in session and returns it
        /// </summary>
        public double CurrentTaskCalculatedProgress
        {
            get
            {
                // Calculate the value from current time left and task time
                var calculatedValue = (mCurrentTaskTime - mSessionTimer.CurrentTaskTimeLeft).TotalSeconds / mCurrentTaskTime.TotalSeconds;

                // Task can't be done in more than 100%
                if (calculatedValue > 1)
                    calculatedValue = 1;

                // Return as two-digit rounded value
                return Math.Round(calculatedValue, 2);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public TimeTasksService(ITimeTasksCalculator timeTasksCalculator, ITimeTasksRepository timeTasksRepository, ISessionTimer sessionTimer, TimeTasksMapper tasksMapper)
        {
            // Get injected DI services
            mTimeTasksCalculator = timeTasksCalculator;
            mTimeTasksRepository = timeTasksRepository;
            mSessionTimer = sessionTimer;
            mTimeTasksMapper = tasksMapper;
        }

        #endregion

        #region Interface Implementation

        #region Database
        /// <summary>
        /// Saves new task to the database and adds it to the application's task list
        /// </summary>
        /// <param name="context">The context of a task to add</param>
        public void SaveTask(TimeTaskContext context)
        {
            // Map it to the entity
            var entity = mTimeTasksMapper.ReverseMap(context);

            // Save it into database
            mTimeTasksRepository.SaveTask(entity);
        }

        /// <summary>
        /// Removes single specified task from the database
        /// </summary>
        /// <param name="context">The task to delete</param>
        public void RemoveTask(TimeTaskContext context)
        {
            // Prepare an one element list with provided task's id
            var list = new List<int> { context.Id };

            // Send it to the repository to delete this task
            mTimeTasksRepository.RemoveTasks(list);
        }

        /// <summary>
        /// Removes finished tasks from the database
        /// Ommits "immortal" tasks
        /// </summary>
        public void RemoveFinishedTasks(List<TimeTaskContext> contexts)
        {
            // Get list of task ids...
            var taskIds = contexts
                         // That are not immortal
                         .Where(x => x.IsImmortal == false)
                         // Take ids instead of tasks
                         .Select(x => x.Id);

            // Send collected ids to the repository to remove associated tasks
            mTimeTasksRepository.RemoveTasks(taskIds);
        }

        /// <summary>
        /// Loads every saved task from the database
        /// </summary>
        /// <param name="queryString">The filter text for tasks</param>
        /// <returns>A list of found tasks as <see cref="TimeTaskContext"/></returns>
        public List<TimeTaskContext> LoadStoredTasks(string queryString)
        {
            // Get every task in the database
            var dbTasks = mTimeTasksRepository.GetSavedTasksForToday(queryString);

            // Map entities as contexts
            var result = mTimeTasksMapper.ListMap(dbTasks.ToList());

            // Return every found task
            return result;
        }
        #endregion

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
            mSessionTimer.SetupSession(timerAction, taskAction);

            // Start first task
            StartNextTask(mCurrentTasks.Head);

            // Return current list of tasks
            return mCurrentTasks;
        }

        /// <summary>
        /// Starts new task in current session
        /// </summary>
        /// <param name="context">The task context to start</param>
        public void StartNextTask(TimeTaskContext context)
        {
            // Pass task's time to the timer to start
            mSessionTimer.StartNextTask(context.AssignedTime);

            // Save it for progress calculations
            mCurrentTaskTime = context.AssignedTime;
        }

        /// <summary>
        /// Starts the break time
        /// </summary>
        public void StartBreak() => mSessionTimer.StartBreak();

        /// <summary>
        /// Ends the break time
        /// </summary>
        public void EndBreak()
        {
            // If we should recalculate provided tasks...
            if (DI.Settings.RecalculateTasksAfterBreak)
            {
                // Reduce session duration
                mSessionTime -= mCurrentTasks.Head.AssignedTime - CurrentTaskTimeLeft;
                mSessionTime -= CurrentBreakDuration;

                // Shrink tasks to new session time
                SetSessionTasks(GetCalculatedTasks());
            }
            if (mCurrentTasks.WholeList.Count > 0)
                // Reset task with new time
                StartNextTask(mCurrentTasks.Head);
        }

        #endregion

        #region Private Helpers

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

        #endregion
    }
}
