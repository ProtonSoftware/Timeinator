using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Timeinator.Core;

namespace Timeinator.Mobile.Domain
{
    /// <summary>
    /// The view model for time task control
    /// </summary>
    public class TimeTaskViewModel : MvxViewModel
    {
        #region Public Properties
        #region Session temporary fields

        /// <summary>
        /// Stores progress of the task
        /// </summary>
        public double Progress { get; set; }

        /// <summary>
        /// Stores static time assigned to the task
        /// </summary>
        public TimeSpan AssignedTime { get; set; }

        /// <summary>
        /// Stores dynamic time assigned to the task
        /// </summary>
        public TimeSpan DynamicTime { get; set; }

        #endregion

        /// <summary>
        /// Unique id number of the task
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Unique ordinal number of the task
        /// </summary>
        public int OrderId { get; set; }

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
        /// Used by the user to mark as important (optional)
        /// </summary>
        public bool IsImportant { get; set; }

        /// <summary>
        /// Accurate description of the task
        /// </summary>
        public string Description { get; set; }

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
        public List<string> Tags { get; set; }

        /// <summary>
        /// The date when this task was initially created
        /// </summary>
        public DateTime CreationDate { get; set; }

        #endregion
    }
}
