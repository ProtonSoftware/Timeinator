using System;
using System.Collections.Generic;

namespace Timeinator.Mobile
{
    /// <summary>
    /// Interface for Service responsible for handling TimeTask session
    /// </summary>
    public interface ISessionService
    {
        bool Active { get; }
        void Interval(TimeSpan assignedT);
        void Stop();
        void Start();
        void Kill();

        event Action TimerElapsed;
    }
}
