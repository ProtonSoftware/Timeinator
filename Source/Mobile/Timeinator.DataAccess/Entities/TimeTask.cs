using System;
using Timeinator.Core;

namespace Timeinator.Mobile.DataAccess
{
    /// <summary>
    /// The time task that came from user and is saved in the database
    /// </summary>
    public class TimeTask : BaseObject<int>
    {
        public int OrderId { get; set; }
        public int Priority { get; set; }
        public float Progress { get; set; }
        public bool Enabled { get; set; }
        public bool Important { get; set; }
        public bool Started { get; set; }
        public TimeSpan Cycle { get; set; }

        #region Constructor
        public TimeTask()
        {

        }
        #endregion
    }
}
