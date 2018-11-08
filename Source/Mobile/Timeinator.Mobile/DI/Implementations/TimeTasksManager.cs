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

        private List<TimeTaskContext> mTaskContexts = new List<TimeTaskContext>();

        private TimeSpan mAvailableTime;

        #endregion

        #region Public Properties

        /// <summary>
        /// Current tasks that user defined, this stores newest data about tasks
        /// </summary>
        public List<TimeTaskContext> TaskContexts
        {
            get => mTaskContexts;
            set
            {
                mTaskContexts = value;
                RefreshContexts();
            }
        }

        /// <summary>
        /// Time when user was ready and declared free time, a moment when user clicked READY
        /// </summary>
        public DateTime ReadyTime { get; set; }

        /// <summary>
        /// Remaining free time since user clicked READY
        /// </summary>
        public TimeSpan AvailableTime
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
        public void UploadTasksList(List<TimeTaskContext> contexts)
        {
            // TODO: Maciej
            throw new NotImplementedException();
        }

        /// <summary>
        /// Calculates assigned time for every task in the manager based on provided user free time
        /// </summary>
        /// <param name="userTime">User's free time</param>
        /// <returns>The list of tasks with calculated time</returns>
        public List<TimeTaskContext> GetCalculatedTasksListForSpecifiedTime(TimeSpan userTime)
        {
            // TODO: Maciej
            throw new NotImplementedException();
        }

        #endregion

        #region Private Methods

        private void AddTask(TimeTaskContext timeTask)
        {
            TaskContexts.Add(timeTask);
            RefreshContexts();
        }

        private void RemoveTask(TimeTaskContext timeTask)
        {
            TaskContexts.Remove(timeTask);
            RefreshContexts();
        }

        /// <summary>
        /// Initialize time managing - user is ready, sets new amount of free time and calculates times for each task
        /// </summary>
        private void UserReady(TimeSpan freetime)
        {
            AvailableTime = freetime;
            ReadyTime = DateTime.Now;
            mTaskContexts = CalcAssignedTimes(GetEnabled(TaskContexts)).Concat(GetEnabled(TaskContexts, true)).ToList();
        }

        /// <summary>
        /// Recalculates AssignedTime in TimeTasks loaded to target TimeTasks
        /// </summary>
        private List<TimeTaskContext> CalcAssignedTimes(List<TimeTaskContext> target)
        {
            TimeSpan avt = AvailableTime - SumTimes(GetConstant(target));
            List<TimeTaskContext> tmp = GetConstant(target, true);
            double priors = SumPriorities(tmp);
            for (int i = 0; i < tmp.Count; i++)
                tmp[i].AssignedTime = new TimeSpan((long)(avt.Ticks * (GetRealPriority(tmp[i]) / priors)));
            return tmp.Concat(GetConstant(target)).ToList();
        }

        /// <summary>
        /// Refreshes TaskContexts assigning them a new OrderId
        /// </summary>
        private void RefreshContexts()
        {
            mTaskContexts = ReorderTasks(TaskContexts);
            mTaskContexts = mTaskContexts.OrderBy(x => x.OrderId).ToList();
        }

        /// <summary>
        /// Returns only important tasks from provided TimeTasks
        /// </summary>
        private List<TimeTaskContext> GetImportant(List<TimeTaskContext> contexts, bool inverse = false) => contexts.FindAll(x => inverse ? !x.IsImportant : x.IsImportant);

        /// <summary>
        /// Returns only enabled tasks from provided TimeTasks
        /// </summary>
        private List<TimeTaskContext> GetEnabled(List<TimeTaskContext> contexts, bool inverse = false) => contexts.FindAll(x => inverse ? x.IsDisabled : !x.IsDisabled);

        /// <summary>
        /// Returns only constant time tasks from provided TimeTasks
        /// </summary>
        private List<TimeTaskContext> GetConstant(List<TimeTaskContext> contexts, bool inverse = false) => contexts.FindAll(x => inverse ? !x.HasConstantTime : x.HasConstantTime);

        /// <summary>
        /// Returns only not finished tasks from provided TimeTasks
        /// </summary>
        private List<TimeTaskContext> GetNotReady(List<TimeTaskContext> contexts, bool inverse = false) => contexts.FindAll(x => inverse ? !(x.Progress < 1) : (x.Progress < 1));

        /// <summary>
        /// Returns priority taking progress into account
        /// </summary>
        private double GetRealPriority(TimeTaskContext tc) => (int)tc.Priority * (1.0 - tc.Progress);

        /// <summary>
        /// Sets order in tasks basing on importance and priority
        /// </summary>
        /// <param name="target">Tasks to sort</param>
        private List<TimeTaskContext> ReorderTasks(List<TimeTaskContext> target)
        {
            target = new List<TimeTaskContext>(target);
            int uplim = target.Count;
            List<TimeTaskContext> final = new List<TimeTaskContext>();
            for (int g = 0; g < uplim; g++) // g - new OrderId
            {
                TimeTaskContext top;
                List<TimeTaskContext> tmp = GetImportant(target);
                top = GetHighestPriority(tmp.Count > 0 ? tmp : target);
                top.OrderId = g;
                final.Add(top);
                target.Remove(top);
            }
            return final;
        }

        private double SumPriorities(List<TimeTaskContext> l)
        {
            double s = 0;
            foreach (TimeTaskContext c in l)
                s += GetRealPriority(c);
            return s;
        }

        private TimeSpan SumTimes(List<TimeTaskContext> l)
        {
            TimeSpan res = new TimeSpan(0);
            foreach (TimeTaskContext c in l)
                res += c.AssignedTime;
            return res;
        }

        private TimeTaskContext GetHighestPriority(List<TimeTaskContext> l)
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
