using System;
using Timeinator.Core;

namespace Timeinator.Mobile.Core
{
    /// <summary>
    /// The interface for handling everything related to time in task session
    /// </summary>
    public interface ISessionTimer
    {
        double CurrentTaskCalculatedProgress { get; }
        bool Paused { get; }
        TimeSpan SessionDuration { get; }
        TimeSpan CurrentTimeLeft { get; }
        TimeSpan CurrentBreakDuration { get; set; }

        event Action TaskFinished;

        #region Kernel

        void SetupSession(Action timerAction, Action taskAction);

        void StartNextTask(TimeTaskContext context);
        void StartBreak();
        void EndBreak();

        #endregion

        void Resume();
        void Pause();
        void Finish();
        void EndSession();

        HeadList<TimeTaskContext> GetTasks();
    }
}
