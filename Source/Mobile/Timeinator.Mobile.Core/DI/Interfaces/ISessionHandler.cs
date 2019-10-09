using System;
using System.Collections.Generic;
using Timeinator.Core;

namespace Timeinator.Mobile.Core
{
    /// <summary>
    /// The interface for handling everything related to time in task session
    /// </summary>
    public interface ISessionHandler
    {
        double CurrentTaskCalculatedProgress { get; }
        bool Paused { get; }
        TimeSpan SessionDuration { get; }
        TimeSpan CurrentTimeLeft { get; }
        TimeSpan CurrentBreakDuration { get; set; }

        event Action TaskFinished;

        #region Kernel

        void SetupSession(Action timerAction, Action taskAction);
        bool UpdateDuration(TimeSpan duration);
        void UpdateTasks(List<TimeTaskContext> tasks);
        void StartNextTask(TimeTaskContext context);

        #endregion

        void Resume();
        void Pause();
        void Finish();
        void EndSession();

        HeadList<TimeTaskContext> GetTasks();
        TimeTaskContext GetCurrentTask();
    }
}
