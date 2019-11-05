using System;

namespace Timeinator.Mobile.Domain
{
    /// <summary>
    /// The view model for list item control in session page
    /// </summary>
    public class SessionTimeTaskItemViewModel : TimeTaskViewModel
    {
        /// <summary>
        /// The progress of this task in the single session scope
        /// </summary>
        public double SessionProgress { get; set; }

        /// <summary>
        /// The dynamic time assigned to the task temporarily in a single session
        /// </summary>
        public TimeSpan SessionDynamicTime { get; set; }
    }
}
