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
        public TimeTasksManager(List<TimeTaskContext> timecontexts)
        {
            TaskContexts = timecontexts;
        }
        #endregion

        #region PublicFunctions
        /// <summary>
        /// Sets order in tasks basing on importance and priority
        /// </summary>
        /// <param name="target">tasks to sort</param>
        public List<TimeTaskContext> Order(List<TimeTaskContext> target)
        {
            target = new List<TimeTaskContext>(target);
            int uplim = target.Count;
            List<TimeTaskContext> final = new List<TimeTaskContext>();
            for (int g = 0; g < uplim; g++)
            {
                TimeTaskContext top;
                List<TimeTaskContext> tmp = target.FindAll(x => x.Important);
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
        }

        public List<TimeTaskContext> GetEnabled()
        {
            return TaskContexts.FindAll(x => x.Enabled);
        }

        public double GetRealPriority(TimeTaskContext tc)
        {
            return tc.Priority * (1.0-tc.Progress);
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
