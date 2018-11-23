using System;
using System.Collections.Generic;
using System.Text;

namespace Timeinator.Mobile
{
    public static class TaskListHelpers
    {
        /// <summary>
        /// Returns only important tasks from provided TimeTasks
        /// </summary>
        public static List<TimeTaskContext> GetImportant(List<TimeTaskContext> contexts, bool inverse = false) => contexts.FindAll(x => inverse ? !x.IsImportant : x.IsImportant);

        /// <summary>
        /// Returns only constant time tasks from provided TimeTasks
        /// </summary>
        public static List<TimeTaskContext> GetConstant(List<TimeTaskContext> contexts, bool inverse = false) => contexts.FindAll(x => inverse ? !x.HasConstantTime : x.HasConstantTime);

        /// <summary>
        /// Returns only not finished tasks from provided TimeTasks
        /// </summary>
        public static List<TimeTaskContext> GetNotReady(List<TimeTaskContext> contexts, bool inverse = false) => contexts.FindAll(x => inverse ? !(x.Progress < 1) : (x.Progress < 1));

        /// <summary>
        /// Returns priority taking progress into account
        /// </summary>
        public static double GetRealPriority(TimeTaskContext tc) => (int)tc.Priority * (1.0 - tc.Progress);
    }
}
