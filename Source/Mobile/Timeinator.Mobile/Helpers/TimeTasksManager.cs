using System.Collections.Generic;

namespace Timeinator.Mobile
{
    /// <summary>
    /// The manager that handles time tasks interactions
    /// </summary>
    public class TimeTasksManager
    {
        public List<TimeTaskContext> TaskContexts { get; set; }

        #region Constructor
        public TimeTasksManager(List<TimeTaskContext> timecontexts)
        {
            TaskContexts = timecontexts;
        }
        #endregion

        public void tets()
        {
            
        }
    }
}
