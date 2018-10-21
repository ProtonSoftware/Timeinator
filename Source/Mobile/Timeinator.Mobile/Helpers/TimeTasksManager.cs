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
        /// <summary>
        /// Current tasks that user defined, this stores newest data about tasks
        /// </summary>
        public List<TimeTaskContext> TaskContexts { get; set; }

        /// <summary>
        /// Time when user was ready and declared free time, a moment when user clicked READY
        /// </summary>
        public DateTime ReadyTime { get; set; }

        private TimeSpan m_availableTime;
        /// <summary>
        /// Remaining free time since user clicked READY
        /// </summary>
        public TimeSpan AvailableTime
        {
            get { return (m_availableTime - (DateTime.Now - ReadyTime)); }
            set { m_availableTime = value; }
        }

        #region Constructor
        public TimeTasksManager()
        {
        }
        #endregion

        #region PublicFunctions
        public void AddTask(TimeTaskContext timeTask)
        {
            TaskContexts.Add(timeTask);
            RefreshContexts();
        }

        public void RemoveTask(TimeTaskContext timeTask)
        {
            TaskContexts.Remove(timeTask);
            RefreshContexts();
        }

        /// <summary>
        /// Initialize time managing - user is ready, sets new amount of free time and calculates times for each task
        /// </summary>
        public void UserReady(TimeSpan freetime)
        {
            AvailableTime = freetime;
            ReadyTime = DateTime.Now;
            CalcAssignedTimes(GetEnabled(TaskContexts));
        }

        /// <summary>
        /// Initialize manager with new TimeTasks, automatically re-orders tasks discarding user changes
        /// </summary>
        public void UpdateTaskList(List<TimeTaskContext> timecontexts)
        {
            TaskContexts = timecontexts;
            RefreshContexts();
        }

        /// <summary>
        /// Recalculates AssignedTime in TimeTasks loaded to target TimeTasks
        /// </summary>
        public void CalcAssignedTimes(List<TimeTaskContext> target)
        {
            double priors = sumPriorities(target);
            for (int i = 0; i < target.Count; i++)
            {
                target[i].AssignedTime = new TimeSpan((long)(AvailableTime.Ticks * (GetRealPriority(target[i]) / priors)));
            }
        }

        /// <summary>
        /// Refreshes TaskContexts assigning them a new OrderId
        /// </summary>
        public void RefreshContexts()
        {
            TaskContexts = reOrder(TaskContexts);
            TaskContexts = TaskContexts.OrderBy(x => x.OrderId).ToList();
        }
        
        /// <summary>
        /// Returns only important tasks from provided TimeTasks
        /// </summary>
        public List<TimeTaskContext> GetImportant(List<TimeTaskContext> contexts)
        {
            return contexts.FindAll(x => x.IsImportant);
        }

        /// <summary>
        /// Returns only enabled tasks from provided TimeTasks
        /// </summary>
        public List<TimeTaskContext> GetEnabled(List<TimeTaskContext> contexts)
        {
            return contexts.FindAll(x => !x.IsDisabled);
        }

        /// <summary>
        /// Returns priority taking progress into account
        /// </summary>
        public double GetRealPriority(TimeTaskContext tc)
        {
            return (int)tc.Priority * (1.0 - tc.Progress);
        }
        #endregion

        #region PrivateFunctions
        /// <summary>
        /// Sets order in tasks basing on importance and priority
        /// </summary>
        /// <param name="target">tasks to sort</param>
        List<TimeTaskContext> reOrder(List<TimeTaskContext> target)
        {
            target = new List<TimeTaskContext>(target);
            int uplim = target.Count;
            List<TimeTaskContext> final = new List<TimeTaskContext>();
            for (int g = 0; g < uplim; g++)
            {
                TimeTaskContext top;
                List<TimeTaskContext> tmp = GetImportant(target);
                top = getHighestPriority(tmp.Count > 0 ? tmp : target);
                top.OrderId = g;
                final.Add(top);
                target.Remove(top);
            }
            return final;
        }

        double sumPriorities(List<TimeTaskContext> l)
        {
            double s = 0;
            foreach (TimeTaskContext c in l)
                s += GetRealPriority(c);
            return s;
        }

        TimeTaskContext getHighestPriority(List<TimeTaskContext> l)
        {
            TimeTaskContext context = null;
            foreach (TimeTaskContext c in l)
            {
                if (context == null || context.Priority < c.Priority)
                    context = c;
            }
            return context;
        }
        #endregion
    }
}
