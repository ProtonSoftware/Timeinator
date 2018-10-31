using System.Collections.Generic;

namespace Timeinator.Mobile
{
    /// <summary>
    /// The interface for service that handles tasks mediation in code
    /// </summary>
    public interface ITimeTasksService
    {
        bool LoadStoredTasks();
        void ConveyTasksToManager(List<TimeTaskContext> tasks);

        void SaveNewTask(TimeTaskContext context);
    }
}
