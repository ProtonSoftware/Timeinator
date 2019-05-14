using System;

namespace Timeinator.Mobile.Core
{
    /// <summary>
    /// The interface for handling everything related to time in task session
    /// </summary>
    public interface ISessionTimer
    {
        TimeSpan SessionDuration { get; }
        void StartSession(Action action);
    }
}
