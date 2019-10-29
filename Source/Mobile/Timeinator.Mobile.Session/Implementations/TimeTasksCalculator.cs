using System;
using System.Collections.Generic;
using System.Linq;
using Timeinator.Mobile.Domain;

namespace Timeinator.Mobile.Session
{
    /// <summary>
    /// The helper that calculates session tasks
    /// </summary>
    public class TimeTasksCalculator : ITimeTasksCalculator
    {
        #region Private Members

        private readonly ISettingsProvider mSettingsProvider;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public TimeTasksCalculator(ISettingsProvider settingsProvider)
        {
            // Inject DI services
            mSettingsProvider = settingsProvider;
        }

        #endregion

        #region Interface Implementation

        /// <summary>
        /// Calculates how much time is at least required for provided tasks
        /// </summary>
        /// <param name="contexts">Tasks to calculate for</param>
        /// <returns>Required time as <see cref="TimeSpan"/></returns>
        public TimeSpan CalculateMinimumTimeForTasks(List<TimeTaskContext> contexts)
        {
            // Calculate time required for constant tasks
            var taskConstantTime = SumTimes(GetConstant(contexts));

            // Calculate time for remaining tasks
            var taskPrioritiesTime = TimeSpan.FromMinutes(SumPriorities(GetConstant(contexts, true)) * mSettingsProvider.MinimumTaskTime);

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
            return mSettingsProvider.HighestPrioritySetAsFirst ? calculatedTasks.OrderBy(x => x.Priority).ToList() : calculatedTasks;
        }

        /// <summary>
        /// Recalculates dynamic time according to break duration
        /// </summary>
        /// <param name="contexts">Tasks resumed</param>
        /// <param name="remainingTime">Duration of last break</param>
        /// <returns>List of recalculated tasks</returns>
        public List<TimeTaskContext> CalculateTasksAfterResume(List<TimeTaskContext> contexts, TimeSpan remainingTime)
        {
            // If we have no remaining tasks then do nothing
            if (contexts.Count <= 0)
                return contexts;

            // Get sum of non constant priorities
            var sumOfPriorities = SumPriorities(GetConstant(contexts, true));

            var constant = GetConstant(contexts);
            var free = GetConstant(contexts, false);

            // Iterate constant tasks
            foreach (var task in constant)
                // Shrink according to progress not priority
                task.SessionDynamicTime = ShrinkProgressedTask(task);

            // Subtract already used time
            remainingTime -= SumDynamicTimes(constant);

            // For each task in the remaining list...
            foreach (var task in free)
            {
                // Get new expected time
                var newTime = Fit(task, remainingTime, sumOfPriorities);

                // Skip tasks that would be too short after substraction, we still want to keep minimum time requirement for them 
                var minTaskTime = TimeSpan.FromMinutes(mSettingsProvider.MinimumTaskTime);
                if (newTime < minTaskTime)
                    newTime = minTaskTime;
                
                // Substract the time from task
                task.SessionDynamicTime = newTime;

                // Extend AssignedTime if dynamically has been assigned more
                if (newTime > task.AssignedTime)
                    task.AssignedTime = newTime;
            }

            // Return ready list
            return contexts;
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Returns only constant time tasks from provided TimeTasks
        /// </summary>
        private List<TimeTaskContext> GetConstant(List<TimeTaskContext> contexts, bool inverse = false) => contexts.FindAll(x => inverse ? !x.HasConstantTime : x.HasConstantTime);

        /// <summary>
        /// Returns priority taking progress into account
        /// </summary>
        private double GetRealPriority(TimeTaskContext context) => (int)context.Priority * (1.0 - context.SessionProgress);

        /// <summary>
        /// Sums priorities of every element
        /// </summary>
        private double SumPriorities(List<TimeTaskContext> contexts) => contexts.Select(x => GetRealPriority(x)).Sum();

        /// <summary>
        /// Sums Assigned Times of every element
        /// </summary>
        private TimeSpan SumTimes(List<TimeTaskContext> contexts) => new TimeSpan(contexts.Select(x => x.AssignedTime.Ticks).Sum());

        /// <summary>
        /// Sums Assigned Times of every element
        /// </summary>
        private TimeSpan SumDynamicTimes(List<TimeTaskContext> contexts) => new TimeSpan(contexts.Select(x => x.SessionDynamicTime.Ticks).Sum());

        /// <summary>
        /// Returns dynamic time according to task progress
        /// </summary>
        private TimeSpan ShrinkProgressedTask(TimeTaskContext constTask) => (constTask.AssignedTime * (1.0 - constTask.SessionProgress));

        private TimeSpan Fit(TimeTaskContext task, TimeSpan timeLeft, double sumPriority)
        {
            // Calculate how much overall time should it take based on priorities
            var timePart = GetRealPriority(task) / sumPriority;

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
            var constantTasks = GetConstant(contexts);

            // Get every not assigned task
            var freeTasks = GetConstant(contexts, true);

            // Calculate how much time left to assign to non-constant tasks
            var timeLeft = sessionTime - SumTimes(constantTasks);

            // Get a sum of priorities for left tasks
            var prioritySum = SumPriorities(freeTasks);

            // For every task
            foreach (var task in freeTasks)
            {
                // Calculate and assign time to task
                task.AssignedTime = Fit(task, timeLeft, prioritySum);

                // Set dynamic time
                task.SessionDynamicTime = new TimeSpan(task.AssignedTime.Ticks);

                // Reset progress
                task.SessionProgress = 0;
            }

            // For every constant task
            foreach (var task in constantTasks)
            {
                // Set dynamic time
                task.SessionDynamicTime = new TimeSpan(task.AssignedTime.Ticks);

                // Reset progress
                task.SessionProgress = 0;
            }

            // Return both task lists combined together 
            return freeTasks.Concat(constantTasks).ToList();
        }

        #endregion
    }
}
