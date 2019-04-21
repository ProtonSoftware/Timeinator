using System;
using System.Collections.Generic;
using System.Linq;

namespace Timeinator.Mobile.Core
{
    /// <summary>
    /// The manager that handles time tasks interactions
    /// </summary>
    public class TimeTasksManager : ITimeTasksManager
    {
        #region Private Members

        private TimeSpan mAvailableTime;

        /// <summary>
        /// The list of tasks that user has specified to do in next session
        /// </summary>
        private List<TimeTaskContext> mTaskContexts;

        /// <summary>
        /// Time when user was ready and declared free time, a moment when user clicked READY
        /// </summary>
        private DateTime mReadyTime;

        #endregion

        #region Private Properties

        /// <summary>
        /// Remaining free time since user clicked READY
        /// </summary>
        private TimeSpan AvailableTime
        {
            get => mAvailableTime - (DateTime.Now - mReadyTime);
            set => mAvailableTime = value;
        }

        #endregion

        #region Interface Implementation

        /// <summary>
        /// Updates User specified session time
        /// </summary>
        /// <param name="userTime">User-declared session time</param>
        public void UploadTime(TimeSpan userTime)
        {
            if (userTime != default(TimeSpan))
            {
                mReadyTime = DateTime.Now;
                AvailableTime = userTime;
            }
        }

        /// <summary>
        /// Uploads provided list of tasks to this manager (overrides previous one)
        /// </summary>
        /// <param name="contexts">The tasks to upload</param>
        public void UploadTasksList(List<TimeTaskContext> contexts, TimeSpan userTime = default(TimeSpan))
        {
            UploadTime(userTime);
            mTaskContexts = contexts;
        }

        /// <summary>
        /// Calculates assigned time for every task in the manager
        /// </summary>
        /// <returns>Ordered list of tasks with calculated times</returns>
        public List<TimeTaskContext> GetCalculatedTasksListForSpecifiedTime()
        {
            mTaskContexts = CalculateAssignedTimes();
            return mTaskContexts.OrderBy(x => x.OrderId).ToList();
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Calculates assigned times for every task in the manager
        /// </summary>
        /// <returns>List of tasks with calculated times</returns>
        private List<TimeTaskContext> CalculateAssignedTimes()
        {
            // Get every task with already assigned times
            var constantTasks = mTaskContexts.GetConstant();

            // Get every not assigned task
            var freeTasks = mTaskContexts.GetConstant(true);

            // Calculate how much time left to assign to non-constant tasks
            var timeLeft = AvailableTime - constantTasks.SumTimes();

            // Get a sum of priorities for left tasks
            var prioritySum = freeTasks.SumPriorities();

            // For every task
            foreach (var task in freeTasks)
            {
                // Calculate how much overall time should it take based on priorities
                var timePart = task.GetRealPriority() / prioritySum;

                // Calculate and assign time to task
                task.AssignedTime = TimeSpan.FromSeconds(Math.Ceiling(new TimeSpan((long)(timeLeft.Ticks * timePart)).TotalSeconds));
            }

            // Return both task lists combined together 
            return freeTasks.Concat(constantTasks).ToList();
        }

        #endregion
    }
}
