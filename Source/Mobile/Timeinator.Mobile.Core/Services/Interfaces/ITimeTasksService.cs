using System;
using System.Collections.Generic;

namespace Timeinator.Mobile.Core
{
    /// <summary>
    /// The interface for service that handles tasks mediation in code
    /// </summary>
    public interface ITimeTasksService
    {
        TimeSpan SessionDuration { get; }
        TimeSpan CurrentTaskTimeLeft { get; }

        #region Database Interaction

List<TimeTaskContext> LoadStoredTasks();

        void SaveTask(TimeTaskContext context);
        void RemoveTask(TimeTaskContext context);
        void RemoveFinishedTasks(List<TimeTaskContext> contexts);

        #endregion

        List<TimeTaskContext> GetCalculatedTasks();
        void SetSessionTasks(List<TimeTaskContext> contexts);
        bool SetSessionTime(TimeSpan userTime);
        void ClearSessionTasks();
        List<TimeTaskContext> SwitchOrder(List<TimeTaskContext> contexts, TimeTaskContext swap, int newid);
        List<TimeTaskContext> StartSession(Action timerAction, Action taskAction);
    }
}
