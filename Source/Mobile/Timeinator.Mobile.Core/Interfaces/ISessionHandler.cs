using System;
using System.Collections.Generic;
using Timeinator.Core;

namespace Timeinator.Mobile.Domain
{
    /// <summary>
    /// The interface for handling everything related to time in task session
    /// </summary>
    public interface ISessionHandler
    {
        double CurrentTaskCalculatedProgress { get; }
        bool Paused { get; }
        TimeSpan SessionTime { get; }
        TimeSpan SessionDuration { get; }
        TimeSpan CurrentTimeLeft { get; }
        TimeSpan CurrentBreakDuration { get; set; }

        event Action TaskStarted, TaskFinished, SessionFinished, SessionStarted;

        void SetupSession(Action timerAction);
        bool UpdateDuration(TimeSpan duration);
        void UpdateTasks(List<TimeTaskContext> tasks);
        void Calculate();
        HeadList<TimeTaskContext> GetTasks();
        TimeTaskContext GetCurrentTask();
        void ClearSessionTasks();

        void Resume();
        void Pause();
        void Finish();
        void EndSession();
    }
}
