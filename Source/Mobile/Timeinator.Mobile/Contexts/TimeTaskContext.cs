using System;

namespace Timeinator.Mobile
{
    /// <summary>
    /// The context for time task to use as an object in this app
    /// </summary>
    public class TimeTaskContext
    {
        /// <summary>
        /// User
        /// </summary>
        public int OrderId { get; set; }
        public float Progress { get; set; }
        public bool Started { get; set; }
        public int Priority { get; set; }
        public bool Enabled { get; set; }
        public bool Important { get; set; }
        public TimeSpan Cycle { get; set; }
    }
}
