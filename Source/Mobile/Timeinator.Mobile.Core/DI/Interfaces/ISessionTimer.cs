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
        TimeSpan CurrentTaskTimeLeft { get; }

        event Action TaskFinished;
        void SetupSession(Action timerAction, Action taskAction);
        void StartNextTask(TimeSpan taskTime);
    }
}
