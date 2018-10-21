using System;
using Timeinator.Core;

namespace Timeinator.Mobile
{
    /// <summary>
    /// The context for time task to use as an object in this app
    /// </summary>
    public class TimeTaskContext
    {
        /// <summary>
        /// Unique ordinal number of the task
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Name of the task
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Shows priority of the task
        /// </summary>
        public Priority Priority { get; set; }

        /// <summary>
        /// Used by the user to mark as important (optional)
        /// </summary>
        public bool IsImportant { get; set; }

        /// <summary>
        /// Accurate description of the task
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Stores progress of the task
        /// </summary>
        public float Progress { get; set; }

        /// <summary>
        /// Sets particular task as temporary invisible for <see cref="TimeTasksManager"/>
        /// </summary>
        public bool IsDisabled { get; set; }

        /// <summary>
        /// Stores time assigned to the task
        /// </summary>
        public TimeSpan AssignedTime { get; set; }

        /// <summary>
        /// Indicates whether task has started
        /// </summary>
        public bool HasStarted => Progress > 0;
<<<<<<< HEAD
=======

        /// <summary>
        /// Tag set by user that helps finding the task
        /// </summary>
        public string Tag { get; set; }

>>>>>>> Development/Michal
    }
}
