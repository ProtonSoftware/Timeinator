using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Timeinator.Core;

namespace Timeinator.Mobile.Core
{
    /// <summary>
    /// The view model for time task control
    /// </summary>
    public class TimeTaskViewModel : MvxViewModel
    {
        #region Public Properties

        /// <summary>
        /// Unique id number of the task
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of this task
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The priority of this task
        /// </summary>
        public Priority Priority { get; set; }
        
        /// <summary>
        /// Current progress of this task
        /// </summary>
        public double Progress { get; set; }

        /// <summary>
        /// The description of this task
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The list of tags for this task
        /// </summary>
        public List<string> Tags { get; set; }

        /// <summary>
        /// Indicates if this task is important
        /// </summary>
        public bool IsImportant { get; set; }

        /// <summary>
        /// The time assigned for this specific task
        /// </summary>
        public TimeSpan AssignedTime { get; set; }

        /// <summary>
        /// Indicates if this task should still be saved after its done
        /// Set to false if it should be removed after completion
        /// </summary>
        public bool IsImmortal { get; set; }

        #endregion
    }
}
