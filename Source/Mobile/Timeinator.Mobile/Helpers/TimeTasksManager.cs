using System;
using System.Collections.Generic;
using System.Linq;

namespace Timeinator.Mobile
{
    /// <summary>
    /// The manager that handles time tasks interactions
    /// </summary>
    public class TimeTasksManager
    {
        public List<TimeTaskContext> TaskContexts { get; set; }
        public DateTime ReadyTime { get; set; }
        public TimeSpan AvailableTime { get; set; }

        #region Constructor
        public TimeTasksManager()
        {
        }
        #endregion

        #region PublicFunctions
        /// <summary>
        /// Initialize manager with new TimeTasks
        /// </summary>
        public void UpdateTaskList(List<TimeTaskContext> timecontexts)
        {
            TaskContexts = timecontexts;
            TaskContexts = ReOrder(TaskContexts);
            TaskContexts = TaskContexts.OrderBy(x => x.OrderId).ToList();
        }

        /// <summary>
        /// Recalculates durations in TimeTasks loaded
        /// </summary>
        public void CalcDurations()
        {
            double priors = sumPriorities(TaskContexts);
            for (int i = 0; i < TaskContexts.Count; i++)
            {
            }
        }

        /// <summary>
        /// Sets order in tasks basing on importance and priority
        /// </summary>
        /// <param name="target">tasks to sort</param>
        public List<TimeTaskContext> ReOrder(List<TimeTaskContext> target)
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
        
        /// <summary>
        /// Initialize time managing - user is ready
        /// </summary>
        public void UserReady(TimeSpan freetime)
        {
            AvailableTime = freetime;
            ReadyTime = DateTime.Now;
            CalcDurations();
        }

        public List<TimeTaskContext> GetImportant(List<TimeTaskContext> contexts)
        {
            return contexts.FindAll(x => x.Important);
        }

        public double GetRealPriority(TimeTaskContext tc)
        {
            return tc.Priority * (1.0 - tc.Progress);
        }
        #endregion

        #region PrivateFunctions
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
