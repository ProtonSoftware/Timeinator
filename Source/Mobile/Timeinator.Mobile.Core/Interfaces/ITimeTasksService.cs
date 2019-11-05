using System;
using System.Collections.Generic;
using Timeinator.Core;

namespace Timeinator.Mobile.Domain
{
    /// <summary>
    /// The interface for service that handles tasks mediation in code
    /// </summary>
    public interface ITimeTasksService
    {
        List<TimeTaskContext> LoadStoredTasks(string queryString = "");

        void SaveTask(TimeTaskContext context);
        void RemoveTask(TimeTaskContext context);
        void RemoveFinishedTasks(List<TimeTaskContext> contexts);
    }
}
