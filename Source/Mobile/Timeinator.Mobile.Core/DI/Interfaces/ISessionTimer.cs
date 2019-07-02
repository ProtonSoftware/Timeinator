using System;
using System.Collections.Generic;

namespace Timeinator.Mobile.Core
{
    /// <summary>
    /// The interface for handling everything related to time in task session
    /// </summary>
    public interface ISessionTimer
    {
        TimeSpan SessionDuration { get; }
        TimeSpan TaskDuration { get; }
        TimeSpan CurrentTaskTimeLeft { get; }
        TimeSpan CurrentBreakDuration { get; }

        event Action TaskFinished;

        void SetupSession(Action timerAction, Action taskAction);
        void StartNextTask(TimeSpan taskTime);
        void StartBreak();
    }
}
