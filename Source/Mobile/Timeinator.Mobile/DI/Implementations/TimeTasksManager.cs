using System;
using System.Collections.Generic;
using System.Linq;

namespace Timeinator.Mobile
{
    /// <summary>
    /// The manager that handles time tasks interactions
    /// </summary>
    public class TimeTasksManager : ITimeTasksManager
    {
        #region Private Members

        private TimeSpan mAvailableTime;

        /// <summary>
        /// Current tasks that user defined
        /// </summary>
        private List<TimeTaskContext> TaskContexts { get; set; }

        /// <summary>
        /// Time when user was ready and declared free time, a moment when user clicked READY
        /// </summary>
        private DateTime ReadyTime { get; set; }

        /// <summary>
        /// Remaining free time since user clicked READY
        /// </summary>
        private TimeSpan AvailableTime
        {
            get => mAvailableTime - (DateTime.Now - ReadyTime);
            set => mAvailableTime = value;
        }

        #endregion

        #region Interface Implementation

        /// <summary>
        /// Uploads provided list of tasks to this manager (overrides previous one)
        /// </summary>
        /// <param name="contexts">The tasks to upload</param>
        public void UploadTasksList(List<TimeTaskContext> contexts, TimeSpan userTime = default(TimeSpan))
        {
            if (userTime != default(TimeSpan))
            {
                ReadyTime = DateTime.Now;
                AvailableTime = userTime;
            }
            TaskContexts = contexts;
        }

        /// <summary>
        /// Calculates assigned time for every task in the manager based on provided user free time
        /// </summary>
        /// <param name="userTime">User's free time</param>
        /// <returns>Ordered list of tasks with calculated time</returns>
        public List<TimeTaskContext> GetCalculatedTasksListForSpecifiedTime()
        {
            TaskContexts = CalcAssignedTimes(TaskContexts).ToList();
            return TaskContexts.OrderBy(x => x.OrderId).ToList();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Recalculates AssignedTime in TimeTasks loaded to target TimeTasks
        /// </summary>
        /// <returns>Ready list</returns>
        private List<TimeTaskContext> CalcAssignedTimes(List<TimeTaskContext> target)
        {
            var avt = AvailableTime - SumTimes(target.GetConstant());
            var tmp = target.GetConstant(true);
            var priors = SumPriorities(tmp);
            for (var i = 0; i < tmp.Count; i++)
                tmp[i].AssignedTime = TimeSpan.FromSeconds((int)Math.Ceiling(new TimeSpan((long)(avt.Ticks * (tmp[i].GetRealPriority() / priors))).TotalSeconds));
            return tmp.Concat(target.GetConstant()).ToList();
        }

        private double SumPriorities(List<TimeTaskContext> l)
        {
            double s = 0;
            foreach (var c in l)
                s += c.GetRealPriority();
            return s;
        }

        private TimeSpan SumTimes(List<TimeTaskContext> l)
        {
            var res = new TimeSpan(0);
            foreach (var c in l)
                res += c.AssignedTime;
            return res;
        }

        #endregion
    }
}
