﻿using System;
using Timeinator.Core;

namespace Timeinator.Mobile
{
    /// <summary>
    /// The view model for time task control
    /// </summary>
    public class TimeTaskViewModel : BaseViewModel
    {
        /// <summary>
        /// The name of this task
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Unique ordinal number of the task
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Shows priority of the task
        /// </summary>
        public Priority Priority { get; set; }

        /// <summary>
        /// The description of this task
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The tag of this task
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Indicates if this task is important
        /// </summary>
        public bool IsImportant { get; set; }

        /// <summary>
        /// Shows if task is intended to be done in the next session
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Current progress of this task
        /// </summary>
        public double Progress { get; set; }

        /// <summary>
        /// The time assigned for this specific task
        /// </summary>
        public TimeSpan AssignedTime { get; set; }
    }
}
