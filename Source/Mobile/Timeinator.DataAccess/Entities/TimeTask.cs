using System;
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
        /// Tag set by user that helps finding the task
        /// </summary>
        public string Tag { get; set; }
    }
}
