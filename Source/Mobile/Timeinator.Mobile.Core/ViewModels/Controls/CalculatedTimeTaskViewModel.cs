﻿using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using Timeinator.Core;

namespace Timeinator.Mobile.Core
{
    /// <summary>
    /// The view model for calculated time task control (shown right before session starts)
    /// </summary>
    public class CalculatedTimeTaskViewModel : MvxViewModel
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
        /// The list of tags for this task
        /// </summary>
        public List<string> Tags { get; set; }

        /// <summary>
        /// Indicates if this task is important
        /// </summary>
        public bool IsImportant { get; set; }

        /// <summary>
        /// Current progress of this task
        /// </summary>
        public double Progress { get; set; }

        /// <summary>
        /// The time assigned for this specific task
        /// </summary>
        public TimeSpan AssignedTime { get; set; }

        /// <summary>
        /// Indicates if this task should still be saved after its done
        /// Set to false if it should be removed after completion
        /// </summary>
        public bool IsImmortal { get; set; }
    }
}
