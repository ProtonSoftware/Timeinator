using System;
using System.Collections.Generic;

namespace Timeinator.Mobile.Domain
{
    /// <summary>
    /// The context for time task to use as an object in this app
    /// </summary>
    public class TimeTaskContext
    {
        /// <summary>
        /// Unique id number of the task
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of this task
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The type of this task
        /// </summary>
        public TimeTaskType Type { get; set; }

        /// <summary>
        /// The priority of this task
        /// </summary>
        public Priority Priority { get; set; }

        /// <summary>
        /// Indicates if this task is marked as important (optional)
        /// </summary>
        public bool IsImportant { get; set; }

        /// <summary>
        /// The user-friendly description for the task
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The progress of this task in the single session scope
        /// </summary>
        public double SessionProgress { get; set; }

        /// <summary>
        /// The task-specific progress that is saved after every session and can reach up to <see cref="MaxProgress"/>
        /// </summary>
        public double Progress { get; set; }

        /// <summary>
        /// The maximum possible progress for the task that once reached, completes the task
        /// </summary>
        public double MaxProgress { get; set; }

        /// <summary>
        /// Indicates if this task has constant user-defined assigned time
        /// </summary>
        public bool HasConstantTime { get; set; }

        /// <summary>
        /// Indicates if this task should still be saved after its done
        /// Set to false if it should be removed after completion
        /// </summary>
        public bool IsImmortal { get; set; }

        /// <summary>
        /// List of tags set by user that help finding this task
        /// </summary>
        public ICollection<string> Tags { get; set; }

        /// <summary>
        /// The date when this task was initially created
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// The time assigned to the task 
        /// </summary>
        public TimeSpan AssignedTime { get; set; }

        /// <summary>
        /// The dynamic time assigned to the task temporarily in a single session
        /// </summary>
        public TimeSpan SessionDynamicTime { get; set; }
    }
}
