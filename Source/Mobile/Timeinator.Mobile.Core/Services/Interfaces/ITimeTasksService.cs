using System;
using System.Collections.Generic;

namespace Timeinator.Mobile.Core
{
    /// <summary>
    /// The interface for service that handles tasks mediation in code
    /// </summary>
    public interface ITimeTasksService
    {
        List<TimeTaskContext> SwitchOrder(List<TimeTaskContext> contexts, TimeTaskContext swap, int newid);
        List<TimeTaskContext> LoadStoredTasks();
        void ConveyTasksToManager(List<TimeTaskContext> tasks, TimeSpan userTime);
        void ConveyTasksToManager(List<TimeTaskContext> tasks);
        List<TimeTaskContext> GetCalculatedTasksFromManager();
        void ConveyTasksToTimeHandler(List<TimeTaskContext> tasks);

        void SaveTask(TimeTaskContext context);
        void RemoveTask(TimeTaskContext context);
        void RemoveFinishedTasks(List<TimeTaskContext> contexts);
    }
}
