using System;
using System.Collections.Generic;
using System.Linq;

namespace Timeinator.Mobile.Core
{
    /// <summary>
    /// The helper that calculates session tasks
    /// </summary>
    public class TimeTasksCalculator : ITimeTasksCalculator
    {
        #region Interface Implementation

        /// <summary>
        /// Calculates how much time is at least required for provided tasks
        /// </summary>
        /// <param name="contexts">Tasks to calculate for</param>
        /// <returns>Required time as <see cref="TimeSpan"/></returns>
        public TimeSpan CalculateMinimumTimeForTasks(List<TimeTaskContext> contexts)
        {
            // Calculate time required for constant tasks
            var taskConstantTime = contexts.GetConstant().SumTimes();

            // Calculate time for remaining tasks
            var taskPrioritiesTime = TimeSpan.FromMinutes(contexts.GetConstant(true).SumPriorities());

            // Sum the times and return
            return taskConstantTime + taskPrioritiesTime;
        }

        /// <summary>
        /// Calculates and assigns the time for every provided task to fill the session
        /// </summary>
        /// <param name="contexts">Tasks to calculate for</param>
        /// <param name="sessionTime">The user's time for session</param>
        /// <returns>List of calculated tasks</returns>
        public List<TimeTaskContext> CalculateTasksForSession(List<TimeTaskContext> contexts, TimeSpan sessionTime)
        {
            // Calculate all the tasks
            var calculatedTasks = CalculateAssignedTimes(contexts, sessionTime);

            // Return them in order
            if (DI.Settings.HighestPrioritySetAsFirst)
                return calculatedTasks.OrderBy(x => x.Priority).ToList();
            else
                return calculatedTasks;
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Calculates assigned times for every provided task
        /// </summary>
        /// <returns>List of tasks with calculated times</returns>
        private List<TimeTaskContext> CalculateAssignedTimes(List<TimeTaskContext> contexts, TimeSpan sessionTime)
        {
            // Get every task with already assigned times
            var constantTasks = contexts.GetConstant();

            // Get every not assigned task
            var freeTasks = contexts.GetConstant(true);

            // Calculate how much time left to assign to non-constant tasks
            var timeLeft = sessionTime - constantTasks.SumTimes();

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
