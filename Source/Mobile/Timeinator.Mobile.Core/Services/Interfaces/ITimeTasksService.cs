using System;
using System.Collections.Generic;
using Timeinator.Core;

namespace Timeinator.Mobile.Core
{
    /// <summary>
    /// The interface for service that handles tasks mediation in code
    /// </summary>
    public interface ITimeTasksService
    {
        TimeSpan SessionDuration { get; }
        TimeSpan TaskDuration { get; }
        TimeSpan CurrentTaskTimeLeft { get; }
        TimeSpan CurrentBreakDuration { get; }
        double CurrentTaskCalculatedProgress { get; }

        #region Database Interaction

        List<TimeTaskContext> LoadStoredTasks(string queryString);

        void SaveTask(TimeTaskContext context);
        void RemoveTask(TimeTaskContext context);
        void RemoveFinishedTasks(List<TimeTaskContext> contexts);

        #endregion

        List<TimeTaskContext> GetCalculatedTasks();
        void SetSessionTasks(List<TimeTaskContext> contexts);
        bool SetSessionTime(TimeSpan userTime);
        void ClearSessionTasks();
        HeadList<TimeTaskContext> StartSession(Action timerAction, Action taskAction);
        void StartNextTask(TimeTaskContext context);
        void StartBreak();
        void EndBreak();
    }
}
