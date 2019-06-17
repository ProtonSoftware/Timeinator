using System;
using System.Collections.Generic;
using System.Linq;

namespace Timeinator.Mobile.Core
{
    /// <summary>
    /// TODO: Get rid of this class, all the tasks methods are used exclusively by calculator, others are not needed here, so this shouldnt exist
    /// </summary>
    public static class TaskListHelpers
    {
        /// <summary>
        /// Returns only important tasks from provided TimeTasks
        /// </summary>
        public static List<TimeTaskContext> GetImportant(this List<TimeTaskContext> contexts, bool inverse = false) => contexts.FindAll(x => inverse ? !x.IsImportant : x.IsImportant);

        /// <summary>
        /// Returns only constant time tasks from provided TimeTasks
        /// </summary>
        public static List<TimeTaskContext> GetConstant(this List<TimeTaskContext> contexts, bool inverse = false) => contexts.FindAll(x => inverse ? !x.HasConstantTime : x.HasConstantTime);

        /// <summary>
        /// Returns only not finished tasks from provided TimeTasks
        /// </summary>
        public static List<TimeTaskContext> GetNotReady(this List<TimeTaskContext> contexts, bool inverse = false) => contexts.FindAll(x => inverse ? !(x.Progress < 1) : (x.Progress < 1));

        /// <summary>
        /// Returns priority taking progress into account
        /// </summary>
        public static double GetRealPriority(this TimeTaskContext tc) => (int)tc.Priority * (1.0 - tc.Progress);

        /// <summary>
        /// Sums priorities of every element
        /// </summary>
        public static double SumPriorities(this List<TimeTaskContext> l)
        {
            double s = 0;
            foreach (var c in l)
                s += c.GetRealPriority();
            return s;
        }

        /// <summary>
        /// Sums Assigned Times of every element
        /// </summary>
        public static TimeSpan SumTimes(this List<TimeTaskContext> l)
        {
            var res = new TimeSpan(0);
            foreach (var c in l)
                res += c.AssignedTime;
            return res;
        }

        public static event Action RefreshUITasks;

        public static void RaiseRefreshEvent() => RefreshUITasks.Invoke();

        public static List<string> SplitTagsString(this string tagsString) => tagsString.Split(' ').ToList();
        public static string CreateTagsString(this List<string> tagsList) => string.Join(" ", tagsList);
    }
}
