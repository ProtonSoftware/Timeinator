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
        private List<TimeTaskContext> m_taskContexts;
        /// <summary>
        /// Current tasks that user defined, this stores newest data about tasks
        /// </summary>
        public List<TimeTaskContext> TaskContexts {
            get { return m_taskContexts; }
            set {
                m_taskContexts = value;
                RefreshContexts();
            }
        }

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
            m_taskContexts = CalcAssignedTimes(GetEnabled(TaskContexts)).Concat(GetEnabled(TaskContexts, true)).ToList();
        }

        /// <summary>
        /// Recalculates AssignedTime in TimeTasks loaded to target TimeTasks
        /// </summary>
        public List<TimeTaskContext> CalcAssignedTimes(List<TimeTaskContext> target)
        {
            TimeSpan avt = AvailableTime - sumTimes(GetConstant(target));
            List<TimeTaskContext> tmp = GetConstant(target, true);
            double priors = sumPriorities(tmp);
            for (int i = 0; i < tmp.Count; i++)
                tmp[i].AssignedTime = new TimeSpan((long)(avt.Ticks * (GetRealPriority(tmp[i]) / priors)));
            return tmp.Concat(GetConstant(target)).ToList();
        }

        /// <summary>
        /// Refreshes TaskContexts assigning them a new OrderId
        /// </summary>
        public void RefreshContexts()
        {
            m_taskContexts = reOrder(TaskContexts);
            m_taskContexts = m_taskContexts.OrderBy(x => x.OrderId).ToList();
        }
        
        /// <summary>
        /// Returns only important tasks from provided TimeTasks
        /// </summary>
        public List<TimeTaskContext> GetImportant(List<TimeTaskContext> contexts, bool inverse = false) => contexts.FindAll(x => inverse ? !x.IsImportant : x.IsImportant);

        /// <summary>
        /// Returns only enabled tasks from provided TimeTasks
        /// </summary>
        public List<TimeTaskContext> GetEnabled(List<TimeTaskContext> contexts, bool inverse = false) => contexts.FindAll(x => inverse ? x.IsDisabled : !x.IsDisabled);

        /// <summary>
        /// Returns only constant time tasks from provided TimeTasks
        /// </summary>
        public List<TimeTaskContext> GetConstant(List<TimeTaskContext> contexts, bool inverse = false) => contexts.FindAll(x => inverse ? !x.HasConstantTime : x.HasConstantTime);

        /// <summary>
        /// Returns only not finished tasks from provided TimeTasks
        /// </summary>
        public List<TimeTaskContext> GetNotReady(List<TimeTaskContext> contexts, bool inverse = false) => contexts.FindAll(x => inverse ? !(x.Progress < 1) : (x.Progress < 1));

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
            for (int g = 0; g < uplim; g++) // g - new OrderId
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

        TimeSpan sumTimes(List<TimeTaskContext> l)
        {
            TimeSpan res = new TimeSpan(0);
            foreach (TimeTaskContext c in l)
                res += c.AssignedTime;
            return res;
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
