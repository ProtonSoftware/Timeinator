using System;

namespace Timeinator.Mobile
{
    /// <summary>
    /// The context for time task to use as an object in this app
    /// </summary>
    public class TimeTaskContext
    {
        /// <summary>
        /// User defined order
        /// </summary>
        public int OrderId { get; set; }
        public int Priority { get; set; }
        public float Progress { get; set; }
        /// <summary>
        /// Will be first to be offered
        /// </summary>
        public bool Important { get; set; }
        public bool Started => Progress > 0;
    }
}
