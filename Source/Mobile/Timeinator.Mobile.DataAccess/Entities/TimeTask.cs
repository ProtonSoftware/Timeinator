﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Timeinator.Core;

namespace Timeinator.Mobile.DataAccess
{
    /// <summary>
    /// The time task that came from user and is saved in the database
    /// </summary>
    public class TimeTask : BaseObject<int>
    {
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
        /// Shows progress of the task
        /// </summary>
        public float Progress { get; set; }
        
        /// <summary>
        /// Accurate description of the task
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The collection of every tag associated with this task
        /// This collection is not directly stored in the databased, it's rather created based on TagsString
        /// </summary>
        [NotMapped]
        public ICollection<string> Tags { get; set; }

        /// <summary>
        /// The collection of tags put together as string to save it in the database
        /// </summary>
        public string TagsString
        {
            get
            {
                if (Tags != null)
                    return string.Join("\n", Tags);
                else
                    return null;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    Tags = value.Split('\n').ToList();
            }
        }

        /// <summary>
        /// Indicates if this task should still be saved after its done
        /// Set to false if it should be removed after completion
        /// </summary>
        public bool IsImmortal { get; set; }

        /// <summary>
        /// The date when this task was created and added to the database
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// The date that this task will show up in the list
        /// </summary>
        public DateTime TargetStartDate { get; set; }

        /// <summary>
        /// The constant time assigned to this task
        /// </summary>
        public TimeSpan AssignedTime { get; set; }
    }
}
