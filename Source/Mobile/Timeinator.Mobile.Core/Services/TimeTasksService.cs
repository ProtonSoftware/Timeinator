using System;
using System.Collections.Generic;
using System.Linq;
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
        private List<TimeTaskContext> mCurrentTasks;

        /// <summary>
        /// The time for the current session
        /// </summary>
        private TimeSpan mSessionTime;

        #endregion

        public TimeSpan SessionDuration => mSessionTimer.SessionDuration;

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
            // Prepare a list of task ids to remove
            var taskIds = new List<int>();

            // For each of provided tasks...
            foreach (var task in contexts)
            {
                // Immortal tasks won't be removed
                if (task.IsImmortal)
                    continue;

                // Add the id to the list
                taskIds.Add(task.Id);
            }

            // Send collected ids to the repository to remove associated tasks
            mTimeTasksRepository.RemoveTasks(taskIds);
        }

        /// <summary>
        /// Loads every saved task from the database
        /// </summary>
        /// <returns>A list of found tasks as <see cref="TimeTaskContext"/></returns>
        public List<TimeTaskContext> LoadStoredTasks()
        {
            // Get every task in the database
            var dbTasks = mTimeTasksRepository.GetSavedTasksForToday();

            // Map entities as contexts
            var result = mTimeTasksMapper.ListMap(dbTasks.ToList());

            // Return every found task
            return result;
        }

        /// <summary>
        /// Sets provided tasks in our internal task list
        /// </summary>
        /// <param name="contexts">The tasks to set</param>
        public void SetSessionTasks(List<TimeTaskContext> contexts)
        {
            // Set our internal list with provided tasks
            mCurrentTasks = contexts;
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
                throw new Exception("Attempted to calculate tasks without proper time set.");
            }

            // Everything is nice and set, calculate our session
            var calculatedTasks = mTimeTasksCalculator.CalculateTasksForSession(mCurrentTasks, mSessionTime);

            // Return the tasks
            return calculatedTasks;
        }

        /// <summary>
        /// TODO: Use/fix it, for now its not even called anywhere
        /// Switches orderId of given context
        /// </summary>
        /// <param name="contexts">Current list of contexts</param>
        /// <param name="swap">Context to change order of</param>
        /// <param name="newid">New position of swap context</param>
        /// <returns>Reordered list</returns>
        public List<TimeTaskContext> SwitchOrder(List<TimeTaskContext> contexts, TimeTaskContext swap, int newid)
        {
            contexts.Find(x => x == swap).OrderId = newid;
            contexts = contexts.OrderBy(x => x.OrderId).ToList();
            var orig = contexts.FindIndex(x => x.OrderId == newid);
            for (var i = orig; i < contexts.Count; i++)
            {
                if (contexts[i] != swap)
                    contexts[i].OrderId++;
            }
            return contexts;
        }

        /// <summary>
        /// Starts the task session
        /// </summary>
        /// <param name="action">The action that will be attached to the timer elapsed event</param>
        public void StartSession(Action action)
        {
            // Start the timer session
            mSessionTimer.StartSession(action);
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Validates if we have proper tasks inside service
        /// </summary>
        private void ValidateTasks()
        {
            // Tasks must be set and we need at least one of them
            if (mCurrentTasks == null || mCurrentTasks.Count < 1)
            {
                // Throw exception because it should not ever happen in the code, so something needs a fix
                throw new Exception("Attempted to calculate tasks without providing them.");
            }
        }

        /// <summary>
        /// Validates if session time is enough for current tasks
        /// </summary>
        private bool ValidateTime(TimeSpan time)
        {
            // Calculate what time we need for current session
            var neededTime = mTimeTasksCalculator.CalculateMinimumTimeForTasks(mCurrentTasks);

            // Time must be greater or equal to minimum needed
            return time >= neededTime;
        }

        #endregion
    }
}
