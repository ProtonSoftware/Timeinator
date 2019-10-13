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
            var taskPrioritiesTime = TimeSpan.FromMinutes(contexts.GetConstant(true).SumPriorities() * DI.Settings.MinimumTaskTime);

            // Sum the times and return
            return taskConstantTime + taskPrioritiesTime;
        }

        /// <summary>
        /// Fits tasks assigned times to session and resets dynamic time
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

        /// <summary>
        /// Recalculates dynamic time according to break duration
        /// </summary>
        /// <param name="contexts">Tasks resumed</param>
        /// <param name="remainingTime">Duration of last break</param>
        /// <returns>List of recalculated tasks</returns>
        public List<TimeTaskContext> CalculateTasksAfterResume(List<TimeTaskContext> contexts, TimeSpan remainingTime)
        {
            var tasksLeft = contexts.Count;
            // If we have no remaining tasks then do nothing
            if (tasksLeft <= 0)
                return contexts;

            // Get sum of non constant priorities
            var sumOfPriorities = contexts.GetConstant(true).SumPriorities();

            var constant = contexts.GetConstant();
            var free = contexts.GetConstant(true);

            // Iterate constant tasks
            foreach (var task in constant)
                // Shrink according to progress not priority
                task.DynamicTime = ShrinkProgressedTask(task);

            // For each task in the remaining list...
            foreach (var task in free)
            {
                // Get new expected time
                var newTime = Fit(task, remainingTime, sumOfPriorities);

                // Skip tasks that would be too short after substraction, we still want to keep minimum time requirement for them 
                if (newTime < TimeSpan.FromMinutes(DI.Settings.MinimumTaskTime))
                    newTime = TimeSpan.FromMinutes(DI.Settings.MinimumTaskTime);
                
                // Substract the time from task
                task.DynamicTime = newTime;
            }

            // Return ready list
            return contexts;
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Returns dynamic time according to task progress
        /// </summary>
        private TimeSpan ShrinkProgressedTask(TimeTaskContext constTask) => (constTask.AssignedTime * (1.0 - constTask.Progress));

        private TimeSpan Fit(TimeTaskContext task, TimeSpan timeLeft, double sumPriority)
        {
            // Calculate how much overall time should it take based on priorities
            var timePart = task.GetRealPriority() / sumPriority;

            // Calculate and assign time to task
            return TimeSpan.FromSeconds(Math.Ceiling(new TimeSpan((long)(timeLeft.Ticks * timePart)).TotalSeconds));
        }

        /// <summary>
        /// Calculates assigned times for every task and copies it to dynamic time field
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
                // Calculate and assign time to task
                task.AssignedTime = Fit(task, timeLeft, prioritySum);

                // Set dynamic time
                task.DynamicTime = new TimeSpan(task.AssignedTime.Ticks);

                // Reset progress
                task.Progress = 0;
            }

            // For every constant task
            foreach (var task in constantTasks)
            {
                // Set dynamic time
                task.DynamicTime = new TimeSpan(task.AssignedTime.Ticks);

                // Reset progress
                task.Progress = 0;
            }

            // Return both task lists combined together 
            return freeTasks.Concat(constantTasks).ToList();
        }

        #endregion
    }
}
